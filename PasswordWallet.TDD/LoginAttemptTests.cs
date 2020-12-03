using NUnit.Framework;
using PasswordWallet.BusinessLogic;

namespace PasswordWallet.TDD
{
    [TestFixture]
    public class LoginAttemptTests
    {
        [TestCase(0, 0)]
        [TestCase(1, 30)]
        [TestCase(2, 60)]
        [TestCase(3, 120)]
        [TestCase(4, 120)]
        [TestCase(5, 120)]
        [TestCase(10, 120)]
        public void Test_GetBanTimeoutDurationToShow_IsValidDurationToShow(int failsCount, int expectedDurationToShow)
        {
            var valueToTest = failsCount;

            var result = AccountManagement.GetBanTimeoutDurationToShow(valueToTest);

            Assert.AreEqual(expectedDurationToShow, result);
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(2, 30)]
        [TestCase(3, 120)]
        [TestCase(4, 120)]
        [TestCase(5, 120)]
        [TestCase(10, 120)]
        public void Test_GetBanTimeoutDuration_IsValidDuration(int failsCount, int expectedDurationToShow)
        {
            var valueToTest = failsCount;

            var result = AccountManagement.GetBanTimeoutDuration(valueToTest);

            Assert.AreEqual(expectedDurationToShow, result);
        }
    }
}