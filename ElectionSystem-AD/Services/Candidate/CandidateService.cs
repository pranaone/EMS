using ElectionSystem_AD.Models.ActionLog;
using ElectionSystem_AD.Models.Candidate;
using ElectionSystem_AD.Services.ActionLog;
using ElectionSystem_AD.Services.BaseService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ElectionSystem_AD.Services.Candidate
{
    public class CandidateService : BaseAppTenant
    {
        /// <summary>
        /// Service Method To Check If The Candidate Exists
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public static async Task<bool> CandidateExists(CandidateModel candidate)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection(candidate.Location)))
            {
                var isExistingCandidateQuery = "SELECT * from Candidate WHERE Name ='" + candidate.Name + "' OR ID = '" + candidate.ID + "'";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingCandidateQuery, dbConn);
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
                        UserID = candidate.UserID,
                        ActionPerformed = "Candidate Exists Error : " + ex.Message,
                        MethodName = "CandidateExists",
                        IsError = true
                    },
                    candidate.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = candidate.UserID,
                        ActionPerformed = "Check If Candidate Exists ",
                        MethodName = "CandidateExists",
                        IsError = false
                    },
                    candidate.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Add A New Candidate
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public static async Task<bool> AddNewCandidate(CandidateModel candidate)
        {
            String SQL = @"INSERT INTO Candidate (Name, PartyID, DistrictID, Votes) " +
                                    "VALUES('" + candidate.Name + "', '" + candidate.PartyID + "', '" + candidate.DistrictID + "','" + candidate.Votes + "')";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(candidate.Location)))
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
                        UserID = candidate.UserID,
                        ActionPerformed = "Candidate Insert Error : " + ex.Message,
                        MethodName = "AddNewCandidate",
                        IsError = true
                    },
                    candidate.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = candidate.UserID,
                        ActionPerformed = "Add New Candidate",
                        MethodName = "AddNewCandidate",
                        IsError = false
                    },
                    candidate.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Get All The Candidates
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public static async Task<List<CandidateModel>> GetCandidates(CandidateModel candidate)
        {
            List<CandidateModel> Candidates = new List<CandidateModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(candidate.Location)))
            {
                var Query = "SELECT * from Candidate";
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
                            CandidateModel candidateItem = new CandidateModel();
                            candidateItem.ID = reader.GetInt32(0);
                            candidateItem.Name = reader.GetString(1);
                            candidateItem.PartyID = reader.GetInt32(2);
                            candidateItem.DistrictID = reader.GetInt32(3);
                            candidateItem.Votes = reader.GetInt32(4);
                            Candidates.Add(candidateItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = candidate.UserID,
                        ActionPerformed = "Candidates Error : " + ex.Message,
                        MethodName = "GetCandidates",
                        IsError = true
                    },
                    candidate.Location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = candidate.UserID,
                        ActionPerformed = "Get All Existing Candidates ",
                        MethodName = "GetCandidates",
                        IsError = false
                    },
                    candidate.Location);
                }

                return Candidates;
            }
        }

        /// <summary>
        /// Service Method To Update A Candidate
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateCandidate(CandidateModel candidate)
        {
            String SQL = @"UPDATE Candidate SET Name = '" + candidate.Name + "',PartyID = '" + candidate.PartyID + "', DistrictID = '" + candidate.DistrictID + "', Votes = '" + candidate.Votes + "' WHERE ID = '" + candidate.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(candidate.Location)))
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
                    Console.WriteLine(ex);
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = candidate.UserID,
                        ActionPerformed = "Update Candidate Error : " + ex.Message,
                        MethodName = "UpdateCandidate",
                        IsError = true
                    },
                    candidate.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = candidate.UserID,
                        ActionPerformed = "Update Candidate",
                        MethodName = "UpdateCandidate",
                        IsError = false
                    },
                    candidate.Location);
                }
            }
        }

        /// <summary>
        /// Service Method to Delete A Candidate
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteCandidate(CandidateModel candidate)
        {
            String SQL = "DELETE FROM Candidate WHERE Name = '" + candidate.Name + "' AND ID = '" + candidate.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(candidate.Location)))
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
                    Console.WriteLine(ex);
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = candidate.UserID,
                        ActionPerformed = "Delete Candidate Error : " + ex.Message,
                        MethodName = "DeleteCandidate",
                        IsError = true
                    },
                    candidate.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = candidate.UserID,
                        ActionPerformed = "Delete Candidate",
                        MethodName = "DeleteCandidate",
                        IsError = false
                    },
                    candidate.Location);
                }
            }
        }

    }
}