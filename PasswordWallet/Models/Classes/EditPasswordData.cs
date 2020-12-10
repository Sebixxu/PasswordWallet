namespace PasswordWallet.Models.Classes
{
    public class EditPasswordData
    {
        public int PasswordId { get; set; }
        public string NewLogin { get; set; }
        public string NewPassword { get; set; }
        public string NewWebAddress { get; set; }
        public string NewDescription { get; set; }
    }
}