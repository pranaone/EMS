using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionSystem_AD.Models.Common;

namespace ElectionSystem_AD.Models.District
{
    /// <summary>
    /// Details Of The Districts
    /// </summary>
    public class DistrictModel : AuthBaseModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
    }
}