using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTrackerApp.Data;
using MoneyTrackerApp.Models;
using MoneyTracker.Service.Dtos;
using MoneyTracker.Service.Extensions;
using System.Globalization;

namespace MoneyTrackerApp.Controllers;

[ApiController]
[Route("[controller]")]
//http://localhost:5200/Expenses
public class ExpensesController : ControllerBase
{
    private readonly ILogger<ExpensesController> _logger;
    private readonly ApiDbContext _context;
    // private readonly UsersController usercontroller;
    public ExpensesController(ILogger<ExpensesController> logger, ApiDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    [Route("get-expenses-of-user/{UserID}")]
    public async Task<ActionResult<GetExpenseDto>> Get(Guid userId)
    {
        var allexpenses = await _context.Expenses.Where(expense => expense.UserID == userId).Select(expense => expense.AsDto()).ToListAsync();
        if (allexpenses.Count == 0)
            return NotFound();
        return Ok(allexpenses);
    }

    [HttpGet]
    [Route("get-expense/{id}")]
    public async Task<ActionResult<GetExpenseDto>> GetExpense(Guid id)
    {
        var expense = await _context.FindAsync<Expense>(id);
        if (expense == null)
            return NotFound();
        return Ok(expense.AsDto());
    }

    // to-do
    // [HttpGet]
    // [Route("get-expense-by-date/{Date}")]
    // public async Task<ActionResult<List<GetExpenseDto>>> GetExpenses(string date)
    // {
    // string format = "yyyy-MM-dd";
    // if (DateTimeOffset.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result))
    // {
    //     var allexpenses = await _context.Expenses
    //         .Where(expense => expense.CreationDate.Date == result.Date)
    //         .Select(expense => expense.AsDto())
    //         .ToListAsync();

    //     return Ok(allexpenses);
    // }

    // // Console.Write((DateTime)date);
    // Console.Write(date);
    // Console.Write(result.Date);
    // return NotFound("Date conversion failed or invalid format.");
    // }


    [HttpPost]
    public async Task<ActionResult<GetExpenseDto>> Post(CreateExpenseDto newExpense)
    {
        if (string.IsNullOrWhiteSpace(newExpense.Amount.ToString()))
            return BadRequest("Expense must have a valid amount");
        if (newExpense.Amount <= 0)
            return BadRequest("Expense must have a valid amount");
        if (string.IsNullOrWhiteSpace(newExpense.UserId.ToString()))
            return BadRequest("User must have a valid UserID");
        if (string.IsNullOrWhiteSpace(newExpense.CategoryType.ToString()))
            return BadRequest("User must have a valid CategoryID");

        var expenseUser = await _context.Users.FindAsync(newExpense.UserId);
        if (expenseUser == null)
            return NotFound("User is not found");
        var expenseCategory = await _context.Categories.Where(category => category.Type == newExpense.CategoryType).FirstOrDefaultAsync();
        if (expenseCategory == null)
            return NotFound("Category is not found");

        double newBalance = expenseUser.Balance - newExpense.Amount;
        if (newBalance < 0)
            return BadRequest("User doesn't have enough balance for this expense");
        expenseUser.Balance=newBalance;
        _context.Users.Update(expenseUser);
        _context.SaveChanges();

        var expense = new Expense()
        {
            Id = Guid.NewGuid(),
            Amount = newExpense.Amount,
            CreationDate = DateTime.UtcNow,
            UserID = newExpense.UserId,
            CategoryID = expenseCategory.Id,
            ExpenseUser = expenseUser,
            ExpenseCategory = expenseCategory
        };
        await _context.AddAsync(expense);
        await _context.SaveChangesAsync();
        return await GetExpense(expense.Id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GetExpenseDto>> Put(Guid id, UpdateExpenseDto updatedexpense)
    {
        var expense = await _context.FindAsync<Expense>(id);
        if (expense == null)
            return NotFound();
        expense.Amount = updatedexpense.Amount;
        expense.CategoryID = updatedexpense.CategoryId;
        _context.Expenses.Update(expense);
        _context.SaveChanges();
        return await GetExpense(id);
    }

    [HttpDelete]
    public async Task<ActionResult<GetExpenseDto>> Delete(Guid id)
    {
        var expense = await _context.FindAsync<Expense>(id);
        if (expense == null)
            return NotFound();
        _context.Remove(expense);
        _context.SaveChanges();
        return Ok(expense.AsDto());
    }
}
