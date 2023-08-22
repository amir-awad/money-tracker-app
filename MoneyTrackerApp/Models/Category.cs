using System.Diagnostics.CodeAnalysis;

namespace MoneyTrackerApp.Models;

public class Category
{
    public Guid Id { get; set; }

    public required string Type { get; set; }

    [SetsRequiredMembers]
    public Category(Guid Id, string type)
    {
        this.Id = Id;
        this.Type=type;
    }

}