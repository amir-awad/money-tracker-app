using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTracker.Service.Dtos;
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
    public async Task<ActionResult<User>> Get()
    {
        var allUsers = await _context.Users.ToListAsync();
        return Ok(allUsers);
    }

    [HttpGet]
    [Route("get1/{id}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        var user = await _context.FindAsync<User>(id);
        if (user == null)
            return NotFound();
        return user;
    }

    [HttpGet]
    [Route("get2/{userid}")]
    public async Task<ActionResult<Expense>> GetAllExpensesOfUser(int userid)
    {
        var expenses = await _context.Expenses.Where(x => x.UserID == userid).ToListAsync();
        if (expenses == null)
            return NotFound();
        return Ok(expenses);
    }

    [HttpPost(Name = "PostNewUser")]
    public async Task<ActionResult<User>> Post(User newuser)
    {
        var user = new User(newuser.Id, newuser.Username, newuser.Password, newuser.Email, newuser.Balance);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return await Get(user.Id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> Put(int id, UpdateUserDto updatedUserDto)
    {
        var user = Get(id).Result.Value as User;
        user.Username=updatedUserDto.Username;
        user.Password=updatedUserDto.Password;
        user.Email=updatedUserDto.Email;
        user.Balance=updatedUserDto.Balance;

        _context.Users.Update(user);
        _context.SaveChanges();
        return await Get(id);
    }

}
