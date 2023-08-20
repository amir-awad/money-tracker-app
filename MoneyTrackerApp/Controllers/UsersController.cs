using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTrackerApp.Data;
using MoneyTrackerApp.Models;

namespace MoneyTrackerApp.Controllers;

[ApiController]
[Route("[controller]")]
//http://localhost:5200/Users
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly ApiDbContext _context;
    public UsersController(ILogger<UsersController> logger, ApiDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var allUsers = await _context.Users.ToListAsync();
        return Ok(allUsers);
    }

    [HttpGet("{id}")]
    public async Task<User> Get(int id)
    {
        var user = await _context.FindAsync<User>(id);
        Console.Write("here");
        return user;
    }

    [HttpPost(Name = "PostNewUser")]
    public async Task<User> Post(User newuser)
    {
        var user=new User(newuser.Id,newuser.Username,newuser.Password,newuser.Email,newuser.Balance );
        await _context.Users.AddAsync(user);
        return await Get(user.Id);
    }

}
