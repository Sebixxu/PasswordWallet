using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordWallet.Data.DbModels
{
    public class PasswordDb
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
        public byte[] PasswordHash { get; set; } //Hash?

        [Column("WebAddress", Order = 3)]
        public string WebAddress { get; set; }

        [Column("Description", Order = 4)]
        public string Description { get; set; }

        [Required]
        [Column("IdUser", Order = 5)]
        public int IdUser { get; set; }

        public UserDb User { get; set; }

        public IList<PendingPasswordSharesDb> PendingPasswordShares { get; set; }
        //public IList<PendingPasswordSharesDb> PendingSharedPasswordShares { get; set; }
    }
}