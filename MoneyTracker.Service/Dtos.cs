using System;

namespace MoneyTracker.Service.Dtos
{
    public record UserDto(string Username, string Password, double Balance);
}