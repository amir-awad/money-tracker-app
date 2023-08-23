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
    public class Tests
    {
        private UsersController _controller;
        private ApiDbContext context;

        [SetUp]
        public void Setup()
        {
            var logger = Mock.Of<ILogger<UsersController>>();
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            context = new ApiDbContext(options);

            // Populate in-memory database with test data
            context.Users.AddRange(new List<User>
            {
                new User(Guid.NewGuid(), "User1", "user1@example.com", "user1_username", 100.0),
                new User(Guid.NewGuid(), "User2", "user2@example.com", "user2_username", 200.0),
    // Add more users as needed
            });

            context.SaveChanges();
            _controller = new UsersController(logger, context);
        }

        [Test]
        public async Task GetAllUsers_ReturnsListOfUsers()
        {
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
            Guid id = Guid.NewGuid();
            var user3 = new User(id, "User3", "Email2", "Password3", 300.0);
            context.Users.Add(user3);
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
            Assert.AreEqual("Email2", user.Email);
            Assert.AreEqual("Password3", user.Password);
            Assert.AreEqual(300.0, user.Balance);

        }

        // [Test]
        // public async Task PostNewUser_ReturnsNewUser()
        // {
        //     // Arrange
        //     var newUser = new CreateUserDto
        //     (
        //         "User3",
        //         "Email2",
        //         "Password2",
        //         300.0
        //     );

        //     // Act
        //     var result = await _controller.Post(newUser);

        //     // Assert
        //     Assert.IsInstanceOf<ActionResult<User>>(result); // Check if the result is of type ActionResult<User>
        //     var createdResult = result.Result as CreatedAtActionResult;
        //     if(createdResult == null)
        //         Assert.Fail("CreatedAtActionResult is null!");
        //     var user = createdResult.Value as User;
        //     Assert.AreEqual(newUser.Username, user.Username);
        //     Assert.AreEqual(newUser.Email, user.Email);
        //     Assert.AreEqual(newUser.Password, user.Password);
        //     Assert.AreEqual(newUser.Balance, user.Balance); 
        // }
    }
}
