using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoCWebApp.Models;

namespace LoCWebApp.Controllers
{
    [RoutePrefix("api/errors")]
    public class ErrorsController : Controller
    {
        [Route("~/api/errors/bydate/{daysAgo:int?}")]
        // GET: Errors
        public ActionResult ByDate(int daysAgo = 1)
        {
            List<ErrorModel> Errors = new List<ErrorModel>();
            foreach(BaseFileStorageModel file in Startup.Storage.ErrorFiles)
            {
                if(file.Start > DateTime.UtcNow.AddDays(-daysAgo) && file.End < DateTime.UtcNow)
                {
                    Errors.AddRange(new ErrorStorageModel(@"C:\WebData\Errors\ErrorStorage\" + file.fileId + ".xml").ErrorLog);
                }
            }
            return Json(Errors, JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/errors/last")]
        public ActionResult Last()
        {
            return Json(new ErrorStorageModel(@"C:\WebData\Errors\ErrorStorage\" + Startup.Storage.ErrorFiles.OrderBy(c => c.End).Last().fileId + ".xml").ErrorLog, JsonRequestBehavior.AllowGet);
        }
    }
}