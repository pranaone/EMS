using ElectionSystem_AD.Models.ActionLog;
using ElectionSystem_AD.Models.Role;
using ElectionSystem_AD.Services.ActionLog;
using ElectionSystem_AD.Services.BaseService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ElectionSystem_AD.Services.Role
{
    public class RoleService : BaseAppTenant
    {
        /// <summary>
        /// Check If The Role Exists
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task<bool> RoleExists(RoleModel role)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
            {
                var isExistingUserQuery = "SELECT * from Role WHERE ID ='" + role.ID + "'";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingUserQuery, dbConn);
                    reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = role.UserID,
                        ActionPerformed = "Role Exists Error : " + ex.Message,
                        MethodName = "RoleExists",
                        IsError = true
                    },
                    role.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = role.UserID,
                        ActionPerformed = "Check If User Exists ",
                        MethodName = "RoleExists",
                        IsError = false
                    },
                    role.Location);
                }

            }

        }

        /// <summary>
        /// Service Method To Get User Roles
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task<List<RoleModel>> GetUserRoles(RoleModel role)
        {
            List<RoleModel> Roles = new List<RoleModel>();
            using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
            {
                var isExistingUserQuery = "SELECT * from Role";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingUserQuery, dbConn);
                    reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            RoleModel roleItem = new RoleModel();
                            roleItem.ID = reader.GetInt32(0);
                            roleItem.Name = reader.GetString(1);
                            roleItem.DateAdded = reader.GetDateTime(2);
                            Roles.Add(roleItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    reader = null;
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = role.UserID,
                        ActionPerformed = "Role Exists Error : " + ex.Message,
                        MethodName = "RoleExists",
                        IsError = true
                    },
                    role.Location);
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = role.UserID,
                        ActionPerformed = "Check If User Exists ",
                        MethodName = "UserExists",
                        IsError = false
                    },
                    role.Location);
                }

                return Roles;
            }
        }

        /// <summary>
        /// Add New Role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task<bool> AddNewRole(RoleModel role)
        {
            var dateCreated = DateTime.Now;
            String SQL = "INSERT INTO Role(Name,DateAdded)" +
                "VALUES('" + role.Name + "','" + dateCreated + "')";

            using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
            {
                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    await cmd.ExecuteNonQueryAsync();
                    dbConn.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = role.UserID,
                        ActionPerformed = "Role Add Error : " + ex.Message,
                        MethodName = "AddNewRole",
                        IsError = true
                    },
                    role.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = role.UserID,
                        ActionPerformed = "Role Added",
                        MethodName = "AddNewRole",
                        IsError = false
                    },
                    role.Location);
                }
            }
        }

        /// <summary>
        /// Update Role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateRole(RoleModel role)
        {
            String SQL = "UPDATE Role SET Name = '" + role.Name + "' WHERE ID = '" + role.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
            {
                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    await cmd.ExecuteNonQueryAsync();
                    dbConn.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = role.UserID,
                        ActionPerformed = "Role Update Error : " + ex.Message,
                        MethodName = "UpdateRole",
                        IsError = true
                    },
                    role.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = role.UserID,
                        ActionPerformed = "Role Updated",
                        MethodName = "UpdateRole",
                        IsError = false
                    },
                    role.Location);
                }
            }
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteRole(RoleModel role)
        {
            String SQL = "DELETE FROM Role WHERE Name = '" + role.Name + "' AND ID = '" + role.ID + "'";

            using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
            {
                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    await cmd.ExecuteNonQueryAsync();
                    dbConn.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = role.UserID,
                        ActionPerformed = "Role Delete Error : " + ex.Message,
                        MethodName = "DeleteRole",
                        IsError = true
                    },
                    role.Location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = role.UserID,
                        ActionPerformed = "Role Deleted",
                        MethodName = "DeleteRole",
                        IsError = false
                    },
                    role.Location);
                }
            }
        }

    }
}