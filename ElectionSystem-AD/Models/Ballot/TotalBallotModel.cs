using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionSystem_AD.Models.Ballot
{
    public class TotalBallotModel
    {
        public int CandidateID { get; set; }
        public int TotalNoOfVotes { get; set; }
        public DateTime DateTallied { get; set; }
    }
}