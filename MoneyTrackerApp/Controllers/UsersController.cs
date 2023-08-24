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
    [Route("get-user/{id}")]
    public async Task<ActionResult<User>> Get(Guid id)
    {
        var user = await _context.FindAsync<User>(id);
        if (user == null)
            return NotFound("User not found!");
        return Ok(user);
    }

    [HttpGet]
    [Route("get-expenses/{userid}")]
    public async Task<ActionResult<Expense>> GetAllExpensesOfUser(Guid userid)
    {
        var user = await _context.Users.FindAsync(userid);
        if (user == null)
            return NotFound("User not found!");

        var expenses = await _context.Expenses.Where(expense => expense.UserID == user.Id).ToListAsync();
        return Ok(expenses);
    }

    [HttpPost(Name = "PostNewUser")]
    public async Task<ActionResult<User>> Post(CreateUserDto newuser)
    {
        if (newuser.Balance < 0)
            return BadRequest("Balance cannot be negative!");

        var user = new User(Guid.NewGuid(), newuser.Username, newuser.Password, newuser.Email, newuser.Balance);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        Console.Write(user.ToString());
        return await Get(user.Id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> Put(Guid id, UpdateUserDto updatedUserDto)
    {
        if (updatedUserDto.Balance < 0)
            return BadRequest("Balance cannot be negative!");

        var user =await _context.FindAsync<User>(id);
        user.Username = updatedUserDto.Username;
        user.Password = updatedUserDto.Password;
        user.Email = updatedUserDto.Email;
        user.Balance = updatedUserDto.Balance;

        _context.Users.Update(user);
        _context.SaveChanges();
        return await Get(id);
    }

}
