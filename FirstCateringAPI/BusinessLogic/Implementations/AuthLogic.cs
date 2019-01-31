using FirstCateringAPI.BusinessLogic.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FirstCateringAPI.BusinessLogic.Implementations
{
    public class AuthLogic : IAuthLogic
    {
        private readonly IConfiguration _config;

        public AuthLogic(IConfiguration config)
        {
            _config = config;
        }

        public bool AuthorizedUser(string username, string password)
        {
            var storedUsername = _config.GetValue<string>("AppSettings:Secrets:ApiUsername");
            var storedPassword = _config.GetValue<string>("AppSettings:Secrets:ApiPassword");

            return (username == storedUsername && password == storedPassword);
        }


        public JwtSecurityToken CreateSecurityToken(string username)
        {
            var securityKey = _config.GetValue<string>("AppSettings:Secrets:SecurityKey");
            var validIssuer = _config.GetValue<string>("AppSettings:Secrets:ValidIssuer");
            var validAudience = _config.GetValue<string>("AppSettings:Secrets:ValidAudience");
           // var tokenExpires = _config.GetValue<int>("AppSettings:TokenTimeout");

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new Claim("user",username)
            };

            var token = new JwtSecurityToken(
                issuer: validIssuer,
                audience: validAudience,
                //expires: DateTime.Now.AddMinutes(tokenExpires),
                expires: DateTime.Now.AddMonths(1), // setting expiration to one month for testing purposes.
                claims: claims,
                signingCredentials: signingCredentials);

            return token;
        }


        public string[] GetUsernameAndPassword(string authHeader)
        {
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");

            var encodedUsernameAndPassword = authHeader.Substring("Basic ".Length).Trim();
            var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernameAndPassword));
            var usernamePasswordArray = usernamePassword.Split(':');

            return usernamePasswordArray;
        }
    }
}