using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using LoCWebApp.Controllers;

namespace LoCWebApp.Models
{
    public enum Good
    {
        Troops,
        Jets,
        Turrets,
        Tanks,
        Bushels,
        Barrels,
        Military,
        Medical,
        Business,
        Residential,
        Agriculture,
        Warfare,
        Military_Strat,
        Weapons,
        Industrial,
        Spy,
        SDI
    }

    public class Transaction
    {
        public int serverid { get; set; }
        public int resetid { get; set; }
        public int transactionid { get; set; }
        public DateTime timestamp { get; set; }
        public Good good { get; set; }
        public long cost { get; set; }
        public long quantity { get; set; }
        public int standing_order { get; set; }

        public Transaction()
        {

        }

        public Transaction(string[] columns)
        {
            if (columns.Count() == 8)
            {
                transactionid = Int32.Parse(columns[2]);
                serverid = Int16.Parse(columns[0]);
                resetid = Int16.Parse(columns[1]);
                timestamp = UnixTimeStampToDateTime(double.Parse(columns[3]));
                good = DetermineGood(Int16.Parse(columns[4]));
                cost = Int32.Parse(columns[5]);
                quantity = Int64.Parse(columns[6]);
                standing_order = Int16.Parse(columns[7]);
            }
        }

        public Good DetermineGood(int goodid)
        {
            switch(goodid)
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

        /*
             * Convert Unix Timestamp to DateTime Method
             * 
             * Purpose:
             * Convert a double Unix Timestamp to a DateTime object
             */
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }
    }

    public class MarketTransFileStorageModel : BaseFileStorageModel
    {
        public int lastRecordedTransactionId { get; set; }
        public int curReset { get; set; }

        public MarketTransFileStorageModel()
        {
            fileId = Guid.NewGuid();
        }

        public MarketTransFileStorageModel(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(MarketTransFileStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (MarketTransFileStorageModel)xs.Deserialize(sr);
                this.Start = temp.Start;
                this.End = temp.End;
                this.fileId = temp.fileId;
                this.lastRecordedTransactionId = temp.lastRecordedTransactionId;
            }
        }
    }

    public class MarketTransStorageModel
    {
        public List<Transaction> Transactions { get; set; }

        public MarketTransStorageModel()
        {
            Transactions = new List<Transaction>();
        }
        public MarketTransStorageModel(string file)
        {
            Transactions = new List<Transaction>();
            XmlSerializer xs = new XmlSerializer(typeof(MarketTransStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (MarketTransStorageModel)xs.Deserialize(sr);
                this.Transactions = temp.Transactions;
            }
        }
    }

    public class MarketStat
    {
        public Good good { get; set; }
        public string Avg { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Sold { get; set; }
        public string NumTrans { get; set; }
        public string PercTrans { get; set; }

        public MarketStat()
        {

        }
        public MarketStat(Good _good, double avg, double min, double max, double sold, long numTrans, double percTrans)
        {
            good = _good;
            Avg = ConvertToDisplayBreak(avg);
            Min = ConvertToDisplayBreak(min);
            Max = ConvertToDisplayBreak(max);
            Sold = ConvertToDisplayBreak(sold);
            NumTrans = ConvertToDisplayBreak(numTrans);
            PercTrans = ConvertToDisplayBreak(percTrans);
        }

        public string ConvertToDisplayBreak(double number)
        {
            string convertedNumberString = "";

            if (number < 1000)
            {
                convertedNumberString = number.ToString("N0");
            }
            else if (number <= 1000000)
            {
                convertedNumberString = (number / 1000.00).ToString("N1") + "K";
            }
            else if (number <= 1000000000)
            {
                convertedNumberString = (number / 1000000.00).ToString("N1") + "M";
            }
            else
            {
                convertedNumberString = (number / 1000000000.00).ToString("N1") + "B";
            }

            return convertedNumberString;
        }
        public string ConvertToDisplayBreak(long number)
        {
            string convertedNumberString = "";

            if (number < 1000)
            {
                convertedNumberString = number.ToString("N0");
            }
            else if (number <= 1000000)
            {
                convertedNumberString = (number / 1000.00).ToString("N1") + "K";
            }
            else if (number <= 1000000000)
            {
                convertedNumberString = (number / 1000000.00).ToString("N1") + "M";
            }
            else
            {
                convertedNumberString = (number / 1000000000.00).ToString("N1") + "B";
            }

            return convertedNumberString;
        }
    }

    public class MarketStatsModel
    {
        public List<MarketStat> MarketStats { get; set; }

        public MarketStatsModel()
        {
            MarketStats = new List<MarketStat>();
        }

        public MarketStatsModel(List<Transaction> Transactions)
        {
            MarketStats = new List<MarketStat>();
            foreach (Good good in GoodsList())
            {
                List<Transaction> goodTrans;
                switch (good)
                {
                    case Good.Agriculture:
                        goodTrans = Transactions.Where(c => c.good == Good.Agriculture).ToList();                        
                        break;
                    case Good.Barrels:
                        goodTrans = Transactions.Where(c => c.good == Good.Barrels).ToList();
                        break;
                    case Good.Bushels:
                        goodTrans = Transactions.Where(c => c.good == Good.Bushels).ToList();
                        break;
                    case Good.Business:
                        goodTrans = Transactions.Where(c => c.good == Good.Business).ToList();
                        break;
                    case Good.Industrial:
                        goodTrans = Transactions.Where(c => c.good == Good.Industrial).ToList();
                        break;
                    case Good.Jets:
                        goodTrans = Transactions.Where(c => c.good == Good.Jets).ToList();
                        break;
                    case Good.Medical:
                        goodTrans = Transactions.Where(c => c.good == Good.Medical).ToList();
                        break;
                    case Good.Military:
                        goodTrans = Transactions.Where(c => c.good == Good.Military).ToList();
                        break;
                    case Good.Military_Strat:
                        goodTrans = Transactions.Where(c => c.good == Good.Military_Strat).ToList();
                        break;
                    case Good.Residential:
                        goodTrans = Transactions.Where(c => c.good == Good.Residential).ToList();
                        break;
                    case Good.SDI:
                        goodTrans = Transactions.Where(c => c.good == Good.SDI).ToList();
                        break;
                    case Good.Spy:
                        goodTrans = Transactions.Where(c => c.good == Good.Spy).ToList();
                        break;
                    case Good.Tanks:
                        goodTrans = Transactions.Where(c => c.good == Good.Tanks).ToList();
                        break;
                    case Good.Troops:
                        goodTrans = Transactions.Where(c => c.good == Good.Troops).ToList();
                        break;
                    case Good.Turrets:
                        goodTrans = Transactions.Where(c => c.good == Good.Turrets).ToList();
                        break;
                    case Good.Warfare:
                        goodTrans = Transactions.Where(c => c.good == Good.Warfare).ToList();
                        break;
                    case Good.Weapons:
                        goodTrans = Transactions.Where(c => c.good == Good.Weapons).ToList();
                        break;
                    default:
                        goodTrans = Transactions.Where(c => c.good == Good.Troops).ToList();
                        break;
                }
                MarketStats.Add(new MarketStat(good,
                            (goodTrans.Count > 0) ? goodTrans.Select(c => c.cost).Average() : 0,
                            (goodTrans.Count > 0) ? goodTrans.Select(c => c.cost).Min() : 0,
                            (goodTrans.Count > 0) ? goodTrans.Select(c => c.cost).Max() : 0,
                            (goodTrans.Count > 0) ? goodTrans.Select(c => c.quantity).Sum() : 0,
                            (goodTrans.Count > 0) ? goodTrans.Count() : 0,
                            (goodTrans.Count > 0) ? (Transactions.Count() / goodTrans.Count()) * 100 : 0
                            ));
            }
        }

        public List<Good> GoodsList()
        {
            List<Good> Goods = new List<Good>();
            Goods.Add(Good.Troops);
            Goods.Add(Good.Jets);
            Goods.Add(Good.Turrets);
            Goods.Add(Good.Tanks);
            Goods.Add(Good.Bushels);
            Goods.Add(Good.Barrels);
            Goods.Add(Good.Military);
            Goods.Add(Good.Medical);
            Goods.Add(Good.Business);
            Goods.Add(Good.Residential);
            Goods.Add(Good.Agriculture);
            Goods.Add(Good.Warfare);
            Goods.Add(Good.Military_Strat);
            Goods.Add(Good.Weapons);
            Goods.Add(Good.Industrial);
            Goods.Add(Good.SDI);
            Goods.Add(Good.Spy);
            return Goods;
        }
    }
}