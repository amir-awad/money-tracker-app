using System.Diagnostics.CodeAnalysis;

namespace MoneyTrackerApp.Models;

public class User
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }

    public required string Email { get; set; }

    public double Balance { get; set; }

    [SetsRequiredMembers]
    public User(int id,string username,string password,string email,double balance)
    {
        this.Id=id;
        this.Username=username;
        this.Password=password;
        this.Email=email;
        this.Balance=balance;
        
    }

}