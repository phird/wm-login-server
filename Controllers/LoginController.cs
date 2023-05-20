using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;
using wm_api.Data;
using MongoDB.Driver;
using System.Net;
using System.Security.Claims;

using wm_api.Models;

namespace wm_api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    // Store password hash 
    // Retrive User from MangoDB
    // Verify Password 
    // token Generate  ✅

    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private DbContext _context;

        public LoginController(DbContext context, IConfiguration configuration)
        {
            _config = configuration;
            _context = context;

        }


        // this method authen user 
        private Users AuthenticateUser(Users user)
        { // we sent user Username, Password
            Users authenticatedUser = null;
            //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password);

            authenticatedUser = _context.Users.Find(u => u.username == user.username).FirstOrDefault();

            if (authenticatedUser != null && BCrypt.Net.BCrypt.Verify(user.password, authenticatedUser.password))
            {
                Users response = new Users
                {
                    username = authenticatedUser.username,
                    firstName = authenticatedUser.firstName,
                    lastName = authenticatedUser.lastName,
                };
                return response;
            }
            return null;
        }


        // after authen we generate token for user 


        [AllowAnonymous]
        [HttpPost]

        public IActionResult Login(Users user)
        {
            Users authUser = AuthenticateUser(user);
            if (authUser != null)
            {
                var token = GenerateToken(authUser);
                return Ok(new { token = token });
            }
            else
            {
                return Ok(new { token = "", status = HttpStatusCode.Unauthorized });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private string GenerateToken(Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                new Claim[]
                {
            new Claim("username", user.username),
            new Claim("firstName", user.firstName),
            new Claim("lastName", user.lastName),
            new Claim("userId", user.Id.ToString())
                },
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }
}
