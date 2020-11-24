using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordWallet.Data.DbModels
{
    public class LoginAttemptsDb
    {
        [Key]
        [Required]
        [Column("Id", Order = 0)]
        public int Id { get; set; }

        [Required]
        [Column("LoginAttemptDate", Order = 1)]
        public DateTime LoginAttemptDate { get; set; }

        [Required]
        [Column("WasSuccess", Order = 2)]
        public bool WasSuccess { get; set; }

        [Required]
        [Column("IsStale", Order = 3)]
        public bool IsStale { get; set; }

        [Required]
        [Column("IdUser", Order = 4)]
        public int IdUser { get; set; }

        public UserDb User { get; set; }
    }
}