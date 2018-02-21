using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace LoCWebApp.Controllers
{
    [RoutePrefix("api/insults")]
    public class InsultsController : Controller
    {
        [Route("~/api/insults")]
        public ActionResult Index()
        {
            return Json(Startup.Storage.InsultsStorage.GetRandomInsult(), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/insults/add")]
        [HttpPost]
        public ActionResult Add(string insult)
        {
            try
            {
                if(Startup.Storage.InsultsStorage != null && insult != null)
                {
                    Startup.Storage.InsultsStorage.Insults.Add(insult);
                    Startup.Storage.newlyAddedInsults = true;
                    return Json(true, JsonRequestBehavior.DenyGet);
                }
            }
            catch
            {

            }
            return Json(false, JsonRequestBehavior.DenyGet);
        }

        [Route("~/api/insults/delete")]
        [HttpPost]
        public ActionResult Delete(string insult)
        {
            try
            {
                if (Startup.Storage.InsultsStorage != null && insult != null)
                {
                    if (Startup.Storage.InsultsStorage.Insults.RemoveAll(c => c == insult) > 0)
                    {
                        Startup.Storage.newlyAddedInsults = true;
                        return Json(true, JsonRequestBehavior.DenyGet);
                    }
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
               { "insult", "This is a test." }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("http://localhost:56109/Insults/Add", content);

            return Json(await response.Content.ReadAsStringAsync(), JsonRequestBehavior.AllowGet);
        }*/
    }
}