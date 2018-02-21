using Discord;
using Discord.Commands;
using LoCWebApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MadBot_v4.Models
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Bot will say what is echoed to it")]
        public async Task SayAsync([Summary("Causes the Bot to Say Hello")] string echo, [Remainder] string user)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo + " " + user);
        }

        [Command("help")]
        [Summary("Bot will PM user a list of commands and instructions")]
        public async Task HelpAsync([Summary("Bot will PM user a list of commands and instructions")][Remainder] string msg = null)
        {
            try
            {
                List<string> help = new List<string>();
                help.Add("Command: !say || Parameters: 1)message 2)user || Example: !say Hello MadNudist");
                help.Add("Command: !own || Parameters: 1)Country Number || Example: !own 1 or !own 1,2,3");
                help.Add("Command: !disown || Parameters: 1)Country Number || Example: !disown 1 or !disown 1,2,3");
                help.Add("Command: !assign || Parameters: 1)Country Number 2)username || Example: !assign " + '"' + "1" + '"' + " " + '"' + "MadNudist" + '"' + " or !assign " + '"' + "1,2,3" + '"' + " " + '"' + "MadNudist" + '"');
                help.Add("Command: !unassign || Parameters: 1)Country Number || Example: !unassign " + '"' + "1" + '"' + " or !unassign " + '"' + "1,2,3" + '"');
                help.Add("Command: !whois || Parameters: 1)Country Number || Example: !whois 1");
                help.Add("Command: !lookup || Parameters: 1)user[optional, will default to your username] || Example: !lookup or !lookup MadNudist");
                help.Add("Command: !cty.info || Parameters: 1)Country Number || Example: !cty.info 1 or !cty.info 2");
                help.Add("Command: !cty.op || Parameters: 1)Country Number || Example: !cty.op 1 or !cty.op 2");
                //help.Add("Command: !online || Parameters: 1)Country Number or Tag || Example: !online 1 or !online LoC");
                help.Add("Command: !insult || Parameters: 1)user, tag, inanimate objects, or mob || Example: !insult mob or !insult CC");
                //help.Add("Command: !user.add || Parameters: 1)ApiKey 2)Discord UserId  || Example: !user.add " + '"' + "ApiKey123" + '"' + " 123456789");
                //help.Add("Command: !user.remove || Parameters: 1)Discord UserId  || Example: !user.remove 123456789");
                //help.Add("Command: !user.update || Parameters: 1)ApiKey 2)Username 3)Discord UserId  || Example: !user.update " + '"' + "ApiKey123" + '"' + " " + '"' + "MadNudist" + '"' + " 123456789");
                //help.Add("Command: !user.update || Parameters: 1)Username 3)Discord UserId  || Example: !user.update " + '"' + "MadNudist" + '"' + " 123456789");
                help.Add("Command: !stats.net || No Parameters Needed || Example: !stats.net");
                help.Add("Command: !stats.war || No Parameters Needed || Example: !stats.war");
                help.Add("Command: !stats.market || Parameters: 1)Timeperiod || Example: !stats.market 1d or !stats.market 1h");
                //help.Add("Command: !relation.add || Parameters: 1)tag 2)pactType 3)notes 4)selfRenewing || Example: !relation.add " + '"' + "IMP" + '"' + " " + '"' + "LDP" + '"' + " " + '"' + "n/a" + '"' + " " + '"' + "true" + '"');
                //help.Add("Command: !relation.remove || Parameters: 1)tag || Example: !relation.remove IMP");
                //help.Add("Command: !relations || Parameters: none || Example: !relations");
                //help.Add("Command: !announcement.add || Parameters: 1) announcement in quotes || Example: !announcement.add " + '"' + "This is my note in quotes" + '"');
                //help.Add("Command: !announcement.remove || Parameters: 1) announcement in quotes || Example: !announcement.remove " + '"' + "This is my note in quotes" + '"');
                //help.Add("Command: !announcements || Parameters: none || Examples: !announcements or !whatsnew or !whatsup");
                // ReplyAsync is a method on ModuleBase
                string message = "```";
                foreach (var command in help)
                {
                    if ((message.Length + command.Length + Environment.NewLine.Length) < 2000)
                        message += command + Environment.NewLine;
                    else
                    {
                        message += "```";
                        await Context.Message.Author.SendMessageAsync(message);
                        message = "```";
                        message += command + Environment.NewLine;
                    }
                }
                if (message.Length > 3)
                {
                    message += "```";
                    await Context.Message.Author.SendMessageAsync(message);
                }
            }
            catch (Exception c)
            {
                Program.Errors.Add(new ErrorModel(c));
                await ReplyAsync("I encountered a problem, and it was documented, please contact MadNudist.");
            }
        }

        [Command("own")]
        [Summary("This will own a country from the database")]
        [Alias("claim")]
        public async Task OwnAsync([Summary("This will own a country from the database")][Remainder] string cnum)
        {
            List<int> cnums = new List<int>();
            int tester = 0;
            foreach(var num in cnum.Split(','))
            {
                if(int.TryParse(num, out tester))
                {
                    cnums.Add(tester);
                }
            }
            string json = new JavaScriptSerializer().Serialize(new
            {
                countries = cnums.ToArray(),
                user = Context.Message.Author.Username
            });
            if (PostToAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/claim", json) == "true")
                await Context.Message.Author.SendMessageAsync("Countries were successfully claimed.");
            else
                await Context.Message.Author.SendMessageAsync("Something went wrong while claiming your countries, and has been reported to the admin.");
            /*var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/claim");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    countries = cnums.ToArray(),
                    user = Context.Message.Author.Username
                });

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                if (result == "true")
                    await Context.Message.Author.SendMessageAsync("Countries were successfully claimed.");                    
            }
            await Context.Message.Author.SendMessageAsync("Something went wrong while claiming your countries, and has been reported to the admin.");*/
        }

        [Command("assign")]
        [Summary("This will assign a country from the database to the provided user")]
        public async Task AssignAsync([Summary("This will assign a country from the database to the provided user")]string cnum, string user)
        {
            try
            {
                List<int> cnums = new List<int>();
                int tester = 0;
                foreach(var num in cnum.Split(','))
                {
                    if(int.TryParse(num, out tester))
                    {
                        cnums.Add(tester);
                    }
                }
                string json = new JavaScriptSerializer().Serialize(new
                {
                    countries = cnums.ToArray(),
                    user = user
                });
                if (PostToAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/claim", json) == "true")
                    await Context.Message.Author.SendMessageAsync("Countries were successfully claimed.");
                else
                    await Context.Message.Author.SendMessageAsync("Something went wrong while claiming your countries, and has been reported to the admin.");
            }
            catch (Exception c)
            {
                Program.Errors.Add(new ErrorModel(c));
                await ReplyAsync("I encountered a problem, and it was documented, please contact MadNudist.");
            }
        }

        [Command("disown")]
        [Summary("This will disown a country from the database")]
        [Alias("unclaim", "unassign")]
        public async Task DisownAsync([Summary("This will disown a country from the database")][Remainder] string cnum)
        {
            List<int> cnums = new List<int>();
            int tester = 0;
            foreach (var num in cnum.Split(','))
            {
                if (int.TryParse(num, out tester))
                {
                    cnums.Add(tester);
                }
            }
            string json = new JavaScriptSerializer().Serialize(new
            {
                countries = cnums.ToArray()
            });
            if (PostToAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/unclaim", json) == "true")
                await Context.Message.Author.SendMessageAsync("Countries were successfully unclaimed.");
            else
                await Context.Message.Author.SendMessageAsync("Something went wrong while unclaiming your countries, and has been reported to the admin.");            
        }

        [Command("whois")]
        [Summary("Provides owner information based on a countries number provided.")]
        public async Task WhoisAsync([Summary("Provides owner information based on a countries number provided.")] [Remainder] int cnum)
        {
            try
            {
                var jsonOutput = GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/bynumber/" + cnum);

                var cty = JsonConvert.DeserializeObject<Country>(jsonOutput);

                if (cty.User == "" || cty.User == null)
                    await ReplyAsync("Country #" + cnum + " belongs to a unknown user.");
                else
                    await ReplyAsync("Country #" + cnum + " belongs to " + cty.User);
            }
            catch (Exception c)
            {
                Program.Errors.Add(new ErrorModel(c));
                await ReplyAsync("I encountered a problem, and it was documented, please contact MadNudist.");
            }
        }

        [Command("lookup")]
        [Summary("Provides the countries numbers owned by the user provided.")]
        public async Task LookupAsync([Summary("Provides the countries numbers owned by the user provided.")] [Remainder] string user = null)
        {
            try
            {
                string message;
                if (user == null)
                    user = Context.Message.Author.Username;
                var jsonOutput = GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/byuser/" + user);

                var ctys = JsonConvert.DeserializeObject<List<Country>>(jsonOutput);

                var liveCtyNums = ctys.Where(c => c.User == user && c.Status != CountryStatus.Dead).OrderBy(c => c.Number).Select(c => c.Number).ToArray();
                var deadCtyNums = ctys.Where(c => c.User == user && c.Status == CountryStatus.Dead).OrderBy(c => c.Number).Select(c => c.Number).ToArray();
                if (deadCtyNums.Count() > 0 && liveCtyNums.Count() > 0)
                {
                    message = "User owns the following live countries ";

                    foreach (var num in liveCtyNums)
                    {
                        message += num + ", ";
                    }
                    message = message.TrimEnd(' ').TrimEnd(',') + " and the following dead countries ";

                    foreach (var num in deadCtyNums)
                    {
                        message += num + ", ";
                    }
                    message = message.TrimEnd(' ').TrimEnd(',') + ".";
                    await ReplyAsync(message);
                }
                else if (deadCtyNums.Count() == 0 && liveCtyNums.Count() > 0)
                {
                    message = "User owns the following live countries ";

                    foreach (var num in liveCtyNums)
                    {
                        message += num + ", ";
                    }

                    message = message.TrimEnd(' ').TrimEnd(',') + ".";
                    await ReplyAsync(message);
                }
                else if (deadCtyNums.Count() > 0 && liveCtyNums.Count() == 0)
                {
                    message = "User owns the following dead countries ";

                    foreach (var num in deadCtyNums)
                    {
                        message += num + ", ";
                    }

                    message = message.TrimEnd(' ').TrimEnd(',') + ".";
                    await ReplyAsync(message);
                }
                else
                    await ReplyAsync("The username " + user + " does not own any countries currently.");
            }
            catch (Exception c)
            {
                Program.Errors.Add(new ErrorModel(c));
                await ReplyAsync("I encountered a problem, and it was documented, please contact MadNudist.");
            }
        }
        
        [Command("cty.info")]
        [Summary("This will provide country information based on the number provided.")]
        [Alias("ctyinfo", "countryinfo")]
        public async Task CtyInfoAsync([Summary("This will provide country information based on the number provided.")][Remainder] string cnum)
        {
            try
            {
                int countryNumber = 0;
                if (int.TryParse(cnum, out countryNumber))
                {
                    var jsonOutput = GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/bynumber/" + countryNumber);

                    var cty = JsonConvert.DeserializeObject<Country>(jsonOutput);

                    if (cty != null)
                    {
                        //Output FORMAT
                        // Max 32 Chars  7 Chars 9 Chars     10 chars      12 chars     6 chars  Max 25 chars
                        // Country Name   (#1)    [Tag]   | Net:999.9M | Land:999,999 | DR:999   | User: |
                        string countryName = MessageFormattingModels.AddSpacesToMessage(cty.Name, MessageFormattingModels.DetermineSpacing(34, cty.Name.Length));
                        string countryNum = MessageFormattingModels.AddSpacesToMessage("(#" + cty.Number + ")", MessageFormattingModels.DetermineSpacing(9, ("(#" + cty.Number + ")").Length));
                        string tag;
                        if (cty.Tag != "" || cty.Tag != null)
                        {
                            tag = MessageFormattingModels.AddSpacesToMessage(cty.Tag, MessageFormattingModels.DetermineSpacing(11, cty.Tag.Length));
                        }
                        else
                        {
                            tag = MessageFormattingModels.AddSpacesToMessage("[]", MessageFormattingModels.DetermineSpacing(11, "[]".Length));
                        }
                        string net = MessageFormattingModels.ConvertNumberToAbrNumberString(cty.Networth);
                        string netMsg = MessageFormattingModels.AddSpacesToMessage("Net:" + net, MessageFormattingModels.DetermineSpacing(12, ("Net:" + net).Length));
                        string land = MessageFormattingModels.ConvertNumberToAbrNumberString(cty.Land);
                        string landMsg = MessageFormattingModels.AddSpacesToMessage("Land:" + land, MessageFormattingModels.DetermineSpacing(14, ("Land:" + land).Length));
                        int dr = DetermineDR(cty.Number);
                        string drMsg = MessageFormattingModels.AddSpacesToMessage("DR:" + dr.ToString(), MessageFormattingModels.DetermineSpacing(8, ("DR:" + dr).Length));
                        string user = cty.User;
                        if (user == "" || user == null)
                            user = "Unknown";
                        string userMsg = MessageFormattingModels.AddSpacesToMessage("User:" + user, MessageFormattingModels.DetermineSpacing(27, ("User:" + user).Length));

                        await ReplyAsync("```" + countryName + countryNum + tag + "|" + netMsg + "|" + landMsg + "|" + drMsg + "|" + userMsg + "```");
                    }
                    else
                        await Context.Message.Author.SendMessageAsync("I was unable to locate country #" + cnum + ".");

                }
                else
                    await Context.Message.Author.SendMessageAsync("Please only provide me an actual number for the country you are looking for, do not include a # sign.");
            }
            catch (Exception c)
            {
                Program.Errors.Add(new ErrorModel(c));
                await ReplyAsync("I encountered a problem, and it was documented, please contact MadNudist.");
            }
        }

        [Command("cty.op")]
        [Summary("This will provide country Spy Op info based on the number provided.")]
        [Alias("ctyop", "countryop", "op")]
        public async Task CtyOpAsync([Summary("This will provide country Spy Op info based on the number provided.")][Remainder] string cnum)
        {
            try
            {
                //TODO
                int countryNumber = 0;
                if (int.TryParse(cnum, out countryNumber))
                {
                    var opJson = GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/spyops/bynumber/" + countryNumber);
                    var op = JsonConvert.DeserializeObject<SpyOp>(opJson);
                    if (op != null)
                        Program.Channels.SendSpyOpToChannel(new SpyOpInfo(op.json, op.type, op.uploader_api_key), Context.Channel);
                    else
                        await Context.Channel.SendMessageAsync("I could not locate an Spy Op for that Country number.");
                }
                else
                    await Context.Message.Author.SendMessageAsync("Please only provide me an actual number for the country you are looking for, do not include a # sign.");
            }
            catch (Exception c)
            {
                Program.Errors.Add(new ErrorModel(c));
                await ReplyAsync("I encountered a problem, and it was documented, please contact MadNudist.");
            }
        }
        
        [Command("online")]
        [Summary("Will provide online data for either a tag or country number provided.")]
        [Alias("whoonline", "cty.online")]
        public async Task OnlineAsync([Summary("Will provide online data for either a tag or country number provided.")][Remainder] string input)
        {
            
        }

        [Command("insult")]
        [Summary("Bot will insult the input provided.")]
        public async Task SayAsync([Summary("Bot will insult the input provided.")] [Remainder] string user)
        {
            try
            {
                if (user.ToLower() == "madnudist" || user.ToLower() == "nudist" || user.ToLower() == "mad")
                    user = Context.Message.Author.Username;
                // ReplyAsync is a method on ModuleBase
                if (user.ToLower() == "madbot" || user.ToLower() == "bot")
                    await ReplyAsync("You think I am that dumb, you've just been outsmarted by a bot, boom.");
                await ReplyAsync(user + ", " + GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/insults").TrimStart('{').TrimEnd('}'));
            }
            catch (Exception c)
            {
                Program.Errors.Add(new ErrorModel(c));
                await ReplyAsync("I encountered a problem, and it was documented, please contact MadNudist.");
            }
        }

        [Command("stats.net")]
        [Summary("This command will provide a growth report of country networths and related information.")]
        [Alias("netstats", "statsnet")]
        public async Task StatsNet()
        {
            /* |                                 73 chars                                |
             * |__________________________________24 Hrs_________________________________|
             * |   Username   |   SS    |    PS   |   Land+/-  |    GA+     |   Net+/-   |
             * |   14 chars   | 9 chars | 9 chars |  12 chars  |  12 chars  |  12 chars  |
             * |__________________________________48 Hrs_________________________________|
             * |   Username   |   SS    |    PS   |   Land+/-  |    GA+     |   Net+/-   |
             * |__________________________________72 Hrs_________________________________|
             * |   Username   |   SS    |    PS   |   Land+/-  |    GA+     |   Net+/-   |
             */
            string message = "```", tempMessage = "";

            // Get list of Users based on who has claimed countries
            var userList = JsonConvert.DeserializeObject<List<string>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/GetClaimedCountryUserList"));

            var news = JsonConvert.DeserializeObject<List<Event>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/news/ByDate/" + DateTime.UtcNow.AddDays(-3).ToShortDateString().Replace('/','-') + "/" + DateTime.UtcNow.ToShortDateString().Replace('/', '-')));

            var CurrentRankings = JsonConvert.DeserializeObject<List<Country>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/currentRanks"));

            // Get ranking data 24 hrs ago and build netstat object
            var netstat_24hrs_ago = new netStatModel(
                userList, 
                CurrentRankings,
                JsonConvert.DeserializeObject<List<Country>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/ByDate/" + DateTime.UtcNow.AddDays(-1).ToShortDateString().Replace('/', '-'))),
                news.Where(c => c.timestamp > DateTime.UtcNow.AddDays(-1)).ToList());

            // Build 24 hour message lines
            message += "|__________________________________24 Hrs_________________________________|" + Environment.NewLine;
            message += "|   Username   |   SS    |    PS   |   Land+/-  |    GA+     |   Net+/-   |" + Environment.NewLine;
            foreach (var userStat in netstat_24hrs_ago.UserStats)
            {
                tempMessage = "";
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.userName, MessageFormattingModels.DetermineSpacing(14, userStat.userName.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.SS, MessageFormattingModels.DetermineSpacing(9, userStat.SS.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.PS, MessageFormattingModels.DetermineSpacing(9, userStat.PS.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.LandChanges, MessageFormattingModels.DetermineSpacing(12, userStat.LandChanges.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.GhostAcreGains, MessageFormattingModels.DetermineSpacing(12, userStat.GhostAcreGains.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.NetworthChanges, MessageFormattingModels.DetermineSpacing(12, userStat.NetworthChanges.Length)) + "|";
                tempMessage += Environment.NewLine;
                if ((message.Length + tempMessage.Length) < 2000)
                    message += tempMessage;
                else
                {
                    message += "```";
                    await ReplyAsync(message);
                    message = "```" + tempMessage;
                }
            }
            // Get Ranking data 48 hrs ago and build netstat object
            var netstat_48hrs_ago = new netStatModel(
                userList,
                CurrentRankings,
                JsonConvert.DeserializeObject<List<Country>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/ByDate/" + DateTime.UtcNow.AddDays(-2).ToShortDateString().Replace('/', '-'))),
                news.Where(c => c.timestamp > DateTime.UtcNow.AddDays(-2)).ToList());

            // build 48 hour message lines
            message += Environment.NewLine;
            message += "|__________________________________48 Hrs_________________________________|" + Environment.NewLine;
            message += "|   Username   |   SS    |    PS   |   Land+/-  |    GA+     |   Net+/-   |" + Environment.NewLine;
            foreach (var userStat in netstat_48hrs_ago.UserStats)
            {
                tempMessage = "";
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.userName, MessageFormattingModels.DetermineSpacing(14, userStat.userName.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.SS, MessageFormattingModels.DetermineSpacing(9, userStat.SS.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.PS, MessageFormattingModels.DetermineSpacing(9, userStat.PS.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.LandChanges, MessageFormattingModels.DetermineSpacing(12, userStat.LandChanges.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.GhostAcreGains, MessageFormattingModels.DetermineSpacing(12, userStat.GhostAcreGains.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.NetworthChanges, MessageFormattingModels.DetermineSpacing(12, userStat.NetworthChanges.Length)) + "|";
                tempMessage += Environment.NewLine;
                if ((message.Length + tempMessage.Length) < 2000)
                    message += tempMessage;
                else
                {
                    message += "```";
                    await ReplyAsync(message);
                    message = "```" + tempMessage;
                }
            }

            // Get Ranking data 72 hours ago and build netstat object
            var netstat_72hrs_ago = new netStatModel(
                userList,
                CurrentRankings,
                JsonConvert.DeserializeObject<List<Country>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/ByDate/" + DateTime.UtcNow.AddDays(-3).ToShortDateString().Replace('/', '-'))),
                news);

            // build 72 hour lines
            message += Environment.NewLine;
            message += "|__________________________________72 Hrs_________________________________|" + Environment.NewLine;
            message += "|   Username   |   SS    |    PS   |   Land+/-  |    GA+     |   Net+/-   |" + Environment.NewLine;
            foreach (var userStat in netstat_72hrs_ago.UserStats)
            {
                tempMessage = "";
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.userName, MessageFormattingModels.DetermineSpacing(14, userStat.userName.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.SS, MessageFormattingModels.DetermineSpacing(9, userStat.SS.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.PS, MessageFormattingModels.DetermineSpacing(9, userStat.PS.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.LandChanges, MessageFormattingModels.DetermineSpacing(12, userStat.LandChanges.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.GhostAcreGains, MessageFormattingModels.DetermineSpacing(12, userStat.GhostAcreGains.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.NetworthChanges, MessageFormattingModels.DetermineSpacing(12, userStat.NetworthChanges.Length)) + "|";
                tempMessage += Environment.NewLine;
                if ((message.Length + tempMessage.Length) < 2000)
                    message += tempMessage;
                else
                {
                    message += "```";
                    await ReplyAsync(message);
                    message = "```" + tempMessage;
                }
            }
            if (message.Length > 3)
            {
                message += "```";
                await ReplyAsync(message);
            }
        }

        [Command("stats.war")]
        [Summary("This command will provide war stats for users.")]
        [Alias("warstats", "statswar")]
        public async Task WarStats()
        {
            string message = "```", tempMessage = "";
            //|                                     84 chars                                       |
            //|_______________________________________24 Hrs_______________________________________|
            //|   Username   |  GS  |  BR  |  AB  | Missiles |  CM  |  NM  | Kills | Deaths | Defs |
            //|    14 chars  |6 char|6 char|6 char| 10 chars |6 char|6 char|7 chars| 8 chars|6 char|

            // get list of users
            var userList = JsonConvert.DeserializeObject<List<string>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/GetClaimedCountryUserList"));

            // Get news data 24 hrs ago and build warstat object
            var news = JsonConvert.DeserializeObject<List<Event>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/news/ByDate/" + DateTime.UtcNow.AddDays(-3).ToShortDateString().Replace('/', '-') + "/" + DateTime.UtcNow.ToShortDateString().Replace('/', '-')));

            var CurrentRankings = JsonConvert.DeserializeObject<List<Country>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/countries/currentRanks"));

            var warstat_24hrs_ago = new warStatModel(userList, news.Where(c => c.timestamp > DateTime.UtcNow.AddDays(-1)).ToList(), CurrentRankings);

            //Build 24 hour stat message
            message += "|_______________________________________24 Hrs_______________________________________|" + Environment.NewLine;
            message += "|   Username   |  GS  |  BR  |  AB  | Missiles |  CM  |  NM  | Kills | Deaths | Defs |" + Environment.NewLine;
            foreach (var userStat in warstat_24hrs_ago.WarUserStats)
            {
                tempMessage = "";
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.userName, MessageFormattingModels.DetermineSpacing(14, userStat.userName.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.GS, MessageFormattingModels.DetermineSpacing(6, userStat.GS.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.BR, MessageFormattingModels.DetermineSpacing(6, userStat.BR.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.AB, MessageFormattingModels.DetermineSpacing(6, userStat.AB.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Missiles, MessageFormattingModels.DetermineSpacing(10, userStat.Missiles.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.CM, MessageFormattingModels.DetermineSpacing(6, userStat.CM.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.NM, MessageFormattingModels.DetermineSpacing(6, userStat.NM.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Kills, MessageFormattingModels.DetermineSpacing(7, userStat.Kills.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Deaths, MessageFormattingModels.DetermineSpacing(8, userStat.Deaths.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Defends, MessageFormattingModels.DetermineSpacing(6, userStat.Defends.Length)) + "|";
                tempMessage += Environment.NewLine;
                if ((message.Length + tempMessage.Length) < 2000)
                    message += tempMessage;
                else
                {
                    message += "```";
                    await ReplyAsync(message);
                    message = "```" + tempMessage;
                }
            }

            var warstat_48hrs_ago = new warStatModel(userList, news.Where(c => c.timestamp > DateTime.UtcNow.AddDays(-2)).ToList(), CurrentRankings);

            //Build 48 hour stat message
            message += "|_______________________________________48 Hrs_______________________________________|" + Environment.NewLine;
            message += "|   Username   |  GS  |  BR  |  AB  | Missiles |  CM  |  NM  | Kills | Deaths | Defs |" + Environment.NewLine;
            foreach (var userStat in warstat_48hrs_ago.WarUserStats)
            {
                tempMessage = "";
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.userName, MessageFormattingModels.DetermineSpacing(14, userStat.userName.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.GS, MessageFormattingModels.DetermineSpacing(6, userStat.GS.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.BR, MessageFormattingModels.DetermineSpacing(6, userStat.BR.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.AB, MessageFormattingModels.DetermineSpacing(6, userStat.AB.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Missiles, MessageFormattingModels.DetermineSpacing(10, userStat.Missiles.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.CM, MessageFormattingModels.DetermineSpacing(6, userStat.CM.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.NM, MessageFormattingModels.DetermineSpacing(6, userStat.NM.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Kills, MessageFormattingModels.DetermineSpacing(7, userStat.Kills.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Deaths, MessageFormattingModels.DetermineSpacing(8, userStat.Deaths.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Defends, MessageFormattingModels.DetermineSpacing(6, userStat.Defends.Length)) + "|";
                tempMessage += Environment.NewLine;
                if ((message.Length + tempMessage.Length) < 2000)
                    message += tempMessage;
                else
                {
                    message += "```";
                    await ReplyAsync(message);
                    message = "```" + tempMessage;
                }
            }

            var warstat_72hrs_ago = new warStatModel(userList, news, CurrentRankings);

            //Build 48 hour stat message
            message += "|_______________________________________72 Hrs_______________________________________|" + Environment.NewLine;
            message += "|   Username   |  GS  |  BR  |  AB  | Missiles |  CM  |  NM  | Kills | Deaths | Defs |" + Environment.NewLine;
            foreach (var userStat in warstat_72hrs_ago.WarUserStats)
            {
                tempMessage = "";
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.userName, MessageFormattingModels.DetermineSpacing(14, userStat.userName.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.GS, MessageFormattingModels.DetermineSpacing(6, userStat.GS.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.BR, MessageFormattingModels.DetermineSpacing(6, userStat.BR.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.AB, MessageFormattingModels.DetermineSpacing(6, userStat.AB.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Missiles, MessageFormattingModels.DetermineSpacing(10, userStat.Missiles.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.CM, MessageFormattingModels.DetermineSpacing(6, userStat.CM.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.NM, MessageFormattingModels.DetermineSpacing(6, userStat.NM.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Kills, MessageFormattingModels.DetermineSpacing(7, userStat.Kills.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Deaths, MessageFormattingModels.DetermineSpacing(8, userStat.Deaths.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(userStat.Defends, MessageFormattingModels.DetermineSpacing(6, userStat.Defends.Length)) + "|";
                tempMessage += Environment.NewLine;
                if ((message.Length + tempMessage.Length) < 2000)
                    message += tempMessage;
                else
                {
                    message += "```";
                    await ReplyAsync(message);
                    message = "```" + tempMessage;
                }
            }
            if (message.Length > 3)
            {
                message += "```";
                await ReplyAsync(message);
            }
        }

        [Command("stats.market")]
        [Summary("This command will provide market stats for users.")]
        [Alias("marketstats", "statsmarket")]
        public async Task MarketStats([Summary("This command will provide market stats for users.")]string timePerioid)
        {
            string message = "```", tempMessage = "";
            List<Transaction> Transactions;
            // |                        65 chars                                 |
            // |     Prod     |  Min  |  Avg  |  Max  |  Sold  | #Trans | %Trans |
            // |   14 chars   |7 chars|7 chars|7 chars|8 chars | 8 chars| 8 chars|

            if (timePerioid.Contains('h'))
            {
                Transactions = JsonConvert.DeserializeObject<List<Transaction>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/market/AllGoodsByDate/" + DateTime.UtcNow.AddHours(-int.Parse(timePerioid.ToLower().TrimEnd('h'))).ToShortDateString() + "/" + DateTime.UtcNow.ToShortDateString()));
                //Transactions = Program.storage.GetMarketTransactions(DateTime.UtcNow.AddHours(-int.Parse(timePerioid.ToLower().TrimEnd('h'))), DateTime.UtcNow);
                //         "|                 Start: 20:00 GMT - To: 23:50 GMT                |
                string temp = "Start: " + DateTime.UtcNow.AddHours(-int.Parse(timePerioid.ToLower().TrimEnd('h'))).ToShortTimeString() + " GMT - To: " + DateTime.UtcNow.ToShortTimeString() + " GMT";
                message += "|" + MessageFormattingModels.AddSpacesToMessage(temp, MessageFormattingModels.DetermineSpacing(65, temp.Length)) + "|" + Environment.NewLine;
                //message += "|                 Start: "+ DateTime.UtcNow.AddHours(-int.Parse(timePerioid.ToLower().TrimEnd('h'))).ToShortTimeString() + " GMT - To: "+ DateTime.UtcNow.ToShortTimeString() + " GMT                |";
                message += "|     Prod     |  Min  |  Avg  |  Max  |  Sold  | #Trans | %Trans |" + Environment.NewLine;
            }
            else if (timePerioid.Contains('d'))
            {
                Transactions = JsonConvert.DeserializeObject<List<Transaction>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/market/AllGoodsByDate/" + DateTime.UtcNow.AddDays(-int.Parse(timePerioid.ToLower().TrimEnd('d'))).ToShortDateString() + "/" + DateTime.UtcNow.ToShortDateString()));
                //Transactions = Program.storage.GetMarketTransactions(DateTime.UtcNow.AddDays(-int.Parse(timePerioid.ToLower().TrimEnd('d'))), DateTime.UtcNow);
                //         "|               Start: 12/31/2017 - To: 12/31/2018                |
                string temp = "Start: " + DateTime.UtcNow.AddDays(-int.Parse(timePerioid.ToLower().TrimEnd('d'))).ToShortDateString() + " - To: " + DateTime.UtcNow.ToShortDateString();
                message += "|" + MessageFormattingModels.AddSpacesToMessage(temp, MessageFormattingModels.DetermineSpacing(65, temp.Length)) + "|" + Environment.NewLine;
                //message += "|               Start: "+ DateTime.UtcNow.AddDays(-int.Parse(timePerioid.ToLower().TrimEnd('d'))).ToShortDateString() + " - To: "+ DateTime.UtcNow.ToShortDateString() + "                |";
                message += "|     Prod     |  Min  |  Avg  |  Max  |  Sold  | #Trans | %Trans |" + Environment.NewLine;
            }
            else
            {
                Transactions = JsonConvert.DeserializeObject<List<Transaction>>(GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/market/AllGoodsByDate/" + DateTime.UtcNow.AddDays(-1).ToShortDateString() + "/" + DateTime.UtcNow.ToShortDateString()));
                //Transactions = Program.storage.GetMarketTransactions(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);
                string temp = "Start: " + DateTime.UtcNow.AddDays(-1).ToShortDateString() + " - To: " + DateTime.UtcNow.ToShortDateString();
                message += "|" + MessageFormattingModels.AddSpacesToMessage(temp, MessageFormattingModels.DetermineSpacing(65, temp.Length)) + "|" + Environment.NewLine;
                //message += "|               Start: " + DateTime.UtcNow.AddDays(-1).ToShortDateString() + " - To: " + DateTime.UtcNow.ToShortDateString() + "                |";
                message += "|     Prod     |  Min  |  Avg  |  Max  |  Sold  | #Trans | %Trans |" + Environment.NewLine;
            }

            var marketStats = new marketStatModel(Transactions);

            foreach (var productStat in marketStats.ProductStats)
            {
                tempMessage = "";
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(productStat.Good.ToString(), MessageFormattingModels.DetermineSpacing(14, productStat.Good.ToString().Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(productStat.Min, MessageFormattingModels.DetermineSpacing(7, productStat.Min.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(productStat.Avg, MessageFormattingModels.DetermineSpacing(7, productStat.Avg.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(productStat.Max, MessageFormattingModels.DetermineSpacing(7, productStat.Max.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(productStat.Sold, MessageFormattingModels.DetermineSpacing(8, productStat.Sold.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(productStat.NumTrans, MessageFormattingModels.DetermineSpacing(8, productStat.NumTrans.Length));
                tempMessage += "|" + MessageFormattingModels.AddSpacesToMessage(productStat.PercTrans, MessageFormattingModels.DetermineSpacing(8, productStat.PercTrans.Length)) + "|";
                tempMessage += Environment.NewLine;
                if ((message.Length + tempMessage.Length) < 2000)
                    message += tempMessage;
                else
                {
                    message += "```";
                    await ReplyAsync(message);
                    message = "```" + tempMessage;
                }
            }

            if (message.Length > 3)
            {
                message += "```";
                await ReplyAsync(message);
            }
        }

        public static string PostToAPI(string url, string json)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static string GetFromAPI(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static int DetermineDR(int cnum)
        {
            int dr = 0;
            // get news data for the last 24 hours where this country was either the attacker or defender
            var allNewsJson = GetFromAPI("http://loc-ee.us-east-1.elasticbeanstalk.com/api/news/bynumber/" + cnum + "/" + DateTime.UtcNow.AddDays(-1).ToShortDateString().Replace('/', '-') + "/" + DateTime.UtcNow.ToShortDateString().Replace('/', '-'));
            var allNews = JsonConvert.DeserializeObject<List<Event>>(allNewsJson);
            //calc dr
            dr = allNews.Where(c => c.defender_num == cnum).Count() - allNews.Where(c => c.defender_num == cnum).Count();
            if (dr < 0)
                dr = 0;
            return dr;
        }
    }
}
