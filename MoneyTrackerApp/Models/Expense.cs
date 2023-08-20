namespace MoneyTrackerApp.Models;

public class Expense
{
    public int Id { get; set; }

    public double Amount { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public int UserID { get; set; }

    public int CategoryID { get; set; }

    // Navigation properties
    public User User { get; set; }
    public Category Category { get; set; }


    public Expense(int id,double amount)
    {
        this.Id=id;
        this.Amount=amount;
        this.UserID=User.Id;
        this.CategoryID=Category.Id;
    }
}