using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionSystem_AD.Models.ActionLog
{
    public class ActionLogModel
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserID { get; set; }
        public string ActionPerformed { get; set; }
        public string MethodName { get; set; }
        public bool IsError { get; set; }
    }
}