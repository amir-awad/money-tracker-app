using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq; // You'll need to add the Moq package for mocking
using NUnit.Framework;
using MoneyTrackerApp.Controllers;
using MoneyTrackerApp.Data;
using MoneyTrackerApp.Models;
using System.Collections.Generic;
using System.Linq;
using MoneyTracker.Service.Dtos;

namespace MoneyTrackerApp.Tests
{
    public class CategoriesTests
    {
        private CategoriesController _controller;
        private ApiDbContext context;

        [SetUp]
        public void Setup()
        {
            var logger = Mock.Of<ILogger<CategoriesController>>();
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            context = new ApiDbContext(options);

            _controller = new CategoriesController(logger, context);
        }

        public void RemoveInMemoryDatabase()
        {
            context.Users.RemoveRange(context.Users); 
            context.Expenses.RemoveRange(context.Expenses);
            context.Categories.RemoveRange(context.Categories);
        }

        [Test]
        public async Task GetAllCategories_ReturnsListOfCategories()
        {
            //Arrange
            RemoveInMemoryDatabase();
            Category category1 = new Category(Guid.NewGuid(), "Category1");
            Category category2 = new Category(Guid.NewGuid(), "Category2");
            context.Categories.AddRange(new List<Category>
            {
                category1,
                category2
            });
            context.SaveChanges();

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOf<ActionResult<Category>>(result); // Check if the result is of type ActionResult<category>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<List<Category>>(okResult.Value);
            var categories = okResult.Value as List<Category>;
            Assert.AreEqual(2, categories.Count);

        }

        [Test]
        public async Task GetCategoryById_ReturnsCategory()
        {
            // Arrange
            RemoveInMemoryDatabase();
            Guid id = Guid.NewGuid();
            context.Categories.AddRange(new List<Category>
            {
                new Category(id, "Category3"),
            });
            context.SaveChanges();

            // Act
            var result = await _controller.Get(id);

            // Assert
            Assert.IsInstanceOf<ActionResult<Category>>(result); // Check if the result is of type ActionResult<category>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<Category>(okResult.Value);
            var category = okResult.Value as Category;
            Assert.AreEqual(id, category.Id);
            Assert.AreEqual("Category3", category.Type);
        }

        [Test]
        public async Task PostNewCategory_ReturnsNewCategory()
        {
            // Arrange
            RemoveInMemoryDatabase();
            var newCategory = new CreateCategoryDto
            (
                "NewCategory3"
            );

            // Act
            var result = await _controller.Post(newCategory);

            // Assert
            Assert.IsInstanceOf<ActionResult<Category>>(result); // Check if the result is of type ActionResult<User>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<Category>(okResult.Value);
            var category = okResult.Value as Category;
            Assert.AreEqual(newCategory.Type, category.Type);
        }

        [Test]
        public async Task GetUserExpenses_ReturnExpenses()
        {
            RemoveInMemoryDatabase();
            var user = new User(Guid.NewGuid(), "User4", "Password4", "user4@gmail.com", 400.0);
            context.Users.AddRange(new List<User>
            {
                user,
            });

            var category = new Category(Guid.NewGuid(), "Category1");
            context.Categories.AddRange(new List<Category>
            {
                category,
            });

            var expense = new Expense()
            {
                Id = Guid.NewGuid(),
                Amount = 500,
                CreationDate = DateTime.UtcNow,
                UserID = user.Id,
                CategoryID = category.Id,
                ExpenseUser = await context.Users.FindAsync(user.Id),
                ExpenseCategory = await context.Categories.FindAsync(category.Id)
            };

            context.Expenses.AddRange(new List<Expense>
            {
                expense,
            });

            context.SaveChanges();


            // Act
            var result = await _controller.GetCategoryExpensesOfUser(user.Id, category.Type);

            // Assert
            Assert.IsInstanceOf<ActionResult<GetExpenseDto>>(result); // Check if the result is of type ActionResult<Expense>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<List<GetExpenseDto>>(okResult.Value);
            var expenses = okResult.Value as List<GetExpenseDto>;
            Assert.AreEqual(1, expenses.Count);
            Assert.AreEqual(expense.Id, expenses[0].Id);
            Assert.AreEqual(expense.Amount, expenses[0].Amount);
            Assert.AreEqual(expense.UserID, expenses[0].UserId);
            Assert.AreEqual(expense.CategoryID, expenses[0].CategoryId);
            Assert.AreEqual(expense.CreationDate, expenses[0].Creationdate);
        }

    }
}
