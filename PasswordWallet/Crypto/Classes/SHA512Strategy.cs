using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using PasswordWallet.Crypto.Interfaces;

namespace PasswordWallet.Crypto.Classes
{
    public class SHA512Strategy : ICryptoStrategy
    {
        public void Encrypt(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            passwordSalt = new byte[1024];

            rng.GetBytes(passwordSalt);
            string salt = BitConverter.ToString(passwordSalt); //save random bytes as salt string

            string pepper;
            using (StreamReader streamReader = new StreamReader(".\\Pepper.txt", Encoding.UTF8)) //open Pepper file
            {
                pepper = streamReader.ReadToEnd(); //read pepper file
            }

            var preparedData = password + salt + pepper; //concat all of components

            using var sha = new SHA512Managed();
            passwordHash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(preparedData));
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if (passwordSalt == null)
                return false;

            string pepper;
            using (StreamReader streamReader = new StreamReader(".\\Pepper.txt", Encoding.UTF8))
            {
                pepper = streamReader.ReadToEnd();
            }

            string salt = BitConverter.ToString(passwordSalt);

            using var sha = new SHA512Managed();
            var computedHash = sha.ComputeHash(Encoding.UTF8.GetBytes(password + salt + pepper)); //get hash from concat components

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i]) return false;
            }

            return true;
        }
    }
}