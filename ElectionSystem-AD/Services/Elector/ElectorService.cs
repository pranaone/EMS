using ElectionSystem_AD.Models.ActionLog;
using ElectionSystem_AD.Models.Elector;
using ElectionSystem_AD.Services.ActionLog;
using ElectionSystem_AD.Services.BaseService;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ElectionSystem_AD.Services.Elector
{
    public class ElectorService : BaseAppTenant
    {
        /// <summary>
        /// Service Method To Check Is Voter Of Legal Age
        /// </summary>
        /// <param name="voter"></param>
        /// <returns></returns>
        public static bool CheckIsVoterLegalAge(ElectorModel voter)
        {
            var today = DateTime.Now;

            var age = DateTime.Today.Year - voter.DOB.Year;
            if (voter.DOB.AddYears(age) > DateTime.Today)
            {
                age--;
            }

            if(age >= 18)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Elector Exists
        /// </summary>
        /// <param name="elector"></param>
        /// <returns></returns>
        public static async Task<bool> ElectorExists(ElectorModel elector)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection(elector.Location)))
            {
                var isExistingUserQuery = "SELECT * from Elector WHERE NIC = '" + elector.NIC + "'";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingUserQuery, dbConn);
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
                        UserID = elector.UserID,
                        ActionPerformed = "Elector Exists Error : " + ex.Message,
                        MethodName = "ElectorExists",
                        IsError = true
                    },
                    elector.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = elector.UserID,
                        ActionPerformed = "Check If Elector Exists ",
                        MethodName = "ElectorExists",
                        IsError = false
                    },
                    elector.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Check If The Elector Has Voted
        /// </summary>
        /// <param name="elector"></param>
        /// <returns></returns>
        public static async Task<bool> ElectorHasVoted(CastVoteModel elector)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection(elector.Location)))
            {
                var isExistingUserQuery = "SELECT * from Elector WHERE ID ='" + elector.ElectorID + "' AND HasVoted = 1 ";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingUserQuery, dbConn);
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
                        UserID = elector.UserID,
                        ActionPerformed = "Elector Has Voted Error : " + ex.Message,
                        MethodName = "ElectorHasVoted",
                        IsError = true
                    },
                    elector.Location);
                    return false;
                }
                finally
                
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = elector.UserID,
                        ActionPerformed = "Check If Elector Has Voted ",
                        MethodName = "ElectorHasVoted",
                        IsError = false
                    },
                    elector.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Add A New Elector
        /// </summary>
        /// <param name="elector"></param>
        /// <returns></returns>
        public static async Task<bool> AddNewElector(ElectorModel elector)
        {
            var dateAdded = DateTime.Now;
            String SQL = "INSERT INTO Elector(Name, NIC, DOB, Address, Contact, HasVoted, CityID, CenterID, DateAdded)" +
                "VALUES('" + elector.Name + "','"+ elector.NIC + "','"+ elector.DOB + "','" + elector.Address + "','" + elector.Contact + "','" + elector.HasVoted + "','" + elector.CityID + "','" + elector.CenterID + "',GETDATE())";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(elector.Location)))
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
                        UserID = elector.UserID,
                        ActionPerformed = "Elector Add Error : " + ex.Message,
                        MethodName = "AddNewElector",
                        IsError = true
                    },
                    elector.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = elector.UserID,
                        ActionPerformed = "Elector Added",
                        MethodName = "AddNewElector",
                        IsError = false
                    },
                    elector.Location);
                }
                    
            }
        }

        /// <summary>
        /// Get Electors
        /// </summary>
        /// <param name="elector"></param>
        /// <returns></returns>
        public static async Task<List<ElectorModel>> GetElectors(ElectorModel elector)
        {
            List<ElectorModel> Electors = new List<ElectorModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(elector.Location)))
            {
                var Query = "SELECT * from Elector";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(Query, dbConn);
                    reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ElectorModel electorItem = new ElectorModel();
                            
                            electorItem.ID = reader.GetInt32(0);
                            electorItem.Name = reader.GetString(1);
                            electorItem.NIC = reader.GetString(2);
                            electorItem.DOB = reader.GetDateTime(3);
                            electorItem.Address = reader.GetString(4);
                            electorItem.Contact = reader.GetString(5);
                            electorItem.HasVoted = reader.GetBoolean(6);
                            electorItem.CityID = reader.GetInt32(7);
                            electorItem.CenterID = reader.GetInt32(8);
                            electorItem.DateAdded = reader.GetDateTime(9);


                            Electors.Add(electorItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = elector.UserID,
                        ActionPerformed = "Electors Error : " + ex.Message,
                        MethodName = "GetElectors",
                        IsError = true
                    },
                    elector.Location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = elector.UserID,
                        ActionPerformed = "Get All Existing Electors ",
                        MethodName = "GetElectors",
                        IsError = false
                    },
                    elector.Location);
                }

                return Electors;
            }
        }

        /// <summary>
        /// Update Elector
        /// </summary>
        /// <param name="elector"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateElector(ElectorModel elector)
        {
            String SQL = @"UPDATE Elector SET Name = '" + elector.Name + "'" +
                            " ,NIC = '" + elector.NIC + "'" +
                            " ,DOB = '" + elector.DOB + "'" +
                            " ,Address = '" + elector.Address + "'" +
                            " ,Contact = '" + elector.Contact + "'" +
                            " ,HasVoted = '" + elector.HasVoted + "'" +
                            " ,CityID = '" + elector.CityID + "'" +
                            " ,CenterID = '" + elector.CenterID + "'" +
                            " WHERE ID = '" + elector.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(elector.Location)))
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
                        UserID = elector.UserID,
                        ActionPerformed = "Elector Update Error : " + ex.Message,
                        MethodName = "UpdateElector",
                        IsError = true
                    },
                    elector.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = elector.UserID,
                        ActionPerformed = "Elector Updated",
                        MethodName = "UpdateElector",
                        IsError = false
                    },
                    elector.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Updated when the user has Voted
        /// </summary>
        /// <param name="elector"></param>
        /// <returns></returns>
        public static async Task<bool> ElectorVoted(CastVoteModel elector)
        {
            String SQL = @"UPDATE Elector SET HasVoted = 1" +
                            " WHERE ID = '" + elector.ElectorID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(elector.Location)))
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
                        UserID = elector.UserID,
                        ActionPerformed = "Elector Voted Error : " + ex.Message,
                        MethodName = "ElectorVoted",
                        IsError = true
                    },
                    elector.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = elector.UserID,
                        ActionPerformed = "Elector Voted",
                        MethodName = "ElectorVoted",
                        IsError = false
                    },
                    elector.Location);
                }
            }
        }

        /// <summary>
        /// Delete Elector
        /// </summary>
        /// <param name="elector"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteElector(ElectorModel elector)
        {
            String SQL = "DELETE FROM Elector WHERE Name = '" + elector.Name + "' AND ID = '" + elector.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(elector.Location)))
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
                        UserID = elector.UserID,
                        ActionPerformed = "Elector Delete Error : " + ex.Message,
                        MethodName = "DeleteElector",
                        IsError = true
                    },
                    elector.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = elector.UserID,
                        ActionPerformed = "Elector Deleted",
                        MethodName = "DeleteElector",
                        IsError = false
                    },
                    elector.Location);
                }
            }
        }

    }
}