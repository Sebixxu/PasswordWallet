using System;
using NUnit.Framework;
using PasswordWallet.BusinessLogic;
using PasswordWallet.Data.DbModels;

namespace PasswordWallet.TDD
{
    [TestFixture]
    public class LoginTests
    {
        [Test]
        public void Test_CalculateTimeSinceLastLoginAttempt_TimeIsCorrect([ValueSource(nameof(_correctTestData))]TestData testData)
        {
            var result = AccountManagement.CalculateTimeSinceLastLoginAttempt(testData.CurrentTime, testData.LoginAttemptsDb);

            Assert.AreEqual(testData.ExpectedResult, result);
        }

        private static TestData[] _correctTestData =
        {
            new TestData
            {
                CurrentTime = new DateTime(2020, 1, 1, 12, 0, 30),
                LoginAttemptsDb = new LoginAttemptsDb { LoginAttemptDate = new DateTime(2020, 1, 1,12, 0, 0) },
                ExpectedResult = new TimeSpan(0,0,30)
            },
            new TestData
            {
                CurrentTime = new DateTime(2020, 1, 1, 12,0, 15),
                LoginAttemptsDb = new LoginAttemptsDb { LoginAttemptDate = new DateTime(2020, 1, 1, 12, 0, 0) },
                ExpectedResult = new TimeSpan(0,0,15)
            }
        };
    }


    public class TestData
    {
        public DateTime CurrentTime { get; set; }
        public LoginAttemptsDb LoginAttemptsDb { get; set; }
        public TimeSpan ExpectedResult { get; set; }
    }
}