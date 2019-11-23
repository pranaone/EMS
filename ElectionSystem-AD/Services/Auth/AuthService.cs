using ElectionSystem_AD.Models.ActionLog;
using ElectionSystem_AD.Models.Common;
using ElectionSystem_AD.Models.User;
using ElectionSystem_AD.Services.ActionLog;
using ElectionSystem_AD.Services.BaseService;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ElectionSystem_AD.Services.Auth
{
    public class AuthService : BaseAppTenant
    {
        /// <summary>
        /// Check If the user is already Existing In the System
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<bool> UserExists(string username, string location, int userID)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
            {
                var isExistingUserQuery = "SELECT * from [User] WHERE email ='" + username + "'";
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
                        UserID = userID,
                        ActionPerformed = "User Exists Error : " + ex.Message,
                        MethodName = "UserExists",
                        IsError = true
                    }, 
                    location);
                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = userID,
                        ActionPerformed = "Check If User Exists ",
                        MethodName = "UserExists",
                        IsError = false
                    },
                    location);
                }

            }

        }

        /// <summary>
        /// Regster User
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<CommonResponse> Register(User user, string password)
        {
            var response = new CommonResponse();
            string passwordHash, passwordSalt;

            GeneratePasswordHash(password, out passwordHash, out passwordSalt);

            user.Password = passwordHash;
            user.Salt = passwordSalt;

            try
            {
                using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
                {
                    dbConn.Open();
                    String SQL = GetUserInsertQuery(user);
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    await cmd.ExecuteNonQueryAsync();

                    dbConn.Close();
                }


                response.IsError = false;


                //var verificationMailSent = SendEmailVerification(user);

                //if (verificationMailSent)
                //{
                //    response.IsError = false;
                //}
                //else
                //{
                //    response.IsError = true;
                //    response.Message = "Error in sending email with verification link";
                //}
            }
            catch (Exception)
            {
                response.IsError = true;
                response.Message = "Error in Registering User!";
            }
            finally
            {
                ActionLogService.LogAction(new ActionLogModel()
                {
                    UserID = user.UserID,
                    ActionPerformed = "Check If User Exists ",
                    MethodName = "UserExists",
                    IsError = false
                },
                    user.Location);
            }

            return response;

        }

        /// <summary>
        /// Generate The Password Hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        private static void GeneratePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var salt = new byte[256];
            rng.GetBytes(salt);
            passwordSalt = Convert.ToBase64String(salt);

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password + passwordSalt);
            System.Security.Cryptography.SHA256Managed sha256hashString = new System.Security.Cryptography.SHA256Managed();
            byte[] hash = sha256hashString.ComputeHash(bytes);

            passwordHash = Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Insert User Query
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static string GetUserInsertQuery(User user)
        {
            return "INSERT INTO [User](Firstname,Lastname,Address_line_1,Address_line_2,PostalCode,Email," +
                    "Gender,MobileNo,Password,Salt,RoleID,RegisteredDate,ActiveStatus) " +
                    "VALUES('" + user.Firstname + "','" +
                                    user.Lastname + "','" +
                                    user.Address_line_1 + "','" +
                                    user.Address_line_2 + "','" +
                                    user.PostalCode + "','" +
                                    user.Email + "','" +
                                    user.Gender + "','" +
                                    user.MobileNo + "','" +
                                    user.Password + "','" +
                                    user.Salt + "','" +
                                    user.RoleID + "',GETDATE(),'"+ user.ActiveStatus + "')";
        }

        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<User> Login(string email, string password, string location)
        {
            User logginInUser = new User();
            using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
            {
                var isExistingUserQuery = "SELECT * from [User] WHERE Email ='" + email + "'";
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
                            logginInUser.ID = reader.GetInt32(0);
                            logginInUser.Firstname = reader.GetString(1);
                            logginInUser.Lastname = reader.GetString(2);
                            logginInUser.Address_line_1 = reader.GetString(3);
                            logginInUser.Address_line_2 = reader.GetString(4);
                            logginInUser.PostalCode = reader.GetString(5);
                            logginInUser.Email = reader.GetString(6);
                            logginInUser.Password = reader.GetString(7);
                            logginInUser.Salt = reader.GetString(8);
                            logginInUser.Gender = reader.GetString(9);
                            logginInUser.MobileNo = reader.GetString(10);
                            logginInUser.RoleID = reader.GetInt32(11);
                            logginInUser.RegisteredDate = reader.GetDateTime(12);
                            logginInUser.ActiveStatus = reader.GetInt32(13);
                        }
                    }
                    else
                    {
                        logginInUser = null;
                    }
                }
                catch (Exception)
                {
                    reader = null;
                    logginInUser = null;
                }
                finally
                {
                    dbConn.Close();
                }

            }

            if (!VerifyPasswordHash(password, logginInUser.Password, logginInUser.Salt))
            {
                return null;
            }

            return logginInUser;
        }

        /// <summary>
        /// Verify The Passsword hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="user_passwordHash"></param>
        /// <param name="user_password_salt"></param>
        /// <returns></returns>
        private static bool VerifyPasswordHash(string password, string user_passwordHash, string user_password_salt)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password + user_password_salt);
            System.Security.Cryptography.SHA256Managed sha256hashString = new System.Security.Cryptography.SHA256Managed();
            byte[] hash = sha256hashString.ComputeHash(bytes);

            var enteredPasswordHas = Convert.ToBase64String(hash);

            if (enteredPasswordHas != user_passwordHash)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Service Method To Login The User Login Information
        /// </summary>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<CommonResponse> UserLoginDetailsAdded(LoggedInUserDTO user, string location)
        {
            var response = new CommonResponse();
            String SQL = "INSERT INTO User_Login(userID," +
                                                "user_role," +
                                                "user_login_os," +
                                                "user_login_date," +
                                                "user_logged_in_timezone," +
                                                "user_logged_in_IP," +
                                                "user_logged_out_date," +
                                                "token)" +
                        "VALUES('" + user.ID + "','"
                                     + user.RoleID + "','"
                                     + user.UserLoginOs + "', GETDATE() ,'"
                                     + user.UserLoggedInTimezone + "','"
                                     + user.UserLoggedInIP + "','"
                                     + user.UserLoggedOutDate + "','"
                                     + user.Token
                                     + "')";
            try
            {
                using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    await cmd.ExecuteNonQueryAsync();

                    dbConn.Close();
                }

            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.Message = "Error in Registering User!";
                ActionLogService.LogAction(new ActionLogModel()
                {
                    UserID = user.UserID,
                    ActionPerformed = "User Login Error : " + ex.Message,
                    MethodName = "UserLoginDetailsAdded",
                    IsError = true
                },
                    location);
            }

            return response;
        }

        /// <summary>
        /// Get The Name Of the User From The Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetName(string token)
        {
            string secret = "super secret key for creating the token";
            var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(token, validations, out var tokenSecure);
            return claims.Identity.Name;
        }

        /// <summary>
        /// Service Method To Get The Role From The Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetRole(string token)
        {
            var newToken = new JwtSecurityToken(jwtEncodedString: token);
            //Console.WriteLine("email => " + newToken.Claims.First(c => c.Type == "Role").Value);
            var role = newToken.Claims.First(c => c.Type == "role").Value;
            return role;
        }

        /// <summary>
        /// Method to Validate the Token And The User
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userID"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static async Task<bool> ValidateUserAndToken(string token, int userID, string email, string location)
        {
            HttpContext httpContext = HttpContext.Current;
            string authHeader = httpContext.Request.Headers["Authorization"];

            if (authHeader == null)
            {
                return false;
            }
            else
            {
                var name = GetName(token);

                if (checkIfTokenHasExpired(token))
                {
                    var response = await DeleteTokenInfo(token, userID, email, location);
                    if(!response.IsError)
                    {
                        ActionLogService.LogAction(new ActionLogModel()
                        {
                            UserID = userID,
                            ActionPerformed = "Error In Deleteing Token Info",
                            MethodName = "DeleteTokenInfo",
                            IsError = true
                        },
                        location);
                    }
                    return false;
                }
                else
                {
                    if (AuthorizeUser(token, userID, location))
                    {
                        if (email == name)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Service Method to Check if the Token Has Expired
        /// </summary>
        /// <param name="tokenString"></param>
        /// <returns></returns>
        public static bool checkIfTokenHasExpired(string tokenString)
        {
            bool hasExpired = false;
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(tokenString) as JwtSecurityToken;
            var tokenExpiryDate = token.ValidTo;

            // If there is no valid `exp` claim then `ValidTo` returns DateTime.MinValue
            if (tokenExpiryDate == DateTime.MinValue)
            {
                hasExpired = true;
                throw new Exception("Could not get exp claim from token");
            }

            // If the token is in the past then you can't use it
            if (tokenExpiryDate < DateTime.UtcNow)
            {
                hasExpired = true;
                throw new Exception($"Token expired on: {tokenExpiryDate}");
            }

            return hasExpired;

        }

        /// <summary>
        /// Authorize User Based on Information in the User Login Table
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userID"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool AuthorizeUser(string token, int userID, string location)
        {
            using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
            {
                var isExistingUserQuery = "SELECT * from User_Login WHERE Token = '" + token + "' AND userID ='" + userID + "' AND user_logged_out_date IS NOT NULL ";
                SqlDataReader reader;

                try
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand(isExistingUserQuery, dbConn);
                    reader = cmd.ExecuteReader();
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
                        UserID = userID,
                        ActionPerformed = "User Login Error : " + ex.Message,
                        MethodName = "UserLoginDetailsAdded",
                        IsError = true
                    },
                    location);

                    return false;
                }
                finally
                {
                    dbConn.Close();
                    ActionLogService.LogAction(new ActionLogModel()
                    {
                        UserID = userID,
                        ActionPerformed = "Check If User Exists ",
                        MethodName = "AuthorizeUser",
                        IsError = false
                    },
                    location);
                }
            }
        }

        /// <summary>
        /// Update The User Login Table When the User Logs out
        /// </summary>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<CommonResponse> UserLoginDetailsUpdate(LoggedInUserDTO user, string location)
        {
            var response = new CommonResponse();
            String SQL = "UPDATE User_Login " +
                            "SET user_logged_out_date= GETDATE() WHERE Token = '" + user.Token + "' AND UserID = '" + user.ID + "'";
            try
            {
                using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    await cmd.ExecuteNonQueryAsync();

                    dbConn.Close();
                }

            }
            catch (Exception)
            {
                response.IsError = true;
                response.Message = "Error in Updating the Logout Details For User!";
            }

            return response;
        }

        /// <summary>
        /// Service Method To Delete the token from the db if the token has expired
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userID"></param>
        /// <param name="email"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<CommonResponse> DeleteTokenInfo(string token, int userID, string email, string location)
        {
            var response = new CommonResponse();
            String SQL = "DELETE FROM User_Login WHERE Token = '" + token + "' AND UserID = '" + userID + "'";
            try
            {
                using (SqlConnection dbConn = new SqlConnection(selectConnection("main")))
                {
                    dbConn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = dbConn;
                    await cmd.ExecuteNonQueryAsync();

                    dbConn.Close();
                }

            }
            catch (Exception)
            {
                response.IsError = true;
                response.Message = "Error in Deleting the Logout Details For User!";
            }

            return response;
        }
    }
}