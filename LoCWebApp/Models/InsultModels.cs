using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LoCWebApp.Models
{
    public class InsultStorageModel
    {
        public List<string> Insults { get; set; }

        public InsultStorageModel()
        {
            Insults = new List<string>();
        }

        public InsultStorageModel(string file)
        {
            Insults = new List<string>();
            XmlSerializer xs = new XmlSerializer(typeof(InsultStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (InsultStorageModel)xs.Deserialize(sr);
                this.Insults = temp.Insults;
            }
        }

        public void SaveInsultsToXML()
        {
            try
            {
                //check to see if a file already exists
                var files = Directory.GetFiles(@"C:\WebData\Insults");
                if (files.Count() > 0)
                {
                    string endFile = @"C:\WebData\Insults\Deleted\" + files[0].Split('\\')[3];
                    //move file to deleted
                    File.Move(files[0], endFile);
                }

                XmlSerializer xs = new XmlSerializer(typeof(InsultStorageModel));
                using (TextWriter tw = new StreamWriter(@"C:\WebData\Insults\" + Guid.NewGuid() + ".xml"))
                {
                    xs.Serialize(tw, this);
                    tw.Close();
                }
            }
            catch (Exception c)
            {
                Startup.Storage.ErrorsToSave.Add(new ErrorModel(c));
            }
        }

        public string GetRandomInsult()
        {
            Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));

            int rnd = rndNum.Next(0, Insults.Count() - 1);

            return Insults[rnd];
        }
    }
}