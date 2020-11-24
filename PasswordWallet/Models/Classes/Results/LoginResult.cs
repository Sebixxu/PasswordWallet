namespace PasswordWallet.Models.Classes.Results
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public int TimeoutDurationInSeconds { get; set; }
    }
}