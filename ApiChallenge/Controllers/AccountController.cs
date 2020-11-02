using ApiChallenge.Helpers;
using ApiChallenge.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI.WebControls;

namespace ApiChallenge.Controllers
{
    public class AccountController : ApiController
    {
        // GET: Account
        // Post api/Account/Login
        [HttpPost]
        [ResponseType(typeof(User))]
        [AllowAnonymous]
        public IHttpActionResult Login(Models.Login loginReqest)
        {
            try
            {
                if (loginReqest == null)
                    return Content(HttpStatusCode.BadRequest, new Response<string>("400", "Request should not be null"));
                else if (!ModelState.IsValid)
                    return Content(HttpStatusCode.BadRequest, new Response<string>("400", "This feild is mandatory", null, ModelState));
                var user = new User();
                if (loginReqest.U_Email != null)
                {
                    string query = "SELECT * FROM AppUser where U_Email= @userEmail";
                    SqlConnection conn = new SqlConnection(Utils.connStr);
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@userEmail", loginReqest.U_Email);
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        user = new User()
                        {
                            U_ID = (int)reader["U_ID"],
                            U_Name = reader["U_Name"].ToString(),
                            U_Email = reader["U_Email"].ToString(),
                            U_Password = reader["U_Password"].ToString(),
                        };
                    }
                    else
                    {
                        conn.Close();
                        return Content(HttpStatusCode.BadRequest, new Response<string>("400", "Email or Password dosen't match."));
                    }
                    conn.Close();
                    if (user.U_Email != null)
                    {
                        var auth = BCrypt.Net.BCrypt.Verify(loginReqest.U_Password, user.U_Password);
                        if (auth)
                        {
                            var token = JwtManager.GenerateToken(user);
                            user.Token = token;
                            return Content(HttpStatusCode.OK, new Response<User>("200", "Login Succesfully.", user));
                        }
                        else
                            return Content(HttpStatusCode.BadRequest, new Response<string>("400", "Email or Password doesn't match."));
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new Response<string>(ex.StackTrace, ex.Message));
            };
            return Content(HttpStatusCode.InternalServerError, new Response<string>("500", "Something went wrong.Please try again later."));
        }

        // Post api/Account/Signup
        [HttpPost]
        [ResponseType(typeof(string))]
        [AllowAnonymous]
        public IHttpActionResult Signup(User userRequest)
        {
            try
            {
                if (userRequest == null)
                    return Content(HttpStatusCode.BadRequest, new Response<string>("400", "Request should not be null"));
                else if (!ModelState.IsValid)
                    return Content(HttpStatusCode.BadRequest, new Response<string>("400", "This feild is mandatory", null, ModelState));

                string query = "SELECT * FROM AppUser where U_Email= @userEmail";
                SqlConnection conn = new SqlConnection(Utils.connStr);
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@userEmail", userRequest.U_Email);
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    conn.Close();
                    return Content(HttpStatusCode.BadRequest, new Response<string>("1", "Email already registered, Use another Email."));
                }
                else
                {
                    userRequest.U_Email = BCrypt.Net.BCrypt.HashPassword(userRequest.U_Password);
                    using (conn)
                    {
                        string insertQuerry = "INSERT INTO AppUser(U_Name,U_Email,U_Password) VALUES(@name,@email,@password)";
                        using (SqlCommand insertCommand = new SqlCommand(insertQuerry, conn))
                        {
                            insertCommand.Parameters.AddWithValue("@name", userRequest.U_Name);
                            insertCommand.Parameters.AddWithValue("@email", userRequest.U_Email);
                            insertCommand.Parameters.AddWithValue("@password", userRequest.U_Password);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                    return Content(HttpStatusCode.OK, new Response<string>("200", "Signup created successfully."));
                }
            }
            catch { }
            return Content(HttpStatusCode.InternalServerError, new Response<string>("500", "Something went wrong.Please try again later.")); ;

        }
    }
}