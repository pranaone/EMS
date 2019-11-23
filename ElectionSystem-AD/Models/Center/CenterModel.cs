using ElectionSystem_AD.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionSystem_AD.Models.Center
{
    public class CenterModel : AuthBaseModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CityID { get; set; }
        public int NoOfVoters { get; set; }
    }
}