using ElectionSystem_AD.Models.Party;
using ElectionSystem_AD.Services.Auth;
using ElectionSystem_AD.Services.Party;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectionSystem_AD.Controllers.Party
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PartyController : ApiController
    {
        [HttpPost, Route("api/party/AddNewParty")]
        public async Task<IHttpActionResult> AddNewParty(PartyModel party)
        {
            if (party == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (string.IsNullOrEmpty(party.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(party.Token, party.UserID, party.Email, party.Location))
            {
                if(await PartyService.PartyExists(party))
                {
                    return BadRequest("Party Already Exists");
                }
                else
                {
                    if(await PartyService.AddNewParty(party))
                    {
                        return Ok("Party Added Successfully!");
                    }
                    else
                    {
                        return BadRequest("Party Adding Failed!");
                    }
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet, Route("api/party/GetParties")]
        public async Task<IHttpActionResult> GetParties(PartyModel party)
        {
            if (await AuthService.ValidateUserAndToken(party.Token, party.UserID, party.Email, party.Location))
            {
                var parties = await PartyService.GetParty(party);
                if (parties.Count > 0)
                {
                    return Ok(parties);
                }
                else
                {
                    return BadRequest("No Party Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Route("api/party/UpdateParty")]
        public async Task<IHttpActionResult> UpdateParty(PartyModel party)
        {
            if (party == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (party.ID == 0)
            {
                return BadRequest("Please provide valid party ID!");
            }

            if (string.IsNullOrEmpty(party.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(party.Token, party.UserID, party.Email, party.Location))
            {
                if (await PartyService.PartyExists(party))
                {
                    if (await PartyService.UpdateParty(party))
                    {
                        return Ok("Party Updated Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Update Party!");
                    }
                }
                else
                {
                    return BadRequest("No Such Party Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Route("api/party/DeleteParty")]
        public async Task<IHttpActionResult> DeleteParty(PartyModel party)
        {
            if (party == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (party.ID == 0)
            {
                return BadRequest("Please provide valid party ID!");
            }

            if (string.IsNullOrEmpty(party.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(party.Token, party.UserID, party.Email, party.Location))
            {
                if (await PartyService.PartyExists(party))
                {
                    if (await PartyService.DeleteParty(party))
                    {
                        return Ok("Party Deleted Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Delete Party!");
                    }
                }
                else
                {
                    return BadRequest("No Such Party Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}