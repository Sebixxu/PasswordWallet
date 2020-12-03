using NUnit.Framework;
using PasswordWallet.BusinessLogic;

namespace PasswordWallet.TDD
{
    [TestFixture]
    public class IpAttemptTests
    {
        [TestCase(0, 0)]
        [TestCase(1, 30)]
        [TestCase(2, 60)]
        [TestCase(3, -1)]
        [TestCase(4, -1)]
        [TestCase(5, -1)]
        [TestCase(10, -1)]
        public void Test_GetBanTimeoutDurationToShowForIp_IsValidDurationToShow(int failsCount, int expectedDurationToShow)
        {
            var valueToTest = failsCount;

            var result = AccountManagement.GetBanTimeoutDurationToShowForIp(valueToTest);

            Assert.AreEqual(expectedDurationToShow, result);
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(2, 30)]
        [TestCase(3, 60)]
        [TestCase(4, -1)]
        [TestCase(5, -1)]
        [TestCase(10, -1)]
        public void Test_GetBanTimeoutDurationForIp_IsValidDuration(int failsCount, int expectedDurationToShow)
        {
            var valueToTest = failsCount;

            var result = AccountManagement.GetBanTimeoutDurationForIp(valueToTest);

            Assert.AreEqual(expectedDurationToShow, result);
        }
    }
}