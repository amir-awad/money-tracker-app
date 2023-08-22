using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTrackerApp.Data;
using MoneyTrackerApp.Models;
using MoneyTracker.Service.Extensions;
using MoneyTracker.Service.Dtos;

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
        return category;
    }

    [HttpGet]
    [Route("/{id}/Expenses")]
    public async Task<ActionResult<GetExpenseDto>> GetExpenses(Guid id)
    {
        var expenses = await _context.Expenses.Where(expense => expense.CategoryID == id).Select(expense => expense.AsDto()).ToListAsync();
        return Ok(expenses);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Post(Category newcategory)
    {
        var category = new Category(Guid.NewGuid(), newcategory.Type);
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return await Get(category.Id);
    }

}
