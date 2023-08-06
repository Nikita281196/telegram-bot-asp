using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Services.Users
{
    public interface IUserService
    {
        IReadOnlyList<User> GetUsers();
        User GetById(long Id);
        void AddUser(User user);
        void SetPremium(long userId);
        void Subscribe(long userId, int traiderId);
        void Unsubscribe(long userId, int traiderId);
    }
}
