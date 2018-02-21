using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoCWebApp.Models;

namespace LoCWebApp.Controllers
{
    [RoutePrefix("api/news")]
    public class NewsController : Controller
    {
        [Route("~/api/news/bynumber/{countryNumber:int}/{start?}/{end?}")]
        // GET: News
        public ActionResult ByNumber(int countryNumber, string start, string end)
        {
            if (start != null && end != null)
            {
                List<Event> newsEvents = new List<Event>();
                var splitDate = start.Split('-');
                DateTime startDate = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[0]), int.Parse(splitDate[1]));
                splitDate = end.Split('-');
                DateTime endDate = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[0]), int.Parse(splitDate[1]));
                foreach (NewsFileStorageModel file in Startup.Storage.NewsFiles)
                {
                    if (file.Start > startDate && file.End < endDate)
                    {
                        newsEvents.AddRange(new NewsStorageModel(@"C:\WebData\News\" + Startup.Storage.Reset + @"\NewsStorage\" + file.fileId + ".xml").Events.Where(c => c.attacker_num == countryNumber || c.defender_num == countryNumber));
                    }
                }
                newsEvents.AddRange(Startup.Storage.Events.Where(c => c.attacker_num == countryNumber || c.defender_num == countryNumber));
                return Json(newsEvents, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<Event> newsEvents = new List<Event>();
                foreach (NewsFileStorageModel file in Startup.Storage.NewsFiles)
                {
                    if(file.AttackersInFile.Contains(countryNumber) || file.DefendersInFile.Contains(countryNumber))
                    {
                        newsEvents.AddRange(new NewsStorageModel(@"C:\WebData\News\" + Startup.Storage.Reset + @"\NewsStorage\" + file.fileId + ".xml").Events.Where(c => c.attacker_num == countryNumber || c.defender_num == countryNumber));
                    }
                }
                newsEvents.AddRange(Startup.Storage.Events.Where(c => c.attacker_num == countryNumber || c.defender_num == countryNumber));
                return Json(newsEvents, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("~/api/news/bytag/{tag}/{start?}/{end?}")]
        public ActionResult ByTag(string tag, string start, string end)
        {
            if (start != null && end != null)
            {
                List<Event> newsEvents = new List<Event>();
                var splitDate = start.Split('-');
                DateTime startDate = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[0]), int.Parse(splitDate[1]));
                splitDate = end.Split('-');
                DateTime endDate = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[0]), int.Parse(splitDate[1]));
                foreach (NewsFileStorageModel file in Startup.Storage.NewsFiles)
                {
                    if (file.Start > startDate && file.End < endDate)
                    {
                        newsEvents.AddRange(new NewsStorageModel(@"C:\WebData\News\" + Startup.Storage.Reset + @"\NewsStorage\" + file.fileId + ".xml").Events.Where(c => c.a_tag == tag || c.d_tag == tag));
                    }
                }
                newsEvents.AddRange(Startup.Storage.Events.Where(c => c.a_tag == tag || c.d_tag == tag));
                return Json(newsEvents, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<Event> newsEvents = new List<Event>();
                foreach (NewsFileStorageModel file in Startup.Storage.NewsFiles)
                {
                    if (file.TagsInFile.Contains(tag))
                    {
                        newsEvents.AddRange(new NewsStorageModel(@"C:\WebData\News\" + Startup.Storage.Reset + @"\NewsStorage\" + file.fileId + ".xml").Events.Where(c => c.a_tag == tag || c.d_tag == tag));
                    }
                }
                newsEvents.AddRange(Startup.Storage.Events.Where(c => c.a_tag == tag || c.d_tag == tag));
                return Json(newsEvents, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("~/api/news/stats/{countryNumber:int}")]
        public ActionResult Stats(int countryNumber)
        {

            //TODO
            return Json(Startup.Storage.Events.Where(c => c.attacker_num == countryNumber || c.defender_num == countryNumber), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/news/ByDate/{start}/{end}")]
        public ActionResult ByDate(string start, string end)
        {
            List<Event> newsEvents = new List<Event>();
            DateTime startDate = DateTime.Parse(start);
            DateTime endDate = DateTime.Parse(end);
            foreach (NewsFileStorageModel file in Startup.Storage.NewsFiles)
            {
                if (file.Start > startDate && file.End < endDate)
                {
                    newsEvents.AddRange(new NewsStorageModel(@"C:\WebData\News\" + Startup.Storage.Reset + @"\NewsStorage\" + file.fileId + ".xml").Events);
                }
            }
            newsEvents.AddRange(Startup.Storage.Events.Where(c => c.timestamp > startDate && c.timestamp < endDate));
            return Json(newsEvents, JsonRequestBehavior.AllowGet);
        }
    }
}