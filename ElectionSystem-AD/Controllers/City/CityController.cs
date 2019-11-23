using ElectionSystem_AD.Models.City;
using ElectionSystem_AD.Services.Auth;
using ElectionSystem_AD.Services.City;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectionSystem_AD.Controllers.City
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CityController : ApiController
    {
        [HttpPost, Route("api/city/AddNewCity")]
        public async Task<IHttpActionResult> AddNewCity(CityModel city)
        {
            if (city == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (city.DistrictID == 0)
            {
                return BadRequest("Please provide valid district ID!");
            }

            if (string.IsNullOrEmpty(city.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(city.Token, city.UserID, city.Email, city.Location))
            {
                if (await CityService.CityExists(city))
                {
                    return BadRequest("City Already Exists");
                }
                else
                {
                    if (await CityService.AddNewCity(city))
                    {
                        return Ok("City Added Successfully!");
                    }
                    else
                    {
                        return BadRequest("City Adding Failed!");
                    }
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet, Route("api/city/GetCities")]
        public async Task<IHttpActionResult> GetCities(CityModel city)
        {
            if (await AuthService.ValidateUserAndToken(city.Token, city.UserID, city.Email, city.Location))
            {
                var cities = await CityService.GetCity(city);
                if (cities.Count > 0)
                {
                    return Ok(cities);
                }
                else
                {
                    return BadRequest("No Such City Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Route("api/city/UpdateCity")]
        public async Task<IHttpActionResult>UpdateCity(CityModel city)
        {
            if (city == null)
            {
                return BadRequest("Please provide valid inputs!");
            }
            if (city.ID == 0)
            {
                return BadRequest("Please provide valid city ID!");
            }

            if (city.DistrictID == 0)
            {
                return BadRequest("Please provide valid district ID!");
            }

            if (string.IsNullOrEmpty(city.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(city.Token, city.UserID, city.Email, city.Location))
            {
                if (await CityService.CityExists(city))
                {
                    if (await CityService.UpdateCity(city))
                    {
                        return Ok("City Updated Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Update City!");
                    }
                }
                else
                {
                    return BadRequest("No Such City Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Route("api/city/DeleteCity")]
        public async Task<IHttpActionResult> DeleteCity(CityModel city)
        {
            if (city == null)
            {
                return BadRequest("Please provide valid inputs!");
            }
            if (city.ID == 0)
            {
                return BadRequest("Please provide valid city ID!");
            }

            if (city.DistrictID == 0)
            {
                return BadRequest("Please provide valid district ID!");
            }

            if (string.IsNullOrEmpty(city.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(city.Token, city.UserID, city.Email, city.Location))
            {
                if (await CityService.CityExists(city))
                {
                    if (await CityService.DeleteCity(city))
                    {
                        return Ok("City Deleted Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Delete City!");
                    }
                }
                else
                {
                    return BadRequest("No Such City Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}