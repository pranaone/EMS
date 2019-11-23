using ElectionSystem_AD.Models.Elector;
using ElectionSystem_AD.Services.Auth;
using ElectionSystem_AD.Services.Elector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectionSystem_AD.Controllers.Elector
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ElectorController : ApiController
    {
        [HttpPost, Route("api/elector/AddNewElector")]
        public async Task<IHttpActionResult> AddNewElector(ElectorModel elector)
        {
            if (elector == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (elector.CenterID == 0)
            {
                return BadRequest("Please provide valid center ID!");
            }

            if (string.IsNullOrEmpty(elector.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(elector.Token, elector.UserID, elector.Email, elector.Location))
            {
                if (await ElectorService.ElectorExists(elector))
                {
                    return BadRequest("Elector Already Exists");
                }
                else
                {
                    if(ElectorService.CheckIsVoterLegalAge(elector))
                    {
                        if (await ElectorService.AddNewElector(elector))
                        {
                            return Ok("Elector Added Successfully!");
                        }
                        else
                        {
                            return BadRequest("Elector Adding Failed!");
                        }
                    }
                    else
                    {
                        return BadRequest("Elector Not Of Legal Age!");
                    }
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet, Route("api/elector/GetElectors")]
        public async Task<IHttpActionResult> GetElectors(ElectorModel elector)
        {
            if (await AuthService.ValidateUserAndToken(elector.Token, elector.UserID, elector.Email, elector.Location))
            {
                var electors = await ElectorService.GetElectors(elector);
                if (electors.Count > 0)
                {
                    return Ok(electors);
                }
                else
                {
                    return BadRequest("No Electors Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Route("api/elector/UpdateElector")]
        public async Task<IHttpActionResult> UpdateElector(ElectorModel elector)
        {
            if (elector == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (elector.ID == 0)
            {
                return BadRequest("Please provide valid elector ID!");
            }

            if (elector.CenterID == 0)
            {
                return BadRequest("Please provide valid center ID!");
            }

            if (string.IsNullOrEmpty(elector.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(elector.Token, elector.UserID, elector.Email, elector.Location))
            {
                if (await ElectorService.ElectorExists(elector))
                {
                    if (await ElectorService.UpdateElector(elector))
                    {
                        return Ok("Elector Updated Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Update Elector!");
                    }
                }
                else
                {
                    return BadRequest("No Such Elector Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Route("api/elector/DeleteElector")]
        public async Task<IHttpActionResult> DeleteElector(ElectorModel elector)
        {
            if (elector == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (elector.ID == 0)
            {
                return BadRequest("Please provide valid elector ID!");
            }

            if (elector.CenterID == 0)
            {
                return BadRequest("Please provide valid center ID!");
            }

            if (string.IsNullOrEmpty(elector.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(elector.Token, elector.UserID, elector.Email, elector.Location))
            {
                if (await ElectorService.ElectorExists(elector))
                {
                    if (await ElectorService.DeleteElector(elector))
                    {
                        return Ok("Elector Deleted Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Delete Elector!");
                    }
                }
                else
                {
                    return BadRequest("No Such Elector Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}