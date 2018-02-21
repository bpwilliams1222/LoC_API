using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoCWebApp.Models;

namespace LoCWebApp.Controllers
{
    [RoutePrefix("api/market")]
    public class MarketController : Controller
    {
        [Route("~/api/market/bygoodid/{goodId:int}/{start}/{end}")]
        public ActionResult ByGoodId(int goodId, string start, string end)
        {
            List<Transaction> Transactions = new List<Transaction>();
            List<Transaction> GoodTransactions = new List<Transaction>();
            DateTime startDate, endDate;
            Good good = DetermineGood(goodId);
            if (start != null && end != null)
            {
                var splitDate = start.Split('-');
                startDate = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[0]), int.Parse(splitDate[1]));
                splitDate = end.Split('-');
                endDate = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[0]), int.Parse(splitDate[1]));
                foreach (MarketTransFileStorageModel file in Startup.Storage.MarketFiles)
                {
                    if (file.Start >= startDate && file.End <= endDate)
                    {
                        Transactions.AddRange(new MarketTransStorageModel(@"C:\WebData\Market\" + file.curReset + @"\MarketTransactionStorage\" + file.fileId + ".xml").Transactions.Where(c => (c.timestamp >= startDate && c.timestamp <= endDate)));
                    }
                }
            }
            else
            {
                foreach (MarketTransFileStorageModel file in Startup.Storage.MarketFiles)
                {                    
                    Transactions.AddRange(new MarketTransStorageModel(@"C:\WebData\Market\" + file.curReset + @"\MarketTransactionStorage\" + file.fileId + ".xml").Transactions);                    
                }
            }
            
            GoodTransactions = Transactions.Where(c => c.good == good).ToList();

            return Json(new MarketStat(
                good,
                GoodTransactions.Average(c => c.cost),
                GoodTransactions.Min(c => c.cost),
                GoodTransactions.Max(c => c.cost),
                GoodTransactions.Sum(c => c.quantity),
                GoodTransactions.Count(),
                GoodTransactions.Count() / Transactions.Count()
                ), JsonRequestBehavior.AllowGet);
        }

        [Route("~/api/market/allgoodsbydate/{start}/{end}")]
        public ActionResult AllGoodsByDate(string start, string end)
        {
            MarketStatsModel Stats;
            List<Transaction> Transactions = new List<Transaction>();
            DateTime startDate, endDate;
            if (start != null && end != null)
            {
                var splitDate = start.Split('-');
                startDate = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[0]), int.Parse(splitDate[1]));
                splitDate = end.Split('-');
                endDate = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[0]), int.Parse(splitDate[1]));
                foreach (MarketTransFileStorageModel file in Startup.Storage.MarketFiles)
                {
                    if (file.Start >= startDate && file.End <= endDate)
                    {
                        Transactions.AddRange(new MarketTransStorageModel(@"C:\WebData\Market\" + file.curReset + @"\MarketTransactionStorage\" + file.fileId + ".xml").Transactions.Where(c => (c.timestamp >= startDate && c.timestamp <= endDate)));
                    }
                }
            }
            else
            {
                foreach (MarketTransFileStorageModel file in Startup.Storage.MarketFiles)
                {
                    Transactions.AddRange(new MarketTransStorageModel(@"C:\WebData\Market\" + file.curReset + @"\MarketTransactionStorage\" + file.fileId + ".xml").Transactions);
                }
            }
            Stats = new MarketStatsModel(Transactions);

            return Json(Stats, JsonRequestBehavior.AllowGet);
        }

        public Good DetermineGood(int goodid)
        {
            switch (goodid)
            {
                case 1:
                    return Good.Troops;
                case 2:
                    return Good.Jets;
                case 3:
                    return Good.Turrets;
                case 4:
                    return Good.Tanks;
                case 5:
                    return Good.Bushels;
                case 6:
                    return Good.Barrels;
                case 7:
                    return Good.Military;
                case 8:
                    return Good.Medical;
                case 9:
                    return Good.Business;
                case 10:
                    return Good.Residential;
                case 11:
                    return Good.Agriculture;
                case 12:
                    return Good.Warfare;
                case 13:
                    return Good.Military_Strat;
                case 14:
                    return Good.Weapons;
                case 15:
                    return Good.Industrial;
                case 16:
                    return Good.Spy;
                case 17:
                    return Good.SDI;
                default:
                    return Good.Troops;
            }
        }
    }
}