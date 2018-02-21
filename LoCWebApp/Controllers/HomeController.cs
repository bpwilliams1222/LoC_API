using LoCWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LoCWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ReceiveOp(string player_site_id, string player_cnum, string uploader_cnum, int serverid, string spyoptype, string spyop_json, string buffer)
        {
            if (player_site_id != null && player_cnum != null && uploader_cnum != null && serverid > 0 && spyoptype != null && spyop_json != null)
            {
                try
                {
                    //SendOpToBot(spyoptype + "|" + properJSON);
                    if (serverid == 12)
                    {
                        byte[] bytes = Encoding.Default.GetBytes(spyop_json);
                        string properJSON = Encoding.UTF8.GetString(bytes);
                        var op = new SpyOp();
                        op.json = properJSON;
                        op.serverid = serverid;
                        op.subject_number = int.Parse(player_cnum);
                        op.type = spyoptype;
                        op.uploader_api_key = player_site_id;
                        op.uploader_number = int.Parse(uploader_cnum);
                        op.SaveOpToXML();

                        //var opInfo = new Models.SpyOpInfo(properJSON, spyoptype);
                        //opInfo.SaveDataToXML();
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                catch (Exception c)
                {
                    Startup.Storage.ErrorsToSave.Add(new ErrorModel(c));
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}