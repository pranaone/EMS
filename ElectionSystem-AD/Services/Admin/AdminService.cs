
using ElectionSystem_AD.Models.ActionLog;
using ElectionSystem_AD.Models.Ballot;
using ElectionSystem_AD.Models.Candidate;
using ElectionSystem_AD.Models.Common;
using ElectionSystem_AD.Services.ActionLog;
using ElectionSystem_AD.Services.BaseService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ElectionSystem_AD.Services.Admin
{
    public class AdminService : BaseAppTenant
    {
        // public static string conString = selectConnection("colombo");

        /// <summary>
        /// Service Method To Get The Total Ballot Count
        /// </summary>
        /// <param name="authBaseModel"></param>
        /// <returns></returns>
        public static async Task<List<TotalBallotModel>> GetTotalBallotCount(AuthBaseModel authBaseModel)
        {
            List<TotalBallotModel> TotalBallots = new List<TotalBallotModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(authBaseModel.Location)))
            {
                var Query = "SELECT CandidateID, COUNT(Voted) AS TotalNoOfVotes FROM Ballot WHERE Voted = 1 GROUP BY CandidateID ORDER BY TotalNoOfVotes DESC";
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
                            TotalBallotModel Ballots = new TotalBallotModel();
                            Ballots.CandidateID = reader.GetInt32(0);
                            Ballots.TotalNoOfVotes = reader.GetInt32(1);
                            Ballots.DateTallied = DateTime.Now;

                            TotalBallots.Add(Ballots);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = authBaseModel.UserID,
                        ActionPerformed = "Get Total Ballot Count Error : " + ex.Message,
                        MethodName = "GetTotalBallotCount",
                        IsError = true
                    },
                    authBaseModel.Location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = authBaseModel.UserID,
                        ActionPerformed = "Get Total Ballot Count For All Candidates",
                        MethodName = "GetTotalBallotCount",
                        IsError = false
                    },
                    authBaseModel.Location);
                }

                return TotalBallots;
            }
        }

        /// <summary>
        /// Service Method To Get Consolidated Votes
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <returns></returns>
        public static List<CandidateModel> ConsolidateVotes(AuthBaseModel loggedInUser)
        {
            List<CandidateModel> Candidates = GetCandidates(loggedInUser.Location, loggedInUser.UserID);

            foreach (ConnectionStringSettings c in System.Configuration.ConfigurationManager.ConnectionStrings)
            {
                //use c.Name
                if(c.Name != "LocalSqlServer" && c.Name != "main")
                {
                    using (SqlConnection dbConn = new SqlConnection(selectConnection(c.Name)))
                    {
                        var Query = "SELECT CandidateID, COUNT(Voted) AS TotalNoOfVotes FROM Ballot WHERE Voted = 1 GROUP BY CandidateID ORDER BY CandidateID";
                        SqlDataReader reader;

                        try
                        {
                            dbConn.Open();
                            SqlCommand cmd = new SqlCommand(Query, dbConn);
                            reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TotalBallotModel votedCandidate = new TotalBallotModel();
                                    votedCandidate.CandidateID = reader.GetInt32(0);
                                    votedCandidate.TotalNoOfVotes = reader.GetInt32(1);
                                    votedCandidate.DateTallied = DateTime.Now;

                                    foreach (var candidate in Candidates)
                                    {
                                        if(candidate.ID == votedCandidate.CandidateID)
                                        {
                                            candidate.Votes = votedCandidate.TotalNoOfVotes;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            reader = null;
                            ActionLogService.LogAction(new ActionLogModel()
                            {
                                UserID = loggedInUser.UserID,
                                ActionPerformed = " Error In Consolidating Votes : " + ex.Message,
                                MethodName = "ConsolidateVotes",
                                IsError = true
                            },
                            loggedInUser.Location);
                        }
                        finally
                        {
                            dbConn.Close();
                            ActionLogService.LogAction(new ActionLogModel()
                            {
                                UserID = loggedInUser.UserID,
                                ActionPerformed = "Consolidate Votes",
                                MethodName = "ConsolidateVotes",
                                IsError = false
                            },
                            loggedInUser.Location);
                        }
                    }
                }
            }

            return Candidates;
        }

        /// <summary>
        /// Service Method To Get All The Candidates (Since All The Locations Will Have The Same candidates, Can Get Them From One Location)
        /// </summary>
        /// <param name="location"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static List<CandidateModel> GetCandidates(string location, int userID)
        {
            List<CandidateModel> candidates = new List<CandidateModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(location)))
            {
                string query = @"SELECT * FROM Candidate";

                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(query, dbConn);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            CandidateModel candidate = new CandidateModel();
                            candidate.ID = reader.GetInt32(0);
                            candidate.Name = reader.GetString(1);
                            candidate.Votes = reader.GetInt32(2);

                            candidates.Add(candidate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = userID,
                        ActionPerformed = " Error In Consolidating Votes : " + ex.Message,
                        MethodName = "ConsolidateVotes",
                        IsError = true
                    },
                    location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = userID,
                        ActionPerformed = "Consolidate Votes",
                        MethodName = "ConsolidateVotes",
                        IsError = false
                    },
                    location);
                }

                return candidates;
            }
        }

        /// <summary>
        /// Service Method To Record All The Consolidated Votes In The Main Database
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <param name="candidates"></param>
        /// <returns></returns>
        public static CommonResponse RecordConsolidatedVotes(AuthBaseModel loggedInUser, List<CandidateModel> candidates)
        {
            CommonResponse commonResponse = new CommonResponse();

            using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
            {
                try
                {

                    foreach (var candidate in candidates)
                    {
                        String SQL = @"INSERT INTO ConsolidatedVotes(CandidateID, CandidateName, TotalNoOfVotes, Year, DateAdded)" +
                                    "VALUES('" + candidate.ID + "','" + candidate.Name + "','" + candidate.Votes + "', GetDate(), GetDate())";

                        dbConn.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SQL;
                        cmd.Connection = dbConn;
                        cmd.ExecuteNonQuery();
                        dbConn.Close();
                    }

                }
                catch (Exception ex)
                {
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = loggedInUser.UserID,
                        ActionPerformed = "Error In Recording Consolidated Votes : " + ex.Message,
                        MethodName = "RecordConsolidatedVotes",
                        IsError = true
                    },
                    loggedInUser.Location);

                    commonResponse.IsError = true;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = loggedInUser.UserID,
                        ActionPerformed = "Record Consolidated Votes Completed!",
                        MethodName = "RecordConsolidatedVotes",
                        IsError = false
                    },
                    loggedInUser.Location);
                }
            }

            return commonResponse;
        }

        /// <summary>
        /// Service Method To Get ActionLog Details
        /// </summary>
        /// <param name="authBaseModel"></param>
        /// <returns></returns>
        public static async Task<List<ActionLogModel>> GetActionLogDetails(AuthBaseModel authBaseModel)
        {
            List<ActionLogModel> actionLogs = new List<ActionLogModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(authBaseModel.Location)))
            {
                var Query = "SELECT * FROM ActionLog ORDER BY ID DESC";
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
                            ActionLogModel actionLog = new ActionLogModel();
                            actionLog.ID = reader.GetInt32(0);
                            actionLog.DateCreated = reader.GetDateTime(1);
                            actionLog.UserID = reader.GetInt32(2);
                            actionLog.ActionPerformed = reader.GetString(3);
                            actionLog.MethodName = reader.GetString(4);
                            actionLog.IsError = reader.GetBoolean(5);

                            actionLogs.Add(actionLog);
                        }
                    }
                    dbConn.Close();
                }
                catch
                {
                    reader = null;
                }
                finally
                {
                    dbConn.Close();
                }

                return actionLogs;
            }
        }
    }
}