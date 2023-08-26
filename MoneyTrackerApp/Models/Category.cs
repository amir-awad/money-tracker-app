using System.Diagnostics.CodeAnalysis;

namespace MoneyTrackerApp.Models;

public class Category
{
    public Guid Id { get; set; }

    public required string Type { get; set; }

    public required Guid UserID { get; set; }

}