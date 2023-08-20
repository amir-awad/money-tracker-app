using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTrackerApp.Data;
using MoneyTrackerApp.Models;

namespace MoneyTrackerApp.Controllers;

[ApiController]
[Route("[controller]")]
//http://localhost:5200/Expenses
public class ExpensesController : ControllerBase
{
    private readonly ILogger<ExpensesController> _logger;
    private readonly ApiDbContext _context;
    public ExpensesController(ILogger<ExpensesController> logger, ApiDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetAllExpenses")]
    public async Task<IActionResult> Get()
    {
        // var expense = new Expense()
        // {
        //     Id = 1,
        //     Amount = 300.5,
        //     CreationDate = DateTime.UtcNow,
        //     UserID = 1,
        //     CategoryID = 1
        // };

        // _context.Add(expense);
        //await _context.SaveChangesAsync();
        var allDrivers = await _context.Expenses.ToListAsync();
        return Ok(allDrivers);
    }

}
