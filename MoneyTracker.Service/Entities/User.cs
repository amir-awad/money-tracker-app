using Microsoft.AspNetCore.Identity;

namespace MoneyTracker.Service.Entities
{
    public class User : IdentityUser 
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public double Balance { get; set; }


    }
}