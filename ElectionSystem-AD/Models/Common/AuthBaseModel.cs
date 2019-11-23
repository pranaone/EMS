using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionSystem_AD.Models.Common
{
    public class AuthBaseModel
    {
        public int UserID { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
    }
}