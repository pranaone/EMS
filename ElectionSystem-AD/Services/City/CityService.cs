using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ElectionSystem_AD.Models.ActionLog;
using ElectionSystem_AD.Models.City;
using ElectionSystem_AD.Services.ActionLog;
using ElectionSystem_AD.Services.BaseService;

namespace ElectionSystem_AD.Services.City
{
    public class CityService : BaseAppTenant
    {
        /// <summary>
        /// Service Method To Check If A City Exists
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public static async Task<bool> CityExists(CityModel city)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection(city.Location)))
            {
                var query = "SELECT * from City WHERE Name ='" + city.Name + "' OR ID = '" + city.ID + "'";
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
                        UserID = city.UserID,
                        ActionPerformed = "City Exists Error : " + ex.Message,
                        MethodName = "CityExists",
                        IsError = true
                    },
                    city.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = city.UserID,
                        ActionPerformed = "Check If City Exists ",
                        MethodName = "CityExists",
                        IsError = false
                    },
                    city.Location);
                }

            }

        }
        
        /// <summary>
        /// Service Method To Get Cities
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public static async Task<List<CityModel>> GetCity(CityModel city)
        {
            List<CityModel> Cities = new List<CityModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(city.Location)))
            {
                var isExistingCity = "SELECT * from City";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingCity, dbConn);
                    reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            CityModel cityItem = new CityModel();
                            cityItem.ID = reader.GetInt32(0);
                            cityItem.Name = reader.GetString(1);
                            cityItem.DistrictID = reader.GetInt32(2);
                            Cities.Add(cityItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = city.UserID,
                        ActionPerformed = "City Exists Error : " + ex.Message,
                        MethodName = "GetCity",
                        IsError = true
                    },
                    city.Location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = city.UserID,
                        ActionPerformed = "Check If City Exists ",
                        MethodName = "GetCity",
                        IsError = false
                    },
                    city.Location);
                }

                return Cities;
            }
        }

        /// <summary>
        /// Service Method To Add A New City
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public static async Task<bool> AddNewCity(CityModel city)
        {
            String SQL = "INSERT INTO City(Name,DistrictID)" +
                "VALUES('" + city.Name + "','" + city.DistrictID + "')";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(city.Location)))
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
                        UserID = city.UserID,
                        ActionPerformed = "City Add Error : " + ex.Message,
                        MethodName = "AddNewCity",
                        IsError = true
                    },
                    city.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = city.UserID,
                        ActionPerformed = "Added New City",
                        MethodName = "AddNewCity",
                        IsError = false
                    },
                    city.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Update A City
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateCity(CityModel city)
        {
            String SQL = "UPDATE City SET Name = '" + city.Name + "', DistrictID = '" + city.DistrictID + "' WHERE ID = '" + city.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(city.Location)))
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
                        UserID = city.UserID,
                        ActionPerformed = "City Update Error : " + ex.Message,
                        MethodName = "UpdateCity",
                        IsError = true
                    },
                    city.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = city.UserID,
                        ActionPerformed = "Updated City",
                        MethodName = "UpdateCity",
                        IsError = false
                    },
                    city.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Delete A City
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteCity(CityModel city)
        {
            String SQL = "DELETE FROM City WHERE Name = '" + city.Name + "' AND ID = '" + city.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(city.Location)))
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
                        UserID = city.UserID,
                        ActionPerformed = "City Delete Error : " + ex.Message,
                        MethodName = "DeleteCity",
                        IsError = true
                    },
                    city.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = city.UserID,
                        ActionPerformed = "Deleted City",
                        MethodName = "DeleteCity",
                        IsError = false
                    },
                    city.Location);
                }
            }
        }

    }
}