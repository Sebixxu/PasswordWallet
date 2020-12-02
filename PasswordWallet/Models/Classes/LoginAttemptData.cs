using System;

namespace PasswordWallet.Models.Classes
{
    public class LoginAttemptData
    {
        public DateTime LoginAttemptDate { get; set; }
        public bool WasSuccess { get; set; }
    }
}