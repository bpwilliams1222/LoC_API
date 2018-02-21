using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using LoCWebApp.Models;

namespace LoCWebApp.Controllers
{
    [RoutePrefix("api/announcements")]
    public class AnnouncementsController : Controller
    {
        [Route("~/api/announcements/index/{user}")]
        public ActionResult Index(string user)
        {
            if (user == "" || user == null)
                return Json(Startup.Storage.Announcements.Announcements, JsonRequestBehavior.AllowGet);
            else
            {
                List<Announcement> announcements = Startup.Storage.Announcements.Announcements.Where(c => c.UsersReceived.Contains(user) == false).ToList();
                if (announcements.Count() > 0)
                {
                    foreach (Announcement announcement in announcements)
                    {
                        announcement.UsersReceived.Add(user);
                    }
                    Startup.Storage.Announcements.newlyAddedAnnouncements = true;
                }
                return Json(announcements, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("~/api/announcements/add")]
        [HttpPost]
        public ActionResult Add(Announcement _announcement)
        {
            try
            {
                Startup.Storage.Announcements.Announcements.Add(_announcement);
                Startup.Storage.Announcements.newlyAddedAnnouncements = true;
                return Json(true, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [Route("~/api/announcements/delete")]
        [HttpPost]
        public ActionResult Delete(Announcement _announcement)
        {
            try
            {
                if(Startup.Storage.Announcements.Announcements.RemoveAll(c => c.Message == _announcement.Message) > 0)
                {
                    Startup.Storage.Announcements.newlyAddedAnnouncements = true;
                    return Json(true, JsonRequestBehavior.DenyGet);
                }
            }
            catch
            {

            }
            return Json(false, JsonRequestBehavior.DenyGet);
        }

        /*public async System.Threading.Tasks.Task<ActionResult> Test()
        {
            HttpClient client = new HttpClient();

            var values = new Dictionary<string, string>
            {
               { "Message", "This is a test." },
                {"Author", "MadNudist" }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("http://localhost:56109/Announcements/Add", content);

            return Json(await response.Content.ReadAsStringAsync(), JsonRequestBehavior.AllowGet);
        }*/
    }
}