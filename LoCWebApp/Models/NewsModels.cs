using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using LoCWebApp.Controllers;

namespace LoCWebApp.Models
{
    public class Event
    {
        public int newsid { get; set; }
        public int resetid { get; set; }
        public int serverid { get; set; }
        public DateTime timestamp { get; set; }
        public string Type { get; set; }
        public int win { get; set; }
        public string attacker_name { get; set; }
        public int attacker_num { get; set; }
        public string a_tag { get; set; }
        public string defender_name { get; set; }
        public int defender_num { get; set; }
        public string d_tag { get; set; }
        public long result1 { get; set; }
        public long result2 { get; set; }
        public int killhit { get; set; }

        public Event()
        {

        }

        public Event(string[] columns)
        {
            if (columns.Count() == 15)
            {
                serverid = Int16.Parse(columns[0]);
                resetid = Int16.Parse(columns[1]);
                newsid = Int32.Parse(columns[2]);
                timestamp = BaseStorageModels.UnixTimeStampToDateTime(double.Parse(columns[3]));
                Type = columns[4];
                win = Int16.Parse(columns[5]);
                attacker_num = Int16.Parse(columns[6]);
                attacker_name = columns[7];
                defender_num = Int16.Parse(columns[8]);
                defender_name = columns[9];
                result1 = Convert.ToInt32(columns[10]);
                result2 = (columns[11] != "") ? Convert.ToInt32(columns[11]) : 0;
                a_tag = columns[12];
                d_tag = columns[13];
                killhit = Int16.Parse(columns[14]);
            }
        }
    }

    public class NewsFileStorageModel : BaseFileStorageModel
    {
        public int lastRecordedNewsId { get; set; }
        public List<int> AttackersInFile { get; set; }
        public List<int> DefendersInFile { get; set; }
        public List<string> TagsInFile { get; set; }

        public NewsFileStorageModel()
        {
            fileId = Guid.NewGuid();
        }

        public NewsFileStorageModel(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(NewsFileStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (NewsFileStorageModel)xs.Deserialize(sr);
                this.Start = temp.Start;
                this.End = temp.End;
                this.fileId = temp.fileId;
                this.lastRecordedNewsId = temp.lastRecordedNewsId;
                this.AttackersInFile = temp.AttackersInFile;
                this.DefendersInFile = temp.DefendersInFile;
                this.TagsInFile = temp.TagsInFile;
            }
        }
    }

    public class NewsStorageModel
    {
        public List<Event> Events { get; set; }

        public NewsStorageModel()
        {
            Events = new List<Event>();
        }

        public NewsStorageModel(string file)
        {
            Events = new List<Event>();
            XmlSerializer xs = new XmlSerializer(typeof(NewsStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (NewsStorageModel)xs.Deserialize(sr);
                this.Events = temp.Events;
            }
        }
    }


}