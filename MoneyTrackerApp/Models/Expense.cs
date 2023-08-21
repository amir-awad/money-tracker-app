using System.Diagnostics.CodeAnalysis;

namespace MoneyTrackerApp.Models;

public class Expense
{
    public int Id { get; set; }

    public double Amount { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public required int UserID { get; set; }

    public required int CategoryID { get; set; }

    // Navigation properties
    public required User ExpenseUser { get; set; }
    public required Category ExpenseCategory { get; set; }


}