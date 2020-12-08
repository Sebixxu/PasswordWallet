namespace PasswordWallet.Models.Classes
{
    public class PendingPasswordSharesInfo
    {
        public int Id { get; set; }
        public string WebsiteAddress { get; set; }
        public string Description { get; set; }
        public int SourceUserId { get; set; }
        public string SourceUsername { get; set; }
    }
}