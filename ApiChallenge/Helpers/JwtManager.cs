using ApiChallenge.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;

namespace ApiChallenge.Helpers
{
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private const string Secret = "bzWGnxyrTkf0F+DFuCb5xCBQWJtFRa2AAAcns8rBsUwHqOE/Sj0wp2sIDGvJpRrgNL/pAzBuhI2fnIuQbT4VPA==";

        public static string GenerateToken(User data, int expireMinutes = 1000)
        {

            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, data.U_Email),
                            new Claim(ClaimTypes.NameIdentifier, data.U_ID.ToString()),
                            new Claim("FullName", data.U_Name),
                            new Claim("Pass", data.U_Password)
                        }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal;
            }

            catch (Exception)
            {
                return null;
            }
        }

        public static User GetProfile(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var tokenData = handler.ReadToken(token) as JwtSecurityToken;
                var profile = new User()
                {
                    U_ID = int.Parse(tokenData.Claims.First(claim => claim.Type == "nameid").Value),
                    U_Email = tokenData.Claims.First(claim => claim.Type == "unique_name").Value,
                    U_Name = tokenData.Claims.First(claim => claim.Type == "FullName").Value,
                    U_Password = tokenData.Claims.First(claim => claim.Type == "Pass").Value
                };
                return profile;

            }
            catch (Exception ex) { }
            return new User();
        }
    }
}