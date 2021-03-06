﻿using System.IdentityModel.Tokens.Jwt;
using FirstCateringAPI.BusinessLogic.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FirstCateringAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthLogic _logic;

        public AuthController(IConfiguration config, IAuthLogic logic)
        {
            _config = config;
            _logic = logic;
        }



        /// <summary>
        /// User sends basic authorization header to generate a JwtBearer token which expires in 15 minutes. 
        /// The token is used as authentication for the rest of the end-points, so this should always be the first
        /// call to the API. The call will return unauthorized unless Basic authorization is supplied. The token is
        /// sent in a JSON object, { "token": "generatedToken" }. 
        /// </summary>
        /// <response code="200">Authorized, token in body</response>
        /// <response code="401">Unauthorized, basic auth not provided or credentials incorrect</response>
        [HttpPost("Token")]
        [ProducesResponseType(200),ProducesResponseType(401)]
        public IActionResult GetToken()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            string username, password;

            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                var usernameAndPassword = _logic.GetUsernameAndPassword(authHeader);
                username = usernameAndPassword[0];
                password = usernameAndPassword[1];
            }
            else
            {
                return Unauthorized();
            }
            if (!_logic.AuthorizedUser(username, password))
            {
                return Unauthorized();
            }

            var token = _logic.CreateSecurityToken(username);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });
        }
    }
}