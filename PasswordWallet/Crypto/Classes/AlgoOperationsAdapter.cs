namespace PasswordWallet.Crypto.Classes
{
    public class AlgoOperationsAdapter : AlgoOperations
    {
        public override bool IsValidPassword(string password, string passwordToCheck)
        {
            return true;
        }

        public override bool IsEnoughCharacters(string potentialPassword)
        {
            return true;
        }
    }
}