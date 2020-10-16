using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PasswordWallet.Crypto.Classes;
using PasswordWallet.Data;
using PasswordWallet.Data.DbModels;
using PasswordWallet.Models;

namespace PasswordWallet.BussinessLogic
{
    public class PasswordManagement : Configuration
    {
        public static void StorePassword(PasswordData passwordData) //other data
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

        public static string ReleasePassword(byte[] passwordHash)
        {
            var aesLogic = new AesLogic();

            var encryptPassword = aesLogic.DecryptPassword(passwordHash, Password);

            return encryptPassword;
        }

        public static PasswordData GetPasswordData(int id)
        {
            var passwordDb = Context.Passwords.First(x => x.Id == id);

            var password = ReleasePassword(passwordDb.PasswordHash);

            return new PasswordData
            {
                Login = passwordDb.Login,
                Password = password,
                WebAddress = passwordDb.WebAddress,
                Description = passwordDb.Description
            };
        }

        public static IEnumerable<PasswordData> GetPasswordsData()
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