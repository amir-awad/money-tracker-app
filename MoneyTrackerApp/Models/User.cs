using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MoneyTrackerApp.Models;

public class User
{
    public Guid Id { get; set; }

    public required string Username { get; set; }

    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }

    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public required string Email { get; set; }

    public required double Balance { get; set; }

    [SetsRequiredMembers]
    public User(Guid Id, string username, byte[] passwordHash, byte[] passwordSalt, string email, double balance)
    {
        this.Id=Id;
        this.Username=username;
        this.PasswordHash=passwordHash;
        this.PasswordSalt=passwordSalt;
        this.Email=email;
        this.Balance=balance;
        
    }

}