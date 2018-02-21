using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LoCWebApp.Models
{
    public class ErrorStorageModel
    {
        public List<ErrorModel> ErrorLog { get; set; }

        public ErrorStorageModel()
        {
            ErrorLog = new List<ErrorModel>();
        }

        public ErrorStorageModel(string file)
        {
            ErrorLog = new List<ErrorModel>();
            XmlSerializer xs = new XmlSerializer(typeof(ErrorStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (ErrorStorageModel)xs.Deserialize(sr);
                this.ErrorLog = temp.ErrorLog;
            }
        }
    }
    public class ErrorModel
    {
        public ErrorModel(Exception c)
        {
            LogId = Guid.NewGuid();
            timestamp = DateTime.UtcNow;
            Error = c.Message;
            stackTrace = c.StackTrace;
            if (c.InnerException != null)
            {
                if (c.InnerException.Message != null)
                    InnerException = c.InnerException.Message;
                if (c.InnerException.StackTrace != null)
                    innerStackTrace = c.InnerException.StackTrace;
            }
        }

        public ErrorModel()
        {
            LogId = Guid.NewGuid();
            timestamp = DateTime.UtcNow;
        }

        public Guid LogId { get; set; }
        public string Error { get; set; }
        public string InnerException { get; set; }
        public DateTime timestamp { get; set; }
        public string stackTrace { get; set; }
        public string innerStackTrace { get; set; }

        public void SaveErrorToXML()
        {
            XmlSerializer xs = new XmlSerializer(typeof(ErrorModel));
            using (TextWriter tw = new StreamWriter(@"C:\WebData\Errors\StartupErrors\" + Guid.NewGuid() + ".xml"))
            {
                xs.Serialize(tw, this);
                tw.Close();
            }
        }
    }    
}