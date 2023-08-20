using System.Diagnostics.CodeAnalysis;

namespace MoneyTrackerApp.Models;

public class Category
{
    public int Id { get; set; }

    public required string Type { get; set; }

    [SetsRequiredMembers]
    public Category(int id,string type)
    {
        this.Id=id;
        this.Type=type;
    }

}