using ElectionSystem_AD.Models.Common;
using ElectionSystem_AD.Models.Role;
using ElectionSystem_AD.Services.Admin;
using ElectionSystem_AD.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectionSystem_AD.Controllers.Admin
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AdminController : ApiController
    {
        /// <summary>
        /// API End Point To Get Total Ballot Count
        /// </summary>
        /// <param name="authBaseModel"></param>
        /// <returns></returns>
        [HttpGet, Route("api/admin/GetTotalBallotCount")]
        public async Task<IHttpActionResult> GetTotalBallotCount(AuthBaseModel authBaseModel)
        {
            if (authBaseModel == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (string.IsNullOrEmpty(authBaseModel.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(authBaseModel.Token, authBaseModel.UserID, authBaseModel.Email, authBaseModel.Location))
            {
                var ballots = await AdminService.GetTotalBallotCount(authBaseModel);
                if (ballots.Count > 0)
                {
                    return Ok(ballots);
                }
                else
                {
                    return BadRequest("No Candidates Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API End Point To Consolidate Votes
        /// </summary>
        /// <param name="loggedInUser"></param>
        /// <returns></returns>
        [HttpGet, Route("api/admin/ConsolidateVotes")]
        public IHttpActionResult ConsolidateVotes(AuthBaseModel loggedInUser)
        {
            if (loggedInUser == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (string.IsNullOrEmpty(loggedInUser.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            var finalCount = AdminService.ConsolidateVotes(loggedInUser);

            CommonResponse response = AdminService.RecordConsolidatedVotes(loggedInUser, finalCount);

            if(!response.IsError)
            {
                return Ok("Consolidated All The Votes");
            }
            else
            {
                return BadRequest("Something Went Wrong! Failed To Consolidate All The Votes!");
            }
        }

        /// <summary>
        /// API End Point To Get Action LogDetails
        /// </summary>
        /// <param name="authBaseModel"></param>
        /// <returns></returns>
        [HttpGet, Route("api/admin/GetActionLogDetails")]
        public async Task<IHttpActionResult> GetActionLogDetails(AuthBaseModel authBaseModel)
        {
            if (authBaseModel == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (string.IsNullOrEmpty(authBaseModel.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(authBaseModel.Token, authBaseModel.UserID, authBaseModel.Email, authBaseModel.Location))
            {
                var actionLogs = await AdminService.GetActionLogDetails(authBaseModel);
                if (actionLogs.Count > 0)
                {
                    return Ok(actionLogs);
                }
                else
                {
                    return BadRequest("No ActionLogs Available!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}