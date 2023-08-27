using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTrackerApp.Data;
using MoneyTrackerApp.Models;
using MoneyTracker.Service.Extensions;
using MoneyTracker.Service.Dtos;
using System.Runtime.CompilerServices;
using System.Linq;
using System;

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
        if (UsersController.LoggedInUser == null)
            return Unauthorized("You must be logged in to view categories");

        var allCategories = await _context.Categories.Where(category => category.UserID == UsersController.LoggedInUser.Id).ToListAsync();
        return Ok(allCategories);
    }

    [HttpGet]
    [Route("get-categories-by-type/{type}")]
    public async Task<ActionResult<Category>> Get(string type)
    {
        if (UsersController.LoggedInUser == null)
            return Unauthorized("You must be logged in to view categories");
        var Findcategory = await _context.Categories.Where(category => category.Type == type && category.UserID == UsersController.LoggedInUser.Id).FirstOrDefaultAsync();
        if (Findcategory == null)
            return NotFound("Category not found");
        
        return Ok(Findcategory.AsDto());
    }

    [HttpGet]
    [Route("get-categorie-by-id/{id}")]
    public async Task<ActionResult<Category>> Get(Guid id)
    {
        if (UsersController.LoggedInUser == null)
            return Unauthorized("You must be logged in to view categories");

        var category = await _context.Categories.Where(category => category.Id == id && category.UserID == UsersController.LoggedInUser.Id).FirstOrDefaultAsync();
        if (category == null)
            return NotFound("Category not found");
        
        return Ok(category.AsDto());
    }

    [HttpPost]
    [Route("create-category")]
    public async Task<ActionResult<Category>> Post(CreateCategoryDto newcategory)
    {
        if(UsersController.LoggedInUser == null)
            return Unauthorized("You must be logged in to create a category");

        var searchcategory = await _context.Categories.Where(c => c.Type == newcategory.Type && c.UserID == UsersController.LoggedInUser.Id).FirstOrDefaultAsync();
        if (searchcategory != null)
            return BadRequest("Category already exists");
        if (string.IsNullOrWhiteSpace(newcategory.Type))
            return BadRequest("Category must have a type");
        var category = new Category() 
        { 
            Id = Guid.NewGuid(),
            Type = newcategory.Type,
            UserID = UsersController.LoggedInUser.Id
        };

        Console.WriteLine("Category: ");
        Console.WriteLine(category.Id);
        Console.WriteLine(category.Type);
        Console.WriteLine(category.UserID);

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return await Get(category.Type);
    }

    [HttpGet]
    [Route("get-expenses-per-category/")]
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

    [HttpPut]
    [Route("update-category/")]
    public async Task<ActionResult<Category>> Put(UpdateCategoryDto updatedCategoryDto)
    {
        if (UsersController.LoggedInUser == null)
            return Unauthorized("You must be logged in to update a category");

        if(string.IsNullOrWhiteSpace(updatedCategoryDto.OldType) || string.IsNullOrWhiteSpace(updatedCategoryDto.NewType))
            return BadRequest("Category must have a type");

        // Check if category with new type already exists
        var searchCategory = await _context.Categories.Where(c => c.Type == updatedCategoryDto.NewType && c.UserID == UsersController.LoggedInUser.Id).FirstOrDefaultAsync();
        if (searchCategory != null)
            return BadRequest("Category already exists");

        // Check if category with old type exists
        var category = await _context.Categories.Where(c => c.Type == updatedCategoryDto.OldType && c.UserID == UsersController.LoggedInUser.Id).FirstOrDefaultAsync();
        if (category == null)
            return NotFound("Category not found");
        category.Type = updatedCategoryDto.NewType;
        await _context.SaveChangesAsync();
        return await Get(category.Type);
    }   

}
