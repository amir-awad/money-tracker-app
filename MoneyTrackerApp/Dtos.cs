using System;

namespace MoneyTracker.Service.Dtos
{
    //Expenses
    public record GetExpenseDto(int Id, double Amount, int UserId, int CategoryId, DateTimeOffset Creationdate);

    public record CreateExpenseDto(int Id, double Amount, int UserId, int CategoryId);

    public record UpdateExpenseDto( double Amount, int CategoryId);

    //Users
    public record UpdateUserDto(string Username, string Password, string Email, double Balance);

}