namespace PasswordWallet.Crypto.Classes
{
    public class AlgoOperationsAdapter : AlgoOperations
    {
        public new bool IsValidPassword(string password, string passwordToCheck)
        {
            return true;
        }

        public new bool IsEnoughCharacters(string potentialPassword)
        {
            return true;
        }
    }
}