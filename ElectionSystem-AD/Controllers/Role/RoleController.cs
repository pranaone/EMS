using ElectionSystem_AD.Models.Role;
using ElectionSystem_AD.Services.Auth;
using ElectionSystem_AD.Services.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectionSystem_AD.Controllers.Role
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RoleController : ApiController
    {
        /// <summary>
        /// API END POINT TO ADD A NEW USER ROLE
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost, Route("api/role/AddUserRole")]
        public async Task<IHttpActionResult> AddUserRole(RoleModel role)
        {
            if(role == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (string.IsNullOrEmpty(role.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(role.Token, role.UserID, role.Email, role.Location))
            {
                if(await RoleService.RoleExists(role))
                {
                    return BadRequest("User Role Already Exists");
                }
                else
                {
                    if(await RoleService.AddNewRole(role))
                    {
                        return Ok("User Role Added Successfully!");
                    }
                    else
                    {
                        return BadRequest("User Role Adding Failed!");
                    }
                }
            }
            else
            {
                return Unauthorized();
            }

        }

        /// <summary>
        /// API END POINT TO GET USER ROLE
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet, Route("api/role/GetUserRoles")]
        public async Task<IHttpActionResult> GetUserRoles(RoleModel role)
        {
            if (await AuthService.ValidateUserAndToken(role.Token, role.UserID, role.Email, role.Location))
            {
                var userRoles = await RoleService.GetUserRoles(role);
                if (userRoles.Count > 0)
                {
                    return Ok(userRoles);
                }
                else
                {
                    return BadRequest("No Such Role Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API END POINT TO UPDATE USER ROLE
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost, Route("api/role/UpdateUserRole")]
        public async Task<IHttpActionResult> UpdateUserRole(RoleModel role)
        {
            if (role == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (role.ID == 0)
            {
                return BadRequest("Please provide valid Role ID!");
            }

            if (string.IsNullOrEmpty(role.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(role.Token, role.UserID, role.Email, role.Location))
            {
                if (await RoleService.RoleExists(role))
                {
                    if (await RoleService.UpdateRole(role))
                    {
                        return Ok("User Role Updated Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Update User Role!");
                    }
                }
                else
                {
                    return BadRequest("No Such Role Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// API END POINT TO DELETE A USER ROLE
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost, Route("api/role/DeleteUserRole")]
        public async Task<IHttpActionResult> DeleteUserRole(RoleModel role)
        {
            if (role == null)
            {
                return BadRequest("Please provide valid inputs!");
            }

            if (role.ID == 0)
            {
                return BadRequest("Please provide valid Role ID!");
            }

            if (string.IsNullOrEmpty(role.Location))
            {
                return BadRequest("Please provide valid location!");
            }

            if (await AuthService.ValidateUserAndToken(role.Token, role.UserID, role.Email, role.Location))
            {
                if (await RoleService.RoleExists(role))
                {
                    if (await RoleService.DeleteRole(role))
                    {
                        return Ok("User Role Deleted Successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed To Delete User Role!");
                    }
                }
                else
                {
                    return BadRequest("No Such Role Exists!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}