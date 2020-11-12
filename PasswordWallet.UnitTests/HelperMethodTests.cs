using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using NUnit.Framework;
using PasswordWallet.Helpers;

namespace PasswordWallet.UnitTests
{
    [TestFixture]
    public class HelperMethodTests
    {
        [TestCase("TestString")]
        [TestCase("")]
        [TestCase("SomeLongerRandom String With Add. Values! :)")]
        public void Test_CreateMd5_IsValidLength(string value)
        {
            var @string = value;

            var md5 = CryptoHelper.CreateMd5(@string);

            Assert.AreEqual(16, md5.Length);
        }

        [TestCase("TestString", new byte[] { 91, 86, 244, 15, 136, 40, 112, 31, 151, 250, 69, 17, 221, 205, 37, 251 })]
        [TestCase("", new byte[] { 212, 29, 140, 217, 143, 0, 178, 4, 233, 128, 9, 152, 236, 248, 66, 126 })]
        [TestCase("SomeLongerRandom String With Add. Values! :)", new byte[] { 192, 11, 4, 177, 93, 34, 235, 19, 86, 184, 168, 33, 15, 69, 105, 202 })]
        public void Test_CreateMd5_IsHashValid(string stringValue, byte[] hashValue)
        {
            var @string = stringValue;

            var md5 = CryptoHelper.CreateMd5(@string);

            Assert.AreEqual(hashValue, md5);
        }
    }
}