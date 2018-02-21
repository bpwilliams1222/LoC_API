using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace LoCWebApp.Models
{
    public class JsonModels
    {
        public static string GetJson(string url)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(url);
            }
        }

    }
}