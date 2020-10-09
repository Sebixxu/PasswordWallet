using System.Linq;
using Autofac;
using Microsoft.EntityFrameworkCore;
using PasswordWallet.Crypto;
using PasswordWallet.Data;
using PasswordWallet.Data.Classes;
using PasswordWallet.Data.DbModels;
using PasswordWallet.Data.Interfaces;
using PasswordWallet.Models;

namespace PasswordWallet.BussinessLogic
{
    public class AccountManagement : Configuration
    {
        public static void Register(UserData userData, CryptoEnum cryptoEnum)
        {
            var context = ContextFactory.GetContext();
            
            //hash
            Container.Resolve<ICryptoStrategy>().Encrypt(userData.Password, out var passwordHash, out var passwordSalt);

            bool hmaced = cryptoEnum == CryptoEnum.HMAC;


            context.Users.AddAsync(new UserDb
            {
                IsPasswordHashed = hmaced,
                Login = userData.Login,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });

            //save
            context.SaveChanges();
        }

        public static void Login(UserData userData)
        {
            var context = ContextFactory.GetContext();

            var user = context.Users.First(x => x.Login == userData.Login);

            Container.Resolve<ICryptoStrategy>().VerifyPasswordHash(userData.Password, user.PasswordHash, user.PasswordSalt);
        }

        public static void EditPassword(UserData userData)
        {
            var context = ContextFactory.GetContext();

            var user = context.Users.First(x => x.Login == userData.Login);

            var isPasswordValid = Container.Resolve<ICryptoStrategy>().VerifyPasswordHash(userData.Password, user.PasswordHash, user.PasswordSalt);
        }

        public static CryptoEnum UserCryptoType(string userLogin)
        {
            var context = ContextFactory.GetContext();

            var user = context.Users.First(x => x.Login == userLogin); //TODO obsługa braku usera o podanym loginie

            return user.IsPasswordHashed ? CryptoEnum.HMAC : CryptoEnum.SHA512;
        } 
    }
}