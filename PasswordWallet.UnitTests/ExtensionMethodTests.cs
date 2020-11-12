using System;
using System.Security;
using NUnit.Framework;
using PasswordWallet.Helpers;

namespace PasswordWallet.UnitTests
{
    [TestFixture]
    public class ExtensionMethodTests
    {
        [TestCase("TestString", 10)]
        [TestCase("", 0)]
        [TestCase("SomeLongerRandom String With Add. Values! :)", 44)]
        public void Test_StringToSecureString_IsValidLength(string value, int expectedLength)
        {
            var @string = value;

            var secureString = @string.StringToSecureString();

            Assert.AreEqual(expectedLength, secureString.Length);
        }

        [TestCase("TestString")]
        [TestCase("")]
        [TestCase("SomeLongerRandom String With Add. Values! :)")]
        public void Test_StringToSecureString_IsSecureStringNotNull(string value)
        {
            var @string = value;

            var secureString = @string.StringToSecureString();

            Assert.That(secureString == null, Is.False);
        }

        [TestCase("TestString")]
        [TestCase("")]
        [TestCase("SomeLongerRandom String With Add. Values! :)")]
        public void Test_SecureStringToString_IsStringValidAfterEncoding(string value)
        {
            var secureString = value.StringToSecureString(); //Prep 

            var @string = secureString.SecureStringToString(); //Test

            Assert.AreEqual(value, @string);
        }
    }
}