using System.Linq;
using NUnit.Framework;
using PasswordWallet.Crypto.Classes;

namespace PasswordWallet.UnitTests
{
    [TestFixture]
    public class HMACTests
    {
        [TestCase("TestString")]
        [TestCase("")]
        [TestCase("SomeLongerRandom String With Add. Values! :)")]
        public void Test_Encrypt_IsPasswordHashAndPasswordSaltSet(string value)
        {
            var hmacStrategy = new HMACStrategy();
            var @string = value;

            hmacStrategy.Encrypt(@string, out var passwordHash, out var passwordSalt);

            Assert.That(passwordHash.Any() && passwordSalt.Any(), Is.True);
        }

        [TestCase(
            "TestString",
            new byte[] { 142, 105, 245, 152, 38, 29, 83, 221, 235, 238, 114, 255, 32, 210, 216, 80, 161, 89, 94, 84, 197, 144, 106, 70, 121, 164, 133, 40, 225, 138, 107, 107, 105, 80, 169, 154, 155, 27, 255, 38, 251, 43, 38, 171, 157, 236, 212, 108, 179, 124, 252, 215, 2, 251, 219, 253, 134, 84, 105, 42, 98, 86, 205, 5 },
            new byte[] { 221, 145, 132, 39, 133, 239, 152, 48, 222, 63, 4, 43, 20, 111, 114, 60, 34, 221, 13, 46, 201, 182, 212, 214, 38, 14, 80, 68, 82, 140, 223, 55, 86, 226, 10, 50, 254, 9, 69, 27, 188, 225, 96, 147, 94, 82, 170, 85, 35, 17, 60, 150, 214, 179, 103, 238, 144, 69, 204, 63, 53, 55, 54, 145, 186, 79, 206, 72, 99, 84, 229, 194, 219, 150, 209, 28, 1, 154, 214, 230, 43, 149, 174, 170, 53, 101, 96, 198, 104, 17, 179, 246, 117, 109, 113, 53, 118, 151, 232, 109, 68, 132, 223, 109, 81, 129, 82, 161, 9, 238, 206, 86, 39, 123, 59, 37, 241, 237, 8, 226, 107, 227, 5, 96, 176, 190, 118, 170 })]
        public void Test_VerifyPasswordHash_IsPasswordHashEqualToGivenHashAndSalt(string passwordToCheck, byte[] originalPasswordHash, byte[] originalPasswordSalt)
        {
            var hmacStrategy = new HMACStrategy();

            var result = hmacStrategy.VerifyPasswordHash(passwordToCheck, originalPasswordHash, originalPasswordSalt);

            Assert.IsTrue(result);
        }

        [TestCase(
            "TestString",
            new byte[] { 142, 105, 245, 152, 38, 29, 83, 221, 235, 238, 114, 255, 32, 210, 216, 80, 161, 89, 94, 84, 197, 144, 106, 70, 121, 164, 133, 40, 225, 138, 107, 107, 105, 80, 169, 154, 155, 27, 255, 38, 251, 43, 38, 171, 157, 236, 212, 108, 179, 124, 252, 215, 2, 251, 219, 253, 134, 84, 105, 42, 98, 86, 206, 5 },
            new byte[] { 221, 145, 132, 39, 133, 239, 152, 48, 222, 63, 4, 43, 20, 111, 114, 60, 34, 221, 13, 46, 201, 182, 212, 214, 38, 14, 80, 68, 82, 140, 223, 55, 86, 226, 10, 50, 254, 9, 69, 27, 188, 225, 96, 147, 94, 82, 170, 85, 35, 17, 60, 150, 214, 179, 103, 238, 144, 69, 204, 63, 53, 55, 54, 145, 186, 79, 206, 72, 99, 84, 229, 194, 219, 150, 209, 28, 1, 154, 214, 230, 43, 149, 174, 170, 53, 101, 96, 198, 104, 17, 179, 246, 117, 109, 113, 53, 118, 151, 232, 109, 68, 132, 223, 109, 81, 129, 82, 161, 9, 238, 206, 86, 39, 123, 59, 37, 241, 237, 8, 226, 107, 227, 5, 96, 176, 190, 118, 170 })]
        public void Test_VerifyPasswordHash_IsPasswordHashNotEqualToGivenHash(string passwordToCheck, byte[] originalPasswordHash, byte[] originalPasswordSalt)
        {
            var hmacStrategy = new HMACStrategy();

            var result = hmacStrategy.VerifyPasswordHash(passwordToCheck, originalPasswordHash, originalPasswordSalt);

            Assert.IsFalse(result);
        }

        [TestCase(
            "TestString",
            new byte[] { 142, 105, 245, 152, 38, 29, 83, 221, 235, 238, 114, 255, 32, 210, 216, 80, 161, 89, 94, 84, 197, 144, 106, 70, 121, 164, 133, 40, 225, 138, 107, 107, 105, 80, 169, 154, 155, 27, 255, 38, 251, 43, 38, 171, 157, 236, 212, 108, 179, 124, 252, 215, 2, 251, 219, 253, 134, 84, 105, 42, 98, 86, 205, 5 },
            new byte[] { 221, 145, 132, 39, 133, 239, 152, 48, 222, 63, 4, 43, 20, 111, 114, 60, 34, 221, 13, 46, 201, 182, 212, 214, 38, 14, 80, 68, 82, 140, 223, 55, 86, 226, 10, 50, 254, 9, 69, 27, 188, 225, 96, 147, 94, 82, 170, 85, 35, 17, 60, 150, 214, 179, 103, 238, 144, 69, 204, 63, 53, 55, 54, 145, 186, 79, 206, 72, 99, 84, 229, 194, 219, 150, 209, 28, 1, 154, 214, 230, 43, 149, 174, 170, 53, 101, 96, 198, 104, 17, 179, 246, 117, 109, 113, 53, 118, 151, 232, 109, 68, 132, 223, 109, 81, 129, 82, 161, 9, 238, 206, 86, 39, 123, 59, 37, 241, 237, 8, 226, 107, 227, 5, 96, 176, 190, 118, 175 })]
        public void Test_VerifyPasswordHash_IsPasswordHashNotEqualToGivenSalt(string passwordToCheck, byte[] originalPasswordHash, byte[] originalPasswordSalt)
        {
            var hmacStrategy = new HMACStrategy();

            var result = hmacStrategy.VerifyPasswordHash(passwordToCheck, originalPasswordHash, originalPasswordSalt);

            Assert.IsFalse(result);
        }
    }
}