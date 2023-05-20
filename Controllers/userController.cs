using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using wm_api.Models;
using wm_api.Data;
using MongoDB.Driver;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

[Route("api/v1/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DbContext _context;

    public UserController(DbContext context)
    {
        _context = context;
    }

    private IActionResult GetUsers()
    {
        var users = _context.Users.Find(_ => true).ToList();
        var result = users.Select(u => new
        {
            u.Id,
            u.firstName,
            u.lastName
        }).ToList();
        return Ok(result);
    }

    private IActionResult ByUsername(string username)
    {
        var user = _context.Users.Find(u => u.username == username).FirstOrDefault();

        if (user == null)
        {
            return NotFound(); // User not found
        }

        var userModel = new Users
        {
            Id = user.Id,
            username = user.username,
            //Password = user.Password,
            firstName = user.firstName,
            lastName = user.lastName,

        };

        return Ok(userModel);
    }

    // GET api/v1/users
    [HttpGet]
    public IActionResult Get()
    { // getAllUser 
        return GetUsers();
    }

    // GET api/v1/users/{username}
    [Authorize]
    [HttpGet("{username}")]
    public IActionResult GetByUsername(string username)
    {
        return ByUsername(username);
    }
}
