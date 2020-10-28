using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using Autofac;
using Microsoft.EntityFrameworkCore;
using PasswordWallet.Crypto;
using PasswordWallet.Data;
using PasswordWallet.Data.Classes;
using PasswordWallet.Data.DbModels;
using PasswordWallet.Data.Interfaces;
using PasswordWallet.Helpers;
using PasswordWallet.Models;

namespace PasswordWallet.BussinessLogic
{
    public class AccountManagement : Configuration
    {
        public static string Register(UserData userData, CryptoEnum cryptoEnum) //TODO Kolizja nazw - zajętość
        {
            Container.Resolve<ICryptoStrategy>().Encrypt(userData.Password, out var passwordHash, out var passwordSalt); //hash
            var hmaced = cryptoEnum == CryptoEnum.HMAC;

            Context.Users.Add(new UserDb
            {
                IsHMAC = hmaced,
                Login = userData.Login,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });

            Context.SaveChanges(); //save

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

            Container.Resolve<ICryptoStrategy>().Encrypt(newPassword, out var passwordHash, out var passwordSalt); //new hash

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            PasswordManagement.RecryptPasswordForUser(newPassword.StringToSecureString());

            Password = newPassword.StringToSecureString();

            Context.SaveChanges();
        }

        public static CryptoEnum UserCryptoType(string userLogin)
        {
            var user = Context.Users.First(x => x.Login == userLogin); //TODO obsługa braku usera o podanym loginie

            return user.IsHMAC ? CryptoEnum.HMAC : CryptoEnum.SHA512;
        }
    }
}