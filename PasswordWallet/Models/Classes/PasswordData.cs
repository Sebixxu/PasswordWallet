namespace PasswordWallet.Models.Classes
{
    public class PasswordData
    {
        public int Id { get; set; }
        public string WebAddress { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public bool IsShared { get; set; }
    }
}