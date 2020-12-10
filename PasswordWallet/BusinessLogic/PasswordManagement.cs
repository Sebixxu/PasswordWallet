using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using PasswordWallet.Crypto.Classes;
using PasswordWallet.Data.DbModels;
using PasswordWallet.Models.Classes;
using PasswordWallet.Models.Classes.Results;

namespace PasswordWallet.BusinessLogic
{
    public class PasswordManagement : Configuration
    {
        private static string RandomData =
            "WNA1IRfs9xQilUq3roTCOHeymadXo0K8IHHpgBZKnjDa035fNBkg8gUm4hl5ETc6YlHii6ZuUZwoEjwJrBUBV9iOutadGBO65uN7HZt2957NAjJU4jcbGjCrc8Lv163I";

        public static EditPasswordPossibility CheckEditPasswordPossibility(int passwordId) //Tu raczej wystarczyło by sprawdzenie isShared ale ciul
        {
            var isShared = Context.PendingPasswordShares.FirstOrDefault(x => x.SharedPasswordId == passwordId) != null;
            var currentUserId = Context.Users.FirstOrDefault(x => x.Login == UserName)?.Id;
            var isOwner = Context.PendingPasswordShares
                .Where(x => x.PasswordId == passwordId && x.SourceUserId == currentUserId).ToList().Any();

            if (isShared && !isOwner)
                return EditPasswordPossibility.NoAccess;

            return EditPasswordPossibility.Ok;
        }

        public static void EditPassword(EditPasswordData editPasswordData)
        {
            var aesLogic = new AesLogic();

            //update na password
            var password = Context.Passwords.FirstOrDefault(x => x.Id == editPasswordData.PasswordId);

            if (password == null)
                return;

            if (!string.IsNullOrEmpty(editPasswordData.NewLogin))
                password.Login = editPasswordData.NewLogin;

            if (!string.IsNullOrEmpty(editPasswordData.NewWebAddress))
                password.WebAddress = editPasswordData.NewWebAddress;

            if (!string.IsNullOrEmpty(editPasswordData.NewDescription))
                password.Description = editPasswordData.NewDescription;

            if (!string.IsNullOrEmpty(editPasswordData.NewPassword))
                password.PasswordHash = aesLogic.EncryptPassword(editPasswordData.NewPassword, Password);

            var passwordsIdsFromSharedPasswords = Context.PendingPasswordShares.Where(x => x.PasswordId == editPasswordData.PasswordId)
                .Select(y => y.SharedPasswordId).ToList(); //From Passwords by received from SharedPasswordIds

            foreach (var passwordId in passwordsIdsFromSharedPasswords)
            {
                var passwordDb = Context.Passwords.FirstOrDefault(x => x.Id == passwordId);
                if (passwordDb != null)
                    Context.Passwords.Remove(passwordDb);

                var sharedPasswordDb =
                    Context.PendingPasswordShares.FirstOrDefault(x => x.SharedPasswordId == passwordId);
                if (sharedPasswordDb != null)
                    Context.PendingPasswordShares.Remove(sharedPasswordDb);
            }

            Context.SaveChanges();
        }

        public static void RemovePassword(int passwordId)
        {
            var passwordsIdsFromSharedPasswords = Context.PendingPasswordShares.Where(x => x.PasswordId == passwordId)
                .Select(y => y.SharedPasswordId).ToList(); //From Passwords by received from SharedPasswordIds

            foreach (var passwordIdFromSharedPassword in passwordsIdsFromSharedPasswords)
            {
                var passwordDb = Context.Passwords.FirstOrDefault(x => x.Id == passwordIdFromSharedPassword);
                if (passwordDb != null)
                    Context.Passwords.Remove(passwordDb);

                var sharedPasswordDb =
                    Context.PendingPasswordShares.FirstOrDefault(x => x.SharedPasswordId == passwordIdFromSharedPassword);
                if (sharedPasswordDb != null)
                    Context.PendingPasswordShares.Remove(sharedPasswordDb);
            }

            var mainWebPasswordDb = Context.Passwords.FirstOrDefault(x => x.Id == passwordId);
            if (mainWebPasswordDb != null)
                Context.Passwords.Remove(mainWebPasswordDb);

            Context.SaveChanges();
        }

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

                if (isShared)
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

        public static void SwitchApplicationMode()
        {
            if (ApplicationMode == ApplicationMode.ReadMode)
            {
                ApplicationMode = ApplicationMode.ModifyMode;
            }
            else if (ApplicationMode == ApplicationMode.ModifyMode)
            {
                ApplicationMode = ApplicationMode.ReadMode;
            }
        }

        public static ApplicationMode GetApplicationMode()
        {
            return ApplicationMode;
        }
    }
}