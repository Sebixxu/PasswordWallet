using System.Linq;
using Autofac;
using PasswordWallet.Crypto.Interfaces;
using PasswordWallet.Data.DbModels;
using PasswordWallet.Helpers;
using PasswordWallet.Models.Classes;
using PasswordWallet.Models.Enums;

namespace PasswordWallet.BusinessLogic
{
    public class AccountManagement : Configuration
    {
        public static string Register(UserData userData, CryptoEnum cryptoEnum) //TODO Kolizja nazw - zajętość
        {
            Container.Resolve<ICryptoStrategy>().Encrypt(userData.Password, out var passwordHash, out var passwordSalt); //Hash password
            var hmaced = cryptoEnum == CryptoEnum.HMAC;

            Context.Users.Add(new UserDb //Prepare object and add to User table
            {
                IsHMAC = hmaced,
                Login = userData.Login,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });

            Context.SaveChanges(); //Save to Db

            return "Registration was successful.";
        }

        public new static bool Login(UserData userData)
        {
            var user = Context.Users.FirstOrDefault(x => x.Login == userData.Login);
            if (user == null)
                return false;
            
            var isPasswordValid = Container.Resolve<ICryptoStrategy>().VerifyPasswordHash(userData.Password, user.PasswordHash, user.PasswordSalt);

            if (isPasswordValid)
            {
                UserName = userData.Login;
                Password = userData.Password.StringToSecureString();
            }

            return isPasswordValid;
        }

        public static void ChangePassword(string oldPassword, string newPassword)
        {
            var user = Context.Users.First(x => x.Login == UserName);
            var isPasswordValid = Container.Resolve<ICryptoStrategy>().VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt);

            if (!isPasswordValid) 
                return;

            Container.Resolve<ICryptoStrategy>().Encrypt(newPassword, out var passwordHash, out var passwordSalt); //Get hash for new password

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            PasswordManagement.RecryptPasswordForUser(newPassword.StringToSecureString()); //Recrypt website passwords

            Password = newPassword.StringToSecureString(); //Cache new password

            Context.SaveChanges();
        }

        public static CryptoEnum UserCryptoType(string userLogin)
        {
            var user = Context.Users.First(x => x.Login == userLogin); //TODO Obsługa braku usera o podanym loginie

            return user.IsHMAC ? CryptoEnum.HMAC : CryptoEnum.SHA512;
        }
    }
}