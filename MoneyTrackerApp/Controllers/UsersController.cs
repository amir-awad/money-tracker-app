using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTracker.Service.Dtos;
using MoneyTrackerApp.Data;
using MoneyTrackerApp.Models;
using System.ComponentModel.DataAnnotations;

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
        if (expenses == null)
            return NotFound("User has no expenses!");
        return Ok(expenses);
    }

    [HttpPost(Name = "PostNewUser")]
    public async Task<ActionResult<User>> Post(CreateUserDto newuser)
    {
        if (string.IsNullOrWhiteSpace(newuser.Username))
            return BadRequest("User must have a valid username");
        if (string.IsNullOrWhiteSpace(newuser.Password))
            return BadRequest("User must have a valid password");
        if (string.IsNullOrWhiteSpace(newuser.Email))
            return BadRequest("User must have a valid email");
        // validate the user entered a non-negative balance
        if (newuser.Balance < 0)
            return BadRequest("Balance cannot be negative!");
        // validate the user entered valid email address
        var emailValidator = new EmailAddressAttribute();
        if (!emailValidator.IsValid(newuser.Email))
            return BadRequest("Invalid email address!");
        var searchEmail = await _context.Users.Where(user => user.Email == newuser.Email).FirstOrDefaultAsync();
        if (searchEmail != null)
            return BadRequest("Email already used");


        var user = new User(Guid.NewGuid(), newuser.Username, newuser.Password, newuser.Email, newuser.Balance);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        Console.Write(user.ToString());
        return await Get(user.Id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> Put(Guid id, UpdateUserDto updatedUserDto)
    {
        if (string.IsNullOrWhiteSpace(updatedUserDto.Username))
            return BadRequest("User must have a valid username");
        if (string.IsNullOrWhiteSpace(updatedUserDto.Password))
            return BadRequest("User must have a valid password");
        if (string.IsNullOrWhiteSpace(updatedUserDto.Email))
            return BadRequest("User must have a valid email");
        // validate the user entered a non-negative balance
        if (updatedUserDto.Balance < 0)
            return BadRequest("Balance cannot be negative!");

        // validate the user entered valid email address
        var emailValidator = new EmailAddressAttribute();
        if (!emailValidator.IsValid(updatedUserDto.Email))
            return BadRequest("Invalid email address!");
        var searchEmail = await _context.Users.Where(user => user.Email == updatedUserDto.Email).FirstOrDefaultAsync();
        if (searchEmail != null)
            return BadRequest("Email already used");

        var user = await _context.FindAsync<User>(id);
        if (user == null)
            return NotFound();
        user.Username = updatedUserDto.Username;
        user.Password = updatedUserDto.Password;
        user.Email = updatedUserDto.Email;
        user.Balance = updatedUserDto.Balance;

        _context.Users.Update(user);
        _context.SaveChanges();
        return await Get(id);
    }

}
