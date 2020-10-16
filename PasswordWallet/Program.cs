using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Autofac;
using PasswordWallet.BussinessLogic;
using PasswordWallet.Crypto;
using PasswordWallet.Crypto.Classes;
using PasswordWallet.Models;

namespace PasswordWallet
{
    class Program
    {
        // AccountManagement AccountManagement => new AccountManagement();


        static void Main(string[] args)
        {
            var isAlive = true;
            var loginWasSuccesful = false;
            //var accountManagement = new AccountManagement();
            //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            //var passwordSalt = new byte[512];

            //rng.GetBytes(passwordSalt);
            //string salt = BitConverter.ToString(passwordSalt);
            //Console.WriteLine(salt);

            Console.WriteLine("===| Menu |===");
            Console.WriteLine("===| 1) Create account. |===");
            Console.WriteLine("===| 2) Login. |===");
            Console.WriteLine("===| q) Quit. |===");

            while (isAlive)
            {
                var data = Console.ReadLine();

                if (data == "q")
                    isAlive = false;
                else if (data == "1") //Register
                {
                    //choosing hmac or sha512
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
                    loginWasSuccesful = AccountManagement.Login(userData);

                    if (!loginWasSuccesful)
                        Console.WriteLine("User data was wrong.");
                }

                if (loginWasSuccesful)
                {
                    Console.WriteLine("===| 1) Show passwords. |===");
                    Console.WriteLine("===| 2) Store new password. |===");
                    Console.WriteLine("===| 3) Change master password. |===");
                    Console.WriteLine("===| 0) Go back. |===");
                    Console.WriteLine("===| q) Quit. |===");

                    var x = Console.ReadLine();

                    if (x == "0")
                        return;
                    else if (x == "q")
                        isAlive = false; //TODO możliwe żę tu też return trzeba będzie dać
                    else if (x == "1") //Show passwords
                    {
                        var passwordsData = PasswordManagement.GetPasswordsData();

                        foreach (var password in passwordsData)
                        {
                            Console.WriteLine($"Web Address: { password.WebAddress } | Login: { password.Login } | Password: { password.Password } | Description: { password.Description }");
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


                    //User logic
                    //Show passwords
                    //Store password
                    //Change password -> odszyfruj hasła i zaszyfruj nowym hasłem
                }
            }

            Console.WriteLine("Bye bye!");
        }
    }
}
