using ElectionSystem_AD.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionSystem_AD.Models.User
{
    public class User : AuthBaseModel
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address_line_1 { get; set; }
        public string Address_line_2 { get; set; }
        public string PostalCode { get; set; }
        public new string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public int RoleID { get; set; }
        public DateTime RegisteredDate { get; set; }
        public int ActiveStatus { get; set; }
    }
}