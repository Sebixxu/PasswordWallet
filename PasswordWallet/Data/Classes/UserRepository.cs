using System.Threading.Tasks;
using PasswordWallet.Data.DbModels;
using PasswordWallet.Data.Interfaces;

namespace PasswordWallet.Data.Classes
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public Task<UserDb> Register(UserDb user, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserDb> Login(string login, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SetNewPassword(UserDb user, string password)
        {
            throw new System.NotImplementedException();
        }
    }
}