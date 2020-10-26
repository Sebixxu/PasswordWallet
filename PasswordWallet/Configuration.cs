using System;
using System.Reflection;
using System.Security;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PasswordWallet.BussinessLogic;
using PasswordWallet.Crypto;
using PasswordWallet.Crypto.Classes;
using PasswordWallet.Data;
using PasswordWallet.Data.Classes;
using PasswordWallet.Data.Interfaces;
using PasswordWallet.Models;

namespace PasswordWallet
{
    public class Configuration
    {
        protected static string UserName;
        protected static SecureString Password;
        protected static IContainer Container;
        protected static DataContext Context = ContextFactory.GetContext();

        public static void Configure(CryptoEnum cryptoEnum)
        {
            var builder = new ContainerBuilder();
            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(executingAssembly)
                .AsSelf()
                .AsImplementedInterfaces();

            if (cryptoEnum == CryptoEnum.HMAC) //HMAC
                builder.RegisterType<HMACStrategy>().As<ICryptoStrategy>();
            else if (cryptoEnum == CryptoEnum.SHA512) //SHA
                builder.RegisterType<SHA512Strategy>().As<ICryptoStrategy>();
            else
                Console.WriteLine("===| Wrong |===");

            Container = builder.Build();
        }
    }
}