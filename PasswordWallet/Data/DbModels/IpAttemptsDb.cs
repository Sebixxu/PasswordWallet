using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordWallet.Data.DbModels
{
    public class IpAttemptsDb
    {
        [Key]
        [Required]
        [Column("Id", Order = 0)]
        public int Id { get; set; }

        [Required]
        [Column("IpAddress", Order = 1)]
        public string IpAddress{ get; set; }

        [Required]
        [Column("LoginAttemptDate", Order = 2)]
        public DateTime LoginAttemptDate { get; set; }

        [Required]
        [Column("WasSuccess", Order = 3)]
        public bool WasSuccess { get; set; }

        [Required]
        [Column("IsStale", Order = 4)]
        public bool IsStale { get; set; }

        public IList<LoginAttemptsDb> LoginAttempts { get; set; }
    }
}