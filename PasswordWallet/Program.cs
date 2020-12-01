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
                    var loginResult = AccountManagement.ProcessLoginAttempt2(userData);
                    loginWasSuccessful = loginResult.IsSuccess;

                    if (loginWasSuccessful)
                    {
                        Console.WriteLine("Login was successful.");
                    }
                    else
                    {
                        Console.WriteLine("Your data was wrong.");
                        if (loginResult.TimeoutDurationInSeconds > 0)
                            Console.WriteLine($"Please wait {loginResult.TimeoutDurationInSeconds} second since try login once again.");
                    }
                    //Console.WriteLine(loginWasSuccessful ? "Login was successful." : "User data was wrong.");
                }

                if (loginWasSuccessful)
                {
                    while (true)
                    {
                        Console.WriteLine("===| 1) Show specific password. |===");
                        Console.WriteLine("===| 2) Show passwords. |===");
                        Console.WriteLine("===| 3) Store new password. |===");
                        Console.WriteLine("===| 4) Change master password. |===");
                        Console.WriteLine("===| 5) Show login data. |===");
                        Console.WriteLine("===| q) Quit. |===");

                        var x = Console.ReadLine();

                        if (x == "q")
                        {
                            Console.WriteLine("Shutdown.. Thanks for using app.");
                            Environment.Exit(0);
                        }
                        else if (x == "1") //Show one password
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
                            var decryptedPassword = PasswordManagement.GetDecryptedPasswordData(int.Parse(id)); //TODO Valid / TryParse

                            Console.WriteLine($"Web Address: { decryptedPassword.WebAddress } | Login: { decryptedPassword.Login } | Password: { decryptedPassword.Password } | Description: { decryptedPassword.Description }");
                        }
                        else if (x == "2") //Show all passwords
                        {
                            var passwordsData = PasswordManagement.GetDecryptedPasswordsData().ToList();

                            if (!passwordsData.Any())
                            {
                                Console.WriteLine("There is no password stored for this user.");
                                continue;
                            }

                            foreach (var password in passwordsData)
                                Console.WriteLine($"Web Address: { password.WebAddress } | Login: { password.Login } | Password: { password.Password } | Description: { password.Description }");

                        }
                        else if (x == "3") //Store password
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
                        else if (x == "4") //Change Password
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
                        else if (x == "5")
                        {
                            var allLoginAttemptsDataForUser = AccountManagement.GetLoginAttemptsView();

                            Console.WriteLine("Login Attempt Date  | Was Succesfull?");
                            foreach (var attemptsData in allLoginAttemptsDataForUser)
                            {
                                Console.WriteLine($"{ attemptsData.LoginAttemptDate } | { attemptsData.WasSuccess }");
                            }
                        }
                    }
                }
            }
        }
    }
}
