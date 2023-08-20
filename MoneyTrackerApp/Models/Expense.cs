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
    public User User { get; set; }
    public Category Category { get; set; }


    [SetsRequiredMembers]
    public Expense(int id, double amount)
    {
        this.Id = id;
        this.Amount = amount;
        this.UserID = User.Id;
        this.CategoryID = Category.Id;
    }
}