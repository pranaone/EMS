using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ElectionSystem_AD.Models.ActionLog;
using ElectionSystem_AD.Models.District;
using ElectionSystem_AD.Services.ActionLog;
using ElectionSystem_AD.Services.BaseService;

namespace ElectionSystem_AD.Services.District
{
    public class DistrictService : BaseAppTenant
    {
        /// <summary>
        /// Service Method To Check If A District Exists
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        public static async Task<bool> DistrictExists(DistrictModel district)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection(district.Location)))
            {
                var query = "SELECT * from District WHERE name ='" + district.Name + "' OR ID ='" + district.ID+"'";
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
                        UserID = district.UserID,
                        ActionPerformed = "District Exists Error : " + ex.Message,
                        MethodName = "DistrictExists",
                        IsError = true
                    },
                    district.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    
                }

            }

        }

        /// <summary>
        /// Service Method To Get Districts
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        public static async Task<List<DistrictModel>> GetDistrict(DistrictModel district)
        {
            List<DistrictModel> Districts = new List<DistrictModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(district.Location)))
            {
                var isExistingDistrict = "SELECT * from District";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingDistrict, dbConn);
                    reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DistrictModel districtItem = new DistrictModel();
                            districtItem.ID = reader.GetInt32(0);
                            districtItem.Name = reader.GetString(1);
                            districtItem.IsAvailable = reader.GetBoolean(2);
                            Districts.Add(districtItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = district.UserID,
                        ActionPerformed = "District Exists Error : " + ex.Message,
                        MethodName = "GetDistrict",
                        IsError = true
                    },
                    district.Location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = district.UserID,
                        ActionPerformed = "Check If District Exists ",
                        MethodName = "GetDistrict",
                        IsError = false
                    },
                    district.Location);
                }

                return Districts;
            }
        }

        /// <summary>
        /// Service Method To Add A New District
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        public static async Task<bool> AddNewDistrict(DistrictModel district)
        {
            var Availability = true;
            String SQL = "INSERT INTO District(Name,IsAvailable)" +
                "VALUES('" + district.Name + "','" + Availability + "')";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(district.Location)))
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
                        UserID = district.UserID,
                        ActionPerformed = "District Add Error : " + ex.Message,
                        MethodName = "AddNewDistrict",
                        IsError = true
                    },
                    district.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = district.UserID,
                        ActionPerformed = "Added New District ",
                        MethodName = "AddNewDistrict",
                        IsError = false
                    },
                    district.Location);
                }
            
            }
        }

        /// <summary>
        /// Service Method To Update A District
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateDistrict(DistrictModel district)
        {
            String SQL = "UPDATE District SET Name = '" + district.Name + "', IsAvailable = '" + district.IsAvailable + "' WHERE ID = '" + district.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(district.Location)))
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
                        UserID = district.UserID,
                        ActionPerformed = "District Update Error : " + ex.Message,
                        MethodName = "UpdateDistrict",
                        IsError = true
                    },
                    district.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = district.UserID,
                        ActionPerformed = "Updated District",
                        MethodName = "UpdateDistrict",
                        IsError = false
                    },
                    district.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Delete A District
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteDistrict(DistrictModel district)
        {
            String SQL = "DELETE FROM District WHERE Name = '" + district.Name + "' AND ID = '" + district.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(district.Location)))
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
                        UserID = district.UserID,
                        ActionPerformed = "District Delete Error : " + ex.Message,
                        MethodName = "DeleteDistrict",
                        IsError = true
                    },
                    district.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = district.UserID,
                        ActionPerformed = "Deleted District",
                        MethodName = "DeleteDistrict",
                        IsError = false
                    },
                    district.Location);
                }
            }
        }

    }
}