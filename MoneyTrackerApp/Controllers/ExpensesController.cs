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

    [HttpGet]
    public async Task<ActionResult<Expense>> Get()
    {
        var allexpenses = await _context.Expenses.ToListAsync();
        return Ok(allexpenses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Expense>> Get(int id)
    {
        var expense = await _context.FindAsync<Expense>(id);
        if (expense == null)
            return NotFound();
        return expense;
    }

    [HttpGet("{userid}")]
    public async Task<ActionResult<Expense>> GetAllExpensesOfUser(int userid)
    {
        var expenses = await _context.Expenses.Where(x => x.UserID == userid).ToListAsync();
        if (expenses == null)
            return NotFound();
        return Ok(expenses);
    }

    // [HttpPost]
    // public async Task<ActionResult<Expense>> Post(Expense newExpense)
    // {
    //     var expense = new Expense(newExpense.Id, newExpense.Amount)
    //     {
    //         UserID = newExpense.UserID,
    //         CategoryID = newExpense.CategoryID,
    //         User=newExpense.User,
    //         Category=newExpense.Category,
    //         CreationDate = DateTime.UtcNow
    //     };
    //     await _context.AddAsync(expense);
    //     await _context.SaveChangesAsync();
    //     return await Get(expense.Id);
    // }

}
