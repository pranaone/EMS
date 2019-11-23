using ElectionSystem_AD.Models.District;
using ElectionSystem_AD.Services.Auth;
using ElectionSystem_AD.Services.District;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectionSystem_AD.Controllers.District
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DistrictController : ApiController
    {
        /// <summary>
        /// API END POINT TO ADD A NEW DISTRICT
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        [HttpPost, Route("api/district/AddNewDistrict")]
        public async Task<IHttpActionResult> AddNewDistrict(DistrictModel district)
        {
            if (district == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (string.IsNullOrEmpty(district.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(district.Token, district.UserID, district.Email, district.Location))
            {
                if (int.Parse(AuthService.GetRole(district.Token)) != 1)
                {
                    return Unauthorized();
                }

                if (await DistrictService.DistrictExists(district))
                {
                    return BadRequest("District Already Exists");
                }
                else
                {
                    if (await DistrictService.AddNewDistrict(district))
                    {
                        return Ok("District Added Successfully!");
                    }
                    else
                    {
                        return BadRequest("District Adding Failed!");
                    }
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API END POINT TO GET A DISTRICT
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        [HttpGet, Route("api/district/GetDistricts")]
        public async Task<IHttpActionResult> GetDistrict(DistrictModel district)
        {
            if (district == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (string.IsNullOrEmpty(district.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(district.Token, district.UserID, district.Email, district.Location))
            {
                var districts = await DistrictService.GetDistrict(district);
                if (districts.Count > 0)
                {
                    return Ok(districts);
                }
                else
                {
                    return BadRequest("No Such District Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API END POINT TO UPDATE A DISTRICT
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        [HttpPost, Route("api/district/UpdateDistrict")]
        public async Task<IHttpActionResult> UpdateDistrict(DistrictModel district)
        {
            if (district == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (district.ID == 0)
            {
                return BadRequest("Please provide valid district ID!");
            }

            if (string.IsNullOrEmpty(district.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(district.Token, district.UserID, district.Email, district.Location))
            {
                if (await DistrictService.DistrictExists(district))
                {
                    if (await DistrictService.UpdateDistrict(district))
                    {
                        return Ok("District Updated Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Update District!");
                    }
                }
                else
                {
                    return BadRequest("No Such District Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API END POINT TO DELETE A DISTRICT
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        [HttpPost, Route("api/district/DeleteDistrict")]
        public async Task<IHttpActionResult> DeleteDistrict(DistrictModel district)
        {
            if (district == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (district.ID == 0)
            {
                return BadRequest("Please provide valid district ID!");
            }

            if (string.IsNullOrEmpty(district.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(district.Token, district.UserID, district.Email, district.Location))
            {
                if (await DistrictService.DistrictExists(district))
                {
                    if (await DistrictService.DeleteDistrict(district))
                    {
                        return Ok("District Deleted Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Delete District!");
                    }
                }
                else
                {
                    return BadRequest("No Such District!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}