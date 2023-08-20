namespace MoneyTrackerApp.Models;

public class Expense
{
    public int Id { get; set; }

    public double Amount { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public int UserID { get; set; }

    public int CategoryID { get; set; }

}