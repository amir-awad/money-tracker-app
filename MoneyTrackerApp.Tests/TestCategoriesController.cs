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

            // register some users
            var user1 = new User(Guid.NewGuid(), "user1", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "user1@gmail.com", 5000);
            var user2 = new User(Guid.NewGuid(), "user2", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "user2@gmail.com", 5000);
            var user3 = new User(Guid.NewGuid(), "user3", new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, "user3@gmail.com", 5000);

             context.Users.AddRange(new List<User>
             {
                user1,
                user2,
                user3
             });

        }

        public void RemoveInMemoryDatabase()
        {
            context.Users.RemoveRange(context.Users); 
            context.Expenses.RemoveRange(context.Expenses);
            context.Categories.RemoveRange(context.Categories);
            context.SaveChanges();
        }

        [Test]
        public async Task GetAllCategories_ReturnsListOfCategories()
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
            Console.WriteLine("User ID: " + user1.Id);
            Category category2 = new Category()
            {
                Id = Guid.NewGuid(),
                Type = "Category2",
                UserID = user1.Id
            };
            context.Categories.AddRange(new List<Category>
            {
                category1,
                category2
            });

            context.SaveChanges();

            var result = await _controller.Get(null);
            
            Assert.IsInstanceOf<ActionResult<Category>>(result); // Check if the result is of type ActionResult<category>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<List<Category>>(okResult.Value);
            var categories = okResult.Value as List<Category>;
            Assert.AreEqual(2, categories.Count);

        }

        [Test]
        public async Task GetCategoryByType_ReturnsTheCategory()
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
            context.Categories.AddRange(new List<Category>
            {
                category1
            });

            context.SaveChanges();

            var result = await _controller.Get(category1.Type);

            Assert.IsInstanceOf<ActionResult<Category>>(result); // Check if the result is of type ActionResult<category>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<GetCategoryDto>(okResult.Value);
            var category = okResult.Value as GetCategoryDto;
            Assert.AreEqual(category1.Type, category.Type);
            
        }

        [Test]
        public async Task GetCategoryById_ReturnsTheCategory()
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
            context.Categories.AddRange(new List<Category>
            {
                category1
            });

            context.SaveChanges();

            var result = await _controller.Get(category1.Id);
            
            Assert.IsInstanceOf<ActionResult<Category>>(result); // Check if the result is of type ActionResult<category>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<GetCategoryDto>(okResult.Value);
            var category = okResult.Value as GetCategoryDto;
            Assert.AreEqual(category1.Type, category.Type);
        }

        [Test]
        public async Task CreateCategory_ReturnsTheCreatedCategory()
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

            CreateCategoryDto createCategoryDto = new CreateCategoryDto(category1.Type);

            var result = await _controller.Post(createCategoryDto);
      
            Assert.IsInstanceOf<ActionResult<Category>>(result); // Check if the result is of type ActionResult<category>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type CreatedAtActionResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<GetCategoryDto>(okResult.Value);
            var category = okResult.Value as GetCategoryDto;
            Assert.AreEqual(category1.Type, category.Type);

        }

        [Test]
        public async Task UpdateCategory_ReturnsTheUpdatedCategory()
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

            context.Categories.AddRange(new List<Category>
            {
                category1
            });

            context.SaveChanges();

            UpdateCategoryDto updateCategoryDto = new UpdateCategoryDto(category1.Type,"UpdatedCategory1Type");
            
            var result = await _controller.Put(updateCategoryDto);
            Assert.NotNull(result);
            Assert.IsInstanceOf<ActionResult<Category>>(result); // Check if the result is of type ActionResult<Category>
            Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is of type OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<GetCategoryDto>(okResult.Value);
            var category = okResult.Value as GetCategoryDto;
            Assert.AreEqual(updateCategoryDto.NewType, category.Type);
        }

    }

}
