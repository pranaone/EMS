using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ElectionSystem_AD.Services.BaseService
{
    public class BaseAppTenant
    {
    

        public static string selectConnection(string districtName)
        {
            string connectionString = "";

            connectionString = ConfigurationManager.ConnectionStrings[districtName].ConnectionString;

            return connectionString;
        }
    }
}