using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using LoCWebApp.Controllers;

namespace LoCWebApp.Models
{
    public class ClaimCountryModel
    {
        public int[] countries;
        public string user = "";
    }
    public enum CountryStatus
    {
        Alive,
        Dead,
        Deleted,
        Protection,
        Vacation
    }

    public class Country
    {
        public int Number { get; set; }
        public DateTime Timestamp { get; set; }
        public int serverId { get; set; }
        public int resetId { get; set; }
        public int Rank { get; set; }
        public string Name { get; set; }
        public int Land { get; set; }
        public long Networth { get; set; }
        public string Tag { get; set; }
        public string Gov { get; set; }
        public bool GDI { get; set; }
        public CountryStatus Status { get; set; }
        public string User { get; set; }
        public bool KillList { get; set; }

        public Country()
        {
            Timestamp = DateTime.UtcNow;
            KillList = false;
            User = "";
        }

        public Country(string[] columns)
        {
            Number = Convert.ToInt32(columns[3]);
            Timestamp = DateTime.UtcNow;
            serverId = Convert.ToInt32(columns[0]);
            resetId = Convert.ToInt32(columns[1]);
            Rank = Convert.ToInt32(columns[2]);
            Name = columns[4];
            Land = Convert.ToInt32(columns[5]);
            Networth = Convert.ToInt64(columns[6]);
            Tag = columns[7];
            Gov = columns[8];
            GDI = Convert.ToBoolean(Int16.Parse(columns[9]));
            Status = DetermineCountryStatus(Convert.ToBoolean(Int16.Parse(columns[10])), Convert.ToBoolean(Int16.Parse(columns[11])), Convert.ToBoolean(Int16.Parse(columns[12])), Convert.ToBoolean(Int16.Parse(columns[13])));
            User = "";
            KillList = false;
        }

        // method returns a countries status based on the boolean variables received from EE
        public static CountryStatus DetermineCountryStatus(bool Prot, bool Vac, bool Alive, bool Deleted)
        {
            if (Prot == true && Deleted == false)
                return CountryStatus.Protection;
            else if (Vac == true && Deleted == false)
                return CountryStatus.Vacation;
            else if (Alive == true && Deleted == false)
                return CountryStatus.Alive;
            else if (Alive == false && Deleted == false)
                return CountryStatus.Dead;
            else
                return CountryStatus.Deleted;
        }

        /*
         * Determine DR(Deminishing Return) Method
         * 
         * Purpose:
         * Returns an int representing a countries DR based on the news provided
         * 
         */
        public int DetermineDR(List<Event> News)
        {
            int dr = 0;

            foreach (Event newsEvent in News)
            {
                if (newsEvent.defender_num == this.Number)
                    dr++;

                if (newsEvent.attacker_num == this.Number)
                    dr--;
            }
            if (dr < 0)
                dr = 0;
            return dr;
        }

        /*
         * Add To Kill List Method
         * 
         * Purpose:
         * Modifies the api_currentrank object for this country to flag it as a kill list country
         * 
         */
        public bool AddToKillList()
        {
            try
            {
                KillList = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /*
         * Remove From Kill List Method
         * 
         * Purpose:
         * Modifies the api_currentrank object for this country to remove the flag labelling it as a kill list country
         * 
         */
        public bool RemoveFromKillList()
        {
            try
            {
                KillList = false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class CountryFileStorageModel : BaseFileStorageModel
    {
        public int curReset { get; set; }

        public CountryFileStorageModel()
        {
            fileId = Guid.NewGuid();
        }

        public CountryFileStorageModel(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(CountryFileStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (CountryFileStorageModel)xs.Deserialize(sr);
                this.Start = temp.Start;
                this.End = temp.End;
                this.fileId = temp.fileId;
                this.curReset = temp.curReset;
            }
        }
    }

    public class CountryStorageModel
    {
        public List<Country> Ranks { get; set; }
        public DateTime LastUpdated { get; set; }

        public CountryStorageModel()
        {
            Ranks = new List<Country>();
        }

        public CountryStorageModel(string file)
        {
            Ranks = new List<Country>();
            XmlSerializer xs = new XmlSerializer(typeof(CountryStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (CountryStorageModel)xs.Deserialize(sr);
                this.Ranks = temp.Ranks;
                this.LastUpdated = temp.LastUpdated;
            }
        }
    }

    #region Tag Change Models
    public class TagChange
    {
        public TagChange()
        {
            TagChangeId = Guid.NewGuid();
            timestamp = DateTime.UtcNow;
        }
        public TagChange(string countryName, int countryNum, string fromTag, string toTag, int resetId)
        {
            TagChangeId = Guid.NewGuid();
            timestamp = DateTime.UtcNow;
            Name = countryName;
            Number = countryNum;
            FromTag = FromTag;
            ToTag = toTag;
            ResetId = resetId;
        }
        public Guid TagChangeId { get; set; }
        public DateTime timestamp { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string FromTag { get; set; }
        public string ToTag { get; set; }
        public int ResetId { get; set; }
    }

    public class TagChangeStorageModel
    {
        public List<TagChange> TagChanges { get; set; }

        public TagChangeStorageModel()
        {
            TagChanges = new List<TagChange>();
        }

        public TagChangeStorageModel(string file)
        {
            TagChanges = new List<TagChange>();
            XmlSerializer xs = new XmlSerializer(typeof(TagChangeStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (TagChangeStorageModel)xs.Deserialize(sr);
                this.TagChanges = temp.TagChanges;
            }
        }
    }

    public class TagChangeFileStorageModel : BaseFileStorageModel
    {
        public int curReset { get; set; }

        public TagChangeFileStorageModel()
        {
            fileId = Guid.NewGuid();
        }

        public TagChangeFileStorageModel(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(TagChangeFileStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (TagChangeFileStorageModel)xs.Deserialize(sr);
                this.Start = temp.Start;
                this.End = temp.End;
                this.fileId = temp.fileId;
                this.curReset = temp.curReset;
            }
        }
    }
    #endregion

    #region Online Detection Models
    public enum loginRanges
    {
        hour00to01,
        hour01to02,
        hour02to03,
        hour03to04,
        hour04to05,
        hour05to06,
        hour06to07,
        hour07to08,
        hour08to09,
        hour09to10,
        hour10to11,
        hour11to12,
        hour12to13,
        hour13to14,
        hour14to15,
        hour15to16,
        hour16to17,
        hour17to18,
        hour18to19,
        hour19to20,
        hour20to21,
        hour21to22,
        hour22to23,
        hour23to24,
    }

    public class loginSlotsModel
    {
        public int countryNumber { get; set; }
        public loginRanges range { get; set; }
        public int logins { get; set; }
        public DateTime lastLogin { get; set; }
        public DetectionMethod lastDetectionMethod { get; set; }
        public DateTime lastNonNetworthDetection { get; set; }

        public loginSlotsModel()
        {

        }
        public loginSlotsModel(loginRanges _range)
        {
            range = _range;
        }
    }

    public class loginDetectionModel
    {
        public int countryNumber { get; set; }
        public loginSlotsModel[] detections = new loginSlotsModel[24];
        public string tag { get; set; }
        public int resetId { get; set; }

        public static loginRanges DetermineLoginRange(DateTime login)
        {
            switch (login.Hour)
            {
                case 0:
                    return loginRanges.hour00to01;
                case 1:
                    return loginRanges.hour01to02;
                case 2:
                    return loginRanges.hour02to03;
                case 3:
                    return loginRanges.hour03to04;
                case 4:
                    return loginRanges.hour04to05;
                case 5:
                    return loginRanges.hour05to06;
                case 6:
                    return loginRanges.hour06to07;
                case 7:
                    return loginRanges.hour07to08;
                case 8:
                    return loginRanges.hour08to09;
                case 9:
                    return loginRanges.hour09to10;
                case 10:
                    return loginRanges.hour10to11;
                case 11:
                    return loginRanges.hour11to12;
                case 12:
                    return loginRanges.hour12to13;
                case 13:
                    return loginRanges.hour13to14;
                case 14:
                    return loginRanges.hour14to15;
                case 15:
                    return loginRanges.hour15to16;
                case 16:
                    return loginRanges.hour16to17;
                case 17:
                    return loginRanges.hour17to18;
                case 18:
                    return loginRanges.hour18to19;
                case 19:
                    return loginRanges.hour19to20;
                case 20:
                    return loginRanges.hour20to21;
                case 21:
                    return loginRanges.hour21to22;
                case 22:
                    return loginRanges.hour22to23;
                case 23:
                    return loginRanges.hour23to24;
                default:
                    return loginRanges.hour00to01;
            }
        }

        public static loginRanges DetermineLoginRange(int index)
        {
            switch (index)
            {
                case 0:
                    return loginRanges.hour00to01;
                case 1:
                    return loginRanges.hour01to02;
                case 2:
                    return loginRanges.hour02to03;
                case 3:
                    return loginRanges.hour03to04;
                case 4:
                    return loginRanges.hour04to05;
                case 5:
                    return loginRanges.hour05to06;
                case 6:
                    return loginRanges.hour06to07;
                case 7:
                    return loginRanges.hour07to08;
                case 8:
                    return loginRanges.hour08to09;
                case 9:
                    return loginRanges.hour09to10;
                case 10:
                    return loginRanges.hour10to11;
                case 11:
                    return loginRanges.hour11to12;
                case 12:
                    return loginRanges.hour12to13;
                case 13:
                    return loginRanges.hour13to14;
                case 14:
                    return loginRanges.hour14to15;
                case 15:
                    return loginRanges.hour15to16;
                case 16:
                    return loginRanges.hour16to17;
                case 17:
                    return loginRanges.hour17to18;
                case 18:
                    return loginRanges.hour18to19;
                case 19:
                    return loginRanges.hour19to20;
                case 20:
                    return loginRanges.hour20to21;
                case 21:
                    return loginRanges.hour21to22;
                case 22:
                    return loginRanges.hour22to23;
                case 23:
                    return loginRanges.hour23to24;
                default:
                    return loginRanges.hour00to01;
            }
        }

        public static int DetermineArrayDetectionIndex(DateTime login)
        {
            switch (DetermineLoginRange(login))
            {
                case loginRanges.hour00to01:
                    return 0;
                case loginRanges.hour01to02:
                    return 1;
                case loginRanges.hour02to03:
                    return 2;
                case loginRanges.hour03to04:
                    return 3;
                case loginRanges.hour04to05:
                    return 4;
                case loginRanges.hour05to06:
                    return 5;
                case loginRanges.hour06to07:
                    return 6;
                case loginRanges.hour07to08:
                    return 7;
                case loginRanges.hour08to09:
                    return 8;
                case loginRanges.hour09to10:
                    return 9;
                case loginRanges.hour10to11:
                    return 10;
                case loginRanges.hour11to12:
                    return 11;
                case loginRanges.hour12to13:
                    return 12;
                case loginRanges.hour13to14:
                    return 13;
                case loginRanges.hour14to15:
                    return 14;
                case loginRanges.hour15to16:
                    return 15;
                case loginRanges.hour16to17:
                    return 16;
                case loginRanges.hour17to18:
                    return 17;
                case loginRanges.hour18to19:
                    return 18;
                case loginRanges.hour19to20:
                    return 19;
                case loginRanges.hour20to21:
                    return 20;
                case loginRanges.hour21to22:
                    return 21;
                case loginRanges.hour22to23:
                    return 22;
                case loginRanges.hour23to24:
                    return 23;
                default:
                    return 0;
            }
        }

        public static int DetermineArrayDetectionIndex(loginRanges range)
        {
            switch (range)
            {
                case loginRanges.hour00to01:
                    return 0;
                case loginRanges.hour01to02:
                    return 1;
                case loginRanges.hour02to03:
                    return 2;
                case loginRanges.hour03to04:
                    return 3;
                case loginRanges.hour04to05:
                    return 4;
                case loginRanges.hour05to06:
                    return 5;
                case loginRanges.hour06to07:
                    return 6;
                case loginRanges.hour07to08:
                    return 7;
                case loginRanges.hour08to09:
                    return 8;
                case loginRanges.hour09to10:
                    return 9;
                case loginRanges.hour10to11:
                    return 10;
                case loginRanges.hour11to12:
                    return 11;
                case loginRanges.hour12to13:
                    return 12;
                case loginRanges.hour13to14:
                    return 13;
                case loginRanges.hour14to15:
                    return 14;
                case loginRanges.hour15to16:
                    return 15;
                case loginRanges.hour16to17:
                    return 16;
                case loginRanges.hour17to18:
                    return 17;
                case loginRanges.hour18to19:
                    return 18;
                case loginRanges.hour19to20:
                    return 19;
                case loginRanges.hour20to21:
                    return 20;
                case loginRanges.hour21to22:
                    return 21;
                case loginRanges.hour22to23:
                    return 22;
                case loginRanges.hour23to24:
                    return 23;
                default:
                    return 0;
            }
        }

        public loginDetectionModel()
        {
            for(int i = 0; i < 24; i++)
            {
                detections[i] = new loginSlotsModel(DetermineLoginRange(i));
            }
        }
    }

    public enum DetectionMethod
    {
        Networth,
        Land,
        News
    }

    public class OnlineCountryDetectionStorageModel
    {
        public List<loginDetectionModel> LoginDetections { get; set; }

        public OnlineCountryDetectionStorageModel()
        {
            LoginDetections = new List<loginDetectionModel>();
        }

        public OnlineCountryDetectionStorageModel(string file)
        {
            LoginDetections = new List<loginDetectionModel>();
            XmlSerializer xs = new XmlSerializer(typeof(OnlineCountryDetectionStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (OnlineCountryDetectionStorageModel)xs.Deserialize(sr);
                this.LoginDetections = temp.LoginDetections;
            }
        }
    }

    public class OnlineCountryDetectionFileStorageModel : BaseFileStorageModel
    {
        public List<int> CountriesDetected { get; set; }

        public OnlineCountryDetectionFileStorageModel()
        {
            fileId = Guid.NewGuid();
        }

        public OnlineCountryDetectionFileStorageModel(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(OnlineCountryDetectionFileStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (OnlineCountryDetectionFileStorageModel)xs.Deserialize(sr);
                this.Start = temp.Start;
                this.End = temp.End;
                this.fileId = temp.fileId;
                this.CountriesDetected = temp.CountriesDetected;
            }
        }
    }
    #endregion
}