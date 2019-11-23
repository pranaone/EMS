using ElectionSystem_AD.Models.Center;
using ElectionSystem_AD.Services.Auth;
using ElectionSystem_AD.Services.Center;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectionSystem_AD.Controllers.Center
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CenterController : ApiController
    {
        [HttpPost, Route("api/center/AddNewCenter")]
        public async Task<IHttpActionResult> AddNewCenter(CenterModel center)
        {
            if(center == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (center.CityID == 0)
            {
                return BadRequest("Please provide valid city ID!");
            }

            if (string.IsNullOrEmpty(center.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(center.Token, center.UserID, center.Email, center.Location))
            {
                if (await CenterService.CenterExists(center))
                {
                    return BadRequest("Center Already Exists");
                }
                else
                {
                    if (await CenterService.AddNewCenter(center))
                    {
                        return Ok("Center Added Successfully!");
                    }
                    else
                    {
                        return BadRequest("Center Adding Failed!");
                    }
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet, Route("api/center/GetCenters")]
        public async Task<IHttpActionResult> GetCenters(CenterModel center)
        {
            if (await AuthService.ValidateUserAndToken(center.Token, center.UserID, center.Email, center.Location))
            {
                var centers = await CenterService.GetCenter(center);
                if (centers.Count > 0)
                {
                    return Ok(centers);
                }
                else
                {
                    return BadRequest("No Such Center Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Route("api/center/UpdateCenter")]
        public async Task<IHttpActionResult> UpdateCenter(CenterModel center)
        {
            if (center == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (center.ID == 0)
            {
                return BadRequest("Please provide valid center ID!");
            }

            if (center.CityID == 0)
            {
                return BadRequest("Please provide valid city ID!");
            }

            if (string.IsNullOrEmpty(center.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(center.Token, center.UserID, center.Email, center.Location))
            {
                if (await CenterService.CenterExists(center))
                {
                    if (await CenterService.UpdateCenter(center))
                    {
                        return Ok("Center Updated Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Update Center!");
                    }
                }
                else
                {
                    return BadRequest("No Such Center Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Route("api/center/DeleteCenter")]
        public async Task<IHttpActionResult> DeleteCenter(CenterModel center)
        {
            if (center == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (center.ID == 0)
            {
                return BadRequest("Please provide valid center ID!");
            }

            if (center.CityID == 0)
            {
                return BadRequest("Please provide valid city ID!");
            }

            if (string.IsNullOrEmpty(center.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(center.Token, center.UserID, center.Email, center.Location))
            {
                if (await CenterService.CenterExists(center))
                {
                    if (await CenterService.DeleteCenter(center))
                    {
                        return Ok("Center Deleted Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Delete Center!");
                    }
                }
                else
                {
                    return BadRequest("No Such Center Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}