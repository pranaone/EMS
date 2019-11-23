using ElectionSystem_AD.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using ElectionSystem_AD.Services.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ElectionSystem_AD.Models.Common;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace ElectionSystem_AD.Controllers.Auth
{
    // ********** IMPORTANT *************** //
    // Run the below code in the nuget package manager console
    // Install-Package Microsoft.AspNet.WebApi.Cors

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthController : ApiController
    {
        /// <summary>
        /// User Register End Point
        /// </summary>
        /// <param name="userForRegisterDto"></param>
        /// <returns></returns>
        [HttpPost, Route("api/auth/Register")]
        public async Task<IHttpActionResult> Register(User userForRegisterDto)
        {
            if (userForRegisterDto == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (string.IsNullOrEmpty(userForRegisterDto.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.UserExists(userForRegisterDto.Email, userForRegisterDto.Location, userForRegisterDto.UserID))
            {
                return BadRequest("Email Already Exists");
            }

            var usertocreate = new User
            {
                Firstname = userForRegisterDto.Firstname,
                Lastname = userForRegisterDto.Lastname,
                PostalCode = userForRegisterDto.PostalCode,
                Email = userForRegisterDto.Email,
                Gender = userForRegisterDto.Gender,
                MobileNo = userForRegisterDto.MobileNo,
                RoleID = userForRegisterDto.RoleID,
                Location = userForRegisterDto.Location
            };

            var isRegistrationComplete = await AuthService.Register(usertocreate, userForRegisterDto.Password);

            if (isRegistrationComplete.IsError)
            {
                return BadRequest("Registration Not Successful!");
            }
            else
            {
                return Ok("Registration Successful!");
            }

        }

        /// <summary>
        /// User Login End Point
        /// </summary>
        /// <param name="userForLoginDto"></param>
        /// <returns></returns>
        [HttpPost, Route("api/auth/Login")]
        public async Task<IHttpActionResult> Login(User userForLoginDto)
        {
            if (userForLoginDto == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (string.IsNullOrEmpty(userForLoginDto.Email) || string.IsNullOrEmpty(userForLoginDto.Password))
            {
                return BadRequest("email or password values cannot be empty!");
            }

            if (string.IsNullOrEmpty(userForLoginDto.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            var userFromRepo = await AuthService.Login(userForLoginDto.Email, userForLoginDto.Password, userForLoginDto.Location);

            if (userFromRepo == null)
            {
                return Unauthorized();
            }
            else
            {
                var claimsData = new[] {
                        new Claim(ClaimTypes.NameIdentifier, userForLoginDto.ID.ToString()),
                        new Claim(ClaimTypes.Name, userForLoginDto.Email),
                        new Claim(ClaimTypes.Role, userFromRepo.RoleID.ToString())
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super secret key for creating the token"));

                var signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claimsData),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = signingCreds

                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                Ping ping = new Ping();
                var replay = ping.Send(Dns.GetHostName());
                IPAddress userIP = null;
                if (replay.Status == IPStatus.Success)
                {
                    userIP = replay.Address;
                }

                OperatingSystem os_info = System.Environment.OSVersion;
                
                LoggedInUserDTO loggedInUser = new LoggedInUserDTO()
                {
                    ID = userFromRepo.ID,
                    Firstname = userFromRepo.Firstname,
                    Lastname = userFromRepo.Lastname,
                    Email = userFromRepo.Email,
                    Gender = userFromRepo.Gender,
                    RoleID = userFromRepo.RoleID,
                    RegisteredDate = userFromRepo.RegisteredDate,
                    Token = tokenHandler.WriteToken(token),
                    UserLoggedInTimezone = TimeZone.CurrentTimeZone.ToString(),
                    UserLoginDate = DateTime.Now.ToString(),
                    UserLoggedInIP = userIP.ToString(),
                    UserLoginOs = os_info.VersionString,
                };

                  CommonResponse response = await AuthService.UserLoginDetailsAdded(loggedInUser, userForLoginDto.Location);

                return Ok(loggedInUser);

            }
        }

        /// <summary>
        /// User Logout End Point
        /// </summary>
        /// <param name="userLogoutDto"></param>
        /// <returns></returns>
        [HttpPost, Route("api/auth/Logout")]
        public async Task<IHttpActionResult> Logout(LoggedInUserDTO userLogoutDto)
        {
            if (userLogoutDto == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (userLogoutDto.ID == 0)
            {
                return BadRequest("Please provide valid user ID!");
            }

            if (string.IsNullOrEmpty(userLogoutDto.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(userLogoutDto.Token, userLogoutDto.ID, userLogoutDto.Email, userLogoutDto.Location))
            {
                LoggedInUserDTO loggedOutUserDTO = new LoggedInUserDTO()
                {
                    UserLoggedOutDate = DateTime.Now.ToString(),
                    Token = userLogoutDto.Token,
                    ID = userLogoutDto.ID,
                    Email = userLogoutDto.Email
                };

                var response = await AuthService.UserLoginDetailsUpdate(loggedOutUserDTO, userLogoutDto.Location);

                if (!response.IsError)
                {
                    return Ok("Logged Out Successfully!");
                }
                else
                {
                    return BadRequest("Logout Not Successful!");
                }
            }
            else
            {
                return Unauthorized();
            }

        }
    }
}