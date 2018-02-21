
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoCWebApp.Models;

namespace LoCWebApp.Controllers
{
    [RoutePrefix("api/tagchanges")]
    public class TagChangesController : Controller
    {
        [Route("~/api/tagchanges/bynumber/{countryNumber:int}")]
        public ActionResult ByNumber(int countryNumber)
        {
            List<TagChange> TagChanges = new List<TagChange>();
            foreach (var file in Startup.Storage.TagChangeFiles)
            {
                List<TagChange> tempChanges = new TagChangeStorageModel(@"C:\WebData\Countries\" + file.curReset + @"\TagChanges\TagChangeStorage\" + file.fileId + ".xml").TagChanges;
                if (tempChanges.Exists(c => c.Number == countryNumber))
                {
                    TagChanges.AddRange(tempChanges.Where(c => c.Number == countryNumber));
                }
            }
            return Json(TagChanges, JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/tagchanges/bytag/{Tag}")]
        public ActionResult ByTag(string Tag)
        {
            List<TagChange> TagChanges = new List<TagChange>();
            foreach (var file in Startup.Storage.TagChangeFiles)
            {
                List<TagChange> tempChanges = new TagChangeStorageModel(@"C:\WebData\Countries\" + file.curReset + @"\TagChanges\TagChangeStorage\" + file.fileId + ".xml").TagChanges;
                if (tempChanges.Exists(c => c.FromTag == Tag || c.ToTag == Tag))
                {
                    TagChanges.AddRange(tempChanges.Where(c => c.FromTag == Tag || c.ToTag == Tag));
                }
            }
            return Json(TagChanges, JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/tagchanges/bydate/{_start}/{_end}")]
        public ActionResult ByDate(string _start, string _end)
        {
            var dateSplit = _start.Split('-');
            DateTime start = new DateTime(Int32.Parse(dateSplit[2]), Int32.Parse(dateSplit[0]), Int32.Parse(dateSplit[1]));
            dateSplit = _end.Split('-');
            DateTime end = new DateTime(Int32.Parse(dateSplit[2]), Int32.Parse(dateSplit[0]), Int32.Parse(dateSplit[1]));
            List<TagChange> TagChanges = new List<TagChange>();
            foreach (var file in Startup.Storage.TagChangeFiles)
            {
                List<TagChange> tempChanges = new TagChangeStorageModel(@"C:\WebData\Countries\" + file.curReset + @"\TagChanges\TagChangeStorage\" + file.fileId + ".xml").TagChanges;
                if (tempChanges.Exists(c => c.timestamp >= start && c.timestamp <= end))
                {
                    TagChanges.AddRange(tempChanges.Where(c => c.timestamp >= start && c.timestamp <= end));
                }
            }
            return Json(TagChanges, JsonRequestBehavior.AllowGet);
        }
    }
}