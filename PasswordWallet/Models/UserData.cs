namespace PasswordWallet.Models
{
    public class UserData
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public UserData(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}