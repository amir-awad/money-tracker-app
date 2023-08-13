using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Service.Dtos;

namespace MoneyTracker.Service.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private static readonly List<UserDto> users = new()
        {
            new UserDto("Amir", "Amir123", 10000000000),
        new UserDto("Amir", "Amir123", 10000000000),

        new UserDto("Amir", "Amir123", 10000000000)
        };

        public IEnumerable<UserDto> Get()
        {
            return users;
        }
    }
}