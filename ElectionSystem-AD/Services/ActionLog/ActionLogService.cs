using ElectionSystem_AD.Services.BaseService;
using ElectionSystem_AD.Models.ActionLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionSystem_AD.Models.Common;
using System.Data.SqlClient;
using System.Data;

namespace ElectionSystem_AD.Services.ActionLog
{
    public class ActionLogService : BaseAppTenant
    {
        /// <summary>
        /// Service Method To Log All The Actions
        /// </summary>
        /// <param name="actionLog"></param>
        /// <param name="location"></param>
        public static void LogAction(ActionLogModel actionLog, string location) 
        {
            var dateCreated = DateTime.Now;
            String SQL = "INSERT INTO ActionLog(DateCreated,UserID,ActionPerformed,MethodName,IsError)" +
                "VALUES(GetDate(),'" + actionLog.UserID + "','" + actionLog.ActionPerformed + "','" + actionLog.MethodName + "','" + actionLog.IsError + "')";
                            
            
            using (SqlConnection dbConn = new SqlConnection(selectConnection(location)))
            {
                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    cmd.ExecuteNonQuery();
                    dbConn.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    dbConn.Close();
                }
            }
            

        }
    }
    
}