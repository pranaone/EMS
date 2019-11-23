using ElectionSystem_AD.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionSystem_AD.Models.Candidate
{
    public class CandidateModel : AuthBaseModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int PartyID { get; set; }
        public int DistrictID { get; set; }
        public int Votes { get; set; }
    }
}