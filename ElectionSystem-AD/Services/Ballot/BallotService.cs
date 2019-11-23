using ElectionSystem_AD.Models.ActionLog;
using ElectionSystem_AD.Models.Ballot;
using ElectionSystem_AD.Services.ActionLog;
using ElectionSystem_AD.Services.BaseService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ElectionSystem_AD.Services.Ballot
{
    public class BallotService : BaseAppTenant
    {
        /// <summary>
        /// Service Method To Check If A Ballot Exists
        /// </summary>
        /// <param name="ballot"></param>
        /// <returns></returns>
        public static async Task<bool> BallotExists(BallotModel ballot)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection(ballot.Location)))
            {
                var isExistingBallotQuery = "SELECT * from Ballot WHERE CandidateID ='" + ballot.CandidateID + "' AND DistrictID = '" + ballot.DistrictID + "' AND CenterID = '" + ballot.CenterID + "'";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingBallotQuery, dbConn);
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
                        UserID = ballot.UserID,
                        ActionPerformed = "Ballot Exists Error : " + ex.Message,
                        MethodName = "BallotExists",
                        IsError = true
                    },
                    ballot.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = ballot.UserID,
                        ActionPerformed = "Check If Ballot Exists ",
                        MethodName = "BallotExists",
                        IsError = false
                    },
                    ballot.Location);
                }
            }
        }

        /// <summary>
        /// Service Method to Add New Ballot
        /// </summary>
        /// <param name="vote"></param>
        /// <returns></returns>
        public static async Task<bool> AddNewBallot(BallotModel vote)
        {
            var dateAdded = DateTime.Now;
            vote.Voted = true;
            String SQL = "INSERT INTO Ballot(CandidateID, DistrictID, CenterID, Voted, DateCreated)" +
                "VALUES('" + vote.CandidateID + "','" + vote.DistrictID + "','" + vote.CenterID + "','" + vote.Voted + "',GetDate())";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(vote.Location)))
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
                        UserID = vote.UserID,
                        ActionPerformed = "Add New Ballot Error : " + ex.Message,
                        MethodName = "AddNewBallot",
                        IsError = true
                    },
                    vote.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = vote.UserID,
                        ActionPerformed = "New Ballot Added",
                        MethodName = "AddNewBallot",
                        IsError = false
                    },
                    vote.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Get All Ballots
        /// </summary>
        /// <param name="ballot"></param>
        /// <returns></returns>
        public static async Task<List<BallotModel>> GetBallots(BallotModel ballot)
        {
            List<BallotModel> Ballots = new List<BallotModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(ballot.Location)))
            {
                var Query = "SELECT * from Ballot";
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
                            BallotModel ballotItem = new BallotModel();
                            ballotItem.ID = reader.GetInt32(0);
                            ballotItem.CandidateID = reader.GetInt32(1);
                            ballotItem.DistrictID = reader.GetInt32(2);
                            ballotItem.CenterID = reader.GetInt32(3);
                            ballotItem.Voted = reader.GetBoolean(4);
                            ballotItem.DateCreated = reader.GetDateTime(5);

                            Ballots.Add(ballotItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = ballot.UserID,
                        ActionPerformed = "Ballots Error : " + ex.Message,
                        MethodName = "GetBallots",
                        IsError = true
                    },
                    ballot.Location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = ballot.UserID,
                        ActionPerformed = "Get All Existing Ballots ",
                        MethodName = "GetBallots",
                        IsError = false
                    },
                    ballot.Location);
                }

                return Ballots;
            }
        }

        /// <summary>
        /// Service Method To Get Total No Of Votes
        /// </summary>
        /// <param name="ballot"></param>
        /// <returns></returns>
        public static async Task<List<TotalBallotModel>> GetTotalVotes(BallotModel ballot)
        {
            List<TotalBallotModel> Ballots = new List<TotalBallotModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection(ballot.Location)))
            {
                var Query = "SELECT CandidateID, COUNT(Voted) AS NoOFVotes from Ballot group by CandidateID order by NoOFVotes desc";
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
                            TotalBallotModel ballotItem = new TotalBallotModel();
                            ballotItem.CandidateID = reader.GetInt32(0);
                            ballotItem.TotalNoOfVotes = reader.GetInt32(1); //
                            ballotItem.DateTallied = DateTime.Now;

                            Ballots.Add(ballotItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = ballot.UserID,
                        ActionPerformed = "Ballots Error : " + ex.Message,
                        MethodName = "GetBallots",
                        IsError = true
                    },
                    ballot.Location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = ballot.UserID,
                        ActionPerformed = "Get All Existing Ballots ",
                        MethodName = "GetBallots",
                        IsError = false
                    },
                    ballot.Location);
                }

                return Ballots;
            }
        }

        /// <summary>
        /// Service Method To Update A Ballot
        /// </summary>
        /// <param name="ballot"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateBallot(BallotModel ballot)
        {
            String SQL = @"UPDATE Ballot SET CandidateID = '" + ballot.CandidateID + "'" +
                            " DistrictID = '" + ballot.DistrictID + "'" +
                            " CenterID = '" + ballot.CenterID + "'" +
                            " Voted = '" + ballot.Voted + "'" +
                            " WHERE ID = '" + ballot.ID + "')";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(ballot.Location)))
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
                        UserID = ballot.UserID,
                        ActionPerformed = "Ballot Update Error : " + ex.Message,
                        MethodName = "UpdateBallot",
                        IsError = true
                    },
                    ballot.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = ballot.UserID,
                        ActionPerformed = "Ballot Updated",
                        MethodName = "UpdateBallot",
                        IsError =false
                    },
                    ballot.Location);
                }
            }
        }

        /// <summary>
        /// Service Method To Delete A Ballot
        /// </summary>
        /// <param name="ballot"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteBallot(BallotModel ballot)
        {
            String SQL = "DELETE FROM Ballot WHERE ID = '" + ballot.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection(ballot.Location)))
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
                        UserID = ballot.UserID,
                        ActionPerformed = "Ballot Delete Error : " + ex.Message,
                        MethodName = "DeleteBallot",
                        IsError = true
                    },
                    ballot.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = ballot.UserID,
                        ActionPerformed = "Ballot Deleted",
                        MethodName = "DeleteBallot",
                        IsError = false
                    },
                    ballot.Location);
                }
            }
        }

    }
}