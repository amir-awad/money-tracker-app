namespace MoneyTrackerApp.Models;

public class Category
{
    public int Id { get; set; }

    public required string Type { get; set; }

    public Category(int id,string type)
    {
        this.Id=id;
        this.Type=type;
    }

}