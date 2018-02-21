using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using LoCWebApp.Models;
using Newtonsoft.Json;

namespace LoCWebApp.Controllers
{
    public partial class RelationsController : Controller
    {
        [Route("~/api/relations/bytag/{tag}")]
        public ActionResult ByTag(string tag)
        {
            return Json(Startup.Storage.Relations.FirstOrDefault(c => c.Tag == tag), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/relations/bypact/{_pact}")]
        public ActionResult ByPact(string _pact)
        {
            PactTypes pact = DeterminePactType(_pact);
            return Json(Startup.Storage.Relations.Where(c => c.PactType == pact), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/relations/byselfrenewing/{renewing:bool}")]
        public ActionResult BySelfRenewing(bool renewing)
        {
            return Json(Startup.Storage.Relations.Where(c => c.SelfRenewing == renewing), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/relations/byset/{set:int}")]
        public ActionResult BySet(int set)
        {
            List<Relation> relations = new List<Relation>();
            if (set == Startup.Storage.Reset)
            {
                relations = Startup.Storage.Relations;
            }
            else
            {
                XmlSerializer xs = new XmlSerializer(typeof(RelationStorageModel));
                if (Startup.Storage.CheckFileSystem(StorageOptions.Market))
                {
                    var files = Directory.GetFiles(@"C:\WebData\Relations\" + set);
                    if (files.Count() > 0)
                    {
                        using (var sr = new StreamReader(files[0]))
                        {
                            relations = ((RelationStorageModel)xs.Deserialize(sr)).Relations;
                        }
                    }
                }
            }
            return Json(relations, JsonRequestBehavior.AllowGet);
        }
        
        [Route("~/api/relations/addpact")]
        [HttpPost]
        public ActionResult AddPact(Relation relation)
        {
            if (relation.ValidateRelation())
            {
                Startup.Storage.Relations.Add(relation);
                Startup.Storage.newlyAddedRelations = true;
                if (Request.UrlReferrer.AbsolutePath == "/relations/New")
                    return View("Pacts", JsonConvert.DeserializeObject<List<Relation>>(JsonModels.GetJson((Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/") + "api/relations/byset/" + Startup.Storage.Reset)));
                else
                    return Json(true, JsonRequestBehavior.DenyGet);
            }
            if (Request.UrlReferrer.AbsolutePath == "/relations/New")
                return View("Pacts", JsonConvert.DeserializeObject<List<Relation>>(JsonModels.GetJson((Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/") + "api/relations/byset/" + Startup.Storage.Reset)));
            else
                return Json(false, JsonRequestBehavior.DenyGet);
        }

        [Route("~/api/relations/removepact")]
        [HttpPost]
        public ActionResult RemovePact(string Tag)
        {
            if(Startup.Storage.Relations.Exists(c => c.Tag == Tag))
            {
                if(Startup.Storage.Relations.RemoveAll(c => c.Tag == Tag) > 0)
                {
                    Startup.Storage.newlyAddedRelations = true;
                    return Json(true, JsonRequestBehavior.DenyGet);
                }
            }
            return Json(false, JsonRequestBehavior.DenyGet);
        }
        
        public PactTypes DeterminePactType(string pact)
        {
            switch (pact.ToLower())
            {
                case "dnh":
                    return PactTypes.DNH;
                case "ldp":
                    return PactTypes.LDP;
                case "fdp":
                    return PactTypes.FDP;
                case "nap":
                    return PactTypes.NAP;
                case "unap":
                    return PactTypes.uNAP;
                default:
                    return PactTypes.DNH;
            }
        }

        /*public ActionResult Test()
        {
            return Json(AddPact(new Relation("AoDT", "uNAP", "", false)), JsonRequestBehavior.DenyGet);
        }*/
    }
}