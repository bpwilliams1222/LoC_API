using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoCWebApp.Models;

namespace LoCWebApp.Controllers
{
    [RoutePrefix("api/countries")]
    public partial class CountriesController : Controller
    {
        [Route("~/api/countries/ByDate/{_date}")]
        public ActionResult ByDate(string _date)
        {
            DateTime date = DateTime.Parse(_date);
            var file = Startup.Storage.CountryFiles.SingleOrDefault(c => c.Start.DayOfYear == date.DayOfYear);
            if(file != null)
            {
                return Json(new CountryStorageModel(@"C:\Data\Countries\" + file.curReset + @"\CountryStorage\" + file.fileId + ".xml").Ranks, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<Country>(), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/countries/currentRanks")]
        public ActionResult currentRanks()
        {
            return Json(Startup.Storage.Ranks.ToList(), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/countries/bynumber/{countryNumber:int}/{daysAgo:double?}")]
        // GET: Countries
        public ActionResult ByNumber(int countryNumber, double? daysAgo)
        {
            if (daysAgo != null)
            {
                foreach (var file in Startup.Storage.CountryFiles)
                {
                    if (file.Start.DayOfYear == DateTime.UtcNow.AddDays((double)-daysAgo).DayOfYear)
                    {
                        List<Country> tempCountries = new CountryStorageModel(@"C:\WebData\Countries\" + file.curReset + @"\CountryStorage\" + file.fileId + ".xml").Ranks;
                        return Json(tempCountries.Where(c => c.Number == countryNumber), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return Json(Startup.Storage.Ranks.SingleOrDefault(c => c.Number == countryNumber), JsonRequestBehavior.AllowGet);
            }
            return Json(new List<Models.Country>(), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/countries/bytag/{Tag}/{daysAgo:double?}")]
        public ActionResult ByTag(string Tag, double? daysAgo)
        {
            if (daysAgo != null)
            {
                foreach (var file in Startup.Storage.CountryFiles)
                {
                    if (file.Start.DayOfYear == DateTime.UtcNow.AddDays((double)-daysAgo).DayOfYear)
                    {
                        List<Country> tempCountries = new CountryStorageModel(@"C:\WebData\Countries\" + file.curReset + @"\CountryStorage\" + file.fileId + ".xml").Ranks;
                        return Json(tempCountries.Where(c => c.Tag == Tag), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return Json(Startup.Storage.Ranks.Where(c => c.Tag == Tag), JsonRequestBehavior.AllowGet);
            }
            return Json(new List<Models.Country>(), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/countries/byuser/{user?}")]
        public ActionResult ByUser(string user)
        {
            if(user != null)
                return Json(Startup.Storage.Ranks.Where(c => c.User == user), JsonRequestBehavior.AllowGet);
            else
                return Json(Startup.Storage.Ranks.Where(c => c.User != null && c.User != ""), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/countries/GetClaimedCountryUserList")]
        public ActionResult GetClaimedCountryUserList()
        {
            return Json(Startup.Storage.Ranks.Where(c => c.User != "" && c.User != null).Select(c => c.User).Distinct(), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/countries/killlist")]
        public ActionResult KillList()
        {            
            return Json(Startup.Storage.Ranks.Where(c => c.KillList == true), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/countries/claim")]
        [HttpPost]
        public ActionResult Claim(int[] countries, string user)
        {
            try
            {
                foreach (int country in countries)
                {
                    Startup.Storage.Ranks.SingleOrDefault(c => c.Number == country).User = user;
                }
                Startup.Storage.RecentlyClaimedCountries.Add(new ClaimCountryModel() { countries = countries, user = user });
                return Json(true, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [Route("~/api/countries/unclaim")]
        [HttpPost]
        public ActionResult Unclaim(int[] countries)
        {
            try
            {
                foreach (int country in countries)
                {
                    Startup.Storage.Ranks.SingleOrDefault(c => c.Number == country).User = null;
                }
                Startup.Storage.RecentlyClaimedCountries.Add(new ClaimCountryModel() { countries = countries, user = null });
                return Json(true, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [Route("~/api/countries/addtokilllist")]
        [HttpPost]
        public ActionResult AddToKillList(int[] countries)
        {
            try
            {
                foreach (int country in countries)
                {
                    Startup.Storage.Ranks.SingleOrDefault(c => c.Number == country).KillList = true;
                }
                return Json(true, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [Route("~/api/countries/removefromkilllist")]
        [HttpPost]
        public ActionResult RemoveFromKillList(int[] countries)
        {
            try
            {
                foreach (int country in countries)
                {
                    Startup.Storage.Ranks.SingleOrDefault(c => c.Number == country).KillList = false;
                }
                return Json(true, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }
    }
}