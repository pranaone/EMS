using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionSystem_AD.Models.Common;

namespace ElectionSystem_AD.Models.Party
{
    public class PartyModel : AuthBaseModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}