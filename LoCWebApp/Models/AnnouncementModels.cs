using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LoCWebApp.Models
{
    public class Announcement
    {
        public string Message { get; set; }
        public DateTime dateAdded { get; set; }
        public string Author { get; set; }
        public List<string> UsersReceived { get; set; }

        public Announcement()
        {
            UsersReceived = new List<string>();
            dateAdded = DateTime.UtcNow;
        }

        public Announcement(string message, string author)
        {
            UsersReceived = new List<string>();
            dateAdded = DateTime.UtcNow;
            Message = message;
            Author = author;
        }

        public string BuildAnnouncementMessage()
        {
            return "      " + Message + " -" + Author;
        }
    }

    public class UserNotificaitonModel
    {
        public string username { get; set; }
        public DateTime lastNotificaiton { get; set; }
    }

    public class AnnouncementStorage
    {
        public List<Announcement> Announcements { get; set; }
        public bool newlyAddedAnnouncements = false;

        public AnnouncementStorage()
        {
            Announcements = new List<Announcement>();
        }

        public AnnouncementStorage(string file)
        {
            Announcements = new List<Announcement>();
            XmlSerializer xs = new XmlSerializer(typeof(AnnouncementStorage));
            using (var sr = new StreamReader(file))
            {
                var temp = (AnnouncementStorage)xs.Deserialize(sr);
                this.Announcements = temp.Announcements;
            }
        }

        public void SaveFileToXML()
        {
            string[] files;
            try
            {
                if (newlyAddedAnnouncements)
                {
                    if (Directory.GetDirectories(@"C:\WebData\Announcements").Count() > 0)
                    {
                        files = Directory.GetFiles(@"C:\WebData\Announcements\");
                        if (files.Count() > 0)
                        {
                            string endFile = @"C:\WebData\Announcements\Deleted\" + files[0].Split('\\')[3];
                            //move file to deleted
                            File.Move(files[0], endFile);
                        }

                        newlyAddedAnnouncements = false;

                        XmlSerializer xs = new XmlSerializer(typeof(AnnouncementStorage));
                        using (TextWriter tw = new StreamWriter(@"C:\WebData\Announcements\" + Guid.NewGuid() + ".xml"))
                        {
                            xs.Serialize(tw, this);
                            tw.Close();
                        }
                    }
                }
            }
            catch (Exception c)
            {
                Startup.Storage.ErrorsToSave.Add(new ErrorModel(c));
            }
        }
    }
}