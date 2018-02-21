using LoCWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadBot_v4.Models
{
    public class netUserStatModel
    {
        /* |                                 73 chars                                |
         * |__________________________________24 Hrs_________________________________|
         * |   Username   |   SS    |    PS   |   Land+/-  |    GA+     |   Net+/-   |
         * |   14 chars   | 9 chars | 9 chars |  12 chars  |  12 chars  |  12 chars  |
         * |__________________________________48 Hrs_________________________________|
         * |   Username   |   SS    |    PS   |   Land+/-  |    GA+     |   Net+/-   |
         * |__________________________________72 Hrs_________________________________|
         * |   Username   |   SS    |    PS   |   Land+/-  |    GA+     |   Net+/-   |
         */
        public string userName { get; set; }
        public string SS { get; set; }
        public string PS { get; set; }
        public string LandChanges { get; set; }
        public string GhostAcreGains { get; set; }
        public string NetworthChanges { get; set; }

        public netUserStatModel()
        {

        }
        public netUserStatModel(int SS, int PS, long landChanges, long GAGains, long NetChanges, string username)
        {
            this.SS = ConvertToDisplayBreak(SS);
            this.PS = ConvertToDisplayBreak(PS);
            LandChanges = ConvertToDisplayBreak(landChanges);
            GhostAcreGains = ConvertToDisplayBreak(GAGains);
            NetworthChanges = ConvertToDisplayBreak(NetChanges);
            userName = username;
        }

        public string ConvertToDisplayBreak(int number)
        {
            string convertedNumberString = "";

            if (number < 1000)
            {
                convertedNumberString = number.ToString();
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
        public string ConvertToDisplayBreak(double number)
        {
            string convertedNumberString = "";

            if (number < 1000)
            {
                convertedNumberString = number.ToString();
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
                convertedNumberString = number.ToString();
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

    public class netStatModel
    {
        public List<netUserStatModel> UserStats { get; set; }

        public netStatModel()
        {
            UserStats = new List<netUserStatModel>();
        }
        public netStatModel(List<string> userList, List<Country> CurrentRanks, List<Country> timeSpecificRanks, List<Event> NewsData)
        {
            UserStats = new List<netUserStatModel>();
            foreach (var user in userList)
            {
                int SS = 0, PS = 0;
                long GAGains = 0, landChanges = 0, netChanges = 0;
                foreach (var country in CurrentRanks.Where(c => c.User == user))
                {
                    SS += NewsData.Where(c => c.attacker_num == country.Number && c.Type == "1").Count();
                    PS += NewsData.Where(c => c.attacker_num == country.Number && c.Type == "2").Count();
                    GAGains += (NewsData.Where(c => c.attacker_num == country.Number && (c.Type == "2" || c.Type == "1")).Select(c => c.result2).Sum() - NewsData.Where(c => c.attacker_num == country.Number && (c.Type == "2" || c.Type == "1")).Select(c => c.result1).Sum());
                    landChanges += (CurrentRanks.SingleOrDefault(c => c.Number == country.Number).Land - timeSpecificRanks.SingleOrDefault(c => c.Number == country.Number).Land);
                    netChanges += (CurrentRanks.SingleOrDefault(c => c.Number == country.Number).Networth - timeSpecificRanks.SingleOrDefault(c => c.Number == country.Number).Networth);
                }
                UserStats.Add(new netUserStatModel(
                    SS,
                    PS,
                    landChanges,
                    GAGains,
                    netChanges,
                    user
                    ));
            }
        }
    }

    public class warUserStatModel
    {
        //|_______________________________________24 Hrs_______________________________________|
        //|   Username   |  GS  |  BR  |  AB  | Missiles |  CM  |  NM  | Kills | Deaths | Defs |
        //|    14 chars  |6 char|6 char|6 char| 10 chars |6 char|6 char|7 chars| 8 chars|6 char|
        public string userName { get; set; }
        public string GS { get; set; }
        public string BR { get; set; }
        public string AB { get; set; }
        public string Missiles { get; set; }
        public string CM { get; set; }
        public string NM { get; set; }
        public string Kills { get; set; }
        public string Deaths { get; set; }
        public string Defends { get; set; }

        public warUserStatModel()
        {

        }
        public warUserStatModel(string username, int gs, int br, int ab, int missiles, int cm, int nm, int kills, int deaths, int defends)
        {
            userName = username;
            GS = ConvertToDisplayBreak(gs);
            BR = ConvertToDisplayBreak(br);
            AB = ConvertToDisplayBreak(ab);
            Missiles = ConvertToDisplayBreak(missiles);
            CM = ConvertToDisplayBreak(cm);
            NM = ConvertToDisplayBreak(nm);
            Kills = ConvertToDisplayBreak(kills);
            Deaths = ConvertToDisplayBreak(deaths);
            Defends = ConvertToDisplayBreak(defends);
        }

        public string ConvertToDisplayBreak(int number)
        {
            string convertedNumberString = "";

            if (number < 1000)
            {
                convertedNumberString = number.ToString();
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

    public class warStatModel
    {
        public List<warUserStatModel> WarUserStats { get; set; }

        public warStatModel()
        {
            WarUserStats = new List<warUserStatModel>();
        }

        public warStatModel(List<string> userList, List<Event> NewsData, List<Country> currentranks)
        {
            WarUserStats = new List<warUserStatModel>();
            
            foreach (var user in userList)
            {
                int GS = 0, BR = 0, AB = 0, Missiles = 0, CM = 0, NM = 0, Kills = 0, Deaths = 0, Defends = 0;

                foreach (var country in currentranks.Where(c => c.User == user))
                {
                    GS += NewsData.Where(c => c.attacker_num == country.Number && c.Type == "5").Count();
                    BR += NewsData.Where(c => c.attacker_num == country.Number && c.Type == "6").Count();
                    AB += NewsData.Where(c => c.attacker_num == country.Number && c.Type == "7").Count();
                    CM += NewsData.Where(c => c.attacker_num == country.Number && c.Type == "11").Count();
                    NM += NewsData.Where(c => c.attacker_num == country.Number && c.Type == "10").Count();
                    Missiles += CM + NM + NewsData.Where(c => c.attacker_num == country.Number && c.Type == "12").Count();
                    Kills += NewsData.Where(c => c.attacker_num == country.Number && c.killhit == 1).Count();
                    Deaths += NewsData.Where(c => c.defender_num == country.Number && c.killhit == 1).Count();
                    Defends += NewsData.Where(c => c.defender_num == country.Number).Count();

                }
                WarUserStats.Add(new warUserStatModel(
                    user,
                    GS,
                    BR,
                    AB,
                    Missiles,
                    CM,
                    NM,
                    Kills,
                    Deaths,
                    Defends
                    ));
            }
        }
    }

    public enum Products
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
        MilStat,
        Weapons,
        Industrial,
        Spy,
        SDI
    }

    public class marketProdStatModel
    {
        // |     Prod     |  Min  |  Avg  |  Max  |  Sold  | #Trans | %Trans |
        public Good Good { get; set; }
        public string Avg { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Sold { get; set; }
        public string NumTrans { get; set; }
        public string PercTrans { get; set; }

        public marketProdStatModel()
        {

        }
        public marketProdStatModel(Good good, double avg, double min, double max, double sold, long numTrans, double percTrans)
        {
            Good = good;
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

    public class marketStatModel
    {
        public List<marketProdStatModel> ProductStats { get; set; }

        public marketStatModel()
        {
            ProductStats = new List<marketProdStatModel>();
        }

        public marketStatModel(List<Transaction> Transactions)
        {
            ProductStats = new List<marketProdStatModel>();
            foreach (Good good in ProductList())
            {
                List<Transaction> goodTrans = Transactions.Where(c => c.good == good).ToList();
                ProductStats.Add(new marketProdStatModel(good,
                            goodTrans.Select(c => c.cost).Average(),
                            goodTrans.Select(c => c.cost).Min(),
                            goodTrans.Select(c => c.cost).Max(),
                            goodTrans.Select(c => c.quantity).Sum(),
                            goodTrans.Count(),
                            (Transactions.Count() / goodTrans.Count()) * 100
                            ));
            }
        }

        public List<Good> ProductList()
        {
            List<Good> Products = new List<Good>();
            Products.Add(Good.Troops);
            Products.Add(Good.Jets);
            Products.Add(Good.Turrets);
            Products.Add(Good.Tanks);
            Products.Add(Good.Bushels);
            Products.Add(Good.Barrels);
            Products.Add(Good.Military);
            Products.Add(Good.Medical);
            Products.Add(Good.Business);
            Products.Add(Good.Residential);
            Products.Add(Good.Agriculture);
            Products.Add(Good.Warfare);
            Products.Add(Good.Military_Strat);
            Products.Add(Good.Weapons);
            Products.Add(Good.Industrial);
            Products.Add(Good.SDI);
            Products.Add(Good.Spy);
            return Products;
        }
    }
}
