using Microsoft.EntityFrameworkCore;
using PasswordWallet.Data.DbModels;

namespace PasswordWallet.Data
{
    //stuff for creating db
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserDb>()
                .HasKey(u => u.Id);

            builder.Entity<UserDb>()
                .HasIndex(u => u.Login)
                .IsUnique();

            builder.Entity<PasswordDb>()
                .HasKey(p => p.Id);

            builder.Entity<PasswordDb>()
                .HasOne(p => p.User)
                .WithMany(u => u.Passwords)
                .HasForeignKey(p => p.IdUser);

            builder.Entity<LoginAttemptsDb>()
                .HasKey(p => p.Id);

            builder.Entity<LoginAttemptsDb>()
                .HasOne(p => p.User)
                .WithMany(u => u.LoginAttempts)
                .HasForeignKey(p => p.IdUser);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-ON1599H;Database=TestDb;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }

        #region DB Tables

        public DbSet<UserDb> Users { get; set; }
        public DbSet<PasswordDb> Passwords { get; set; }
        public DbSet<LoginAttemptsDb> LoginAttempts { get; set; }

        #endregion
    }
}