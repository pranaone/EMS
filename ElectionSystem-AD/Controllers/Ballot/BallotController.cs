using ElectionSystem_AD.Models.Ballot;
using ElectionSystem_AD.Models.Elector;
using ElectionSystem_AD.Services.Auth;
using ElectionSystem_AD.Services.Ballot;
using ElectionSystem_AD.Services.Elector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectionSystem_AD.Controllers.Ballot
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BallotController : ApiController
    {
        /// <summary>
        /// API END POINT TO ADD A NEW BALLOT
        /// </summary>
        /// <param name="ballot"></param>
        /// <returns></returns>
        [HttpPost, Route("api/ballot/AddNewBallot")]
        public async Task<IHttpActionResult> AddNewBallot(BallotModel ballot)
        {
            if (ballot == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (ballot.CandidateID == 0)
            {
                return BadRequest("Please provide valid candidate ID!");
            }

            if (string.IsNullOrEmpty(ballot.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(ballot.Token, ballot.UserID, ballot.Email, ballot.Location))
            {
                if (await BallotService.BallotExists(ballot))
                {
                    return BadRequest("Ballot Already Exists");
                }
                else
                {
                    if (await BallotService.AddNewBallot(ballot))
                    {
                        return Ok("Ballot Added Successfully!");
                    }
                    else
                    {
                        return BadRequest("Ballot Adding Failed!");
                    }
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API END POINT TO GET ALL BALLOTS
        /// </summary>
        /// <param name="ballot"></param>
        /// <returns></returns>
        [HttpGet, Route("api/ballot/GetBallots")]
        public async Task<IHttpActionResult> GetBallots(BallotModel ballot)
        {
            if (await AuthService.ValidateUserAndToken(ballot.Token, ballot.UserID, ballot.Email, ballot.Location))
            {
                var ballots = await BallotService.GetBallots(ballot);
                if (ballots.Count > 0)
                {
                    return Ok(ballots);
                }
                else
                {
                    return BadRequest("No Ballots Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API END POINT TO UPDATE A BALLOT
        /// </summary>
        /// <param name="ballot"></param>
        /// <returns></returns>
        [HttpPost, Route("api/ballot/UpdateBallot")]
        public async Task<IHttpActionResult> UpdateBallot(BallotModel ballot)
        {
            if (ballot == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (ballot.ID == 0)
            {
                return BadRequest("Please provide valid ballot ID!");
            }

            if (string.IsNullOrEmpty(ballot.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(ballot.Token, ballot.UserID, ballot.Email, ballot.Location))
            {
                if (await BallotService.BallotExists(ballot))
                {
                    if (await BallotService.UpdateBallot(ballot))
                    {
                        return Ok("Ballot Updated Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Update Ballot!");
                    }
                }
                else
                {
                    return BadRequest("No Such Ballot Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API END POINT TO DELETE A BALLOT
        /// </summary>
        /// <param name="ballot"></param>
        /// <returns></returns>
        [HttpPost, Route("api/ballot/DeleteBallot")]
        public async Task<IHttpActionResult> DeleteBallot(BallotModel ballot)
        {
            if (ballot == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (ballot.ID == 0)
            {
                return BadRequest("Please provide valid ballot ID!");
            }

            if (string.IsNullOrEmpty(ballot.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(ballot.Token, ballot.UserID, ballot.Email, ballot.Location))
            {
                if (await BallotService.BallotExists(ballot))
                {
                    if (await BallotService.DeleteBallot(ballot))
                    {
                        return Ok("Ballot Deleted Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Delete Ballot!");
                    }
                }
                else
                {
                    return BadRequest("No Such Ballot Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }


        /// <summary>
        /// API END POINT TO CAST A VOTE
        /// </summary>
        /// <param name="vote"></param>
        /// <returns></returns>
        [HttpPost, Route("api/ballot/CastVote")]
        public async Task<IHttpActionResult> CastVote(CastVoteModel vote)
        {
            if (vote == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (vote.ElectorID == 0)
            {
                return BadRequest("Please provide valid elector ID!");
            }

            if (vote.CandidateID == 0)
            {
                return BadRequest("Please provide valid candidate ID!");
            }

            if (string.IsNullOrEmpty(vote.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(vote.Token, vote.UserID, vote.Email, vote.Location))
            {
                if (await ElectorService.ElectorHasVoted(vote))
                {
                    return BadRequest("Elector has already voted!");
                }
                else
                {
                    BallotModel newVote = new BallotModel()
                    {
                        CandidateID = vote.CandidateID,
                        DistrictID = vote.DistrictID,
                        CenterID = vote.CenterID,
                        Location = vote.Location
                    };

                    if (await BallotService.AddNewBallot(newVote))
                    {
                        if (await ElectorService.ElectorVoted(vote))
                        {
                            return Ok("Vote Casted Successfully!");
                        }
                        else
                        {
                            return BadRequest("Error In Casting The Vote!");
                        }
                    }
                    else
                    {
                        return BadRequest("Error In Casting The Vote!");
                    }
                }
            }
            else
            {
                return Unauthorized();
            }
        }
        



        [HttpPost, Route("api/ballot/GetTotalNoOfVotes")]
        public async Task<IHttpActionResult> GetTotalNoOfVotes(BallotModel vote)
        {
            if (await AuthService.ValidateUserAndToken(vote.Token, vote.UserID, vote.Email, vote.Location))
            {
                var votes = await BallotService.GetTotalVotes(vote);
                if (votes.Count > 0)
                {
                    return Ok(votes);
                }
                else
                {
                    return BadRequest("No Votes Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }


    }
}