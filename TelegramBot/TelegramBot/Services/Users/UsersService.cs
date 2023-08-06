using TelegramBot.Models;

namespace TelegramBot.Services.Users
{
    public class UsersService: IUserService
    {
        private List<User> Users = new List<User>();
        public IReadOnlyList<User> GetUsers()
        {
            return Users;
        }

        public User GetById(long Id)
        {
            return Users.Where(x => x.Id == Id).FirstOrDefault();
        }

        public void AddUser(User user)
        {
            Users.Add(user);
        }

        public void SetPremium(long userId)
        {
            var user = Users.Where(x => x.Id == userId).FirstOrDefault();
            if (user != null)
            {
                user.IsPremiumActive = true;
            }
        }


        public void Subscribe(long userId, int traiderId)
        {
            var user = Users.Where(x => x.Id == userId).FirstOrDefault();
            if (user != null)
            {
                if (user.IsPremiumActive)
                {
                    user.TraiderIds.Add(traiderId);
                }
                else
                {
                    user.TraiderIds = new List<int> { traiderId };
                }
            }
        }

        public void Unsubscribe(long userId, int traiderId)
        {
            var user = Users.Where(x => x.Id == userId).FirstOrDefault();
            user?.TraiderIds.Remove(traiderId);
        }
    }
}
