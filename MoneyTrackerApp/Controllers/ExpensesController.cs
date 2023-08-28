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
    public async Task<ActionResult<GetExpenseDto>> Get()
    {
        if (UsersController.LoggedInUser == null)
            return NotFound("User is not logged in");

        Console.WriteLine("User is logged in");
        Console.WriteLine(UsersController.LoggedInUser);
        Guid userId = UsersController.LoggedInUser.Id;
        Console.Write("User ID: ");
        Console.Write(userId);
        var allexpenses = await _context.Expenses.Where(expense => expense.UserID == userId).Select(expense => expense.AsDto()).ToListAsync();
        if (allexpenses.Count == 0)
            return NotFound("No expenses so far for this user");
        return Ok(allexpenses);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<GetExpenseDto>> GetExpense(Guid id)
    {
        var expense = await _context.FindAsync<Expense>(id);
        if (expense == null)
            return NotFound();
        return Ok(expense.AsDto());
    }

    [HttpGet]
    [Route("{date?}")]
    public async Task<ActionResult<List<GetExpenseDto>>> GetExpenses(string date)
    {
        string format = "yyyy-MM-dd";
        if (DateTimeOffset.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result))
        {
            var allexpenses = await _context.Expenses
                .Where(expense => expense.CreationDate.Date == result.Date)
                .Select(expense => expense.AsDto())
                .ToListAsync();
            if (allexpenses.Count == 0)
                return NotFound("No Expenses this day");
            return Ok(allexpenses);
        }
        return NotFound("Date conversion failed or invalid format.");
    }

    [HttpGet]
    [Route("{categoryType?}")]
    public async Task<ActionResult<GetExpenseDto>> GetCategoryExpensesOfUser(string categoryType)
    {
        if (UsersController.LoggedInUser == null)
            return Unauthorized("You must be logged in to view expenses");

        var category = await _context.Categories.Where(c => c.Type == categoryType && c.UserID == UsersController.LoggedInUser.Id).FirstOrDefaultAsync();
        if (category == null)
            return NotFound();
        var expenses = await _context.Expenses.Where(expense => expense.CategoryID == category.Id && expense.UserID == UsersController.LoggedInUser.Id).Select(expense => expense.AsDto()).ToListAsync();
        if (expenses.Count == 0)
            return NotFound("No expenses so far");
        return Ok(expenses);
    }

    [HttpPost]
    public async Task<ActionResult<GetExpenseDto>> Post(CreateExpenseDto newExpense)
    {
        var expenseUser = UsersController.LoggedInUser;
        if (expenseUser == null)
            return NotFound("User is not logged in");

        if (string.IsNullOrWhiteSpace(newExpense.Amount.ToString()))
            return BadRequest("Expense must have a valid amount");
        if (newExpense.Amount <= 0)
            return BadRequest("Expense must have a valid amount");
        if (string.IsNullOrWhiteSpace(newExpense.CategoryType.ToString()))
            return BadRequest("User must have a valid CategoryID");

        var expenseCategory = await _context.Categories.Where(category => category.Type == newExpense.CategoryType).FirstOrDefaultAsync();
        if (expenseCategory == null)
            return NotFound("Category is not found");

        double newBalance = expenseUser.Balance - newExpense.Amount;
        if (newBalance < 0)
            return BadRequest("User doesn't have enough balance for this expense");
        expenseUser.Balance = newBalance;
        _context.Users.Update(expenseUser);

        expenseCategory.TotalAmount += newExpense.Amount;
        _context.Categories.Update(expenseCategory);

        _context.SaveChanges();

        var expense = new Expense()
        {
            Id = Guid.NewGuid(),
            Amount = newExpense.Amount,
            CreationDate = DateTime.UtcNow,
            UserID = expenseUser.Id,
            CategoryID = expenseCategory.Id,
            ExpenseUser = expenseUser,
            ExpenseCategory = expenseCategory
        };
        await _context.AddAsync(expense);
        await _context.SaveChangesAsync();
        return await GetExpense(expense.Id);
    }

    [HttpPut]
    public async Task<ActionResult<GetExpenseDto>> Put(Guid id, UpdateExpenseDto updatedexpense)
    {
        if (UsersController.LoggedInUser == null)
            return NotFound("User is not logged in");

        var expense = await _context.FindAsync<Expense>(id);
        if (expense == null)
            return NotFound("Expnese not found");

        if (string.IsNullOrWhiteSpace(updatedexpense.Amount.ToString()))
            return BadRequest("Expense must have a valid amount");
        if (updatedexpense.Amount < 0)
            return BadRequest("Expense must have a valid amount");
        if (string.IsNullOrWhiteSpace(updatedexpense.CategoryType.ToString()))
            return BadRequest("Enter a valid category type");

        var expenseCategory = await _context.Categories.Where(category => category.Type == updatedexpense.CategoryType && category.UserID == UsersController.LoggedInUser.Id).FirstOrDefaultAsync();
        if (expenseCategory == null)
            return NotFound("Category is not found");

        double newBalance = UsersController.LoggedInUser.Balance + expense.Amount - updatedexpense.Amount;
        if (newBalance < 0)
            return BadRequest("User doesn't have enough balance for this expense");

        if(expense.Amount != updatedexpense.Amount) {
            expenseCategory.TotalAmount = expenseCategory.TotalAmount - expense.Amount + updatedexpense.Amount;
            expense.Amount = updatedexpense.Amount;
        }
        expense.CategoryID = expenseCategory.Id;

        UsersController.LoggedInUser.Balance = newBalance;
        _context.Users.Update(UsersController.LoggedInUser);
        _context.Expenses.Update(expense);
        _context.SaveChanges();
        return await GetExpense(expense.Id);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<GetExpenseDto>> Delete(Guid id)
    { 
        var expense = await _context.FindAsync<Expense>(id);
        var expenseCategory = await _context.Categories.Where(category => category.Id == expense.CategoryID && category.UserID == UsersController.LoggedInUser.Id).FirstOrDefaultAsync();
        
        if (expense == null || expenseCategory == null)
            return NotFound();

        expenseCategory.TotalAmount -= expense.Amount;
        _context.Categories.Update(expenseCategory);
        _context.Remove(expense);
        _context.SaveChanges();
        return Ok(expense.AsDto());
    }
}
