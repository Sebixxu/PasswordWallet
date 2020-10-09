namespace PasswordWallet.Crypto
{
    public interface ICryptoStrategy
    {
        void Encrypt(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}