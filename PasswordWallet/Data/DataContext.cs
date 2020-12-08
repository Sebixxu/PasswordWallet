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
            //
            builder.Entity<PasswordDb>()
                .HasKey(p => p.Id);

            builder.Entity<PasswordDb>()
                .HasOne(p => p.User)
                .WithMany(u => u.Passwords)
                .HasForeignKey(p => p.IdUser);
            //
            builder.Entity<LoginAttemptsDb>()
                .HasKey(p => p.Id);

            builder.Entity<LoginAttemptsDb>()
                .HasOne(p => p.User)
                .WithMany(u => u.LoginAttempts)
                .HasForeignKey(p => p.IdUser);
            //
            builder.Entity<IpAttemptsDb>()
                .HasKey(p => p.Id);

            builder.Entity<IpAttemptsDb>()
                .HasOne(p => p.User)
                .WithMany(u => u.IpAttempts)
                .HasForeignKey(p => p.IdUser);
            //
            builder.Entity<PendingPasswordSharesDb>()
                .HasKey(p => p.Id);

            builder.Entity<PendingPasswordSharesDb>()
                .HasOne(p => p.DestinationUser)
                .WithMany(u => u.DestinationPendingPasswordShares)
                .HasForeignKey(p => p.DestinationUserId);

            builder.Entity<PendingPasswordSharesDb>()
                .HasOne(p => p.SourceUser)
                .WithMany(u => u.SourcePendingPasswordShares)
                .HasForeignKey(p => p.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PendingPasswordSharesDb>()
                .HasOne(p => p.Password)
                .WithMany(u => u.PendingPasswordShares)
                .HasForeignKey(p => p.PasswordId)
                .OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<PendingPasswordSharesDb>()
            //    .HasOne(p => p.SharedPassword)
            //    .WithMany(u => u.PendingSharedPasswordShares)
            //    .HasForeignKey(p => p.SharedPasswordId)
            //    .OnDelete(DeleteBehavior.NoAction);
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
        public DbSet<IpAttemptsDb> IpAttempts { get; set; }
        public DbSet<PendingPasswordSharesDb> PendingPasswordShares { get; set; }

        #endregion
    }
}