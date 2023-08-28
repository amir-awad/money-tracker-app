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
            // Arrange
            RemoveInMemoryDatabase();

            Guid id = Guid.NewGuid();
            var user = new User(id, "User1", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "user1@example.com", 400.0);
            context.Users.AddRange(new List<User>
            {
                user,
            });
            UsersController.LoggedInUser =user;
            var category = new Category()
            {
                Id = Guid.NewGuid(),
                Type = "Category1",
                UserID = user.Id
            };
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
            var result = await _controller.Get();

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
            var user = new User(id, "User1", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "user1@example.com", 400.0);
            context.Users.AddRange(new List<User>
            {
                user,
            });
            UsersController.LoggedInUser =user;
            var category = new Category()
            {
                Id = Guid.NewGuid(),
                Type = "Category1",
                UserID = user.Id
            };
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
        public async Task GetExpensesPerCategory_ReturnsTheExpenses()
        {
            RemoveInMemoryDatabase();
            var user1 = new User(Guid.NewGuid(), "user1", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "user1@gmail.com", 5000);
            context.Users.AddRange(new List<User>
            {
                user1
            });

            UsersController.LoggedInUser = user1;

            Category category1 = new Category()
            {
                Id = Guid.NewGuid(),
                Type = "Category1",
                UserID = user1.Id
            };

            Expense expense1 = new Expense()
            {
                Id = Guid.NewGuid(),
                Amount = 100,
                CategoryID = category1.Id,
                CreationDate = DateTime.Now,
                UserID = user1.Id,
                ExpenseUser = user1,
                ExpenseCategory = category1
            };

            Expense expense2 = new Expense()
            {
                Id = Guid.NewGuid(),
                Amount = 200,
                CategoryID = category1.Id,
                CreationDate = DateTime.Now,
                UserID = user1.Id,
                ExpenseUser = user1,
                ExpenseCategory = category1
            };

            context.Categories.AddRange(new List<Category>
            {
                category1
            });

            context.Expenses.AddRange(new List<Expense>
            {
                expense1,
                expense2
            });

            context.SaveChanges();

            var result = await _controller.GetCategoryExpensesOfUser(category1.Type);
            Assert.NotNull(result);
            Assert.IsInstanceOf<ActionResult<GetExpenseDto>>(result); // Check if the result is of type ActionResult<List<GetExpenseDto>>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<List<GetExpenseDto>>(okResult.Value);
            var expenses = okResult.Value as List<GetExpenseDto>;
            Assert.AreEqual(2, expenses.Count);
        }


        [Test]
        public async Task PostNewExpense_ReturnsNewExpense()
        {
            // Arrange
            RemoveInMemoryDatabase();

            Guid id = Guid.NewGuid();
            var user = new User(id, "User1", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "user1@example.com", 400.0);
            context.Users.AddRange(new List<User>
            {
                user,
            });
            UsersController.LoggedInUser =user;
            var category = new Category()
            {
                Id = Guid.NewGuid(),
                Type = "Category1",
                UserID = user.Id
            };
            context.Categories.AddRange(new List<Category>
            {
                category,
            });

            var newExpense = new CreateExpenseDto(300.0,category.Type);

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
            //Assert.AreEqual(newExpense.UserId, expense.UserId);
            // Assert.AreEqual(newExpense.CategoryType, expense.Type);
        }

        [Test]
        public async Task PutExistingExpense_ReturnsUpdatedExpense()
        {
            // Arrange
            RemoveInMemoryDatabase();

            Guid id = Guid.NewGuid();
            var user = new User(id, "User1", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "user1@example.com", 400.0);
            context.Users.AddRange(new List<User>
            {
                user,
            });
            UsersController.LoggedInUser =user;
            var category1 = new Category()
            {
                Id = Guid.NewGuid(),
                Type = "Category1",
                UserID = user.Id
            };
            var category2 = new Category()
            {
                Id = Guid.NewGuid(),
                Type = "Category2",
                UserID = user.Id
            };
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
                category2.Type
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
            // Assert.AreEqual(updatedExpense.CategoryId, expenseResult.C);
        }


        [Test]
        public async Task DeleteExistingExpense_ReturnsDeletedExpense()
        {
            // Arrange
            RemoveInMemoryDatabase();

            Guid id = Guid.NewGuid();
            var user = new User(id, "User1", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "user1@example.com", 400.0);
            context.Users.AddRange(new List<User>
            {
                user,
            });
            UsersController.LoggedInUser =user;
            var category = new Category()
            {
                Id = Guid.NewGuid(),
                Type = "Category1",
                UserID = user.Id
            };
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
