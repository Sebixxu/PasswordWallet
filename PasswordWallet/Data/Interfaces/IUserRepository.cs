using System.Threading.Tasks;
using PasswordWallet.Data.DbModels;

namespace PasswordWallet.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDb> Register(UserDb user, string password);
        Task<UserDb> Login(string login, string password);
        Task<bool> SetNewPassword(UserDb user, string password);
    }
}