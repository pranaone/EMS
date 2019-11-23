using ElectionSystem_AD.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionSystem_AD.Models.Elector
{
    /// <summary>
    /// These are the Details of the Voters
    /// </summary>
    public class ElectorModel : AuthBaseModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string NIC { get; set; }
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public bool HasVoted { get; set; }
        public int CityID { get; set; }
        public int CenterID { get; set; }
        public DateTime DateAdded { get; set; }
    }
}