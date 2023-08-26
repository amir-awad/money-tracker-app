using System;

namespace MoneyTracker.Service.Dtos
{
    //Expenses
    public record GetExpenseDto(Guid Id, double Amount, Guid UserId, Guid CategoryId, DateTimeOffset Creationdate);

    public record CreateExpenseDto(double Amount, string CategoryType);

    public record UpdateExpenseDto(double Amount, Guid CategoryId);

    //Users
    public record LoginUserDto(string Email, string Password);

    public record UpdateUserDto(string Username, string Password, string Email, double Balance);

    public record CreateUserDto(string Username, string Password, string Email, double Balance);

    // Categories
    public record CreateCategoryDto(string Type);

}