using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PasswordWallet.Data.DbModels;

namespace PasswordWallet.TDD
{
    public class UserDb
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsHMAC { get; set; }

        public virtual IList<PasswordDb> Passwords { get; set; }
        public virtual IList<LoginAttemptsDb> LoginAttempts { get; set; }
        public virtual IList<IpAttemptsDb> IpAttempts { get; set; }
    }

    public class Context : DbContext
    {
        public virtual DbSet<UserDb> Users { get; set; }
        //public virtual DbSet<UserDb> Users { get; set; }
    }

    public class ContextService
    {
        private readonly Context Context;

        public ContextService(Context context)
        {
            this.Context = context;
        }

        public UserDb AddUser(string login, byte[] passwordHash, byte[] passwordSalt, bool isHmac)
        {
            var newUser = this.Context.Users.Add(
                new UserDb
                {
                    Login = login,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    IsHMAC = isHmac
                }).Entity;

            this.Context.SaveChanges();

            return newUser;
        }

        public IList<UserDb> GetUser()
        {
            return this.Context.Users.ToList();
        }
    }

    public class Mock
    {
        
    }
}