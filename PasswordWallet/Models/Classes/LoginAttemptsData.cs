using System;

namespace PasswordWallet.Models.Classes
{
    public class LoginAttemptsData
    {
        public DateTime LoginAttemptDate { get; set; }
        public bool WasSuccess { get; set; }
    }
}