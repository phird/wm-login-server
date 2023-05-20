using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using wm_api.Models;
using wm_api.Services;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using wm_api.Data;
using BCrypt.Net;

namespace wm_api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly DbContext _context ;
        private readonly IUserService _userService;

        public RegistrationController(IUserService userService, DbContext context){
            _userService = userService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username is already taken
            var IsUsernameTaken = await _userService.IsUsernameTaken(registrationModel.username);
            if(IsUsernameTaken){
                System.Console.WriteLine("username already taken ");
                ModelState.AddModelError("username", "username is already taken");
                return BadRequest(ModelState);
            }

            // Check if password matches confirmPassword
            if (registrationModel.password != registrationModel.confirm)
            {
                System.Console.WriteLine("Password not match ! ");
                ModelState.AddModelError("ConfirmPassword", "Password and confirm password do not match.");
                return BadRequest(ModelState);
            }

            var user = new Users {
                firstName = registrationModel.firstName,
                lastName= registrationModel.lastName,
                username = registrationModel.username,
                password = BCrypt.Net.BCrypt.HashPassword(registrationModel.password) // hash password 
            };

            _context.Users.InsertOne(user);
            return Ok(new { Message = "Registration successful.", status = 200 });
        }

    }
}
