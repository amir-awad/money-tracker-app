using System.Diagnostics.CodeAnalysis;

namespace MoneyTrackerApp.Models;

public class Expense
{
    public Guid Id { get; set; }

    public required double Amount { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public required Guid UserID { get; set; }

    public required Guid CategoryID { get; set; }

    // Navigation properties
    public required User ExpenseUser { get; set; }
    public required Category ExpenseCategory { get; set; }
}