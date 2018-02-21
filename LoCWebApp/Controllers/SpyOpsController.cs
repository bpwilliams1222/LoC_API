using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using LoCWebApp.Models;

namespace LoCWebApp.Controllers
{
    [RoutePrefix("api/spyops")]
    public class SpyOpsController : Controller
    {
        [Route("~/api/spyops/bynumber/{number:int}")]
        // GET: SpyOps
        public ActionResult ByNumber(int number)
        {
            return Json(Startup.Storage.SpyOpStorage.SingleOrDefault(c => c.subject_number == number), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/spyops/History/{number:int}")]
        public ActionResult History(int number)
        {
            List<SpyOp> SpyOps = new List<SpyOp>();

            XmlSerializer xs = new XmlSerializer(typeof(List<SpyOp>));
             
            if (Startup.Storage.CheckFileSystem(StorageOptions.Ops))
            {
                foreach (SpyOpsFileStorageModel file in Startup.Storage.SpyOpFileStorage)
                {
                    if (file.CountryNumbersIncluded.Contains(number))
                    {
                        using (var sr = new StreamReader(@"C:\WebData\Spyops\12\" + Startup.Storage.Reset + @"\" + file.fileId + ".xml"))
                        {
                            SpyOps.AddRange(((List<SpyOp>)xs.Deserialize(sr)).Where(c => c.subject_number == number));
                        }
                    }
                }
            }

            return Json(SpyOps, JsonRequestBehavior.AllowGet);
        }
    }
}