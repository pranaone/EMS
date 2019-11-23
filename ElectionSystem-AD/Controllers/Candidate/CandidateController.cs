using ElectionSystem_AD.Models.Candidate;
using ElectionSystem_AD.Services.Auth;
using ElectionSystem_AD.Services.Candidate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectionSystem_AD.Controllers.Candidate
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CandidateController : ApiController
    {
        /// <summary>
        /// API END POINT TO ADD A Candidate
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        [HttpPost, Route("api/candidate/AddNewCandidate")]
        public async Task<IHttpActionResult> AddNewCandidate(CandidateModel candidate)
        {
            if (await AuthService.ValidateUserAndToken(candidate.Token, candidate.UserID, candidate.Email, candidate.Location))
            {
                if (candidate == null)
                {
                    return BadRequest("Please provide valid inputs!");
                }

                if (candidate.PartyID == 0)
                {
                    return BadRequest("Please provide valid party ID!");
                }

                if (string.IsNullOrEmpty(candidate.Location))
                {
                    return BadRequest("Please provide valid location!");
                }

                if (await CandidateService.CandidateExists(candidate))
                {
                    return BadRequest("Candidate Already Exists!");
                }
                else
                {
                    if (await CandidateService.AddNewCandidate(candidate))
                    {
                        return Ok("Candidate Added Successfully!");
                    }
                    else
                    {
                        return BadRequest("Error In Adding New Candidate");
                    }
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API END POINT TO GET CANDIDATES
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        [HttpGet, Route("api/candidate/GetCandidates")]
        public async Task<IHttpActionResult> GetCandidates(CandidateModel candidate)
        {
            if (await AuthService.ValidateUserAndToken(candidate.Token, candidate.UserID, candidate.Email, candidate.Location))
            {
                var candidates = await CandidateService.GetCandidates(candidate);
                if (candidates.Count > 0)
                {
                    return Ok(candidates);
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
        /// API END POINT TO UPDATE A Candidate
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        [HttpPost, Route("api/candidate/UpdateCandidate")]
        public async Task<IHttpActionResult> UpdateCandidate(CandidateModel candidate)
        {
            if (candidate == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (candidate.ID == 0)
            {
                return BadRequest("Please provide valid candidate ID!");
            }

            if (candidate.PartyID == 0)
            {
                return BadRequest("Please provide valid party ID!");
            }

            if (string.IsNullOrEmpty(candidate.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(candidate.Token, candidate.UserID, candidate.Email, candidate.Location))
            {
                if (await CandidateService.CandidateExists(candidate))
                {
                    if (await CandidateService.UpdateCandidate(candidate))
                    {
                        return Ok("Candidate Updated Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Update Candidate!");
                    }
                }
                else
                {
                    return BadRequest("No Such Candidate Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API END POINT TO DELETE A Candidate
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        [HttpPost, Route("api/candidate/DeleteCandidate")]
        public async Task<IHttpActionResult> DeleteCandidate(CandidateModel candidate)
        {
            if (candidate == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (candidate.ID == 0)
            {
                return BadRequest("Please provide valid candidate ID!");
            }

            if (candidate.PartyID == 0)
            {
                return BadRequest("Please provide valid party ID!");
            }

            if (string.IsNullOrEmpty(candidate.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(candidate.Token, candidate.UserID, candidate.Email, candidate.Location))
            {
                if (await CandidateService.CandidateExists(candidate))
                {
                    if (await CandidateService.DeleteCandidate(candidate))
                    {
                        return Ok("Candidate Deleted Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Delete Candidate!");
                    }
                }
                else
                {
                    return BadRequest("No Such Candidate Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}