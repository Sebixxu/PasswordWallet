using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using PasswordWallet.Crypto.Classes;
using PasswordWallet.Data;
using PasswordWallet.Data.DbModels;
using PasswordWallet.Models;

namespace PasswordWallet.BussinessLogic
{
    public class PasswordManagement : Configuration
    {
        public static void StorePassword(PasswordData passwordData)
        {
            var aesLogic = new AesLogic();

            var user = Context.Users.First(x => x.Login == UserName);
            var encryptPassword = aesLogic.EncryptPassword(passwordData.Password, Password);

            var passwordDb = new PasswordDb
            {
                Login = passwordData.Login,
                PasswordHash = encryptPassword,
                WebAddress = passwordData.WebAddress,
                Description = passwordData.Description,
                User = user,
                IdUser = user.Id
            };

            Context.Passwords.Add(passwordDb);

            Context.SaveChanges(); //save
        }

        public static void RecryptPasswordForUser(SecureString newUserPassword)
        {
            var aesLogic = new AesLogic();

            var user = Context.Users.First(x => x.Login == UserName);
            var userPasswordsData = Context.Passwords.Where(x => x.IdUser == user.Id).ToList();

            foreach (var userPasswordData in userPasswordsData)
            {
                var websitePassword = ReleasePassword(userPasswordData.PasswordHash);

                userPasswordData.PasswordHash = aesLogic.EncryptPassword(websitePassword, newUserPassword);
            }

            Context.SaveChanges();
        }

        public static string ReleasePassword(byte[] passwordHash)
        {
            var aesLogic = new AesLogic();

            var encryptPassword = aesLogic.DecryptPassword(passwordHash, Password);

            return encryptPassword;
        }

        public static IEnumerable<PasswordData> GetPasswordsList()
        {
            IList<PasswordData> listOfPasswords = new List<PasswordData>();

            var user = Context.Users.First(x => x.Login == UserName);
            var userPasswordsData = Context.Passwords.Where(x => x.IdUser == user.Id).ToList();

            foreach (var userPasswordData in userPasswordsData)
            {
                listOfPasswords.Add(new PasswordData
                {
                    Id = userPasswordData.Id,
                    Login = userPasswordData.Login,
                    WebAddress = userPasswordData.WebAddress,
                    Description = userPasswordData.Description
                });
            }

            return listOfPasswords;
        }

        public static PasswordData GetDecryptedPasswordData(int id)
        {
            var userPasswordData = Context.Passwords.FirstOrDefault(x => x.Id == id);
            var password = ReleasePassword(userPasswordData?.PasswordHash);

            return new PasswordData
            {
                Login = userPasswordData?.Login,
                Password = password,
                WebAddress = userPasswordData?.WebAddress,
                Description = userPasswordData?.Description
            };
        }

        public static IEnumerable<PasswordData> GetDecryptedPasswordsData()
        {
            IList<PasswordData> decryptPasswords = new List<PasswordData>();

            var user = Context.Users.First(x => x.Login == UserName);
            var userPasswordsData = Context.Passwords.Where(x => x.IdUser == user.Id);

            foreach (var userPasswordData in userPasswordsData)
            {
                var password = ReleasePassword(userPasswordData.PasswordHash);

                decryptPasswords.Add(new PasswordData
                {
                    Login = userPasswordData.Login,
                    Password = password,
                    WebAddress = userPasswordData.WebAddress,
                    Description = userPasswordData.Description
                });
            }
            
            return decryptPasswords;
        }
    }
}