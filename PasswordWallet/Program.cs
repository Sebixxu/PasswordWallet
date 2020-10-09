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

                    var algo = Console.ReadLine();

                    CryptoEnum cryptoEnum = CryptoEnum.None;
                    if (algo == "1")
                        cryptoEnum = CryptoEnum.HMAC;
                    else if (algo == "2")
                        cryptoEnum = CryptoEnum.SHA512;
                    Configuration.Configure(cryptoEnum);

                    //data for register

                    Console.WriteLine("Enter login:");
                    var login = Console.ReadLine();

                    Console.WriteLine("Enter password:");
                    var password = Console.ReadLine();

                    AccountManagement.Register(new UserData(login, password), cryptoEnum);


                    //Create user account
                }
                else if (data == "2")
                {
                    //Login
                    Console.WriteLine("Enter login:");
                    var login = Console.ReadLine();

                    Console.WriteLine("Enter password:");
                    var password = Console.ReadLine();

                    var userData = new UserData(login, password);
                    var userCryptoType = AccountManagement.UserCryptoType(userData.Login);

                    Configuration.Configure(userCryptoType);
                    AccountManagement.EditPassword(userData);
                }
                else if (data == "3")
                {
                    //test
                    Console.WriteLine("Enter login:");
                    var login = Console.ReadLine();

                    Console.WriteLine("Enter password:");
                    var password = Console.ReadLine();
                    
                    
                    AccountManagement.EditPassword(new UserData(login, password));
                }
            }

            Console.WriteLine("Bye bye!");
        }
    }
}
