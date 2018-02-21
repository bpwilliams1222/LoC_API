using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LoCWebApp.Models
{
    public enum PactTypes
    {
        DNH,
        LDP,
        FDP,
        NAP,
        uNAP
    }

    public class Relation
    {
        public PactTypes PactType { get; set; }
        [Key]
        public string Tag { get; set; }
        public string Notes { get; set; }
        public bool SelfRenewing { get; set; }

        public Relation()
        {

        }

        public Relation(string tag, string pactType, string notes, bool selfRenewing)
        {
            PactType = DeterminePactType(pactType);
            Tag = tag;
            Notes = notes;
            SelfRenewing = selfRenewing;
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
        
        public bool ValidateRelation()
        {
            if (Tag != null)
            {
                return true;
            }
            return false;
        }
    }

    public class RelationStorageModel
    {
        public List<Relation> Relations { get; set; }

        public RelationStorageModel()
        {
            Relations = new List<Relation>();
        }

        public RelationStorageModel(string file)
        {
            Relations = new List<Relation>();
            XmlSerializer xs = new XmlSerializer(typeof(RelationStorageModel));
            using (var sr = new StreamReader(file))
            {
                var temp = (RelationStorageModel)xs.Deserialize(sr);
                this.Relations = temp.Relations;
            }
        }
    }
}