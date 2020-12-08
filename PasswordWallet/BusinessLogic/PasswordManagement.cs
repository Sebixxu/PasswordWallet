using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using PasswordWallet.Crypto.Classes;
using PasswordWallet.Data.DbModels;
using PasswordWallet.Models.Classes;

namespace PasswordWallet.BusinessLogic
{
    public class PasswordManagement : Configuration
    {
        private static string RandomData =
            "WNA1IRfs9xQilUq3roTCOHeymadXo0K8IHHpgBZKnjDa035fNBkg8gUm4hl5ETc6YlHii6ZuUZwoEjwJrBUBV9iOutadGBO65uN7HZt2957NAjJU4jcbGjCrc8Lv163I";

        public static void SendRequestOfSharingPassword(int destinationUserId, int passwordId) //todo chyba do passmanagement
        {
            //Odszyfruj hasło podane przez usera
            var passwordData = GetDecryptedPasswordData(passwordId);

            //Zapisz do pending zaszyfrowane jakimiś danymi z apki
            StorePasswordInPendingTable(passwordData, destinationUserId);
        }

        public static void AcceptPasswordShare(int passwordShareId)
        {
            var passwordShare = Context.PendingPasswordShares.FirstOrDefault(x => x.Id == passwordShareId);
            var sourcePasswordData = Context.Passwords.FirstOrDefault(x => x.Id == passwordShare.PasswordId);

            var releasedTempPassword = ReleasePassword(passwordShare.PasswordHash, RandomData);

            var newPasswordData = new PasswordData
            {
                WebAddress = sourcePasswordData.WebAddress,
                Login = sourcePasswordData.Login,
                Password = releasedTempPassword,
                Description = sourcePasswordData.Description
            };

            var id = StorePassword(newPasswordData);

            passwordShare.SharedPasswordId = id;
            passwordShare.IsStale = true;
            passwordShare.PasswordHash = null;

            Context.SaveChanges();
        }

        public static void StorePasswordInPendingTable(PasswordData passwordData, int destinationUserId)
        {
            var aesLogic = new AesLogic();

            var destinationUser = Context.Users.First(x => x.Id == destinationUserId);
            var sourceUser = Context.Users.First(x => x.Login == UserName);
            var password = Context.Passwords.First(x => x.Id == passwordData.Id);

            var encryptPassword = aesLogic.EncryptPassword(passwordData.Password, RandomData); //Get encrypted password

            var passwordSharesDb = new PendingPasswordSharesDb
            {
                PasswordHash = encryptPassword,
                PasswordId = passwordData.Id,
                Password = password,
                SourceUserId = sourceUser.Id,
                SourceUser = sourceUser,
                DestinationUserId = destinationUserId,
                DestinationUser = destinationUser,
                IsStale = false,
                SharedPasswordId = null
            };

            Context.PendingPasswordShares.Add(passwordSharesDb);

            Context.SaveChanges(); //save
        }

        public static int StorePassword(PasswordData passwordData)
        {
            var aesLogic = new AesLogic();

            var user = Context.Users.First(x => x.Login == UserName);
            var encryptPassword = aesLogic.EncryptPassword(passwordData.Password, Password); //Get encrypted password

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
            Context.SaveChanges();

            return passwordDb.Id;
        }

        public static void RecryptPasswordForUser(SecureString newUserPassword)
        {
            var aesLogic = new AesLogic();

            var user = Context.Users.First(x => x.Login == UserName);
            var userPasswordsData = Context.Passwords.Where(x => x.IdUser == user.Id).ToList();

            foreach (var userPasswordData in userPasswordsData)
            {
                var websitePassword = ReleasePassword(userPasswordData.PasswordHash); //decrypt website password

                userPasswordData.PasswordHash = aesLogic.EncryptPassword(websitePassword, newUserPassword); //update old password hash new one
            }

            Context.SaveChanges();
        }

        public static string ReleasePassword(byte[] passwordHash)
        {
            var aesLogic = new AesLogic();

            var encryptPassword = aesLogic.DecryptPassword(passwordHash, Password);

            return encryptPassword;
        }

        public static string ReleasePassword(byte[] passwordHash, string keyValue)
        {
            var aesLogic = new AesLogic();

            var encryptPassword = aesLogic.DecryptPassword(passwordHash, keyValue);

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
                Id = userPasswordData.Id,
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
            var userPasswordsData = Context.Passwords.Where(x => x.IdUser == user.Id).ToList();

            foreach (var userPasswordData in userPasswordsData)
            {
                var password = ReleasePassword(userPasswordData.PasswordHash);
                var isShared = Context.PendingPasswordShares.FirstOrDefault(x => x.SharedPasswordId == userPasswordData.Id) != null;

                decryptPasswords.Add(new PasswordData
                {
                    Id = userPasswordData.Id,
                    Login = userPasswordData.Login,
                    Password = password,
                    WebAddress = userPasswordData.WebAddress,
                    Description = userPasswordData.Description,
                    IsShared = isShared
                });
            }

            return decryptPasswords;
        }

        public static IEnumerable<PasswordData> GetPasswordsDataToShare()
        {
            IList<PasswordData> decryptPasswords = new List<PasswordData>();

            var user = Context.Users.First(x => x.Login == UserName);
            var userPasswordsData = Context.Passwords.Where(x => x.IdUser == user.Id).ToList();

            foreach (var userPasswordData in userPasswordsData)
            {
                var password = ReleasePassword(userPasswordData.PasswordHash);
                var isShared = Context.PendingPasswordShares.FirstOrDefault(x => x.SharedPasswordId == userPasswordData.Id) != null;

                if(isShared)
                    continue;

                decryptPasswords.Add(new PasswordData
                {
                    Id = userPasswordData.Id,
                    Login = userPasswordData.Login,
                    Password = password,
                    WebAddress = userPasswordData.WebAddress,
                    Description = userPasswordData.Description
                });
            }

            return decryptPasswords;
        }

        public static IEnumerable<PendingPasswordSharesInfo> GetPendingPasswordForCurrentUser()
        {
            var currentUser = Context.Users.FirstOrDefault(x => x.Login == UserName);

            IList<PendingPasswordSharesInfo> pendingPasswordSharesInfo = new List<PendingPasswordSharesInfo>();
            var passwordSharesForUser = Context.PendingPasswordShares.Where(x => x.DestinationUserId == currentUser.Id && !x.IsStale).ToList();

            foreach (var pendingPasswordShare in passwordSharesForUser)
            {
                var passwordFromShareData = Context.Passwords.FirstOrDefault(x => x.Id == pendingPasswordShare.PasswordId);
                var sourceUsername = Context.Users.FirstOrDefault(x => x.Id == pendingPasswordShare.SourceUserId)?.Login;

                pendingPasswordSharesInfo.Add(new PendingPasswordSharesInfo
                {
                    Id = pendingPasswordShare.Id,
                    WebsiteAddress = passwordFromShareData.WebAddress,
                    SourceUserId = pendingPasswordShare.SourceUserId,
                    SourceUsername = sourceUsername
                });
            }

            return pendingPasswordSharesInfo;
        }
    }
}