
using ApiChallenge.Filters;
using ApiChallenge.Helpers;
using ApiChallenge.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace ApiChallenge.Controllers
{
    public class DeveloperController : ApiController
    {
        // Post api/Developer/GetDevelopers
        [HttpPost]
        [ResponseType(typeof(TotalDeveloper))]
        [JwtAuthentication]
        public IHttpActionResult GetDevelopers(DeveloperRequest developerRequest)
        {
            try
            {
                if (developerRequest == null)
                    return Content(HttpStatusCode.BadRequest, new Response<string>("400", "Mobile Phone is mandatory."));
                else if (!ModelState.IsValid)
                    return Content(HttpStatusCode.BadRequest, new Response<string>("400", "This feild is mandatory", null, ModelState));
                var profile = JwtManager.GetProfile(Request.Headers.Authorization.Parameter);
                string query = "SELECT * FROM Developer where U_ID = @uid and D_CreatedAt >= @FromDate and D_CreatedAt <= @ToDate ORDER BY D_ID DESC OFFSET " + (developerRequest.Page - 1) * 5 + " ROWS FETCH NEXT 5 ROW ONLY";
                SqlConnection conn = new SqlConnection(Utils.connStr);
                conn.Open();

                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@uid", profile.U_ID);
                command.Parameters.AddWithValue("@FromDate", developerRequest.From);
                command.Parameters.AddWithValue("@ToDate", developerRequest.To);
                TotalDeveloper result = new TotalDeveloper();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Developer item = new Developer()
                        {
                            D_ID = (int)reader["D_ID"],
                            U_ID = reader["U_ID"].ToString(),
                            D_Name = reader["D_Name"].ToString(),
                            D_Age = reader["D_Age"].ToString(),
                            D_CreatedBy = reader["D_CreatedBy"].ToString(),
                            D_CreatedAt = Convert.ToDateTime(reader["D_CreatedAt"]).ToString("dd-MMM-yyy")
                        };
                        result.Reports.Add(item);
                    }
                }
                command.Parameters.Clear();
                string selectQuerry = "SELECT COUNT(1) FROM Developer where D_CreatedBy= @emial and D_CreatedAt >= @FromDate and D_CreatedAt <= @ToDate";
                SqlCommand selectCommand = new SqlCommand(selectQuerry, conn);
                selectCommand.Parameters.AddWithValue("@emial", profile.U_Email);
                selectCommand.Parameters.AddWithValue("@FromDate", developerRequest.From);
                selectCommand.Parameters.AddWithValue("@ToDate", developerRequest.To);


                Int32 count = (Int32)selectCommand.ExecuteScalar();
                int pages = (int)Math.Ceiling(count / 5.0);
                result.TotalPages = pages;
                conn.Close();
                return Content(HttpStatusCode.OK, new Response<TotalDeveloper>("200", "Developers Fetch Successfully.", result)); ;
            }
            catch (Exception ex) { }
            return Content(HttpStatusCode.InternalServerError, new Response<string>("500", "Something went wrong.Please try again later.")); ;
        }

        // Post api/Developer/AddDevelopers
        [HttpPost]
        [ResponseType(typeof(string))]
        [JwtAuthentication]
        public IHttpActionResult AddDeveloper(Developer developer)
        {
            try
            {
                if (developer == null)
                    return Content(HttpStatusCode.BadRequest, new Response<string>("400", "Request should not be null"));
                else if (!ModelState.IsValid)
                    return Content(HttpStatusCode.BadRequest, new Response<string>("400", "This feild is mandatory", null, ModelState));

                var profile = JwtManager.GetProfile(Request.Headers.Authorization.Parameter);
                SqlConnection conn = new SqlConnection(Utils.connStr);
                conn.Open();
                using (conn)
                {
                    string insertQuerry = "INSERT INTO Developer(D_Name,D_Email,D_CreatedBy,D_CreatedAt,D_Age,U_ID) VALUES(@dname,@demail,@dCreatedBy,@dCreatedAt,@dage,@uID)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuerry, conn))
                    {
                        insertCommand.Parameters.AddWithValue("@dname", developer.D_Name);
                        insertCommand.Parameters.AddWithValue("@demail", developer.D_Email);
                        insertCommand.Parameters.AddWithValue("@dage", developer.D_Age);
                        insertCommand.Parameters.AddWithValue("@dCreatedBy", developer.D_CreatedBy);
                        insertCommand.Parameters.AddWithValue("@dCreatedAt", developer.D_CreatedAt);
                        insertCommand.Parameters.AddWithValue("@uID", profile.U_ID);
                        insertCommand.ExecuteNonQuery();
                    }
                }
                conn.Close();
                return Content(HttpStatusCode.OK, new Response<string>("200", "Developer Added successfully."));
            }
            catch (Exception ex) { }
            return Content(HttpStatusCode.InternalServerError, new Response<string>("500", "Something went wrong.Please try again later.")); ;

        }
    }
}