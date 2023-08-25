using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTrackerApp.Data;
using MoneyTrackerApp.Models;
using MoneyTracker.Service.Extensions;
using MoneyTracker.Service.Dtos;
using System.Runtime.CompilerServices;

namespace MoneyTrackerApp.Controllers;

[ApiController]
[Route("[controller]")]
//http://localhost:5200/Categories
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly ApiDbContext _context;
    public CategoriesController(ILogger<CategoriesController> logger, ApiDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<Category>> Get()
    {
        var allCategories = await _context.Categories.ToListAsync();
        return Ok(allCategories);
    }

    [HttpGet]
    [Route("get-categories/{id}")]
    public async Task<ActionResult<Category>> Get(Guid id)
    {
        var category = await _context.FindAsync<Category>(id);
        if (category == null)
            return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Post(CreateCategoryDto newcategory)
    {
        var searchcategory = await _context.Categories.Where(category => category.Type == newcategory.Type).FirstOrDefaultAsync();
        if (searchcategory != null)
            return BadRequest("Category already exists");
        if (string.IsNullOrWhiteSpace(newcategory.Type))
            return BadRequest("Category must have a name");
        var category = new Category(Guid.NewGuid(), newcategory.Type);
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return await Get(category.Id);
    }

    [HttpGet]
    [Route("get-expenses-of-user-by-category/{UserID}")]
    public async Task<ActionResult<GetExpenseDto>> GetCategoryExpensesOfUser(Guid UserId, string categoryType)
    {
        var category = await _context.Categories.Where(category => category.Type == categoryType).FirstOrDefaultAsync();
        if (category == null)
            return NotFound();
        var expenses = await _context.Expenses.Where(expense => expense.CategoryID == category.Id && expense.UserID == UserId).Select(expense => expense.AsDto()).ToListAsync();
        if (expenses.Count == 0)
            return NotFound();
        return Ok(expenses);
    }

}
