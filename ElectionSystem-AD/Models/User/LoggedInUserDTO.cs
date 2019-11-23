using ElectionSystem_AD.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionSystem_AD.Models.User
{
    public class LoggedInUserDTO : AuthBaseModel
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public int RoleID { get; set; }
        public DateTime RegisteredDate { get; set; }
        public int ActiveStatus { get; set; }
        public string UserLoginOs { get; set; }
        public string UserLoginDate { get; set; }
        public string UserLoggedInTimezone { get; set; }
        public string UserLoggedInIP { get; set; }
        public string UserLoggedOutDate { get; set; }
    }
}