using MoneyTracker.Service.Dtos;
using MoneyTrackerApp.Models;

namespace MoneyTracker.Service.Extensions
{
    public static class Extensions
    {
        public static GetExpenseDto AsDto(this Expense expense)
        {
            return new GetExpenseDto(expense.Id, expense.Amount, expense.UserID, expense.CategoryID, expense.CreationDate);
        }

        public static GetCategoryDto AsDto(this Category category)
        {
            return new GetCategoryDto(category.Id, category.Type);
        }

    }
}