using ElectionSystem_AD.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionSystem_AD.Models.Ballot
{
    public class BallotModel : AuthBaseModel
    {
        public int ID { get; set; }
        public int CandidateID { get; set; }
        public int DistrictID { get; set; }
        public int CenterID { get; set; }
        public bool Voted { get; set; }
        public DateTime DateCreated { get; set; }
    }
}