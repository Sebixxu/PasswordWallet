using System;
using System.Security.Cryptography;
using NUnit.Framework;
using PasswordWallet.Crypto.Classes;

namespace PasswordWallet.UnitTests
{
    [TestFixture]
    public class AESTests
    {
        [Test]
        public void Test_Decrypt_IsThrowingCryptographicExceptionForWrongData()
        {
            var aesAlgorithm = new AesAlgorithm();

            Assert.Throws<CryptographicException>(() =>
                aesAlgorithm.Decrypt(new byte[] { 12, 20, 25 },
                    new byte[] { 212, 29, 140, 217, 143, 0, 178, 4, 233, 128, 9, 152, 236, 248, 66, 126, 212, 29, 140, 217, 143, 0, 178, 4, 233, 128, 9, 152, 236, 248, 66, 126 },
                    new byte[] { 212, 29, 140, 217, 143, 0, 178, 4, 233, 128, 9, 152, 236, 248, 66, 126 }));
        }
    }
}