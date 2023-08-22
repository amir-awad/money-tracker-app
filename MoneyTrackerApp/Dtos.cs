using System;

namespace MoneyTracker.Service.Dtos
{
    //Expenses
    public record GetExpenseDto(Guid Id, double Amount, Guid UserId, Guid CategoryId, DateTimeOffset Creationdate);

    public record CreateExpenseDto(double Amount, Guid UserId, Guid CategoryId);

    public record UpdateExpenseDto(double Amount, Guid CategoryId);

    //Users
    public record UpdateUserDto(string Username, string Password, string Email, double Balance);

    public record CreateUserDto(string Username, string Password, string Email, double Balance);

}