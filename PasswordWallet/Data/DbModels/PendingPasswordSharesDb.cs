using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordWallet.Data.DbModels
{
    public class PendingPasswordSharesDb
    {
        [Key]
        [Required]
        [Column("Id", Order = 0)]
        public int Id { get; set; }

        [Column("PasswordHash", Order = 1)]
        public byte[] PasswordHash { get; set; }

        [Required]
        [Column("PasswordId", Order = 2)]
        public int PasswordId { get; set; }
        public PasswordDb Password { get; set; }

        [Required]
        [Column("SourceUserId", Order = 3)]
        public int SourceUserId { get; set; }
        public UserDb SourceUser { get; set; }

        [Required]
        [Column("DestinationUserId", Order = 4)]
        public int DestinationUserId { get; set; }
        public UserDb DestinationUser { get; set; }

        [Required]
        [Column("IsStale", Order = 5)]
        public bool IsStale { get; set; }

        [Column("SharedPasswordId", Order = 6)]
        public int? SharedPasswordId { get; set; }
        //public PasswordDb SharedPassword { get; set; }
    }
}