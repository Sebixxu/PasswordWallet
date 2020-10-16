namespace PasswordWallet.Crypto
{
    public interface IAESDefinition
    {
        byte[] Encrypt(string plainText, byte[] Key, byte[] IV);
        string Decrypt(byte[] cipherText, byte[] Key, byte[] IV);
    }
}