using System;
using System.Security.Cryptography;
using System.Text;
using PasswordWallet.Crypto.Interfaces;

namespace PasswordWallet.Crypto.Classes
{
    public class HMACStrategy : ICryptoStrategy
    {
        public void Encrypt(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();

            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            passwordSalt = hmac.Key;
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i]) return false;
            }

            return true;
        }
    }
}