using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTracker.Service.Dtos;
using MoneyTrackerApp.Data;
using MoneyTrackerApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MoneyTrackerApp.Controllers;

[ApiController]
[Route("[controller]")]
//http://localhost:5200/Users
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly ApiDbContext _context;
    private readonly IConfiguration _configuration;

    public static User LoggedInUser;

    public UsersController(ILogger<UsersController> logger, ApiDbContext context, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(CreateUserDto newUser)
    {

        if(LoggedInUser != null)
            return BadRequest("You are already logged in!");

        if (string.IsNullOrWhiteSpace(newUser.Username))
            return BadRequest("User must have a valid username");
        if (string.IsNullOrWhiteSpace(newUser.Password))
            return BadRequest("User must have a valid password");
        if (string.IsNullOrWhiteSpace(newUser.Email))
            return BadRequest("User must have a valid email");
        // validate the user entered a non-negative balance
        if (newUser.Balance < 0)
            return BadRequest("Balance cannot be negative!");
        // validate the user entered valid email address
        var emailValidator = new EmailAddressAttribute();
        if (!emailValidator.IsValid(newUser.Email))
            return BadRequest("Invalid email address!");
        var searchEmail = await _context.Users.Where(user => user.Email == newUser.Email).FirstOrDefaultAsync();
        if (searchEmail != null)
            return BadRequest("Email already used");

        CreatePasswordHash(newUser.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User(Guid.NewGuid(), newUser.Username, passwordHash, passwordSalt, newUser.Email, newUser.Balance);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        Console.Write(user.ToString());
        return await Get(user.Id);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginUserDto user)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            return BadRequest("User must have a valid email");
        if (string.IsNullOrWhiteSpace(user.Password))
            return BadRequest("User must have a valid password");

        var users = await _context.Users.Where(u => u.Email.Equals(user.Email)).ToListAsync();
        if (users.Count == 0)
            return NotFound("User not found!");

        foreach (var u in users)
        {
            Console.WriteLine(u.ToString());
            if (VerifyPasswordHash(user.Password, u.PasswordHash, u.PasswordSalt))
            {
                string token = CreateToken(u);
                LoggedInUser = u;
                return Ok(token);
            }
        }

        return BadRequest("Invalid password!");
    }

    [HttpPost("logout")]
    public async Task<ActionResult<string>> Logout()
    {
        if(LoggedInUser == null)
            return BadRequest("No user logged in!");

        LoggedInUser = null;
        return Ok("Logged out!");
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        }
    }


    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private string CreateToken(User user)
    {

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: cred
         );


        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
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

    [HttpPut]
    [Route("update-my-info/")]
    public async Task<ActionResult<UpdateUserDto>> Update(UpdateUserDto updatedUserDto)
    {
        if(LoggedInUser == null)
            return BadRequest("You must be logged in to update your info!");

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

        var user = await _context.FindAsync<User>(LoggedInUser.Id);

        CreatePasswordHash(updatedUserDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        user.Username = updatedUserDto.Username;
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.Email = updatedUserDto.Email;
        user.Balance = updatedUserDto.Balance;

        _context.Users.Update(user);
        _context.SaveChanges();
        return Ok(updatedUserDto);
    }

}
