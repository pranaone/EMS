using ElectionSystem_AD.Models.ActionLog;
using ElectionSystem_AD.Models.Party;
using ElectionSystem_AD.Services.ActionLog;
using ElectionSystem_AD.Services.BaseService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace ElectionSystem_AD.Services.Party
{
    public class PartyService : BaseAppTenant
    {
        /// <summary>
        /// Service Method To Check If A Party Exists
        /// </summary>
        /// <param name="party"></param>
        /// <returns></returns>
        public static async Task<bool> PartyExists(PartyModel party)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection(party.Location)))
            {
                var isExistingUserQuery = "SELECT * from Party WHERE Name ='" + party.Name + "' OR ID ='" + party.ID + "'";
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
                        UserID = party.UserID,
                        ActionPerformed = "Party Exists Error : " + ex.Message,
                        MethodName = "PartyExists",
                        IsError = true
                    },
                    party.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = party.UserID,
                        ActionPerformed = "Check If User Exists ",
                        MethodName = "UserExists",
                        IsError = false
                    },
                    party.Location);
                }

            }

        }

        /// <summary>
        /// Service Method To Get Parties
        /// </summary>
        /// <param name="party"></param>
        /// <returns></returns>
        public static async Task<List<PartyModel>> GetParty(PartyModel party)
        {
            List<PartyModel> Parties = new List<PartyModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(party.Location)))
            {
                var isExistingParty = "SELECT * from Party";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingParty, dbConn);
                    reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            PartyModel partyItem = new PartyModel();
                            partyItem.ID = reader.GetInt32(0);
                            partyItem.Name = reader.GetString(1);
                            Parties.Add(partyItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = party.UserID,
                        ActionPerformed = "Party Exists Error : " + ex.Message,
                        MethodName = "PartyExists",
                        IsError = true
                    },
                    party.Location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = party.UserID,
                        ActionPerformed = "Check If Party Exists",
                        MethodName = "PartyExists",
                        IsError = false
                    },
                    party.Location);
                }

                return Parties;
            }
        }

        /// <summary>
        /// Service Method To Add A New Party
        /// </summary>
        /// <param name="party"></param>
        /// <returns></returns>
        public static async Task<bool> AddNewParty(PartyModel party)
        {
            /// var dateCreated = DateTime.Now;
            String SQL = "INSERT INTO Party(Name)" +
                "VALUES('" + party.Name + "')";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(party.Location))) 
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
                        UserID = party.UserID,
                        ActionPerformed = "Party Add Error : " + ex.Message,
                        MethodName = "AddNewParty",
                        IsError = true
                    },
                    party.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = party.UserID,
                        ActionPerformed = "Party Added",
                        MethodName = "AddNewParty",
                        IsError = false
                    },
                    party.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Update A Party
        /// </summary>
        /// <param name="party"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateParty(PartyModel party)
        {
            String SQL = "UPDATE Party SET Name = '" + party.Name + "' WHERE ID = '" + party.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(party.Location)))
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
                        UserID = party.UserID,
                        ActionPerformed = "Party Update Error : " + ex.Message,
                        MethodName = "UpdateParty",
                        IsError = true
                    },
                    party.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = party.UserID,
                        ActionPerformed = "Party Updated",
                        MethodName = "UpdateParty",
                        IsError = false
                    },
                    party.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Delete A Party
        /// </summary>
        /// <param name="party"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteParty(PartyModel party)
        {
            String SQL = "DELETE from Party WHERE Name = '" + party.Name + "' AND ID = '" + party.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(party.Location)))
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
                        UserID = party.UserID,
                        ActionPerformed = "Party Delete Error : " + ex.Message,
                        MethodName = "DeleteParty",
                        IsError = true
                    },
                    party.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = party.UserID,
                        ActionPerformed = "Party Deleted",
                        MethodName = "DeleteParty",
                        IsError = false
                    },
                    party.Location);
                }
            }
        }

    }
}