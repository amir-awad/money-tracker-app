using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MoneyTrackerApp.Models;

public class User
{
    public Guid Id { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }

    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public required string Email { get; set; }

    public required double Balance { get; set; }

    [SetsRequiredMembers]
    public User(Guid Id, string username,string password,string email,double balance)
    {
        this.Id=Id;
        this.Username=username;
        this.Password=password;
        this.Email=email;
        this.Balance=balance;
        
    }

}