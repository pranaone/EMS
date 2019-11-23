using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ElectionSystem_AD.Models.ActionLog;
using ElectionSystem_AD.Models.Center;
using ElectionSystem_AD.Services.ActionLog;
using ElectionSystem_AD.Services.BaseService;

namespace ElectionSystem_AD.Services.Center
{
    public class CenterService : BaseAppTenant
    {
        /// <summary>
        /// Service Method To Check If A Center Exists
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        public static async Task<bool> CenterExists(CenterModel center)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection(center.Location)))
            {
                var query = "SELECT * from Center WHERE Name ='" + center.Name + "' OR ID ='" + center.ID + "'";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(query, dbConn);
                    reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = center.UserID,
                        ActionPerformed = "Center Exists Error : " + ex.Message,
                        MethodName = "CenterExists",
                        IsError = true
                    },
                    center.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = center.UserID,
                        ActionPerformed = "Check If Center Exists ",
                        MethodName = "CenterExists",
                        IsError = false
                    },
                    center.Location);
                }

            }

        }

        /// <summary>
        /// Service Method To Get Centers
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        public static async Task<List<CenterModel>> GetCenter(CenterModel center)
        {
            List<CenterModel> centers = new List<CenterModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(center.Location)))
            {
                var isExistingCenter = "SELECT * from Center";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingCenter, dbConn);
                    reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            CenterModel centerItem = new CenterModel();
                            centerItem.ID = reader.GetInt32(0);
                            centerItem.Name = reader.GetString(1);
                            centerItem.CityID = reader.GetInt32(2);
                            centerItem.NoOfVoters = reader.GetInt32(3);
                            centers.Add(centerItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = center.UserID,
                        ActionPerformed = "Center Exists Error : " + ex.Message,
                        MethodName = "CenterExists",
                        IsError = true
                    },
                    center.Location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = center.UserID,
                        ActionPerformed = "Check If Center Exists ",
                        MethodName = "CenterExists",
                        IsError = false
                    },
                    center.Location);
                }

                return centers;
            }
        }

        /// <summary>
        /// Service Method To Add A New Center
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        public static async Task<bool> AddNewCenter(CenterModel center)
        {
            String SQL = "INSERT INTO Center(Name,CityID,Voters)" +
                "VALUES('" + center.Name + "','" + center.CityID + "','" + center.NoOfVoters + "')";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(center.Location)))
            {
                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    await cmd.ExecuteNonQueryAsync();
                    dbConn.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = center.UserID,
                        ActionPerformed = "Center Add Error : " + ex.Message,
                        MethodName = "AddNewCenter",
                        IsError = true
                    },
                    center.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = center.UserID,
                        ActionPerformed = "Center Added",
                        MethodName = "AddNewCenter",
                        IsError = false
                    },
                    center.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Update A Center
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateCenter(CenterModel center)
        {
            String SQL = "UPDATE Center SET Name = '" + center.Name + "', CityID = '" + center.CityID + "', Voters = '" + center.NoOfVoters + "' WHERE ID = '" + center.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(center.Location)))
            {
                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    await cmd.ExecuteNonQueryAsync();
                    dbConn.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = center.UserID,
                        ActionPerformed = "Center Update Error : " + ex.Message,
                        MethodName = "UpdateCenter",
                        IsError = true
                    },
                    center.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = center.UserID,
                        ActionPerformed = "Center Updated",
                        MethodName = "UpdateCenter",
                        IsError = false
                    },
                   center.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Delete A Center
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteCenter(CenterModel center)
        {
            String SQL = "DELETE FROM Center WHERE Name = '" + center.Name + "' OR ID = '" + center.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(center.Location)))
            {
                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    await cmd.ExecuteNonQueryAsync();
                    dbConn.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = center.UserID,
                        ActionPerformed = "Center Delete Error : " + ex.Message,
                        MethodName = "DeleteCenter",
                        IsError = true
                    },
                    center.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = center.UserID,
                        ActionPerformed = "Center Deleted",
                        MethodName = "DeleteCenter",
                        IsError = false
                    },
                   center.Location);
                }
            }
        }
    }
}