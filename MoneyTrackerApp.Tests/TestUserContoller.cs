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

        [Test]
        public async Task UserRegister_RetrunsTheRegisteredUser()
        {
            RemoveInMemoryDatabase();
            var user = new CreateUserDto("Mero", "mero123", "meroooo@gmail.com", 5000);

            var result = await _controller.Register(user);

            Assert.IsInstanceOf<ActionResult<GetUserDto>>(result); // Check if the result is of type ActionResult<User>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<GetUserDto>(okResult.Value);
            var registeredUser = okResult.Value as GetUserDto;
            Assert.AreEqual(user.Username, registeredUser.Username);
            Assert.AreEqual(user.Email, registeredUser.Email);
            Assert.AreEqual(user.Balance, registeredUser.Balance);
        }

        [Test]
        public async Task UserLogin_ReturnsTheLoggedInUser()
        {
            RemoveInMemoryDatabase();
            _controller.Logout();
            var userDto = new CreateUserDto("Amir", "mero123", "mero@gmail.com", 5000);
            Guid id = Guid.NewGuid();
            var user = new User(id, "Amir", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "mero@gmail.com",5000);
            context.Users.AddRange(new List<User>
            {
                user,
            });
            var result = await _controller.Register(userDto);

            var okResult = result.Result as OkObjectResult;
            //Assert.IsInstanceOf<User>(okResult.Value);
            var registeredUser = okResult.Value as GetUserDto;

            UsersController.LoggedInUser=user;

            Assert.AreEqual(registeredUser.Username, UsersController.LoggedInUser.Username);

            Assert.IsInstanceOf<ActionResult<GetUserDto>>(result); // Check if the result is of type ActionResult<User>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult2 = result.Result as OkObjectResult;
            Assert.IsInstanceOf<GetUserDto>(okResult2.Value);
            var loggedInUser = okResult2.Value as GetUserDto;
            Assert.AreEqual(registeredUser.Username, loggedInUser.Username);
            Assert.AreEqual(registeredUser.Email, loggedInUser.Email);
            Assert.AreEqual(registeredUser.Balance, loggedInUser.Balance);

        }

        [Test]
        public async Task UserLogout_ReturnsTheLoggedOutUser()
        {
            

            Assert.NotNull(UsersController.LoggedInUser);

            _controller.Logout();

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
            var users = okResult.Value as List<GetUserDto>;
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
            var registeredUser = okResult.Value as GetUserDto;
            var result2 = await _controller.Get(registeredUser.Id);
            var okResult2 = result2.Result as OkObjectResult;
            var user2 = okResult2.Value as GetUserDto;
            Assert.AreEqual(registeredUser.Username, user2.Username);
            Assert.AreEqual(registeredUser.Email, user2.Email);
            Assert.AreEqual(registeredUser.Balance, user2.Balance);
        }

        [Test]
        public async Task UpdateTheLoggedInUserInfo_ReturnsTheUserWithTheUpdatedInfo()
        {
            RemoveInMemoryDatabase();
            _controller.Logout();
            var userDto = new CreateUserDto("user", "user1123", "user@gmail.com", 1000);
            Guid id = Guid.NewGuid();
            var user = new User(id, "user", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "user@gmail.com",1000);
            context.Users.AddRange(new List<User>
            {
                user,
            });
            var result = await _controller.Register(userDto);
            Console.WriteLine("result");
            var okResult = result.Result as OkObjectResult;
            var registeredUserDto = okResult.Value as GetUserDto;
            UsersController.LoggedInUser=user;
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
