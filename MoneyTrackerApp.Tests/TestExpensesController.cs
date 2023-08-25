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
    public class ExpensesTests
    {
        private ExpensesController _controller;
        private ApiDbContext context;

        [SetUp]
        public void Setup()
        {
            var logger = Mock.Of<ILogger<ExpensesController>>();
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            context = new ApiDbContext(options);

            _controller = new ExpensesController(logger, context);
        }

        public void RemoveInMemoryDatabase()
        {
            context.Users.RemoveRange(context.Users); 
            context.Expenses.RemoveRange(context.Expenses);
            context.Categories.RemoveRange(context.Categories);
        }

        [Test]
        public async Task GetExpensesOfUser_ReturnsListOfExpenses()
        {
            RemoveInMemoryDatabase();

            Guid id = Guid.NewGuid();
            var user = new User(id, "User1", "Password1", "user1@example.com", 400.0);
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
            var result = await _controller.Get(user.Id);

            // Assert
            Assert.IsInstanceOf<ActionResult<GetExpenseDto>>(result); // Check if the result is of type ActionResult<User>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<List<GetExpenseDto>>(okResult.Value);
            var expenses = okResult.Value as List<GetExpenseDto>;
            Assert.AreEqual(1, expenses.Count);
        }

        [Test]
        public async Task GetExpenseById_ReturnsExpense()
        {
            // Arrange
            RemoveInMemoryDatabase();

            Guid id = Guid.NewGuid();
            var user = new User(id, "User1", "Password1", "user1@example.com", 400.0);
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
            var result = await _controller.GetExpense(expense.Id);

            // Assert
            Assert.IsInstanceOf<ActionResult<GetExpenseDto>>(result); // Check if the result is of type ActionResult<User>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<GetExpenseDto>(okResult.Value);
            var expenses = okResult.Value as GetExpenseDto;

            Assert.AreEqual(expense.Id, expenses.Id);
            Assert.AreEqual(expense.Amount, expenses.Amount);
            Assert.AreEqual(expense.UserID, expenses.UserId);
            Assert.AreEqual(expense.CategoryID, expenses.CategoryId);
            Assert.AreEqual(expense.CreationDate, expenses.Creationdate);

        }

        [Test]
        public async Task PostNewExpense_ReturnsNewExpense()
        {
            // Arrange
            RemoveInMemoryDatabase();

            Guid id = Guid.NewGuid();
            var user = new User(id, "User1", "Password1", "user1@gmail.com", 400.0);
            context.Users.AddRange(new List<User>
            {
                user,
            });

            var category = new Category(Guid.NewGuid(), "Category1");
            context.Categories.AddRange(new List<Category>
            {
                category,
            });

            var newExpense = new CreateExpenseDto
            (
                300.0,
                user.Id,
                category.Type
            );

            context.SaveChanges();

            // Act
            var result = await _controller.Post(newExpense);

            // Assert
            Assert.IsInstanceOf<ActionResult<GetExpenseDto>>(result); // Check if the result is of type ActionResult<User>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<GetExpenseDto>(okResult.Value);
            var expense = okResult.Value as GetExpenseDto;

            Assert.AreEqual(newExpense.Amount, expense.Amount);
            Assert.AreEqual(newExpense.UserId, expense.UserId);
            // Assert.AreEqual(newExpense.CategoryType, expense.Type);
        }

        [Test]
        public async Task PutExistingExpense_ReturnsUpdatedExpense()
        {
            // Arrange
            RemoveInMemoryDatabase();

            Guid id = Guid.NewGuid();
            var user = new User(id, "User1", "Password1", "user1@example.com", 400.0);
            context.Users.AddRange(new List<User>
            {
                user,
            });

            var category1 = new Category(Guid.NewGuid(), "Category1");
            var category2 = new Category(Guid.NewGuid(), "Category2");
            context.Categories.AddRange(new List<Category>
            {
                category1,
                category2
            });

            var expense = new Expense()
            {
                Id = Guid.NewGuid(),
                Amount = 500,
                CreationDate = DateTime.UtcNow,
                UserID = user.Id,
                CategoryID = category1.Id,
                ExpenseUser = await context.Users.FindAsync(user.Id),
                ExpenseCategory = await context.Categories.FindAsync(category1.Id)
            };

            context.Expenses.AddRange(new List<Expense>
            {
                expense,
            });

            var updatedExpense = new UpdateExpenseDto
            (
                300.0,
                category2.Id
            );

            context.SaveChanges();

            // Act
            var result = await _controller.Put(expense.Id, updatedExpense);

            // Assert
            Assert.IsInstanceOf<ActionResult<GetExpenseDto>>(result); // Check if the result is of type ActionResult<User>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<GetExpenseDto>(okResult.Value);
            var expenseResult = okResult.Value as GetExpenseDto;

            Assert.AreEqual(updatedExpense.Amount, expenseResult.Amount);
            Assert.AreEqual(updatedExpense.CategoryId, expenseResult.CategoryId);
        }

        
        [Test]
        public async Task DeleteExistingExpense_ReturnsDeletedExpense()
        {
            // Arrange
            RemoveInMemoryDatabase();

            Guid id = Guid.NewGuid();
            var user = new User(id, "User1", "Password1", "user1@example.com", 400.0);
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
            var result = await _controller.Delete(expense.Id);

            // Assert
            Assert.IsInstanceOf<ActionResult<GetExpenseDto>>(result); // Check if the result is of type ActionResult<User>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<GetExpenseDto>(okResult.Value);
            var expenseResult = okResult.Value as GetExpenseDto;
            Assert.AreEqual(0, context.Expenses.Count());
            Assert.AreEqual(expense.Amount, expenseResult.Amount);
            Assert.AreEqual(expense.CategoryID, expenseResult.CategoryId);
        }
    }
}
