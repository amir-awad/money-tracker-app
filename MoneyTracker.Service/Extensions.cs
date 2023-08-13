using MoneyTracker.Service.Dtos;
using MoneyTracker.Service.Entities;

namespace MoneyTracker.Service
{
    public static class Extensions
    {
        public static UserDto AsDto(this User user)
        {
            return new UserDto(user.Username, user.Password, user.Balance);
        }
    }
}