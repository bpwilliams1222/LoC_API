
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LoCWebApp.Models
{
    public class SpyOpsFileStorageModel : BaseFileStorageModel
    {
        public List<int> CountryNumbersIncluded { get; set; }
        public int curReset { get; set; }

        public SpyOpsFileStorageModel()
        {
            fileId = Guid.NewGuid();
        }

        public SpyOpsFileStorageModel(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(SpyOpsFileStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (SpyOpsFileStorageModel)xs.Deserialize(sr);
                this.Start = temp.Start;
                this.End = temp.End;
                this.fileId = temp.fileId;
                this.CountryNumbersIncluded = temp.CountryNumbersIncluded;
                this.curReset = temp.curReset;
            }
        }
    }

    public class SpyOp
    {
        public Guid entry_id { get; set; }
        public string uploader_api_key { get; set; }
        public int subject_number { get; set; }
        public int uploader_number { get; set; }
        public int serverid { get; set; }
        public string type { get; set; }
        public string json { get; set; }
        public DateTime timestamp { get; set; }

        public SpyOp()
        {
            entry_id = Guid.NewGuid();
            timestamp = DateTime.UtcNow;
        }

        public void SaveOpToXML()
        {
            XmlSerializer xs = new XmlSerializer(typeof(SpyOp));
            using (TextWriter tw = new StreamWriter(@"C:\WebData\newSpyOps\newOp" + this.subject_number + "_" + this.timestamp.ToFileTime() + ".xml"))
            {
                xs.Serialize(tw, this);
                tw.Close();
            }
            using (TextWriter tw = new StreamWriter(@"C:\WebData\newSpyOps\Storage\newOp" + this.subject_number + "_" + this.timestamp.ToFileTime() + ".xml"))
            {
                xs.Serialize(tw, this);
                tw.Close();
            }
        }
    }

    public class SpyOpInfo
    {
        public SpyOpInfo()
        {

        }

        public SpyOpInfo(string json, string opType, string apiKey)
        {
            JObject jObject = JObject.Parse(json);
            JToken spyOp = jObject;
            int intTemp;
            long longTemp;
            double doubleTemp;
            if (opType == "selfop")
            {
                this.apiKey = apiKey;
                if (int.TryParse(spyOp["serverid"].ToString(), out intTemp))
                    serverId = intTemp;
                if (int.TryParse(spyOp["resetid"].ToString(), out intTemp))
                    resetId = intTemp;
                if (double.TryParse(spyOp["defense_bonus"].ToString(), out doubleTemp))
                    defenseBonus = doubleTemp;
                if (long.TryParse(spyOp["nw_mil"].ToString(), out longTemp))
                    Military_Net = longTemp;
                if (long.TryParse(spyOp["nw_tech"].ToString(), out longTemp))
                    Technology_Net = longTemp;
                if (long.TryParse(spyOp["nw_land"].ToString(), out longTemp))
                    Land_Net = longTemp;
                if (long.TryParse(spyOp["nw_other"].ToString(), out longTemp))
                    Other_Net = longTemp;
                if (long.TryParse(spyOp["nw_market"].ToString(), out longTemp))
                    Market_Net = longTemp;
                if (int.TryParse(spyOp["atwar"].ToString(), out intTemp))
                    atWar = intTemp;
                GDI = (string)spyOp["gdi"];
                if (int.TryParse(spyOp["tpt"].ToString(), out intTemp))
                    TechPerTurn = intTemp;
                if (int.TryParse(spyOp["explore_rate"].ToString(), out intTemp))
                    ExploreRate = intTemp;
                if (int.TryParse(spyOp["bpt"].ToString(), out intTemp))
                    BuildingRate = intTemp;
                if (long.TryParse(spyOp["expense_spy"].ToString(), out longTemp))
                    SpiesExp = longTemp;
                if (long.TryParse(spyOp["expense_tr"].ToString(), out longTemp))
                    TroopsExp = longTemp;
                if (long.TryParse(spyOp["expense_j"].ToString(), out longTemp))
                    JetsExp = longTemp;
                if (long.TryParse(spyOp["expense_tu"].ToString(), out longTemp))
                    TurretsExp = longTemp;
                if (long.TryParse(spyOp["expense_ta"].ToString(), out longTemp))
                    TanksExp = longTemp;
                if (long.TryParse(spyOp["tech_tot"].ToString(), out longTemp))
                    TechTotal = longTemp;
                if (double.TryParse(spyOp["pt_med"].ToString(), out doubleTemp))
                    MedicalPerc = doubleTemp;
                if (double.TryParse(spyOp["pt_mil"].ToString(), out doubleTemp))
                    MilitaryPerc = doubleTemp;
                if (double.TryParse(spyOp["pt_bus"].ToString(), out doubleTemp))
                    BusinessPerc = doubleTemp;
                if (double.TryParse(spyOp["pt_res"].ToString(), out doubleTemp))
                    ResidentialPerc = doubleTemp;
                if (double.TryParse(spyOp["pt_agri"].ToString(), out doubleTemp))
                    AgriculturePerc = doubleTemp;
                if (double.TryParse(spyOp["pt_war"].ToString(), out doubleTemp))
                    WarfarePerc = doubleTemp;
                if (double.TryParse(spyOp["pt_ms"].ToString(), out doubleTemp))
                    MilitaryStratPerc = doubleTemp;
                if (double.TryParse(spyOp["pt_weap"].ToString(), out doubleTemp))
                    WeaponsPerc = doubleTemp;
                if (double.TryParse(spyOp["pt_indy"].ToString(), out doubleTemp))
                    IndustrialPerc = doubleTemp;
                if (double.TryParse(spyOp["pt_spy"].ToString(), out doubleTemp))
                    SpyPerc = doubleTemp;
                if (double.TryParse(spyOp["pt_sdi"].ToString(), out doubleTemp))
                    SDIPerc = doubleTemp;
                if (long.TryParse(spyOp["corruption"].ToString(), out longTemp))
                    CorruptionExp = longTemp;
                Clan = (string)spyOp["clan"];
                Gov = (string)spyOp["govt"];
                CountryName = (string)spyOp["cname"];
                if (int.TryParse(spyOp["cnum"].ToString(), out intTemp))
                    CountryNumber = intTemp;
                if (int.TryParse(spyOp["turns"].ToString(), out intTemp))
                    turnsLeft = intTemp;
                if (int.TryParse(spyOp["turns_played"].ToString(), out intTemp))
                    turnsTaken = intTemp;
                if (int.TryParse(spyOp["turns_stored"].ToString(), out intTemp))
                    turnsStored = intTemp;
                if (int.TryParse(spyOp["rank"].ToString(), out intTemp))
                    Rank = intTemp;
                if (long.TryParse(spyOp["money"].ToString(), out longTemp))
                    Money = longTemp;
                if (long.TryParse(spyOp["networth"].ToString(), out longTemp))
                    Networth = longTemp;
                if (int.TryParse(spyOp["land"].ToString(), out intTemp))
                    Land = intTemp;
                if (int.TryParse(spyOp["pop"].ToString(), out intTemp))
                    Population = intTemp;
                if (long.TryParse(spyOp["food"].ToString(), out longTemp))
                    FoodStore = longTemp;
                if (long.TryParse(spyOp["foodpro"].ToString(), out longTemp))
                    FoodProd = longTemp;
                if (long.TryParse(spyOp["foodcon"].ToString(), out longTemp))
                    FoodConsumption = longTemp;
                if (long.TryParse(spyOp["foodnet"].ToString(), out longTemp))
                    FoodNetChange = longTemp;
                if (long.TryParse(spyOp["oil"].ToString(), out longTemp))
                    OilStore = longTemp;
                if (long.TryParse(spyOp["revenue"].ToString(), out longTemp))
                    TaxRevenues = longTemp;
                if (long.TryParse(spyOp["taxrate"].ToString(), out longTemp))
                    TaxRate = longTemp;
                if (long.TryParse(spyOp["pci"].ToString(), out longTemp))
                    PerCapita = longTemp;
                if (long.TryParse(spyOp["expenses"].ToString(), out longTemp))
                    Expenses = longTemp;
                if (long.TryParse(spyOp["expensesmil"].ToString(), out longTemp))
                    MilExpenses = longTemp;
                if (long.TryParse(spyOp["expensesally"].ToString(), out longTemp))
                    AllianceGDIExp = longTemp;
                if (long.TryParse(spyOp["expensesland"].ToString(), out longTemp))
                    LandExp = longTemp;
                if (long.TryParse(spyOp["expenses"].ToString(), out longTemp))
                    Expenses = longTemp;
                if (long.TryParse(spyOp["netincome"].ToString(), out longTemp))
                    NetIncome = longTemp;
                if (int.TryParse(spyOp["unbuilt"].ToString(), out intTemp))
                    UnusedLands = intTemp;
                if (int.TryParse(spyOp["b_ent"].ToString(), out intTemp))
                    EnterpriseZones = intTemp;
                if (int.TryParse(spyOp["b_res"].ToString(), out intTemp))
                    Residences = intTemp;
                if (int.TryParse(spyOp["b_indy"].ToString(), out intTemp))
                    IndustrialComplexes = intTemp;
                if (int.TryParse(spyOp["b_mb"].ToString(), out intTemp))
                    MilitaryBases = intTemp;
                if (int.TryParse(spyOp["b_lab"].ToString(), out intTemp))
                    ResearchLabs = intTemp;
                if (int.TryParse(spyOp["b_farm"].ToString(), out intTemp))
                    Farms = intTemp;
                if (int.TryParse(spyOp["b_rig"].ToString(), out intTemp))
                    OilRigs = intTemp;
                if (int.TryParse(spyOp["b_cs"].ToString(), out intTemp))
                    ConstructionSites = intTemp;
                if (long.TryParse(spyOp["m_spy"].ToString(), out longTemp))
                    Spies = longTemp;
                if (long.TryParse(spyOp["m_tr"].ToString(), out longTemp))
                    Troops = longTemp;
                if (long.TryParse(spyOp["m_j"].ToString(), out longTemp))
                    Jets = longTemp;
                if (long.TryParse(spyOp["m_tu"].ToString(), out longTemp))
                    Turrets = longTemp;
                if (long.TryParse(spyOp["m_ta"].ToString(), out longTemp))
                    Tanks = longTemp;
                if (int.TryParse(spyOp["m_cm"].ToString(), out intTemp))
                    ChemicalMissles = intTemp;
                if (int.TryParse(spyOp["m_nm"].ToString(), out intTemp))
                    NuclearMissles = intTemp;
                if (int.TryParse(spyOp["m_em"].ToString(), out intTemp))
                    CruiseMissles = intTemp;
                if (long.TryParse(spyOp["t_mil"].ToString(), out longTemp))
                    MilitaryPts = longTemp;
                if (long.TryParse(spyOp["t_med"].ToString(), out longTemp))
                    MedicalPts = longTemp;
                if (long.TryParse(spyOp["t_bus"].ToString(), out longTemp))
                    BusinessPts = longTemp;
                if (long.TryParse(spyOp["t_res"].ToString(), out longTemp))
                    ResidentialPts = longTemp;
                if (long.TryParse(spyOp["t_agri"].ToString(), out longTemp))
                    AgriculturePts = longTemp;
                if (long.TryParse(spyOp["t_war"].ToString(), out longTemp))
                    WarfarePts = longTemp;
                if (long.TryParse(spyOp["t_ms"].ToString(), out longTemp))
                    MilitaryStratPts = longTemp;
                if (long.TryParse(spyOp["t_weap"].ToString(), out longTemp))
                    WeaponsPts = longTemp;
                if (long.TryParse(spyOp["t_indy"].ToString(), out longTemp))
                    IndustrialPts = longTemp;
                if (long.TryParse(spyOp["t_spy"].ToString(), out longTemp))
                    SpyPts = longTemp;
                if (long.TryParse(spyOp["t_sdi"].ToString(), out longTemp))
                    SDIPts = longTemp;
            }
            else if (opType == "spy")
            {
                this.apiKey = apiKey;
                if (int.TryParse(spyOp["serverid"].ToString(), out intTemp))
                    serverId = intTemp;
                if (int.TryParse(spyOp["resetid"].ToString(), out intTemp))
                    resetId = intTemp;
                Gov = (string)spyOp["govt"];
                CountryName = (string)spyOp["cname"];
                if (int.TryParse(spyOp["cnum"].ToString(), out intTemp))
                    CountryNumber = intTemp;
                if (int.TryParse(spyOp["turns"].ToString(), out intTemp))
                    turnsLeft = intTemp;
                if (int.TryParse(spyOp["turns_played"].ToString(), out intTemp))
                    turnsTaken = intTemp;
                if (int.TryParse(spyOp["turns_stored"].ToString(), out intTemp))
                    turnsStored = intTemp;
                if (int.TryParse(spyOp["rank"].ToString(), out intTemp))
                    Rank = intTemp;
                if (long.TryParse(spyOp["money"].ToString(), out longTemp))
                    Money = longTemp;
                if (long.TryParse(spyOp["networth"].ToString(), out longTemp))
                    Networth = longTemp;
                if (int.TryParse(spyOp["land"].ToString(), out intTemp))
                    Land = intTemp;
                if (int.TryParse(spyOp["pop"].ToString(), out intTemp))
                    Population = intTemp;
                if (long.TryParse(spyOp["food"].ToString(), out longTemp))
                    FoodStore = longTemp;
                if (long.TryParse(spyOp["foodpro"].ToString(), out longTemp))
                    FoodProd = longTemp;
                if (long.TryParse(spyOp["foodcon"].ToString(), out longTemp))
                    FoodConsumption = longTemp;
                if (long.TryParse(spyOp["foodnet"].ToString(), out longTemp))
                    FoodNetChange = longTemp;
                if (long.TryParse(spyOp["oil"].ToString(), out longTemp))
                    OilStore = longTemp;
                if (long.TryParse(spyOp["revenue"].ToString(), out longTemp))
                    TaxRevenues = longTemp;
                if (long.TryParse(spyOp["taxrate"].ToString(), out longTemp))
                    TaxRate = longTemp;
                if (long.TryParse(spyOp["pci"].ToString(), out longTemp))
                    PerCapita = longTemp;
                if (long.TryParse(spyOp["expenses"].ToString(), out longTemp))
                    Expenses = longTemp;
                if (long.TryParse(spyOp["expensesmil"].ToString(), out longTemp))
                    MilExpenses = longTemp;
                if (long.TryParse(spyOp["expensesally"].ToString(), out longTemp))
                    AllianceGDIExp = longTemp;
                if (long.TryParse(spyOp["expensesland"].ToString(), out longTemp))
                    LandExp = longTemp;
                if (long.TryParse(spyOp["expenses"].ToString(), out longTemp))
                    Expenses = longTemp;
                if (long.TryParse(spyOp["netincome"].ToString(), out longTemp))
                    NetIncome = longTemp;
                if (int.TryParse(spyOp["unbuilt"].ToString(), out intTemp))
                    UnusedLands = intTemp;
                if (int.TryParse(spyOp["b_ent"].ToString(), out intTemp))
                    EnterpriseZones = intTemp;
                if (int.TryParse(spyOp["b_res"].ToString(), out intTemp))
                    Residences = intTemp;
                if (int.TryParse(spyOp["b_indy"].ToString(), out intTemp))
                    IndustrialComplexes = intTemp;
                if (int.TryParse(spyOp["b_mb"].ToString(), out intTemp))
                    MilitaryBases = intTemp;
                if (int.TryParse(spyOp["b_lab"].ToString(), out intTemp))
                    ResearchLabs = intTemp;
                if (int.TryParse(spyOp["b_farm"].ToString(), out intTemp))
                    Farms = intTemp;
                if (int.TryParse(spyOp["b_rig"].ToString(), out intTemp))
                    OilRigs = intTemp;
                if (int.TryParse(spyOp["b_cs"].ToString(), out intTemp))
                    ConstructionSites = intTemp;
                if (long.TryParse(spyOp["m_spy"].ToString(), out longTemp))
                    Spies = longTemp;
                if (long.TryParse(spyOp["m_tr"].ToString(), out longTemp))
                    Troops = longTemp;
                if (long.TryParse(spyOp["m_j"].ToString(), out longTemp))
                    Jets = longTemp;
                if (long.TryParse(spyOp["m_tu"].ToString(), out longTemp))
                    Turrets = longTemp;
                if (long.TryParse(spyOp["m_ta"].ToString(), out longTemp))
                    Tanks = longTemp;
                if (int.TryParse(spyOp["m_cm"].ToString(), out intTemp))
                    ChemicalMissles = intTemp;
                if (int.TryParse(spyOp["m_nm"].ToString(), out intTemp))
                    NuclearMissles = intTemp;
                if (int.TryParse(spyOp["m_em"].ToString(), out intTemp))
                    CruiseMissles = intTemp;
                if (long.TryParse(spyOp["t_mil"].ToString(), out longTemp))
                    MilitaryPts = longTemp;
                if (long.TryParse(spyOp["t_med"].ToString(), out longTemp))
                    MedicalPts = longTemp;
                if (long.TryParse(spyOp["t_bus"].ToString(), out longTemp))
                    BusinessPts = longTemp;
                if (long.TryParse(spyOp["t_res"].ToString(), out longTemp))
                    ResidentialPts = longTemp;
                if (long.TryParse(spyOp["t_agri"].ToString(), out longTemp))
                    AgriculturePts = longTemp;
                if (long.TryParse(spyOp["t_war"].ToString(), out longTemp))
                    WarfarePts = longTemp;
                if (long.TryParse(spyOp["t_ms"].ToString(), out longTemp))
                    MilitaryStratPts = longTemp;
                if (long.TryParse(spyOp["t_weap"].ToString(), out longTemp))
                    WeaponsPts = longTemp;
                if (long.TryParse(spyOp["t_indy"].ToString(), out longTemp))
                    IndustrialPts = longTemp;
                if (long.TryParse(spyOp["t_spy"].ToString(), out longTemp))
                    SpyPts = longTemp;
                if (long.TryParse(spyOp["t_sdi"].ToString(), out longTemp))
                    SDIPts = longTemp;
                MilitaryPerc = CalculateAllTech(MilitaryPts, "t_mil");
                MedicalPerc = CalculateAllTech(MedicalPts, "t_med");
                BusinessPerc = CalculateAllTech(BusinessPts, "t_bus");
                ResidentialPerc = CalculateAllTech(ResidentialPts, "t_res");
                AgriculturePerc = CalculateAllTech(AgriculturePts, "t_agri");
                WarfarePerc = CalculateAllTech(WarfarePts, "t_war");
                MilitaryStratPerc = CalculateAllTech(MilitaryStratPts, "t_ms");
                WeaponsPerc = CalculateAllTech(WeaponsPts, "t_weap");
                IndustrialPerc = CalculateAllTech(IndustrialPts, "t_indy");
                SpyPerc = CalculateAllTech(SpyPts, "t_spy");
                SDIPerc = CalculateAllTech(SDIPts, "t_sdi");
                Clan = (Startup.Storage.Ranks.SingleOrDefault(c => c.Number == CountryNumber) != null) ? Startup.Storage.Ranks.SingleOrDefault(c => c.Number == CountryNumber).Tag : "";
            }
        }

        #region Properties
        [JsonProperty("resetid")]
        public int resetId { get; set; }
        [JsonProperty("serverid")]
        public int serverId { get; set; }
        public string apiKey { get; set; }
        public int Att { get; set; }
        public int Def { get; set; }
        public Nullable<DateTime> LastSpyOp { get; set; }
        public Nullable<DateTime> LastActivity { get; set; }
        public bool Available { get; set; }
        [JsonProperty("govt")]
        public string Gov { get; set; }
        [JsonProperty("clan")]
        public string Clan { get; set; }
        [JsonProperty("cname")]
        public string CountryName { get; set; }
        [JsonProperty("cnum")]
        public int CountryNumber { get; set; }
        public int entry_id { get; set; }
        [JsonProperty("defense_bonus")]
        public double defenseBonus { get; set; }
        [JsonProperty("turns")]
        public int turnsLeft { get; set; }
        [JsonProperty("turns_played")]
        public int turnsTaken { get; set; }
        [JsonProperty("turns_stored")]
        public int turnsStored { get; set; }
        [JsonProperty("rank")]
        public int Rank { get; set; }
        [JsonProperty("networth")]
        public long Networth { get; set; }
        [JsonProperty("land")]
        public int Land { get; set; }
        [JsonProperty("money")]
        public long Money { get; set; }
        [JsonProperty("pop")]
        public long Population { get; set; }
        [JsonProperty("atwar")]
        public int atWar { get; set; }
        [JsonProperty("gdi")]
        public string GDI { get; set; }
        [JsonProperty("b_ent")]
        public int EnterpriseZones { get; set; }
        [JsonProperty("b_res")]
        public int Residences { get; set; }
        [JsonProperty("b_indy")]
        public int IndustrialComplexes { get; set; }
        [JsonProperty("b_mb")]
        public int MilitaryBases { get; set; }
        [JsonProperty("b_lab")]
        public int ResearchLabs { get; set; }
        [JsonProperty("b_farm")]
        public int Farms { get; set; }
        [JsonProperty("b_rig")]
        public int OilRigs { get; set; }
        [JsonProperty("b_cs")]
        public int ConstructionSites { get; set; }
        [JsonProperty("unbuilt")]
        public int UnusedLands { get; set; }
        [JsonProperty("t_mil")]
        public long MilitaryPts { get; set; }
        [JsonProperty("pt_mil")]
        public double MilitaryPerc { get; set; }
        [JsonProperty("t_med")]
        public long MedicalPts { get; set; }
        [JsonProperty("pt_med")]
        public double MedicalPerc { get; set; }
        [JsonProperty("t_bus")]
        public long BusinessPts { get; set; }
        [JsonProperty("pt_bus")]
        public double BusinessPerc { get; set; }
        [JsonProperty("t_res")]
        public long ResidentialPts { get; set; }
        [JsonProperty("pt_res")]
        public double ResidentialPerc { get; set; }
        [JsonProperty("t_agri")]
        public long AgriculturePts { get; set; }
        [JsonProperty("pt_agri")]
        public double AgriculturePerc { get; set; }
        [JsonProperty("t_war")]
        public long WarfarePts { get; set; }
        [JsonProperty("pt_war")]
        public double WarfarePerc { get; set; }
        [JsonProperty("t_ms")]
        public long MilitaryStratPts { get; set; }
        [JsonProperty("pt_ms")]
        public double MilitaryStratPerc { get; set; }
        [JsonProperty("t_weap")]
        public long WeaponsPts { get; set; }
        [JsonProperty("pt_weap")]
        public double WeaponsPerc { get; set; }
        [JsonProperty("t_indy")]
        public long IndustrialPts { get; set; }
        [JsonProperty("pt_indy")]
        public double IndustrialPerc { get; set; }
        [JsonProperty("t_spy")]
        public long SpyPts { get; set; }
        [JsonProperty("pt_spy")]
        public double SpyPerc { get; set; }
        [JsonProperty("t_sdi")]
        public long SDIPts { get; set; }
        [JsonProperty("pt_sdi")]
        public double SDIPerc { get; set; }
        [JsonProperty("revenue")]
        public long TaxRevenues { get; set; }
        [JsonProperty("taxrate")]
        public double TaxRate { get; set; }
        [JsonProperty("pci")]
        public double PerCapita { get; set; }
        [JsonProperty("expenses")]
        public long Expenses { get; set; }
        [JsonProperty("netincome")]
        public long NetIncome { get; set; }
        public long Cashing { get; set; }
        [JsonProperty("food")]
        public long FoodStore { get; set; }
        [JsonProperty("foodpro")]
        public long FoodProd { get; set; }
        [JsonProperty("foodcon")]
        public long FoodConsumption { get; set; }
        [JsonProperty("foodnet")]
        public long FoodNetChange { get; set; }
        [JsonProperty("oil")]
        public long OilStore { get; set; }
        [JsonProperty("oilpro")]
        public long OilProduction { get; set; }
        [JsonProperty("bpt")]
        public double BuildingRate { get; set; }
        [JsonProperty("explore_rate")]
        public int ExploreRate { get; set; }
        [JsonProperty("m_spy")]
        public long Spies { get; set; }
        [JsonProperty("m_tr")]
        public long Troops { get; set; }
        [JsonProperty("m_j")]
        public long Jets { get; set; }
        [JsonProperty("m_tu")]
        public long Turrets { get; set; }
        [JsonProperty("m_ta")]
        public long Tanks { get; set; }
        [JsonProperty("m_nm")]
        public int NuclearMissles { get; set; }
        [JsonProperty("m_cm")]
        public int ChemicalMissles { get; set; }
        [JsonProperty("m_em")]
        public int CruiseMissles { get; set; }
        [JsonProperty("expensesmil")]
        public long MilExpenses { get; set; }
        [JsonProperty("expense_spy")]
        public long SpiesExp { get; set; }
        [JsonProperty("expense_tr")]
        public long TroopsExp { get; set; }
        [JsonProperty("expense_j")]
        public long JetsExp { get; set; }
        [JsonProperty("expense_tu")]
        public long TurretsExp { get; set; }
        [JsonProperty("expense_ta")]
        public long TanksExp { get; set; }
        [JsonProperty("expensesally")]
        public long AllianceGDIExp { get; set; }
        [JsonProperty("expensesland")]
        public long LandExp { get; set; }
        [JsonProperty("corruption")]
        public long CorruptionExp { get; set; }
        [JsonProperty("nw_mil")]
        public long Military_Net { get; set; }
        [JsonProperty("nw_tech")]
        public long Technology_Net { get; set; }
        [JsonProperty("nw_land")]
        public long Land_Net { get; set; }
        [JsonProperty("nw_other")]
        public long Other_Net { get; set; }
        [JsonProperty("nw_market")]
        public long Market_Net { get; set; }
        [JsonProperty("tpt")]
        public int TechPerTurn { get; set; }
        public long TechTotal { get; set; }
        public long SpyStr { get; set; }
        public double SPAL { get; set; }
        #endregion

        /*
         * Calculate Tech Percent Method
         * 
         * Purpose:
         * Returns a double representing the tech % for a given technology and points invested in that technology
         * 
         */
        public double CalculateAllTech(long points, string tech)
        {
            double max = 0;
            double govT = 1;
            double govEff = 1;
            double BaseTech = 1.00;
            int c1 = 192;
            double c2 = 6.795;
            switch (tech)
            {
                case "t_mil":
                    c1 = 780;
                    c2 = 5.75;
                    switch (Gov)
                    {
                        case "D":
                            max = 0.8166;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 0.8916;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 0.8333;
                            govEff = 1.2;
                            break;
                        default:
                            max = 0.8333;
                            break;
                    }
                    break;
                case "t_med":
                    c1 = 1650;
                    c2 = 4.62;
                    switch (Gov)
                    {
                        case "D":
                            max = 0.6333;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 0.7833;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 0.6666;
                            govEff = 1.2;
                            break;
                        default:
                            max = 0.6666;
                            break;
                    }
                    break;
                case "t_bus":
                    switch (Gov)
                    {
                        case "D":
                            max = 1.88;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 1.52;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 1.80;
                            govEff = 1.2;
                            break;
                        default:
                            max = 1.80;
                            break;
                    }
                    break;
                case "t_res":
                    switch (Gov)
                    {
                        case "D":
                            max = 1.88;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 1.52;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 1.80;
                            govEff = 1.2;
                            break;
                        default:
                            max = 1.80;
                            break;
                    }
                    break;
                case "t_agri":
                    switch (Gov)
                    {
                        case "D":
                            max = 2.43;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 1.845;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 2.30;
                            govEff = 1.2;
                            break;
                        default:
                            max = 2.30;
                            break;
                    }
                    break;
                case "t_war":
                    BaseTech = 0.002;
                    switch (Gov)
                    {
                        case "D":
                            max = 0.055;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 0.0332;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 0.050;
                            govEff = 1.2;
                            break;
                        default:
                            max = 0.050;
                            break;
                    }
                    break;
                case "t_ms":
                    switch (Gov)
                    {
                        case "D":
                            max = 1.44;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 1.26;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 1.40;
                            govEff = 1.2;
                            break;
                        default:
                            max = 1.40;
                            break;
                    }
                    break;
                case "t_weap":
                    switch (Gov)
                    {
                        case "D":
                            max = 1.55;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 1.325;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 1.5;
                            govEff = 1.2;
                            break;
                        default:
                            max = 1.5;
                            break;
                    }
                    break;
                case "t_indy":
                    switch (Gov)
                    {
                        case "D":
                            max = 1.66;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 1.39;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 1.60;
                            govEff = 1.2;
                            break;
                        default:
                            max = 1.60;
                            break;
                    }
                    break;
                case "t_spy":
                    switch (Gov)
                    {
                        case "D":
                            max = 1.55;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 1.325;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 1.5;
                            govEff = 1.2;
                            break;
                        default:
                            max = 1.5;
                            break;
                    }
                    break;
                case "t_sdi":
                    BaseTech = 0.01;
                    switch (Gov)
                    {
                        case "D":
                            max = 0.989;
                            govT = 1.1;
                            break;
                        case "H":
                            max = 0.5885;
                            govT = 0.65;
                            break;
                        case "C":
                            max = 0.90;
                            govEff = 1.2;
                            break;
                        default:
                            max = 0.90;
                            break;
                    }
                    break;
            }
            //BaseTech%+(MaxTech%-BaseTech%)*GvtTech*(1-EXP(-GvtEff*TechPts/(C1+C2*Land)))
            var test = BaseTech + (max - BaseTech) * govT * (1 - Math.Exp(-govEff * points / (c1 + c2 * Land)));
            return test;
        }

        /*
         * Calculate Spy Strength
         * 
         * Purpose:
         * Returns a long representing the strength of a users spy defense
         * 
         */
        public long CalculateSpyStr(long spies, double spyTech, bool dict)
        {
            long spyStr = 0;
            if (spyTech < 1)
                spyTech = 1;
            else if (spyTech >= 2)
                spyTech = 1;
            if (dict)
            {
                spyStr = spies * Convert.ToInt64(spyTech) * Convert.ToInt64(1.3);
            }
            else
            {
                spyStr = spies * Convert.ToInt64(spyTech);
            }

            return spyStr;
        }

        /*
         * Calculate SPAL(Spies Per Acre of Land)
         * 
         * Purpose:
         * Returns a double representing a countries SPAL given the amount of spy strength and land
         * 
         */
        public double CalculateSPAL(long spyStr, long land)
        {
            double spal = spyStr / land;
            return spal;
        }

        /*
         * Convert To Display Break Method
         * 
         * Purpose:
         * Returns a string representing an abbreviated defensive strength provided the amount of units, tech percent and bonus
         * 
         */
        public string ConvertToDisplayBreak(long unit, double tech, double bonus, bool PS)
        {
            if (tech == 0)
                tech = 1;
            else if ((tech / 100) >= 2)
                tech = 1;
            else if (tech > 100)
                tech = tech / 100;
            if (bonus == 0)
                bonus = 1;
            else if (bonus >= 2)
                bonus = 1;
            double govBonus = 1;

            if (Gov == "I")
                govBonus = 1.2;
            else if (Gov == "R")
                govBonus = 0.8;
            long str = Convert.ToInt64(Convert.ToDouble(tech) * govBonus * Convert.ToDouble(bonus) * Convert.ToDouble(unit));

            if (PS)
                str = Convert.ToInt64(str / 1.5);

            string displayBreak;
            if (str > 1000000)
            {
                var temp = Math.Round(str / 1000000.0, 1);
                displayBreak = temp + "M";
            }
            else if (str > 1000)
            {
                var temp = Math.Round(str / 1000.0, 1);
                displayBreak = temp.ToString("N1") + "K";
            }
            else
            {
                displayBreak = str.ToString("N0");
            }
            return displayBreak;
        }

        /*
         * Get Spy Op Message Method
         * 
         * Purpose:
         * Returns a string parsed from an SpyOpInfo object
         * 
         */
        public string GetSpyOpMessage()
        {
            //TODO
            string population = MessageFormattingModels.ConvertNumberToAbrNumberString(Population),
                    cash = MessageFormattingModels.ConvertNumberToAbrNumberString(Money),
                    oil = MessageFormattingModels.ConvertNumberToAbrNumberString(OilStore),
                    food = MessageFormattingModels.ConvertNumberToAbrNumberString(FoodStore),
                    networth = MessageFormattingModels.ConvertNumberToAbrNumberString(Networth),
                    username = "",//(Program.storage.UserStorage.Users.SingleOrDefault(f => f.ApiKey == apiKey) != null) ? Program.storage.UserStorage.Users.SingleOrDefault(f => f.ApiKey == apiKey).Username : "Unkown",
                    message = "";

            bool dict = false;
            if (Gov == "I")
                dict = true;
            SpyStr = CalculateSpyStr(Spies, SpyPerc, dict);
            SPAL = CalculateSPAL(SpyStr, Land);
            // Cname, Tag, Pop, Cash, Oil, Bushels, Troops, Jets, Turrets, Tanks, SPAL, SDI, Weap

            message = "``` {" + CountryName + "(#" + CountryNumber + ")" + " [" + Clan + "] - Net:" + networth + "  Land: " + Land + "  Cash:" + cash + "  SPAL:" + SPAL
                + "  Troops: " + ConvertToDisplayBreak(Troops, 1.0, 0, false)
                + "  Jets: " + ConvertToDisplayBreak(Jets, 1.0, 0, false)
                + "  Turrets:" + ConvertToDisplayBreak(Turrets, 1.0, 0, false)
                + "  Tanks:" + ConvertToDisplayBreak(Tanks, 1.0, 0, false)
                + "  Missiles:" + (NuclearMissles + ChemicalMissles + CruiseMissles).ToString("N0")
                + "  Turns:" + turnsLeft + "(" + turnsStored + ")  Civs:" + population
                + "  Raw SS Break:" + ConvertToDisplayBreak((Convert.ToInt64(Troops * 0.5) + Turrets + (Tanks * 2)), WeaponsPerc, defenseBonus, false) + " jets "
                + " Raw PS Break:" + ConvertToDisplayBreak((Convert.ToInt64(Troops * 0.5) + Turrets + (Tanks * 2)), WeaponsPerc, defenseBonus, true) + " jets } Uploaded By - " + username + "```";
            return message;
        }
    }    
}