using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Autofac;
using Microsoft.EntityFrameworkCore.Internal;
using PasswordWallet.BusinessLogic;
using PasswordWallet.Crypto;
using PasswordWallet.Crypto.Classes;
using PasswordWallet.Models;
using PasswordWallet.Models.Classes;
using PasswordWallet.Models.Classes.Results;
using PasswordWallet.Models.Enums;

namespace PasswordWallet
{
    class Program
    {
        static void Main(string[] args)
        {
            var isAlive = true;
            var loginWasSuccessful = false;

            while (isAlive)
            {
                Console.WriteLine("===| Menu |===");
                Console.WriteLine("===| 1) Create account. |===");
                Console.WriteLine("===| 2) Login. |===");
                Console.WriteLine("===| q) Quit. |===");

                var data = Console.ReadLine();

                if (data == "q")
                    isAlive = false;
                else if (data == "1") //Register
                {
                    Console.WriteLine("Pick one of algo to store your master password:");
                    Console.WriteLine("===| 1) HMAC |===");
                    Console.WriteLine("===| 2) SHA512 |===");
                    var algorithm = Console.ReadLine();

                    CryptoEnum cryptoEnum = CryptoEnum.None;
                    switch (algorithm)
                    {
                        case "1":
                            cryptoEnum = CryptoEnum.HMAC;
                            break;
                        case "2":
                            cryptoEnum = CryptoEnum.SHA512;
                            break;
                    }
                    Configuration.Configure(cryptoEnum);

                    //data for register
                    Console.WriteLine("Enter login:");
                    var login = Console.ReadLine();

                    Console.WriteLine("Enter password:");
                    var password = Console.ReadLine();

                    var resultString = AccountManagement.Register(new UserData(login, password), cryptoEnum); //Create user account
                    Console.WriteLine(resultString);
                }
                else if (data == "2") //Login
                {
                    Console.WriteLine("Enter login:");
                    var login = Console.ReadLine();

                    Console.WriteLine("Enter password:");
                    var password = Console.ReadLine();

                    var userData = new UserData(login, password);
                    var userCryptoType = AccountManagement.UserCryptoType(userData.Login);

                    Configuration.Configure(userCryptoType);
                    var loginResult = AccountManagement.ProcessLoginAttemptByIp(userData);
                    loginWasSuccessful = loginResult.IsSuccess;

                    if (loginWasSuccessful)
                    {
                        AccountManagement.Login(userData);
                        Console.WriteLine("Login was successful.");
                    }
                    else
                    {
                        Console.WriteLine("Your data was wrong.");
                        if (loginResult.TimeoutDurationInSeconds > 0)
                            Console.WriteLine($"Please wait {loginResult.TimeoutDurationInSeconds} second since try login once again.");
                        else if (loginResult.TimeoutDurationInSeconds == -1)
                            Console.WriteLine("Your address IP are permanently banned.");
                    }
                    //Console.WriteLine(loginWasSuccessful ? "Login was successful." : "User data was wrong.");
                }

                if (loginWasSuccessful)
                {
                    while (true)
                    {
                        Console.WriteLine("===| 1) Manage passwords. |===");
                        Console.WriteLine("===| 2) Store new password. |===");
                        Console.WriteLine("===| 3) Change master password. |===");
                        Console.WriteLine("===| 4) Show user login data. |===");
                        Console.WriteLine("===| 5) Show ip addresses data. |===");
                        Console.WriteLine("===| q) Quit. |===");

                        var x = Console.ReadLine();

                        if (x == "q")
                        {
                            Console.WriteLine("Shutdown.. Thanks for using app.");
                            Environment.Exit(0);
                        }
                        else if (x == "1") //Show one password => Manage passwords
                        {
                            Console.WriteLine($"===| You are in {GetApplicationModeString()} type 0) to change mode. |===");
                            Console.WriteLine($"===| ============================================ |===");
                            Console.WriteLine("===| 1) Show specific password. |===");
                            Console.WriteLine("===| 2) Show passwords. |==="); //todo  remove
                            Console.WriteLine("===| 3) Share specific password. |===");
                            Console.WriteLine("===| 4) Show pending passwords. |===");
                            Console.WriteLine("===| 5) Manage your password data. |==="); //List all, allow change only personal passwords

                            var y = Console.ReadLine();
                            if (y == "0")
                                PasswordManagement.SwitchApplicationMode();
                            else if (y == "1")
                            {
                                var passwordsData = PasswordManagement.GetPasswordsList().ToList();

                                if (!passwordsData.Any())
                                {
                                    Console.WriteLine("There is no password stored for this user.");
                                    continue;
                                }

                                foreach (var password in passwordsData)
                                    Console.WriteLine($"Id: {password.Id} | Web Address: { password.WebAddress } | Login: { password.Login } | Description: { password.Description }");

                                Console.WriteLine("Type id password which you want release:");
                                var id = Console.ReadLine();
                                var decryptedPassword = PasswordManagement.GetDecryptedPasswordData(int.Parse(id));

                                Console.WriteLine($"Web Address: { decryptedPassword.WebAddress } | Login: { decryptedPassword.Login } | Password: { decryptedPassword.Password } | Description: { decryptedPassword.Description }");

                            }
                            else if (y == "2")
                            {
                                ShowPasswordsData(PasswordManagement.GetDecryptedPasswordsData().ToList());
                            }
                            else if (y == "3")
                            {
                                ShowPasswordsData(PasswordManagement.GetPasswordsDataToShare().ToList()); //Only available to share

                                Console.WriteLine("Type id password which you want share:");
                                var idPassword = Console.ReadLine();

                                //show all users
                                var userInfos = AccountManagement.GetAllOtherUserInfo().ToList();

                                foreach (var userInfo in userInfos)
                                    Console.WriteLine($"Id: {userInfo.Id} | Username: {userInfo.Username}");

                                Console.WriteLine("Type id user which you want share chosen password:");
                                var idUser = Console.ReadLine();

                                PasswordManagement.SendRequestOfSharingPassword(int.Parse(idUser), int.Parse(idPassword));
                                //share password
                            }
                            else if (y == "4")
                            {
                                var passwordSharesInfo = PasswordManagement.GetPendingPasswordForCurrentUser().ToList();

                                if (!passwordSharesInfo.Any())
                                {
                                    Console.WriteLine("There is no pending password for this user.");
                                    continue;
                                }

                                foreach (var pendingPasswordShare in passwordSharesInfo)
                                    Console.WriteLine($"Id: {pendingPasswordShare.Id} | WebsiteAddress: {pendingPasswordShare.WebsiteAddress} | Description: {pendingPasswordShare.Description} " +
                                                      $"SourceUsername: {pendingPasswordShare.SourceUsername}");

                                Console.WriteLine("Type id password which you want to accept:");
                                var shareId = Console.ReadLine();

                                PasswordManagement.AcceptPasswordShare(int.Parse(shareId));
                            }
                            else if (y == "5")
                            {
                                Console.WriteLine("Trying enter password modifying platform..");

                                if (PasswordManagement.GetApplicationMode() == ApplicationMode.ReadMode)
                                {
                                    Console.WriteLine("To enter this functionalities you need switch to modify mode in aplication.");
                                    continue;
                                }

                                ShowPasswordsData(PasswordManagement.GetDecryptedPasswordsData().ToList());

                                Console.WriteLine("Type id password which you want manage:");
                                var idPassword = Console.ReadLine();

                                if (PasswordManagement.CheckEditPasswordPossibility(int.Parse(idPassword)) == EditPasswordPossibility.Ok)
                                {
                                    Console.WriteLine("What you want to do?");

                                    Console.WriteLine("1) Edit password data.");
                                    Console.WriteLine("2) Remove password.");

                                    var editType = Console.ReadLine();

                                    if (editType == "1")
                                    {
                                        Console.WriteLine("Fill all new data, if you don't want to change something, leave field empty.");

                                        Console.WriteLine("Login");
                                        var newLogin = Console.ReadLine();
                                        Console.WriteLine("Password");
                                        var newPassword = Console.ReadLine();
                                        Console.WriteLine("Web Address");
                                        var newWebAddress = Console.ReadLine();
                                        Console.WriteLine("Description");
                                        var newDescription = Console.ReadLine();

                                        PasswordManagement.EditPassword(new EditPasswordData
                                        {
                                            PasswordId = int.Parse(idPassword),
                                            NewLogin = newLogin,
                                            NewPassword = newPassword,
                                            NewWebAddress = newWebAddress,
                                            NewDescription = newDescription
                                        });
                                    }
                                    else if (editType == "2")
                                    {
                                        PasswordManagement.RemovePassword(int.Parse(idPassword));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No access to modify this password.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Wrong choise.");
                            }
                        }
                        else if (x == "2") //Store password
                        {
                            Console.WriteLine("Enter web address:");
                            var webAddress = Console.ReadLine();

                            Console.WriteLine("Enter login for this website:");
                            var loginForWebsite = Console.ReadLine();

                            Console.WriteLine("Enter password for this website:");
                            var passwordForWebsite = Console.ReadLine();

                            Console.WriteLine("Enter description (optional)");
                            var description = Console.ReadLine();

                            if (string.IsNullOrEmpty(description)) // TODO XD?? XD
                                description = null;

                            PasswordManagement.StorePassword(new PasswordData { WebAddress = webAddress, Login = loginForWebsite, Password = passwordForWebsite, Description = description });
                        }
                        else if (x == "3") //Change Password
                        {
                            Console.WriteLine("Enter old password:");
                            var password = Console.ReadLine();

                            Console.WriteLine("Enter new password:");
                            var newPassword = Console.ReadLine();

                            Console.WriteLine("Re enter new password:");
                            var reNewPassword = Console.ReadLine();

                            if (newPassword == reNewPassword)
                                AccountManagement.ChangePassword(password, newPassword);
                            else
                                Console.WriteLine("Password doesn't match.");
                        }
                        else if (x == "4")
                        {
                            var allLoginAttemptsDataForUser = AccountManagement.GetLoginAttemptsView();

                            Console.WriteLine("Login Attempt Date  | Was Succesfull?");
                            foreach (var attemptsData in allLoginAttemptsDataForUser)
                            {
                                Console.WriteLine($"{ attemptsData.LoginAttemptDate } | { attemptsData.WasSuccess }");
                            }
                        }
                        else if (x == "5")
                        {
                            var bannedIpAddresses = AccountManagement.GetPermanentlyBannedIpAddresses();

                            Console.WriteLine("Ip Address  | Is Banned Pernamently?");
                            if (bannedIpAddresses.Any())
                                foreach (var bannedIpAddress in bannedIpAddresses)
                                    Console.WriteLine($"{ bannedIpAddress } | true");
                            else
                                Console.WriteLine($"No address found.");

                            Console.WriteLine("Enter ip address which you want unlock or type 0) to skip this process.");
                            var z = Console.ReadLine();

                            if (z == "0")
                                continue;

                            if (bannedIpAddresses.Contains(z))
                            {
                                AccountManagement.UnbanIpAddress(z);
                                Console.WriteLine("Address was unbanned.");
                            }
                        }
                    }
                }
            }
        }

        private static void ShowPasswordsData(IList<PasswordData> passwordsData)
        {
            if (!passwordsData.Any())
            {
                Console.WriteLine("There is no password stored for this user.");
                return;
            }

            foreach (var password in passwordsData)
            {
                string isShared = "<Shared> ";
                var isSharedString = password.IsShared ? isShared : "";

                Console.WriteLine(
                        $"{isSharedString}Id: {password.Id} |Web Address: {password.WebAddress} | Login: {password.Login} | Password: {password.Password} | Description: {password.Description}");
            }
        }

        private static string GetApplicationModeString()
        {
            if (PasswordManagement.GetApplicationMode() == ApplicationMode.ReadMode)
                return "Read Mode";

            if (PasswordManagement.GetApplicationMode() == ApplicationMode.ModifyMode)
                return "Modify Mode";

            return "";
        }
    }
}
