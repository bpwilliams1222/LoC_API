using Discord;
using Discord.WebSocket;
using LoCWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadBot_v4.Models
{

    public class ChannelsModel
    {
        public SocketGuild LoCServer;
        public SocketTextChannel chanHallOfChaos;
        public SocketTextChannel chanEENews;
        public SocketTextChannel chanLeaderboard;
        public SocketTextChannel chanForeignAffairs;
        public SocketTextChannel chanSpyops;
        public SocketTextChannel chanKillRunRoom1;
        public SocketTextChannel chanKillRunRoom2;
        public SocketTextChannel chanKillRunSpy;
        public SocketTextChannel chanBotUpdates;
        public SocketTextChannel chanOnlineCountries;
        public SocketTextChannel chanBotErrors;

        //private BackgroundWorker OnlineUserDetectionService;

        public ChannelsModel(DiscordSocketClient client)
        {
            ulong serverId = 317005397598666753;
            ulong EENewsChanId = 317009651386417153;
            ulong HallOfChaosChanId = 317005397598666753;
            ulong LeaderBoardChanId = 321625978923253760;
            ulong ForeignAffairsChanId = 322185152132546561;
            ulong spyOpsChanId = 322189909811134474;
            ulong KR1ChanId = 322190947951837184;
            ulong KR2ChanId = 322191885449822218;
            ulong KRSpyChanId = 322193057195753473;
            ulong botUpdatesChanId = 324546319047852034;
            ulong onlineCountryFeedChanId = 332955434296279052;
            ulong botErrorsChanId = 335417156084957185;

            LoCServer = client.Guilds.SingleOrDefault(c => c.Id == serverId);
            chanEENews = findTextChannel(LoCServer, EENewsChanId);
            chanHallOfChaos = findTextChannel(LoCServer, HallOfChaosChanId);
            chanLeaderboard = findTextChannel(LoCServer, LeaderBoardChanId);
            chanForeignAffairs = findTextChannel(LoCServer, ForeignAffairsChanId);
            chanSpyops = findTextChannel(LoCServer, spyOpsChanId);
            chanKillRunRoom1 = findTextChannel(LoCServer, KR1ChanId);
            chanKillRunRoom2 = findTextChannel(LoCServer, KR2ChanId);
            chanKillRunSpy = findTextChannel(LoCServer, KRSpyChanId);
            chanBotUpdates = findTextChannel(LoCServer, botUpdatesChanId);
            chanOnlineCountries = findTextChannel(LoCServer, onlineCountryFeedChanId);
            chanBotErrors = findTextChannel(LoCServer, botErrorsChanId);

            /*using (OnlineUserDetectionService = new BackgroundWorker())
            {
                OnlineUserDetectionService.DoWork += CheckForOnlineUsers;
                TimeSpan x = new TimeSpan(0, 5, 0);
                System.Timers.Timer timer = new System.Timers.Timer(x.TotalMilliseconds);
                timer.Elapsed += CheckOnlineUsersService;
                timer.Start();
            }*/
        }

        /*
         * Find Channel Method
         * 
         * Purpose:
         * Returns a channel object based on discordChannelId
         * 
         */
        public SocketTextChannel findTextChannel(SocketGuild server, ulong id)
        {
            try
            {
                if (server != null)
                {
                    foreach (SocketTextChannel channel in server.TextChannels)
                    {
                        if (channel.Id == id)
                            return channel;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /*
         * Send Tag Change Object To Channel Method
         * 
         * Purpose:
         * Parses the TagChange data received and sends a message to the channel provided
         * 
         */
        public async void SendTagChangeToChannel(List<TagChange> TagChanges, SocketGroupChannel chan)
        {
            string message = "```", tempMessage = "";
            foreach (var tagChange in TagChanges)
            {
                string fromTag, toTag;
                if (tagChange.FromTag == "" || tagChange.FromTag == null)
                    fromTag = "Untagged";
                else
                    fromTag = tagChange.FromTag;
                if (tagChange.ToTag == "" || tagChange.ToTag == null)
                    toTag = "Untagged";
                else
                    toTag = tagChange.ToTag;
                tempMessage += "Country (#" + tagChange.Number + ") - Was detected changing from [" + fromTag + "] to [" + toTag + "].";
                if ((message.Length + tempMessage.Length + Environment.NewLine.Length) < 2000)
                {
                    message += tempMessage + Environment.NewLine;
                }
                else
                {
                    await chan.SendMessageAsync(message + "```");
                    message = "```";
                    message += tempMessage + Environment.NewLine;
                }
            }
            await chan.SendMessageAsync(message + "```");
        }

        /*
         * SendSpyOpToChannel
         * 
         * Purpose:
         * This method handles incomming SpyOps as well as !cty.op bot command to send Countries Spy Op Data to the Channel requested
         * 
         */
        public async void SendSpyOpToChannel(SpyOpInfo op, ISocketMessageChannel chan)
        {
            try
            {
                if (chan == null)
                    await chanSpyops.SendMessageAsync(op.GetSpyOpMessage());// frmDiscordBot.Bot.channels.chanSpyops.SendMessage(op.GetSpyOpMessage());
                else
                    await chan.SendMessageAsync(op.GetSpyOpMessage());

                // TO DO:
                // Later we can add relation detection, if relation is detected display a warnign message to user.
            }
            catch (Exception c)
            {
                if (c.Message != "You are being rate limited.")
                {
                    Program.Errors.Add(new ErrorModel(c));
                }
            }
        }

        /*
         * Build Kill List Message Method
         * 
         * Purpose:
         * Returns a string representing a message based on the current kill list
         * 
         */
        /*public string BuildKillListGreetingMessage()
        {
            string message = "```";

            if (frmDiscordBot.Storage.CountryStorage.Get().Where(c => c.KillList == 1).Count() == 0)
            {
                message += "There currently are no countries on our kill list.";
            }
            else
            {
                foreach (var country in frmDiscordBot.Storage.CountryStorage.Get().Where(c => c.KillList == 1))
                {
                    string tagTemp = "";
                    if (country.Tag == "")
                        tagTemp = "Untagged";
                    else
                        tagTemp = country.Tag;
                    message += "Country: " + country.Name + " (#" + country.Number + ")[" + tagTemp +
                        "] Net:" + country.ConvertNumberToDisplay(country.Networth) +
                        " Land:" + country.ConvertNumberToDisplay(country.Land) + Environment.NewLine;
                }
            }

            return message + "```";
        }*/

        /*
         * Build Relation Message Method
         * 
         * Purpose:
         * Returns a string representing a message based on the current relations
         * 
         */
        /*public string BuildRelationGreetingMessage()
        {
            string message = "```";
            if (frmDiscordBot.Storage.RelationsStorage.Get().Count() == 0)
            {
                message += "Your clan leader has not specified any relations yet.";
            }
            else
            {
                foreach (var relation in frmDiscordBot.Storage.RelationsStorage.Get())
                {
                    message += "ClangTag: [" + relation.clanTag + "] -> PactType: " + relation.RelationType + Environment.NewLine;
                }
            }
            return message + "```";
        }*/

        /*
         * Check Online Users Service
         * 
         * Purpose:
         * Checks to see if a process is in operation, if not runs the CheckForOnlineUsers BackgroundWorker
         * 
         */
        /*public void CheckOnlineUsersService(object sender, EventArgs e)
        {
            if (!OnlineUserDetectionService.IsBusy)
                OnlineUserDetectionService.RunWorkerAsync();
        }*/

        /*
         * Check Online Users
         * 
         * Purpose:
         * Checks to see if users are online, if they are sends an automated message(Max 1 message/day) to keep them up to date with clan affairs.
         * If Users are not online for 3 days, bot sends them a message anyway
         * 
         */
        /*public async void CheckForOnlineUsers(object sender, DoWorkEventArgs e)
        {
            try
            {
                foreach (var user in LoCServer.Users)
                {
                    if (!user.Name.Contains("MadBot"))
                    {
                        if (user.Status.Value == UserStatus.Online || user.Status.Value == UserStatus.Idle)
                        {
                            if (LastJoins.Any(c => c.userName == user.Name))
                            {
                                if (LastJoins.Where(c => c.userName == user.Name).OrderByDescending(c => c.timestamp).FirstOrDefault().timestamp < DateTime.UtcNow.AddDays(-1))
                                {
                                    await user.SendMessage(BuildRelationGreetingMessage());
                                    await user.SendMessage(BuildKillListGreetingMessage());
                                }
                                LastJoins.RemoveAll(c => c.userName == user.Name);
                                LastJoins.Add(new UserJoin
                                {
                                    userName = user.Name,
                                    timestamp = DateTime.UtcNow
                                });
                            }
                            else
                            {
                                LastJoins.Add(new UserJoin
                                {
                                    userName = user.Name,
                                    timestamp = DateTime.UtcNow
                                });
                                await user.SendMessage(BuildRelationGreetingMessage());
                                await user.SendMessage(BuildKillListGreetingMessage());
                            }
                        }
                        else
                        {
                            if (LastJoins.Any(c => c.userName == user.Name))
                            {
                                if (LastJoins.FirstOrDefault(C => C.userName == user.Name).timestamp < DateTime.UtcNow.AddDays(-3))
                                {
                                    await user.SendMessage(BuildRelationGreetingMessage());
                                    await user.SendMessage(BuildKillListGreetingMessage());
                                }
                                LastJoins.RemoveAll(c => c.userName == user.Name);
                                LastJoins.Add(new UserJoin
                                {
                                    userName = user.Name,
                                    timestamp = DateTime.UtcNow
                                });
                            }
                            else
                            {
                                LastJoins.Add(new UserJoin
                                {
                                    userName = user.Name,
                                    timestamp = DateTime.UtcNow
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception c)
            {
                ErrorLogs.SaveErrorToXML(new BotError(c));
            }

        }*/

        /*
         * Message all Users Method
         * 
         * Purpose:
         * Takes a message received and sends it to all users
         * 
         */
        public async void PrivateMessageAllUsers(string message)
        {
            foreach (var user in LoCServer.Users)
            {
                if (!user.Username.Contains("MadBot"))
                    await user.SendMessageAsync(message);
            }
        }
    }
}
