using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
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
using System.Threading.Tasks;

namespace MoneyTrackerApp.Tests
{
    public class UsersTests
    {
        private UsersController _controller;
        private ApiDbContext context;
        private IConfiguration configuration;

        [SetUp]
        public void Setup()
        {
            var logger = Mock.Of<ILogger<UsersController>>();
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            context = new ApiDbContext(options);
            configuration = Mock.Of<IConfiguration>();

            _controller = new UsersController(logger, context, configuration);
        }

        public void RemoveInMemoryDatabase()
        {
            context.Users.RemoveRange(context.Users);
            context.Expenses.RemoveRange(context.Expenses);
            context.Categories.RemoveRange(context.Categories);
            context.SaveChanges();
        }
        /*
                [Test]
                public async Task GetAllUsers_ReturnsListOfUsers()
                {
                    RemoveInMemoryDatabase();
                    context.Users.AddRange(new List<User>
                    {
                        new User(Guid.NewGuid(), "User1", "user1@example.com", "user1_username", 100.0),
                        new User(Guid.NewGuid(), "User2", "user2@example.com", "user2_username", 200.0),
                    });
                    context.SaveChanges();

                    // Act
                    var result = await _controller.Get();

                    // Assert
                    Assert.IsInstanceOf<ActionResult<User>>(result); // Check if the result is of type ActionResult<User>
                    Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
                    var okResult = result.Result as OkObjectResult;
                    Assert.IsInstanceOf<List<User>>(okResult.Value);
                    var users = okResult.Value as List<User>;
                    Assert.AreEqual(2, users.Count);
                }

                [Test]
                public async Task GetUserById_ReturnsUser()
                {
                    // Arrange
                    RemoveInMemoryDatabase();
                    Guid id = Guid.NewGuid();
                    context.Users.AddRange(new List<User>
                    {
                        new User(id, "User3", "Password3", "user3@example.com", 300.0),
                    });
                    context.SaveChanges();

                    // Act
                    var result = await _controller.Get(id);


                    // Assert
                    Assert.IsInstanceOf<ActionResult<User>>(result); // Check if the result is of type ActionResult<User>
                    Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
                    var okResult = result.Result as OkObjectResult;
                    Assert.IsInstanceOf<User>(okResult.Value);
                    var user = okResult.Value as User;
                    Assert.AreEqual(id, user.Id);
                    Assert.AreEqual("User3", user.Username);
                    Assert.AreEqual("Password3", user.Password);
                    Assert.AreEqual("user3@example.com", user.Email);
                    Assert.AreEqual(300.0, user.Balance);

                }

                [Test]
                public async Task PostNewUser_ReturnsNewUser()
                {
                    // Arrange
                    RemoveInMemoryDatabase();
                    var newUser = new CreateUserDto
                    (
                        "User3",
                        "Password2",
                        "Email2@gmail.com",
                        300.0
                    );

                    // Act
                    var result = await _controller.Post(newUser);

                    // Assert
                    Assert.IsInstanceOf<ActionResult<User>>(result); // Check if the result is of type ActionResult<User>
                    Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
                    var okResult = result.Result as OkObjectResult;
                    Assert.IsInstanceOf<User>(okResult.Value);
                    var user = okResult.Value as User;
                    Assert.AreEqual(newUser.Username, user.Username);
                    Assert.AreEqual(newUser.Email, user.Email);
                    Assert.AreEqual(newUser.Password, user.Password);
                    Assert.AreEqual(newUser.Balance, user.Balance);
                }

                [Test]
                public async Task PutExistingUser_ReturnsUpdatedUser()
                {
                    // Arrange
                    RemoveInMemoryDatabase();
                    Guid id = Guid.NewGuid();
                    var existinguser = new User(id, "User4", "Password4", "user4@example.com", 400.0);
                    context.Users.AddRange(new List<User>
                    {
                        existinguser,
                    });
                    context.SaveChanges();

                    var existinguserDto = new UpdateUserDto
                    (
                        "User4Update",
                        "Password4",
                        "user4@gmail.com",
                        400.0
                    );

                    // Act
                    var result = await _controller.Put(id, existinguserDto);

                    // Assert
                    Assert.IsInstanceOf<ActionResult<User>>(result); // Check if the result is of type ActionResult<User>
                    Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
                    var okResult = result.Result as OkObjectResult;
                    Assert.IsInstanceOf<User>(okResult.Value);
                    var user = okResult.Value as User;
                    Assert.AreEqual(existinguser.Username, user.Username);
                    Assert.AreEqual(existinguser.Email, user.Email);
                    Assert.AreEqual(existinguser.Password, user.Password);
                    Assert.AreEqual(existinguser.Balance, user.Balance);
                }


                [Test]
                public async Task GetUserExpenses_ReturnExpenses()
                {
                    RemoveInMemoryDatabase();
                    var user = new User(Guid.NewGuid(), "User4", "Password4", "user4@example.com", 400.0);
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
                        Amount = 300,
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
                    var result = await _controller.GetAllExpensesOfUser(user.Id);

                    // Assert
                    Assert.IsInstanceOf<ActionResult<Expense>>(result); // Check if the result is of type ActionResult<Expense>
                    Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
                    var okResult = result.Result as OkObjectResult;
                    Assert.IsInstanceOf<List<Expense>>(okResult.Value);
                    var expenses = okResult.Value as List<Expense>;
                    Assert.AreEqual(1, expenses.Count);
                    Assert.AreEqual(expense.Id, expenses[0].Id);
                    Assert.AreEqual(expense.Amount, expenses[0].Amount);
                    Assert.AreEqual(expense.UserID, expenses[0].UserID);
                    Assert.AreEqual(expense.CategoryID, expenses[0].CategoryID);
                    Assert.AreEqual(expense.CreationDate, expenses[0].CreationDate);
                }
        */

        [Test]
        public async Task UserRegister_RetrunsTheRegisteredUser()
        {
            RemoveInMemoryDatabase();
            var user = new CreateUserDto("Mero", "mero123", "meroooo@gmail.com", 5000);

            var result = await _controller.Register(user);

            Assert.IsInstanceOf<ActionResult<User>>(result); // Check if the result is of type ActionResult<User>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<User>(okResult.Value);
            var registeredUser = okResult.Value as User;
            Assert.AreEqual(user.Username, registeredUser.Username);
            Assert.AreEqual(user.Email, registeredUser.Email);
            Assert.AreEqual(user.Balance, registeredUser.Balance);
        }

        [Test]
        public async Task UserLogin_ReturnsTheLoggedInUser()
        {
            RemoveInMemoryDatabase();
            _controller.Logout();
            var user = new CreateUserDto("Amir", "mero123", "mero@gmail.com", 5000);
            var result = await _controller.Register(user);

            var okResult = result.Result as OkObjectResult;
            //Assert.IsInstanceOf<User>(okResult.Value);
            var registeredUser = okResult.Value as User;

            UsersController.LoggedInUser=registeredUser;

            Assert.AreEqual(registeredUser.Username, UsersController.LoggedInUser.Username);

            Assert.IsInstanceOf<ActionResult<User>>(result); // Check if the result is of type ActionResult<User>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult2 = result.Result as OkObjectResult;
            Assert.IsInstanceOf<User>(okResult2.Value);
            var loggedInUser = okResult2.Value as User;
            Assert.AreEqual(registeredUser.Username, loggedInUser.Username);
            Assert.AreEqual(registeredUser.Email, loggedInUser.Email);
            Assert.AreEqual(registeredUser.Balance, loggedInUser.Balance);

        }

        [Test]
        public async Task UserLogout_ReturnsTheLoggedOutUser()
        {
            RemoveInMemoryDatabase();

            Assert.NotNull(UsersController.LoggedInUser);

            await _controller.Logout();

            Assert.IsNull(UsersController.LoggedInUser);

        }


        [Test]
        public async Task Get_ReturnAllUser()
        {
            RemoveInMemoryDatabase();
            _controller.Logout();

            var user1 = new CreateUserDto("user1", "user1123", "user1@gmail.com", 1000);
            var user2 = new CreateUserDto("user2", "user2123", "user2@gmail.com", 2000);
            var user3 = new CreateUserDto("user3", "user3123", "user3@gmail.com", 3000);

            await _controller.Register(user1);
            await _controller.Register(user2);
            await _controller.Register(user3);
            var result = await _controller.Get();
            var okResult = result.Result as OkObjectResult;
            var users = okResult.Value as List<User>;
            Assert.IsNotNull(users);
            Assert.AreEqual(3, users.Count);
        }

        [Test]
        public async Task GetUsingRegisteredUserID_ReturnsTheRegisteredUser()
        {
            RemoveInMemoryDatabase();
            _controller.Logout();
            var user = new CreateUserDto("user", "user1123", "user@gmail.com", 1000);
            var result = await _controller.Register(user);
            var okResult = result.Result as OkObjectResult;
            var registeredUser = okResult.Value as User;
            var result2 = await _controller.Get(registeredUser.Id);
            var okResult2 = result2.Result as OkObjectResult;
            var user2 = okResult2.Value as User;
            Assert.AreEqual(registeredUser.Username, user2.Username);
            Assert.AreEqual(registeredUser.Email, user2.Email);
            Assert.AreEqual(registeredUser.Balance, user2.Balance);
        }

        [Test]
        public async Task UpdateTheLoggedInUserInfo_ReturnsTheUserWithTheUpdatedInfo()
        {
            RemoveInMemoryDatabase();
            _controller.Logout();
            var user = new CreateUserDto("user", "user1123", "user@gmail.com", 1000);
            var result = await _controller.Register(user);
            Console.WriteLine("result");
            var okResult = result.Result as OkObjectResult;
            var registeredUser = okResult.Value as User;
            UsersController.LoggedInUser=registeredUser;
            var updatedUser = new UpdateUserDto("updatedUser", "updatedUser11234", "updatedUser@gmail.com", 1550);
            var result2 = await _controller.Update(updatedUser);
            var okResult2 = result2.Result as OkObjectResult;
            var user2 = okResult2.Value as UpdateUserDto;
            Assert.AreEqual(updatedUser.Username, user2.Username);
            Assert.AreEqual(updatedUser.Email, user2.Email);
            Assert.AreEqual(updatedUser.Balance, user2.Balance);
        }

    }
}
