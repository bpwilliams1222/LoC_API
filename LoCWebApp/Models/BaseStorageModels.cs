using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LoCWebApp.Models
{
    public enum StorageOptions
    {
        News,
        Errors,
        Market,
        Ops,
        Relations,
        Insults,
        Countries,
        TagChanges,
        LoginDetections,
        Users,
        Announcements
    }

    public class BaseStorageModels
    {
        /*
             * Convert Unix Timestamp to DateTime Method
             * 
             * Purpose:
             * Convert a double Unix Timestamp to a DateTime object
             */
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        /*
         * Convert Unix DateTime to Unix Timestamp Method
         * 
         * Purpose:
         * Convert a DateTime object to a long Unix timestamp
         */
        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalSeconds;
        }
    }

    public class BaseFileStorageModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid fileId { get; set; }

        public BaseFileStorageModel()
        {
            fileId = Guid.NewGuid();
        }

        public BaseFileStorageModel(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(BaseFileStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (BaseFileStorageModel)xs.Deserialize(sr);
                this.Start = temp.Start;
                this.End = temp.End;
                this.fileId = temp.fileId;
            }
        }
    }
}