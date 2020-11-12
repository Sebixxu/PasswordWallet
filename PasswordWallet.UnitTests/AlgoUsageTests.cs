using NUnit.Framework;
using PasswordWallet.Crypto.Classes;

namespace PasswordWallet.UnitTests
{
    [TestFixture]
    public class AlgoUsageTests
    {
        [Test]
        public void Test_ReversePasswordIfIsValid_PasswordChanged()
        {
            string password = "Password!123";
            var algo = new AlgoUsage();

            var reversePassword = algo.ReversePasswordIfIsValid(password);

            Assert.AreNotEqual(password,reversePassword);
        }

        [Test]
        public void Test_ReversePasswordIfIsValid_PasswordChanged_WithAdapter()
        {
            string password = "Password!123";
            var algo = new AlgoUsage(new AlgoOperationsAdapter()); //Injecting adapter mock class

            var reversePassword = algo.ReversePasswordIfIsValid(password);

            Assert.AreNotEqual(password, reversePassword);
        }
    }
}