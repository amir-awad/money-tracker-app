using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTrackerApp.Data;
using MoneyTrackerApp.Models;

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

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> Get(int id)
    {
        var category = await _context.FindAsync<Category>(id);
        if (category == null)
            return NotFound();
        return category;
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Post(Category newcategory)
    {
        var category = new Category(newcategory.Id, newcategory.Type);
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return await Get(category.Id);
    }

}
