using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Xml.Serialization;
using LoCWebApp.Controllers;

namespace LoCWebApp.Models
{
    public class StorageModel
    {
        public int Reset { get; set; }

        public List<Transaction> MarketTransactions { get; set; }
        public List<MarketTransFileStorageModel> MarketFiles { get; set; }
        private BackgroundWorker MarketTransactionsUpdateService;
        private System.Timers.Timer MarketTransactionsUpdateServiceTimer;

        public List<Event> Events { get; set; }
        public List<NewsFileStorageModel> NewsFiles { get; set; }
        private BackgroundWorker NewsUpdateService;
        private System.Timers.Timer NewsUpdateServiceTimer;

        public List<Country> Ranks { get; set; }
        public CountryStorageModel RanksStorage { get; set; }
        public List<CountryFileStorageModel> CountryFiles { get; set; }
        private BackgroundWorker RankingUpdateService;
        private System.Timers.Timer RankingUpdateServiceTimer;
        private DateTime RanksLastSaved { get; set; }
        public List<ClaimCountryModel> RecentlyClaimedCountries { get; set; }

        public List<TagChange> TagChanges { get; set; }
        public List<TagChangeFileStorageModel> TagChangeFiles { get; set; }
        public DateTime lastTagChangeSave { get; set; }

        public List<OnlineCountryDetectionFileStorageModel> LoginFiles { get; set; }
        public List<loginDetectionModel> Logins { get; set; }
        public List<loginDetectionModel> LoginsToSave { get; set; }
        public DateTime lastLoginsSave { get; set; }

        public List<ErrorModel> ErrorsToSave { get; set; }
        public List<BaseFileStorageModel> ErrorFiles { get; set; }

        public List<Relation> Relations { get; set; }
        public bool newlyAddedRelations { get; set; }

        public InsultStorageModel InsultsStorage { get; set; }
        public bool newlyAddedInsults { get; set; }

        public AnnouncementStorage Announcements { get; set; }

        public List<SpyOp> SpyOpStorage { get; set; }
        public List<SpyOp> SpyOpSavingQueue { get; set; }
        public List<SpyOpsFileStorageModel> SpyOpFileStorage { get; set; }
        private BackgroundWorker NewOpDetectionService;
        private System.Timers.Timer NewOpDetectionServiceTimer;

        private BackgroundWorker StorageSavingService;
        private System.Timers.Timer StorageSavingServiceTimer;

        public StorageModel()
        {
            string[] files;

            try
            {
                using (NewsUpdateService = new BackgroundWorker())
                {
                    NewsUpdateService.DoWork += ReceiveNews;
                    NewsUpdateServiceTimer = new System.Timers.Timer(new TimeSpan(0, 0, 4).TotalMilliseconds);
                    NewsUpdateServiceTimer.Elapsed += CheckNewsUpdateProcess;
                    NewsUpdateServiceTimer.Start();
                }
                using (StorageSavingService = new BackgroundWorker())
                {
                    StorageSavingService.DoWork += SaveStorageService;
                    StorageSavingServiceTimer = new System.Timers.Timer(new TimeSpan(0, 0, 50).TotalMilliseconds);
                    StorageSavingServiceTimer.Elapsed += CheckStorageSavingService;
                    StorageSavingServiceTimer.Start();
                }
                using (RankingUpdateService = new BackgroundWorker())
                {
                    RankingUpdateService.DoWork += ReceiveCountryData;
                    RankingUpdateServiceTimer = new System.Timers.Timer(new TimeSpan(0, 5, 0).TotalMilliseconds);
                    RankingUpdateServiceTimer.Elapsed += CheckRanksUpdateProcess;
                    RankingUpdateServiceTimer.Start();
                }
                using (MarketTransactionsUpdateService = new BackgroundWorker())
                {
                    MarketTransactionsUpdateService.DoWork += ReceiveMarketTransactions;
                    MarketTransactionsUpdateServiceTimer = new System.Timers.Timer(new TimeSpan(0, 0, 5).TotalMilliseconds);
                    MarketTransactionsUpdateServiceTimer.Elapsed += CheckMarketUpdateProcess;
                    MarketTransactionsUpdateServiceTimer.Start();
                }
                using (NewOpDetectionService = new BackgroundWorker())
                {
                    NewOpDetectionService.DoWork += CheckForNewOps;
                    NewOpDetectionServiceTimer = new System.Timers.Timer(new TimeSpan(0, 0, 0, 30).TotalMilliseconds);
                    NewOpDetectionServiceTimer.Elapsed += CheckCheckForNewOpsProcess;
                    NewOpDetectionServiceTimer.Start();
                }
                //Initialize, will update when country files are loaded or first update occurs
                Reset = 1514;

                #region Initialize Error Log and Load ErrorFiles
                ErrorsToSave = new List<ErrorModel>();
                ErrorFiles = new List<BaseFileStorageModel>();
                XmlSerializer xs = new XmlSerializer(typeof(List<BaseFileStorageModel>));
                if (CheckFileSystem(StorageOptions.Errors))
                {
                    files = Directory.GetFiles(@"C:\WebData\Errors\StorageFiles");
                    if (files.Count() > 0)
                    {
                        using (var sr = new StreamReader(files[0]))
                        {
                            ErrorFiles = (List<BaseFileStorageModel>)xs.Deserialize(sr);
                        }
                        ErrorFiles = ErrorFiles.OrderByDescending(c => c.End).ToList();
                    }
                }
                #endregion

                #region Load NewsFiles and Recent Events
                NewsFiles = new List<NewsFileStorageModel>();
                xs = new XmlSerializer(typeof(List<NewsFileStorageModel>));
                if (CheckFileSystem(StorageOptions.News))
                {
                    if (Directory.GetDirectories(@"C:\WebData\News").Count() > 0)
                    {
                        files = Directory.GetFiles(@"C:\WebData\News\" + Reset + @"\NewsFiles");
                        if (files.Count() > 0)
                        {
                            using (var sr = new StreamReader(files[0]))
                            {
                                NewsFiles = (List<NewsFileStorageModel>)xs.Deserialize(sr);
                            }

                            #region Load Events for each NewsFile
                            NewsFiles = NewsFiles.OrderByDescending(c => c.Start).ToList();

                            if (NewsFiles.Count() > 0)
                            {
                                if (NewsFiles[0].End > DateTime.UtcNow.AddDays(-3))
                                {
                                    Events = new Models.NewsStorageModel(Directory.GetFiles(@"C:\WebData\News\" + Reset + @"\NewsStorage").Single(c => c.Contains(NewsFiles[0].fileId.ToString()))).Events;
                                    Events.RemoveAll(c => c.timestamp < DateTime.UtcNow.AddDays(-3));
                                }
                                else
                                    Events = new List<Event>();
                            }
                            else
                                Events = new List<Event>();
                        }
                        else
                            Events = new List<Event>();
                        #endregion
                    }
                    else
                    {
                        NewsFiles = new List<NewsFileStorageModel>();
                        Events = new List<Event>();
                    }
                }
                #endregion

                #region Initialize Tag Change Storage & Load Tag Change Files
                TagChanges = new List<TagChange>();
                TagChangeFiles = new List<TagChangeFileStorageModel>();
                xs = new XmlSerializer(typeof(List<TagChangeFileStorageModel>));
                if (CheckFileSystem(StorageOptions.TagChanges))
                {
                    files = Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\TagChanges\TagChangeFiles");
                    if (files.Count() > 0)
                    {
                        using (var sr = new StreamReader(files[0]))
                        {
                            TagChangeFiles = (List<TagChangeFileStorageModel>)xs.Deserialize(sr);
                        }
                    }
                }
                #endregion

                #region Load LoginFiles and Logins Storage
                LoginFiles = new List<OnlineCountryDetectionFileStorageModel>();
                Logins = new List<loginDetectionModel>();
                LoginsToSave = new List<loginDetectionModel>();
                lastLoginsSave = DateTime.UtcNow;
                xs = new XmlSerializer(typeof(List<OnlineCountryDetectionFileStorageModel>));
                if (CheckFileSystem(StorageOptions.LoginDetections))
                {
                    files = Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\LoginDetections\LoginDetectionFiles");
                    if (files.Count() > 0)
                    {
                        using (var sr = new StreamReader(files[0]))
                        {
                            LoginFiles = (List<OnlineCountryDetectionFileStorageModel>)xs.Deserialize(sr);
                        }
                        LoginFiles = LoginFiles.OrderByDescending(c => c.End).ToList();
                        lastLoginsSave = LoginFiles.Last().End;
                    }
                    files = Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\LoginDetections");
                    if (files.Count() > 0)
                    {
                        Logins = new OnlineCountryDetectionStorageModel(files[0]).LoginDetections;
                    }
                }
                #endregion

                #region Load Country Files and Ranks
                RecentlyClaimedCountries = new List<ClaimCountryModel>();
                RanksStorage = new CountryStorageModel();
                CountryFiles = new List<CountryFileStorageModel>();
                xs = new XmlSerializer(typeof(List<CountryFileStorageModel>));
                if (CheckFileSystem(StorageOptions.Countries))
                {
                    files = Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\CountryFiles");
                    if (files.Count() > 0)
                    {
                        using (var sr = new StreamReader(files[0]))
                        {
                            CountryFiles = (List<CountryFileStorageModel>)xs.Deserialize(sr);
                            if(CountryFiles.Count() > 0)
                                Reset = CountryFiles.First().curReset;
                        }

                        CountryFiles = CountryFiles.OrderByDescending(c => c.End).ToList();

                        //RanksStorage = new CountryStorageModel(Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\CountryStorage").Single(c => c.Contains(CountryFiles[0].fileId.ToString())));
                        //RanksStorage.Ranks = RanksStorage.Ranks.OrderBy(c => c.Number).ToList();
                        Ranks = new List<Country>();
                        Ranks.AddRange(new CountryStorageModel(Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\CountryStorage").Single(c => c.Contains(CountryFiles[0].fileId.ToString()))).Ranks);

                        RanksLastSaved = CountryFiles.First().End;                        
                    }
                    else
                    {
                        Ranks = new List<Country>();
                        RanksStorage.Ranks = new List<Country>();
                        ReceiveCountryData(new object(), new DoWorkEventArgs(new object()));
                        SaveFileToXML(RanksStorage);
                        RanksStorage.Ranks.Clear();
                        RanksStorage.Ranks = new List<Country>();
                        SaveFileStorage(StorageOptions.Countries);
                    }
                }
                #endregion

                #region Load Market Files
                MarketTransactions = new List<Transaction>();
                MarketFiles = new List<MarketTransFileStorageModel>();
                xs = new XmlSerializer(typeof(List<MarketTransFileStorageModel>));
                if (CheckFileSystem(StorageOptions.Market))
                {
                    files = Directory.GetFiles(@"C:\WebData\Market\" + Reset + @"\MarketTransactionFiles");
                    if (files.Count() > 0)
                    {
                        using (var sr = new StreamReader(files[0]))
                        {
                            MarketFiles = (List<MarketTransFileStorageModel>)xs.Deserialize(sr);
                        }
                    }
                }
                #endregion

                #region Load Relations
                newlyAddedRelations = false;
                Relations = new List<Relation>();
                xs = new XmlSerializer(typeof(RelationStorageModel));
                if (CheckFileSystem(StorageOptions.Relations))
                {
                    files = Directory.GetFiles(@"C:\WebData\Relations\" + Reset);
                    if (files.Count() > 0)
                    {
                        using (var sr = new StreamReader(files[0]))
                        {
                            Relations = ((RelationStorageModel)xs.Deserialize(sr)).Relations;
                        }
                    }
                }
                #endregion

                #region Load Insults
                InsultsStorage = new InsultStorageModel();
                newlyAddedInsults = false;
                if (CheckFileSystem(StorageOptions.Insults))
                {
                    files = Directory.GetFiles(@"C:\WebData\Insults");
                    if (files.Count() > 0)
                    {
                        InsultsStorage = new InsultStorageModel(files[0]);
                    }
                }
                #endregion

                #region Load Announcements
                Announcements = new AnnouncementStorage();
                if (CheckFileSystem(StorageOptions.Announcements))
                {
                    files = Directory.GetFiles(@"C:\WebData\Announcements");
                    if (files.Count() > 0)
                    {
                        Announcements = new AnnouncementStorage(files[0]);
                    }
                }
                #endregion

                #region Load SpyOps
                SpyOpStorage = new List<SpyOp>();
                SpyOpSavingQueue = new List<SpyOp>();
                SpyOpFileStorage = new List<SpyOpsFileStorageModel>();
                xs = new XmlSerializer(typeof(List<SpyOp>));
                if (CheckFileSystem(StorageOptions.Ops))
                {
                    files = Directory.GetFiles(@"C:\WebData\Spyops\12\" + Reset + @"\CurrentStorage");
                    if (files.Count() > 0)
                    {
                        using (var sr = new StreamReader(files[0]))
                        {
                            SpyOpStorage = (List<SpyOp>)xs.Deserialize(sr);
                        }
                    }
                    xs = new XmlSerializer(typeof(List<SpyOpsFileStorageModel>));
                    files = Directory.GetFiles(@"C:\WebData\Spyops\12\" + Reset + @"\SpyOpFiles");
                    if (files.Count() > 0)
                    {
                        using (var sr = new StreamReader(files[0]))
                        {
                            SpyOpFileStorage = (List<SpyOpsFileStorageModel>)xs.Deserialize(sr);
                        }
                    }
                }
                #endregion
            }
            catch(Exception c)
            {
                new ErrorModel(c).SaveErrorToXML();
            }
        }

        public bool CheckFileSystem(StorageOptions systemToCheck)
        {
            try
            {
                if (!Directory.Exists(@"C:\WebData"))
                {
                    Directory.CreateDirectory(@"C:\WebData");
                }
                switch (systemToCheck)
                {
                    case StorageOptions.News:
                        #region News 
                        if (!Directory.Exists(@"C:\WebData\News"))
                            Directory.CreateDirectory(@"C:\WebData\News");
                        if (!Directory.Exists(@"C:\WebData\News\" + Reset))
                        {
                            Directory.CreateDirectory(@"C:\WebData\News\" + Reset);
                            Directory.CreateDirectory(@"C:\WebData\News\" + Reset + @"\NewsFiles");
                            Directory.CreateDirectory(@"C:\WebData\News\" + Reset + @"\NewsFiles\Deleted");
                            Directory.CreateDirectory(@"C:\WebData\News\" + Reset + @"\NewsStorage");
                        }
                        #endregion
                        break;
                    case StorageOptions.Errors:
                        #region Errors
                        if (!Directory.Exists(@"C:\WebData\Errors"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Errors");
                            Directory.CreateDirectory(@"C:\WebData\Errors\StartupErrors");
                            Directory.CreateDirectory(@"C:\WebData\Errors\StorageFiles");
                            Directory.CreateDirectory(@"C:\WebData\Errors\StorageFiles\Deleted");
                            Directory.CreateDirectory(@"C:\WebData\Errors\ErrorStorage");
                        }
                        #endregion
                        break;
                    case StorageOptions.Market:
                        #region Market

                        if (!Directory.Exists(@"C:\WebData\Market"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Market");
                        }
                        if (!Directory.Exists(@"C:\WebData\Market\" + Reset))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Market\" + Reset);
                            Directory.CreateDirectory(@"C:\WebData\Market\" + Reset + @"\MarketTransactionFiles");
                            Directory.CreateDirectory(@"C:\WebData\Market\" + Reset + @"\MarketTransactionFiles\Deleted");
                            Directory.CreateDirectory(@"C:\WebData\Market\" + Reset + @"\MarketTransactionStorage");
                        }
                        #endregion
                        break;
                    case StorageOptions.Countries:
                        #region Countries
                        if (!Directory.Exists(@"C:\WebData\Countries"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Countries");
                        }
                        if (!Directory.Exists(@"C:\WebData\Countries\" + Reset))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset);
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\CountryFiles");
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\CountryFiles\Deleted");
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\CountryStorage");
                        }
                        #endregion
                        break;
                    case StorageOptions.TagChanges:
                        #region TagChanges
                        CheckFileSystem(StorageOptions.Countries);
                        if (!Directory.Exists(@"C:\WebData\Countries\" + Reset + @"\TagChanges"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\TagChanges");
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\TagChanges\TagChangeFiles");
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\TagChanges\TagChangeFiles\Deleted");
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\TagChanges\TagChangeStorage");
                        }
                        #endregion
                        break;
                    case StorageOptions.LoginDetections:
                        #region Login Detections
                        CheckFileSystem(StorageOptions.Countries);
                        if (!Directory.Exists(@"C:\WebData\Countries\" + Reset + @"\LoginDetections"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\LoginDetections");
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\LoginDetections\Deleted");
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\LoginDetections\LoginDetectionFiles");
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\LoginDetections\LoginDetectionFiles\Deleted");
                            Directory.CreateDirectory(@"C:\WebData\Countries\" + Reset + @"\LoginDetections\LoginDetectionStorage");
                        }
                        #endregion
                        break;
                    case StorageOptions.Insults:
                        #region Insults
                        if (!Directory.Exists(@"C:\WebData\Insults"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Insults");
                            Directory.CreateDirectory(@"C:\WebData\Insults\Deleted");
                        }
                        #endregion
                        break;
                    case StorageOptions.Ops:
                        #region Ops
                        if(!Directory.Exists(@"C:\WebData\newSpyOps"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\newSpyOps");
                            Directory.CreateDirectory(@"C:\WebData\newSpyOps\Storage");
                        }
                        if (!Directory.Exists(@"C:\WebData\Spyops"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Spyops");
                            Directory.CreateDirectory(@"C:\WebData\Spyops\12");    
                        }
                        if(!Directory.Exists(@"C:\WebData\Spyops\12\" + Reset))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Spyops\12\" + Reset);
                            Directory.CreateDirectory(@"C:\WebData\Spyops\12\" + Reset + @"\SpyOpFiles");
                            Directory.CreateDirectory(@"C:\WebData\Spyops\12\" + Reset + @"\SpyOpFiles\Deleted");
                            Directory.CreateDirectory(@"C:\WebData\Spyops\12\" + Reset + @"\CurrentStorage");
                            Directory.CreateDirectory(@"C:\WebData\Spyops\12\" + Reset + @"\CurrentStorage\Deleted");
                        }
                        #endregion
                        break;
                    case StorageOptions.Users:
                        #region Users
                        if (!Directory.Exists(@"C:\WebData\Users"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Users");
                            Directory.CreateDirectory(@"C:\WebData\Users\Deleted");
                        }
                        #endregion
                        break;
                    case StorageOptions.Relations:
                        #region Relations
                        if (!Directory.Exists(@"C:\WebData\Relations"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Relations");
                        }
                        if (!Directory.Exists(@"C:\WebData\Relations\" + Reset))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Relations\" + Reset);
                            Directory.CreateDirectory(@"C:\WebData\Relations\" + Reset + @"\Deleted");
                        }
                        #endregion
                        break;
                    case StorageOptions.Announcements:
                        #region Users
                        if (!Directory.Exists(@"C:\WebData\Announcements"))
                        {
                            Directory.CreateDirectory(@"C:\WebData\Announcements");
                            Directory.CreateDirectory(@"C:\WebData\Announcements\Deleted");
                        }
                        #endregion
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Storage Saving Service and Timer
        public void SaveFileStorage(StorageOptions filesToSave)
        {
            try
            {
                string[] files;
                XmlSerializer xs;
                switch (filesToSave)
                {
                    case StorageOptions.News:
                        #region Save News Files
                        if (CheckFileSystem(StorageOptions.News))
                        {
                            //check for previous file
                            files = Directory.GetFiles(@"C:\WebData\News\" + Reset + @"\NewsFiles");
                            if (files.Count() > 0)
                            {
                                File.Move(files[0], @"C:\WebData\News\" + Reset + @"\NewsFiles\Deleted\" + files[0].Split('\\')[5]);
                            }

                            xs = new XmlSerializer(typeof(List<NewsFileStorageModel>));
                            using (TextWriter tw = new StreamWriter(@"C:\WebData\News\" + Reset + @"\NewsFiles\" + Guid.NewGuid() + ".xml"))
                            {
                                xs.Serialize(tw, NewsFiles);
                                tw.Close();
                            }
                        }
                        #endregion
                        break;
                    case StorageOptions.Errors:
                        #region Save Errors and Error Files
                        if (CheckFileSystem(StorageOptions.Errors))
                        {
                            //check for previous file
                            files = Directory.GetFiles(@"C:\WebData\Errors\StorageFiles");
                            if (files.Count() > 0)
                            {
                                foreach (var file in files)
                                {
                                    File.Move(file, @"C:\WebData\Errors\StorageFiles\Deleted\" + file.Split('\\')[4]);
                                }
                            }

                            xs = new XmlSerializer(typeof(List<BaseFileStorageModel>));
                            using (TextWriter tw = new StreamWriter(@"C:\WebData\Errors\StorageFiles\" + Guid.NewGuid() + ".xml"))
                            {
                                xs.Serialize(tw, ErrorFiles);
                                tw.Close();
                            }
                        }
                        #endregion
                        break;
                    case StorageOptions.Market:
                        #region Save Market Transaction Files
                        if (CheckFileSystem(StorageOptions.Market))
                        {
                            //check for previous file
                            files = Directory.GetFiles(@"C:\WebData\Market\" + Reset + @"\MarketTransactionFiles");
                            if (files.Count() > 0)
                            {
                                File.Move(files[0], @"C:\WebData\Market\" + Reset + @"\MarketTransactionFiles\Deleted\" + files[0].Split('\\')[5]);
                            }
                            
                            xs = new XmlSerializer(typeof(List<MarketTransFileStorageModel>));
                            using (TextWriter tw = new StreamWriter(@"C:\WebData\Market\" + Reset + @"\MarketTransactionFiles\" + Guid.NewGuid() + ".xml"))
                            {
                                xs.Serialize(tw, MarketFiles);
                                tw.Close();
                            }
                        }
                        #endregion
                        break;
                    case StorageOptions.Countries:
                        #region Save Country Files
                        if (CheckFileSystem(StorageOptions.Countries))
                        {
                            //check for previous file
                            files = Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\CountryFiles");
                            if (files.Count() > 0)
                            {
                                try
                                {
                                    File.Move(files[0], @"C:\WebData\Countries\" + Reset + @"\CountryFiles\Deleted\" + files[0].Split('\\')[5]);
                                }
                                catch(IOException)
                                {
                                    File.Delete(@"C:\WebData\Countries\" + Reset + @"\CountryFiles\Deleted\" + files[0].Split('\\')[5]);
                                    File.Move(files[0], @"C:\WebData\Countries\" + Reset + @"\CountryFiles\Deleted\" + files[0].Split('\\')[5]);
                                }
                            }
                            xs = new XmlSerializer(typeof(List<CountryFileStorageModel>));
                            using (TextWriter tw = new StreamWriter(@"C:\WebData\Countries\" + Reset + @"\CountryFiles\" + Guid.NewGuid() + ".xml"))
                            {
                                xs.Serialize(tw, CountryFiles);
                                tw.Close();
                            }
                        }
                        #endregion
                        break;
                    case StorageOptions.TagChanges:
                        #region Save TagChange Files
                        if (CheckFileSystem(StorageOptions.TagChanges))
                        {
                            //check for previous file
                            files = Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\TagChanges\TagChangeFiles");
                            if (files.Count() > 0)
                            {
                                File.Move(files[0], @"C:\WebData\Countries\" + Reset + @"\TagChanges\TagChangeFiles\Deleted\" + files[0].Split('\\')[6]);
                            }
                            
                            xs = new XmlSerializer(typeof(List<TagChangeFileStorageModel>));
                            using (TextWriter tw = new StreamWriter(@"C:\WebData\Countries\" + Reset + @"\TagChanges\TagChangeFiles\" + Guid.NewGuid() + ".xml"))
                            {
                                xs.Serialize(tw, TagChangeFiles);
                                tw.Close();
                            }
                        }
                        #endregion
                        break;
                    case StorageOptions.LoginDetections:
                        #region Save Online Detection Files
                        if (CheckFileSystem(StorageOptions.LoginDetections))
                        {
                            //check for previous file
                            files = Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\LoginDetections\LoginDetectionFiles");
                            if (files.Count() > 0)
                            {
                                File.Move(files[0], @"C:\WebData\Countries\" + Reset + @"\LoginDetections\LoginDetectionFiles\Deleted\" + files[0].Split('\\')[6]);
                            }
                            
                            xs = new XmlSerializer(typeof(List<OnlineCountryDetectionFileStorageModel>));
                            using (TextWriter tw = new StreamWriter(@"C:\WebData\Countries\" + Reset + @"\LoginDetections\LoginDetectionFiles\" + Guid.NewGuid() + ".xml"))
                            {
                                xs.Serialize(tw, LoginFiles);
                                tw.Close();
                            }
                        }
                        #endregion
                        break;
                    case StorageOptions.Ops:
                        #region Save Spy Op Files
                        if (CheckFileSystem(StorageOptions.Ops))
                        {
                            //check for previous file
                            files = Directory.GetFiles(@"C:\WebData\Spyops\12\" + Reset + @"\SpyOpFiles");
                            if (files.Count() > 0)
                            {
                                var sourceFile = files[0];
                                var desintationFile = @"C:\WebData\Spyops\12\" + Reset + @"\SpyOpFiles\Deleted\" + files[0].Split('\\')[6];
                                File.Move(sourceFile, desintationFile);
                            }
                            
                            xs = new XmlSerializer(typeof(List<SpyOpsFileStorageModel>));
                            using (TextWriter tw = new StreamWriter(@"C:\WebData\Spyops\12\" + Reset + @"\SpyOpFiles\" + Guid.NewGuid() + ".xml"))
                            {
                                xs.Serialize(tw, SpyOpFileStorage);
                                tw.Close();
                            }
                        }
                        #endregion
                        break;
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public void CheckStorageSavingService(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(!StorageSavingService.IsBusy)
            {
                StorageSavingService.RunWorkerAsync();
            }
        }

        public void SaveStorageService(object sender, DoWorkEventArgs e)
        {
            #region Save News and NewsFiles
            if(CheckFileSystem(StorageOptions.News))
            {
                if (Events.Count() >= 20000)
                {
                    SaveFileToXML(new NewsStorageModel() { Events = Startup.Storage.NewsFiles.Count() == 0 ? Events : Events.Where(c => c.newsid > Startup.Storage.NewsFiles.First().lastRecordedNewsId).ToList() }, true);
                    SaveFileStorage(StorageOptions.News);
                }
                else
                {
                    SaveFileToXML(new NewsStorageModel() { Events = Startup.Storage.NewsFiles.Count() == 0 ? Events : Events.Where(c => c.newsid > Startup.Storage.NewsFiles.First().lastRecordedNewsId).ToList() }, false);
                    SaveFileStorage(StorageOptions.News);
                }
            }
            #endregion

            #region Save TagChanges
            if (CheckFileSystem(StorageOptions.TagChanges))
            {
                if (TagChanges.Count() > 1000 || (lastTagChangeSave < DateTime.UtcNow.AddDays(-1) && TagChanges.Count() > 0))
                {
                    lastTagChangeSave = DateTime.UtcNow;
                    SaveFileToXML(new TagChangeStorageModel() { TagChanges = TagChanges });
                    TagChanges.Clear();
                    TagChanges = new List<TagChange>();
                    SaveFileStorage(StorageOptions.TagChanges);
                }
            }
            #endregion

            #region Save LoginsStorage and LoginFiles
            if (CheckFileSystem(StorageOptions.LoginDetections))
            {
                if (DateTime.UtcNow.AddDays(-1) > lastLoginsSave)
                {
                    SaveFileToXML(new OnlineCountryDetectionStorageModel() { LoginDetections = LoginsToSave });
                    SaveFileStorage(StorageOptions.LoginDetections);
                    SaveFileToXML(Logins);
                    lastLoginsSave = DateTime.UtcNow;
                }
            }
            #endregion

            #region Save Country Ranks and CountryFiles
            if (CheckFileSystem(StorageOptions.Countries))
            {
               if (RanksLastSaved < DateTime.UtcNow.AddDays(-1))
                {
                    if (RanksStorage.Ranks.Count() == 0)
                    {
                        RanksStorage.Ranks = Ranks;
                    }
                    SaveFileToXML(RanksStorage);
                    RanksStorage.Ranks.Clear();
                    RanksStorage.Ranks = new List<Country>();
                    SaveFileStorage(StorageOptions.Countries);
                    RanksStorage.Ranks = Ranks; 
                    if(RecentlyClaimedCountries.Count() > 0)
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(CountryStorageModel));
                        //Load each file one at a time, modify any users' countries from this list and resave

                        foreach (var file in Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\CountryStorage"))
                        {
                            var oldRanks = new CountryStorageModel(file);
                            foreach(var userClaimedCountries in RecentlyClaimedCountries)
                            {
                                foreach(var country in userClaimedCountries.countries)
                                {
                                    oldRanks.Ranks.SingleOrDefault(c => c.Number == country).User = userClaimedCountries.user;
                                }
                            }
                            using (TextWriter tw = new StreamWriter(file))
                            {
                                xs.Serialize(tw, oldRanks);
                                tw.Close();
                            }
                        }
                        RecentlyClaimedCountries.Clear();
                        RecentlyClaimedCountries = new List<ClaimCountryModel>();
                    }                   
                }
            }
            #endregion

            #region Save Errors and ErrorFiles
            if(CheckFileSystem(StorageOptions.Errors))
            {
                if (ErrorsToSave.Count() > 0)
                {
                    SaveFileToXML(new ErrorStorageModel() { ErrorLog = ErrorsToSave });
                    SaveFileStorage(StorageOptions.Errors);
                }
            }
            #endregion

            #region Save Market Transactions and Market Files
            if (CheckFileSystem(StorageOptions.Market))
            {
                if (MarketTransactions.Count() >= 20000)
                {
                    SaveFileToXML(MarketTransactions);
                    SaveFileStorage(StorageOptions.Market);
                    MarketTransactions.Clear();
                }
            }
            #endregion

            #region Save Relations
            if (CheckFileSystem(StorageOptions.Relations))
            {
                if (newlyAddedRelations)
                {
                    SaveFileToXML(Relations);
                }
            }
            #endregion

            #region Save Insults
            if (CheckFileSystem(StorageOptions.Insults))
            {
                if (newlyAddedInsults)
                {
                    InsultsStorage.SaveInsultsToXML();
                    newlyAddedInsults = false;
                }
            }
            #endregion

            #region Save Announcements
            if (CheckFileSystem(StorageOptions.Announcements))
            {
                Announcements.SaveFileToXML();
            }
            #endregion

            #region Save SpyOps
            if(SpyOpSavingQueue.Count() > 50)
            {
                if (CheckFileSystem(StorageOptions.Ops))
                {
                    SaveFileToXML(SpyOpSavingQueue, false);
                    SpyOpSavingQueue.Clear();

                    SaveFileToXML(SpyOpStorage, true);
                }
            }
            #endregion
        }
        #endregion

        #region EE News API Data Miners
        // checks Background process to determine if it is running, if not executes ReceiveNews method
        private void CheckNewsUpdateProcess(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!NewsUpdateService.IsBusy)
            {
                NewsUpdateService.RunWorkerAsync();
            }
        }

        // makes a call to EE server obtaining news csv, parsing it and saving it to xml
        private void ReceiveNews(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Start of 1433, 2nd to last newid in 1392 = 31424089
                // Start of 1392 set = 31066010
                // Second to last news event in 1353 = 31054833
                // Start of 1474 = 31800999
                int transactionId = 31800999;

                if (Events.Count() > 0)
                    transactionId = Events.Max(c => c.newsid);
                else if (Startup.Storage.NewsFiles.Count() > 0)
                    transactionId = Startup.Storage.NewsFiles.OrderBy(c => c.End).Max(c => c.lastRecordedNewsId);

                string url = "http://www.earthempires.com/news_feed?apicode=838a8f8296991745d36d3b32f5845b6e&serverid=12&startid=" + transactionId;

                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                string download = "";
                using (var client = new HttpClient(handler))
                {
                    HttpResponseMessage response;
                    client.DefaultRequestHeaders.Add("Encoding", "gzip");
                    response = client.GetAsync(url).Result;
                    download = response.Content.ReadAsStringAsync().Result;
                }
                // PARSE CSV and Create Model to save to DB
                if (download.ToString() != "" && download.ToString() != "You already received this update" && download.ToString() != "You are being throttled for not using gzip to 1 out of 4 updates.")
                {
                    var temp = (from line in download.Split(Convert.ToChar(10))
                                let columns = line.Split(',')
                                where columns.Count().Equals(15)
                                select new Event(columns)).OrderBy(c => c.timestamp).ToList();
                    if (temp.Count() == 10000)
                    {
                        NewsUpdateServiceTimer.Interval = new TimeSpan(0, 0, 24).TotalMilliseconds;
                    }
                    else if (temp.Count() >= 5000)
                    {
                        NewsUpdateServiceTimer.Interval = new TimeSpan(0, 0, 12).TotalMilliseconds;
                    }
                    else
                    {
                        NewsUpdateServiceTimer.Interval = new TimeSpan(0, 0, 1).TotalMilliseconds;
                    }

                    if (temp.Count() > 0)
                    {
                        if (temp.First().resetid != temp.Last().resetid)
                        {
                            Events.AddRange(temp.Where(c => c.resetid == temp.First().resetid));
                            SaveFileToXML(new NewsStorageModel{ Events = Events }, true);                            
                            SaveFileStorage(StorageOptions.News);
                            Startup.Storage.NewsFiles = new List<NewsFileStorageModel>();                            
                            Events = new List<Event>();
                            Events.AddRange(temp.Where(c => c.resetid == temp.Last().resetid));
                        }
                        else
                            Events.AddRange(temp);
                    }
                    
                    Thread checkNewsThread = new Thread(() => CheckNewsEvents(temp));
                    checkNewsThread.Start();
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public void CheckNewsEvents(List<Event> Events)
        {
            try
            {
                //TO VERIFY
                foreach (Country rank in Ranks)
                {
                    if (Events.Any(c => c.attacker_num == rank.Number))
                    {
                        var events = Events.Where(c => c.attacker_num == rank.Number).OrderBy(c => c.newsid).ToList();

                        if (!Logins.Exists(c => c.countryNumber == rank.Number))
                        {
                            AddLoginToStorage(rank, DetectionMethod.News, events);
                        }
                        else if (Logins.Single(c => c.countryNumber == rank.Number).detections[loginDetectionModel.DetermineArrayDetectionIndex(events.First().timestamp)].lastLogin > DateTime.UtcNow.AddHours(-1))
                        {
                            AddLoginToStorage(rank, DetectionMethod.News, events);
                        }
                    }
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public void SaveFileToXML(NewsStorageModel dataToSave, bool forceSave)
        {
            try
            {
                if (dataToSave.Events.Count() > 10000 || forceSave)
                {
                    if (dataToSave.Events.Count() > 0)
                    {
                        if (CheckFileSystem(StorageOptions.News))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(NewsStorageModel));

                            dataToSave.Events = dataToSave.Events.OrderBy(c => c.timestamp).ToList();

                            var file = new NewsFileStorageModel()
                            {
                                Start = dataToSave.Events.First().timestamp,
                                End = dataToSave.Events.Last().timestamp,
                                lastRecordedNewsId = dataToSave.Events.Max(c => c.newsid),
                                AttackersInFile = dataToSave.Events.Select(c => c.attacker_num).Distinct().ToList(),
                                DefendersInFile = dataToSave.Events.Select(c => c.defender_num).Distinct().ToList(),
                                TagsInFile = dataToSave.Events.Select(c => c.d_tag).Distinct().ToList()
                            };
                            file.TagsInFile.AddRange(dataToSave.Events.Select(c => c.a_tag).Distinct().ToList());
                            file.TagsInFile = file.TagsInFile.Distinct().ToList();

                            Startup.Storage.NewsFiles.Add(file);
                            SaveFileStorage(StorageOptions.News);

                            using (TextWriter tw = new StreamWriter(@"C:\WebData\News\" + Reset + @"\NewsStorage\" + file.fileId + ".xml"))
                            {
                                xs.Serialize(tw, dataToSave);
                                tw.Close();
                            }
                            Events.RemoveAll(c => c.timestamp < DateTime.UtcNow.AddDays(-3));
                        }
                    }
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }
        #endregion

        #region EE Country Rankings API Data Miners
        // checks Background process to determine if it is running, if not executes ReceiveNews method
        private void CheckRanksUpdateProcess(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!RankingUpdateService.IsBusy)
            {
                RankingUpdateService.RunWorkerAsync();
            }
        }

        //makes a call to EE server for csv data, parses it and stores it
        private void ReceiveCountryData(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<Country> NewRanks = new List<Country>();
                string url = "http://www.earthempires.com/ranks_feed?apicode=838a8f8296991745d36d3b32f5845b6e&serverid=12";
                //GET CSV, PARSE and Build Model
                HttpClientHandler handler = new HttpClientHandler() // Use GZip compression
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                string download = "";
                using (var client = new HttpClient(handler)) // Contact EE's API and store response in a string variable
                {
                    HttpResponseMessage response;
                    client.DefaultRequestHeaders.Add("Encoding", "gzip");
                    response = client.GetAsync(url).Result;
                    download = response.Content.ReadAsStringAsync().Result;
                }
                // Check to ensure a status message was not receievd so that the csv string can be seperated into proper objects
                if (download != "You already received this update" && download != "You are being throttled for not using gzip to 1 out of 4 updates.")
                {
                    NewRanks = (from line in download.Split(Convert.ToChar(10))
                                let columns = line.Split(',')
                                where columns.Count().Equals(14)
                                select new Country(columns)).ToList();
                    if (Ranks.Count() > 0)
                    {
                        //Thread CheckForOnlineCountryThread = new Thread(() => CheckForOnlineCountries(NewRanks, Ranks.ToList()));
                        //CheckForOnlineCountryThread.Start();
                        CheckForOnlineCountries(NewRanks, Ranks);
                        if (Ranks.OrderBy(c => c.resetId).First().resetId != NewRanks.OrderBy(c => c.resetId).Last().resetId)
                        {
                            SaveFileToXML(new CountryStorageModel() { Ranks = Ranks, LastUpdated = DateTime.UtcNow.AddMinutes(-5) });
                            Reset = NewRanks.OrderBy(c => c.resetId).Last().resetId;
                            lastTagChangeSave = DateTime.UtcNow;
                            SaveFileToXML(new TagChangeStorageModel() { TagChanges = TagChanges });
                            TagChanges.Clear();
                            TagChanges = new List<TagChange>();
                            SaveFileStorage(StorageOptions.TagChanges);
                            TagChangeFiles = new List<TagChangeFileStorageModel>();
                            SaveFileToXML(new OnlineCountryDetectionStorageModel() { LoginDetections = LoginsToSave });
                            SaveFileStorage(StorageOptions.LoginDetections);
                            SaveFileToXML(Logins);
                            lastLoginsSave = DateTime.UtcNow;
                            Logins.Clear();
                            Logins = new List<loginDetectionModel>();
                            Ranks = new List<Country>();
                            Ranks.AddRange(NewRanks);
                        }
                        else
                            Reset = NewRanks.OrderBy(c => c.resetId).Last().resetId;
                        foreach (Country rank in NewRanks)
                        {
                            if (Ranks.SingleOrDefault(c => c.Number == rank.Number) != null && Ranks.SingleOrDefault(c => c.Number == rank.Number).resetId == rank.resetId)
                            {
                                Country oldRank = Ranks.SingleOrDefault(c => c.Number == rank.Number);
                                Ranks.Remove(oldRank);

                                oldRank.Networth = rank.Networth;
                                oldRank.Rank = rank.Rank;
                                oldRank.Status = rank.Status;

                                if (oldRank.Tag != rank.Tag)
                                {
                                    TagChanges.Add(new TagChange(rank.Name, rank.Number, (oldRank.Tag == "") ? "Untagged" : oldRank.Tag, (rank.Tag == "") ? "Untagged" : rank.Tag, rank.resetId));
                                    oldRank.Tag = rank.Tag;
                                }

                                oldRank.GDI = rank.GDI;
                                oldRank.Gov = rank.Gov;
                                Ranks.Add(oldRank);
                            }
                            else
                            {
                                Ranks.Add(rank);
                                TagChanges.Add(new TagChange(rank.Name, rank.Number, "Untagged", (rank.Tag == "") ? "Untagged" : rank.Tag, rank.resetId));
                            }
                        }
                    }
                    else
                    {
                        Ranks.AddRange(NewRanks);
                        Reset = NewRanks.OrderBy(c => c.resetId).Last().resetId;

                        if (NewRanks.Any(c => c.Tag != ""))
                        {
                            var tagChangeRanks = NewRanks.Where(c => c.Tag != "").ToList();
                            foreach (var rank in tagChangeRanks)
                            {
                                TagChanges.Add(new TagChange(rank.Name, rank.Number, "Untagged", rank.Tag, rank.resetId));
                            }
                        }
                    }
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public void SaveFileToXML(CountryStorageModel dataToSave)
        {
            try
            {
                if (dataToSave.Ranks.Count() > 0)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(CountryStorageModel));
                    var file = new CountryFileStorageModel()
                    {
                        Start = DateTime.UtcNow,
                        End = DateTime.UtcNow,
                        curReset = dataToSave.Ranks.Max(c => c.resetId)
                    };
                    CountryFiles.Add(file);
                    if (CheckFileSystem(StorageOptions.Countries))
                    {
                        using (TextWriter tw = new StreamWriter(@"C:\WebData\Countries\" + file.curReset + @"\CountryStorage\" + file.fileId + ".xml"))
                        {
                            xs.Serialize(tw, dataToSave);
                            tw.Close();
                        }
                        RanksLastSaved = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public void SaveFileToXML(TagChangeStorageModel dataToSave)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(TagChangeStorageModel));
                dataToSave.TagChanges = dataToSave.TagChanges.OrderBy(c => c.timestamp).ToList();
                var file = new TagChangeFileStorageModel()
                {
                    Start = dataToSave.TagChanges.First().timestamp,
                    End = dataToSave.TagChanges.Last().timestamp,
                    curReset = dataToSave.TagChanges.Max(c => c.ResetId)
                };
                TagChangeFiles.Add(file);
                if (CheckFileSystem(StorageOptions.TagChanges))
                {
                    using (TextWriter tw = new StreamWriter(@"C:\WebData\Countries\" + file.curReset + @"\TagChanges\TagChangeStorage\" + file.fileId + ".xml"))
                    {
                        xs.Serialize(tw, dataToSave);
                        tw.Close();
                    }
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public void SaveFileToXML(OnlineCountryDetectionStorageModel dataToSave)
        {
            try
            {
                if (dataToSave.LoginDetections.Count() > 0)
                {                    
                    XmlSerializer xs = new XmlSerializer(typeof(OnlineCountryDetectionStorageModel));
                    var file = new OnlineCountryDetectionFileStorageModel()
                    {
                        Start = (LoginFiles.Count() > 0) ? LoginFiles.Last().End : DateTime.UtcNow.AddDays(-1),
                        End = DateTime.UtcNow,
                        CountriesDetected = dataToSave.LoginDetections.Select(c => c.countryNumber).ToList()
                    };
                    LoginFiles.Add(file);
                    if (CheckFileSystem(StorageOptions.LoginDetections))
                    {
                        using (TextWriter tw = new StreamWriter(@"C:\WebData\Countries\" + Reset + @"\LoginDetections\LoginDetectionStorage\" + file.fileId + ".xml"))
                        {
                            xs.Serialize(tw, dataToSave);
                            tw.Close();
                        }
                        LoginsToSave.Clear();
                        LoginsToSave = new List<loginDetectionModel>();
                    }                    
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public void SaveFileToXML(List<loginDetectionModel> _logins)
        {
            try
            {
                if (CheckFileSystem(StorageOptions.LoginDetections))
                {
                    var files = Directory.GetFiles(@"C:\WebData\Countries\" + Reset + @"\LoginDetections");
                    if (files.Count() > 0)
                    {
                        File.Move(files[0], @"C:\WebData\Countries\" + Reset + @"\LoginDetections\Deleted\" + files[0].Split('\\')[5]);
                    }
                    XmlSerializer xs = new XmlSerializer(typeof(List<loginDetectionModel>));
                    using (TextWriter tw = new StreamWriter(@"C:\WebData\Countries\" + Reset + @"\LoginDetections\" + Guid.NewGuid() + ".xml"))
                    {
                        xs.Serialize(tw, _logins);
                        tw.Close();
                    }
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public void CheckForOnlineCountries(List<Country> NewRanks, List<Country> OldRanks)
        {
            try
            {
                foreach (Country country in NewRanks)
                {
                    if (OldRanks.Exists(c => c.Number == country.Number))
                    {
                        if ((country.Land - OldRanks.Single(c => c.Number == country.Number).Land) == 20)
                        {
                            AddLoginToStorage(country, DetectionMethod.Land, null);
                        }
                        else if (country.Networth > OldRanks.Single(c => c.Number == country.Number).Networth)
                        {
                            if (!Logins.Exists(c => c.countryNumber == country.Number))
                            {
                                AddLoginToStorage(country, DetectionMethod.Networth, null);
                            }
                            else
                            {
                                if (Logins.Single(c => c.countryNumber == country.Number).detections[loginDetectionModel.DetermineArrayDetectionIndex(DateTime.UtcNow)].lastLogin < DateTime.UtcNow.AddHours(-1))
                                {
                                    AddLoginToStorage(country, DetectionMethod.Networth, null);
                                }
                            }
                        }
                    }
                    else
                    {
                        AddLoginToStorage(country, DetectionMethod.Land, null);
                    }
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public void AddLoginToStorage(Country country, DetectionMethod method, List<Event> events)
        {
            try
            {
                int arrayIndex = loginDetectionModel.DetermineArrayDetectionIndex(DateTime.UtcNow);
                if (!Logins.Exists(c => c.countryNumber == country.Number))
                {
                    Logins.Add(new loginDetectionModel() { countryNumber = country.Number, resetId = country.resetId, tag = (country.Tag == "") ? "Untagged" : country.Tag });
                }
                if (!LoginsToSave.Exists(c => c.countryNumber == country.Number))
                {
                    LoginsToSave.Add(new loginDetectionModel() { countryNumber = country.Number, resetId = country.resetId, tag = (country.Tag == "") ? "Untagged" : country.Tag });
                }
                if (method == DetectionMethod.Land)
                {
                    Logins.Single(c => c.countryNumber == country.Number).detections[arrayIndex].lastNonNetworthDetection = DateTime.UtcNow;
                    LoginsToSave.Single(c => c.countryNumber == country.Number).detections[arrayIndex].lastNonNetworthDetection = DateTime.UtcNow;
                }

                if (method == DetectionMethod.News)
                    Logins.Single(c => c.countryNumber == country.Number).detections[arrayIndex].lastLogin = events.First().timestamp;
                else
                    Logins.Single(c => c.countryNumber == country.Number).detections[arrayIndex].lastLogin = DateTime.UtcNow;


                Logins.Single(c => c.countryNumber == country.Number).detections[arrayIndex].lastDetectionMethod = method;
                Logins.Single(c => c.countryNumber == country.Number).detections[arrayIndex].lastLogin = DateTime.UtcNow;
                Logins.Single(c => c.countryNumber == country.Number).detections[arrayIndex].logins++;
                LoginsToSave.Single(c => c.countryNumber == country.Number).detections[arrayIndex].lastDetectionMethod = method;
                if (method == DetectionMethod.News)
                    LoginsToSave.Single(c => c.countryNumber == country.Number).detections[arrayIndex].lastLogin = events.First().timestamp;
                else
                    LoginsToSave.Single(c => c.countryNumber == country.Number).detections[arrayIndex].lastLogin = DateTime.UtcNow;
                LoginsToSave.Single(c => c.countryNumber == country.Number).detections[arrayIndex].logins++;
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }
        #endregion

        #region EE Market Transactions API Data Miners
        // checks Background process to determine if it is running, if not executes ReceiveNews method
        private void CheckMarketUpdateProcess(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!MarketTransactionsUpdateService.IsBusy)
            {
                MarketTransactionsUpdateService.RunWorkerAsync();
            }
        }

        // makes a call to EE server obtaining news csv, parsing it and saving it to xml
        private void ReceiveMarketTransactions(object sender, DoWorkEventArgs e)
        {
            try
            {
                // transactionId for 2nd to last transaction of 1353 = 36134883
                // transactionId for 1st transaction for 1392 = 36150175
                // 2nd to last TransactionId of 1392 = 37575640
                // 1st transactionId of 1433 = 37591954
                // last transactionId of 1433 = 39007821
                // GET CSV from URL

                int lastTransactionIdRecorded = 39007821;

                if(MarketTransactions.Count() > 0)
                    lastTransactionIdRecorded = MarketTransactions.Max(c => c.transactionid);
                if (MarketFiles.Count() > 0)
                    lastTransactionIdRecorded = MarketFiles.Max(c => c.lastRecordedTransactionId);

                string url = "http://www.earthempires.com/mtrans_feed?apicode=838a8f8296991745d36d3b32f5845b6e&serverid=12&startid=" + lastTransactionIdRecorded;
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                string download = "";
                using (var client = new HttpClient(handler))
                {
                    HttpResponseMessage response;
                    client.DefaultRequestHeaders.Add("Encoding", "gzip");
                    response = client.GetAsync(url).Result;
                    download = response.Content.ReadAsStringAsync().Result;
                }
                // PARSE CSV and Create Model to save to DB
                if (download.ToString() != "" && download.ToString() != "You already received this update" && download.ToString() != "You are being throttled for not using gzip to 1 out of 4 updates.")
                {
                    var temp = (from line in download.Split(Convert.ToChar(10))
                                let columns = line.Split(',')
                                where columns.Count().Equals(8)
                                select new Transaction(columns)).OrderBy(c => c.transactionid).ToList();
                    if(temp.Count() == 10000)
                        MarketTransactionsUpdateServiceTimer.Interval = new TimeSpan(0, 0, 24).TotalMilliseconds;
                    else if(temp.Count() >= 5000)
                        MarketTransactionsUpdateServiceTimer.Interval = new TimeSpan(0, 0, 12).TotalMilliseconds;
                    else
                        MarketTransactionsUpdateServiceTimer.Interval = new TimeSpan(0, 0, 3).TotalMilliseconds;
                    if (temp.Count() > 0)
                    {
                        if (temp.First().resetid != temp.Last().resetid)
                        {
                            MarketTransactions.AddRange(temp.Where(c => c.resetid == temp.First().resetid));
                            SaveFileToXML(MarketTransactions);
                            SaveFileStorage(StorageOptions.Market);
                            MarketFiles = new List<MarketTransFileStorageModel>();
                            MarketTransactions = new List<Transaction>();
                            MarketTransactions.AddRange(temp.Where(c => c.resetid == temp.Last().resetid));
                            SaveFileToXML(MarketTransactions);
                            SaveFileStorage(StorageOptions.Market);
                            MarketTransactions = new List<Transaction>();
                        }
                        else
                            MarketTransactions.AddRange(temp);
                    }
                }
            }
            catch (Exception c)
            {
                ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public void SaveFileToXML(List<Transaction> marketTrans)
        {
            XmlSerializer xs = new XmlSerializer(typeof(MarketTransStorageModel));
            var file = new MarketTransFileStorageModel()
            {
                Start = marketTrans.First().timestamp,
                End = marketTrans.Last().timestamp,
                lastRecordedTransactionId = marketTrans.Max(c => c.transactionid),
                curReset = Reset
            };
            MarketFiles.Add(file);
            
            if (CheckFileSystem(StorageOptions.Market))
            {
                using (TextWriter tw = new StreamWriter(@"C:\WebData\Market\" + Reset + @"\MarketTransactionStorage\" + file.fileId + ".xml"))
                {
                    xs.Serialize(tw, new MarketTransStorageModel() { Transactions = marketTrans });
                    tw.Close();
                }
                SaveFileStorage(StorageOptions.Market);
            }
        }
        #endregion

        #region SpyOp Data Helpers
        // checks Background process to see if is is running, if not executes CheckForNewOps
        private void CheckCheckForNewOpsProcess(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!NewOpDetectionService.IsBusy)
                NewOpDetectionService.RunWorkerAsync();
        }

        // Checks for New Ops, if Found processes them saving them to xml and adding to queue if necessary
        private void CheckForNewOps(object sender, DoWorkEventArgs e)
        {
            try
            {
                string[] files = Directory.GetFiles(@"C:\WebData\newSpyOps\Storage");
                foreach (var file in files)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(SpyOp));
                    using (var sr = new StreamReader(file))
                    {
                        SpyOp op = (SpyOp)xs.Deserialize(sr);
                        if (SpyOpStorage.Where(c => c.subject_number == op.subject_number).Count() > 0)
                        {
                            SpyOpStorage.RemoveAll(c => c.subject_number == op.subject_number);
                        }

                        SpyOpSavingQueue.Add(op);
                        SpyOpStorage.Add(op);
                        
                        sr.Close();
                    }
                    File.Delete(file);
                }
            }
            catch (Exception c)
            {
               ErrorsToSave.Add(new ErrorModel(c));
            }
        }        

        public void SaveFileToXML(List<SpyOp> _ops, bool storage)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<SpyOp>));
            _ops = _ops.OrderBy(c => c.timestamp).ToList();
            if (storage)
            {
                if(Directory.GetFiles(@"C:\WebData\Spyops\12\" + Reset + @"\CurrentStorage").Count() > 0)
                {
                    var sourceFile = Directory.GetFiles(@"C:\WebData\Spyops\12\" + Reset + @"\CurrentStorage")[0];
                    File.Move(sourceFile, @"C:\WebData\Spyops\12\" + Reset + @"\CurrentStorage\Deleted" + sourceFile.Split('\\')[6]);
                }
                using (TextWriter tw = new StreamWriter(@"C:\WebData\Spyops\12\" + Reset + @"\CurrentStorage" + @"\" + Guid.NewGuid() + ".xml"))
                {
                    xs.Serialize(tw, _ops);
                    tw.Close();
                }
            }
            else
            {
                var file = new SpyOpsFileStorageModel()
                {
                    Start = _ops.First().timestamp,
                    End = _ops.Last().timestamp,
                    curReset = Reset,
                    CountryNumbersIncluded = _ops.Select(c => c.subject_number).ToList()
                };
                Startup.Storage.SpyOpFileStorage.Add(file);

                using (TextWriter tw = new StreamWriter(@"C:\WebData\Spyops\12\" + file.curReset + @"\" + file.fileId + ".xml"))
                {
                    xs.Serialize(tw, _ops);
                    tw.Close();
                }
                SaveFileStorage(StorageOptions.Ops);
            }
        }
        #endregion

        public void SaveFileToXML(List<Relation> relations)
        {
            XmlSerializer xs = new XmlSerializer(typeof(RelationStorageModel));
            if (CheckFileSystem(StorageOptions.Relations))
            {
                var files = Directory.GetFiles(@"C:\WebData\Relations\" + Reset);
                if(files.Count() > 0)
                {
                    var newPath = @"C:\WebData\Relations\" + Reset + @"\Deleted\" + files[0].Split('\\')[4];
                    File.Move(files[0], newPath);
                }
                using (TextWriter tw = new StreamWriter(@"C:\WebData\Relations\" + Reset + @"\" + Guid.NewGuid() + ".xml"))
                {
                    xs.Serialize(tw, new RelationStorageModel() {Relations = relations });
                    tw.Close();
                }
                newlyAddedRelations = false;
            }
        }

        public void SaveFileToXML(ErrorStorageModel dataToSave)
        {
            dataToSave.ErrorLog = dataToSave.ErrorLog.OrderBy(c => c.timestamp).ToList();
            XmlSerializer xs = new XmlSerializer(typeof(ErrorStorageModel));
            var file = new BaseFileStorageModel()
            {
                Start = dataToSave.ErrorLog.First().timestamp,
                End = dataToSave.ErrorLog.Last().timestamp
            };
            ErrorFiles.Add(file);
            if (CheckFileSystem(StorageOptions.Errors))
            {
                using (TextWriter tw = new StreamWriter(@"C:\WebData\Errors\ErrorStorage\" + file.fileId + ".xml"))
                {
                    xs.Serialize(tw, dataToSave);
                    tw.Close();
                }
                SaveFileStorage(StorageOptions.Errors);
                ErrorsToSave.Clear();
                ErrorsToSave = new List<ErrorModel>();
            }
        }
    }
}