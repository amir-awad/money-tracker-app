using System;

namespace MoneyTracker.Service.Dtos
{
    public record ExpenseDto(Guid Id, int categoryId, int userId, double amount, DateTimeOffset date);

    public record CreateExpenseDto(int categoryId, int userId, double amount);

    public record UpdateExpenseDto(int categoryId, int userId, double amount);

}