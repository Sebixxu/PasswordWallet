using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordWallet.Data.DbModels
{
    public class UserDb
    {
        [Key]
        [Required]
        [Column("Id", Order = 0)]
        public int Id { get; set; }

        [Required]
        [Column("Login", Order = 1)]
        public string Login { get; set; }

        [Required]
        [Column("PasswordHash", Order = 2)]
        public byte[] PasswordHash { get; set; }

        [Required]
        [Column("PasswordSalt", Order = 3)]
        public byte[] PasswordSalt { get; set; }

        [Required]
        [Column("IsHMAC", Order = 4)]
        public bool IsHMAC { get; set; }

        public IList<PasswordDb> Passwords { get; set; }

        //
        public IList<LoginAttemptsDb> LoginAttempts { get; set; }
        public IList<IpAttemptsDb> IpAttempts { get; set; }
        //

        public IList<PendingPasswordSharesDb> SourcePendingPasswordShares { get; set; }
        public IList<PendingPasswordSharesDb> DestinationPendingPasswordShares { get; set; }
    }
}