using LoCWebApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace LoCWebApp.Controllers
{
    public partial class RelationsController : Controller
    {
        [Authorize(Roles = "Member")]
        [Authorize(Roles = "Leader")]
        [Route("~/relations/")]
        public ActionResult Pacts()
        {
            return View(JsonConvert.DeserializeObject<List<Relation>>(JsonModels.GetJson((Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/") + "api/relations/byset/" + Startup.Storage.Reset)));
        }
        
        [Authorize(Roles = "Leader")]
        [Route("~/relations/Delete/{tag}")]
        public async System.Threading.Tasks.Task<ActionResult> Delete(string tag)
        {
            HttpClient client = new HttpClient();

            var values = new Dictionary<string, string>
            {
               { "Tag", tag }
            };

            var content = new FormUrlEncodedContent(values);

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

            var response = await client.PostAsync(baseUrl + "api/Relations/RemovePact", content);

            return View("Pacts", JsonConvert.DeserializeObject<List<Relation>>(JsonModels.GetJson((Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/") + "api/relations/byset/" + Startup.Storage.Reset)));
        }

        [Authorize(Roles = "Leader")]
        [Route("~/relations/New")]
        public ActionResult New()
        {
            return View(new Relation());
        }
    }
}