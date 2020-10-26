using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using PasswordWallet.Helpers;

namespace PasswordWallet.Crypto.Classes
{
    public class AesLogic
    {
        public byte[] EncryptPassword(string passwordToStore, SecureString userPassword)
        {
            var aes = new AesAlgorithm();

            var userPasswordString = userPassword.SecureStringToString();
            var md5 = CreateMd5(userPasswordString);
            var doubleMd5 = md5.Concat(md5).ToArray();

            var encryptedPassword = aes.Encrypt(passwordToStore, doubleMd5, md5); //IV - 128 bits ; Key - 256 bits

            return encryptedPassword;
        }

        public string DecryptPassword(byte[] passwordBytes, SecureString userPassword)
        {
            var aes = new AesAlgorithm();

            var userPasswordString = userPassword.SecureStringToString();
            var md5 = CreateMd5(userPasswordString);
            var doubleMd5 = md5.Concat(md5).ToArray();
            var decryptedPassword = aes.Decrypt(passwordBytes, doubleMd5, md5); //IV - 128 bits ; Key - 256 bits

            return decryptedPassword;
        }

        private static byte[] CreateMd5(string input) //TODO Do helpera?
        {
            // Use input string to calculate MD5 hash
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return hashBytes;
        }
    }
}