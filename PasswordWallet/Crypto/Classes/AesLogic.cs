using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using PasswordWallet.Helpers;

namespace PasswordWallet.Crypto.Classes
{
    public class AesLogic
    {
        public byte[] EncryptPassword(string passwordToStore, string keyValue)
        {
            var aes = new AesAlgorithm();

            var md5 = CryptoHelper.CreateMd5(keyValue); //prepare necessary data
            var doubleMd5 = md5.Concat(md5).ToArray(); //prepare necessary data

            var encryptedPassword = aes.Encrypt(passwordToStore, doubleMd5, md5); //IV - 128 bits ; Key - 256 bits

            return encryptedPassword;
        }

        public byte[] EncryptPassword(string passwordToStore, SecureString userPassword)
        {
            var aes = new AesAlgorithm();

            var userPasswordString = userPassword.SecureStringToString();
            var md5 = CryptoHelper.CreateMd5(userPasswordString); //prepare necessary data
            var doubleMd5 = md5.Concat(md5).ToArray(); //prepare necessary data

            var encryptedPassword = aes.Encrypt(passwordToStore, doubleMd5, md5); //IV - 128 bits ; Key - 256 bits

            return encryptedPassword;
        }

        public string DecryptPassword(byte[] passwordBytes, SecureString userPassword)
        {
            var aes = new AesAlgorithm();

            var userPasswordString = userPassword.SecureStringToString();
            var md5 = CryptoHelper.CreateMd5(userPasswordString); //prepare necessary data
            var doubleMd5 = md5.Concat(md5).ToArray(); //prepare necessary data
            var decryptedPassword = aes.Decrypt(passwordBytes, doubleMd5, md5); //IV - 128 bits ; Key - 256 bits

            return decryptedPassword;
        }

        public string DecryptPassword(byte[] passwordBytes, string keyValue)
        {
            var aes = new AesAlgorithm();

            var md5 = CryptoHelper.CreateMd5(keyValue); //prepare necessary data
            var doubleMd5 = md5.Concat(md5).ToArray(); //prepare necessary data
            var decryptedPassword = aes.Decrypt(passwordBytes, doubleMd5, md5); //IV - 128 bits ; Key - 256 bits

            return decryptedPassword;
        }
    }
}