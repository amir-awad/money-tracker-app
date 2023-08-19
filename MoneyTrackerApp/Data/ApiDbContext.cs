using Microsoft.EntityFrameworkCore;
using MoneyTrackerApp.Models;

namespace MoneyTrackerApp.Data;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {

    }

    public DbSet<Expense> Expenses { get; set; }

}