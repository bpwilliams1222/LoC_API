using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LoCWebApp.Models;
using MadBot_v4.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MadBot_v4
{
    public class Program
    {
        public static DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public static List<ErrorModel> Errors { get; set; }
        public static ChannelsModel Channels;

        private BackgroundWorker ErrorsSavingService;
        private System.Timers.Timer ErrorsSavingServiceTimer;

        private BackgroundWorker NewOpDetectionService;
        private BackgroundWorker ProcessOpQueueService;
        private System.Timers.Timer NewOpDetectionServiceTimer;
        private System.Timers.Timer ProcessOpQueueServiceTimer;

        private List<SpyOpInfo> OpQueue = new List<SpyOpInfo>();


        public void CheckSaveErrorsProcess(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!ErrorsSavingService.IsBusy)
                ErrorsSavingService.RunWorkerAsync();
        }

        public void SaveErrorsData(object sender, DoWorkEventArgs e)
        {
            List<ErrorModel> ErrorsToSave = Errors;
            Errors.Clear();
            Errors = new List<ErrorModel>();
            if(!Directory.Exists(@"C:\WebData\BotErrors"))
            {
                Directory.CreateDirectory(@"C:\WebData\BotErrors");
            }
            XmlSerializer xs = new XmlSerializer(typeof(List<ErrorModel>));
            using (TextWriter tw = new StreamWriter(@"C:\WebData\BotErrors\" + Guid.NewGuid() + ".xml"))
            {
                xs.Serialize(tw, ErrorsToSave);
                tw.Close();
            }
        }

        [STAThread]
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (ErrorsSavingService = new BackgroundWorker())
            {
                ErrorsSavingService.DoWork += SaveErrorsData;
                ErrorsSavingServiceTimer = new System.Timers.Timer(new TimeSpan(0, 60, 0).TotalMilliseconds);
                ErrorsSavingServiceTimer.Elapsed += CheckSaveErrorsProcess;
                ErrorsSavingServiceTimer.Start();
            }

            OpQueue = new List<SpyOpInfo>();

            using (NewOpDetectionService = new BackgroundWorker())
            {
                NewOpDetectionService.DoWork += CheckForNewOps;
                TimeSpan x = new TimeSpan(0, 0, 0, 0, 500);
                NewOpDetectionServiceTimer = new System.Timers.Timer(x.TotalMilliseconds);
                NewOpDetectionServiceTimer.Elapsed += CheckCheckForNewOpsProcess;
                NewOpDetectionServiceTimer.Start();
                x = new TimeSpan(0, 0, 1);
                ProcessOpQueueServiceTimer = new System.Timers.Timer(x.TotalMilliseconds);
                ProcessOpQueueServiceTimer.Elapsed += CheckQueueServiceProcess;
                ProcessOpQueueServiceTimer.Start();
                using (ProcessOpQueueService = new BackgroundWorker())
                {
                    ProcessOpQueueService.DoWork += ProcessQueue;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider();

            await InstallCommandsAsync();

            string token = "MzU1MzkwNzg1ODQ1MTk4ODQ4.DRHWOw.e7tUoO4E7pZJgHs8S65eoX-Uk2U"; // Remember to keep this private!
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            Thread.Sleep(1500);

            if (_client.ConnectionState == ConnectionState.Connected)
            {
                Channels = new Models.ChannelsModel(_client);
                //_client.UserJoined += Announcements;
            }

            Application.Run(new Form1());
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += HandleCommandAsync;
            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new SocketCommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        // checks Background process to see if is is running, if not executes CheckForNewOps
        private void CheckCheckForNewOpsProcess(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!NewOpDetectionService.IsBusy)
                NewOpDetectionService.RunWorkerAsync();
        }

        // cheks Background process to see if it is running, if not executes ProcessQueue
        private void CheckQueueServiceProcess(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!ProcessOpQueueService.IsBusy)
                ProcessOpQueueService.RunWorkerAsync();
        }

        // Checks for New Ops, if Found processes them saving them to xml and adding to queue if necessary
        private void CheckForNewOps(object sender, DoWorkEventArgs e)
        {
            try
            {
                string[] files = Directory.GetFiles(@"C:\WebData\newSpyOps");
                foreach (var file in files)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(SpyOp));
                    using (var sr = new StreamReader(file))
                    {
                        var op = (SpyOp)xs.Deserialize(sr);                        
                        OpQueue.Add(new SpyOpInfo(op.json, op.type, op.uploader_api_key));                        
                        sr.Close();
                    }
                    File.Delete(file);
                }
            }
            catch (Exception c)
            {
                Errors.Add(new ErrorModel(c));
            }
        }

        // Determines if there are Ops in the Queue and sends them to the channel
        private void ProcessQueue(object sender, DoWorkEventArgs e)
        {
            if (OpQueue.Count() >= 1)
            {
                List<SpyOpInfo> tempQueue = new List<SpyOpInfo>();
                tempQueue = OpQueue.ToList();
                foreach (var op in tempQueue)
                {
                    // Send to Bot
                    try
                    {
                        Channels.SendSpyOpToChannel(op, null);
                    }
                    catch (NullReferenceException)
                    {
                        InitializeChannels();
                        Channels.SendSpyOpToChannel(op, null);
                    }
                    OpQueue.Remove(op);
                }
            }
        }

        private static void InitializeChannels()
        {
            if (Program._client.ConnectionState == ConnectionState.Connected)
            {
                Channels = new Models.ChannelsModel(_client);
            }
        }
    }
}
