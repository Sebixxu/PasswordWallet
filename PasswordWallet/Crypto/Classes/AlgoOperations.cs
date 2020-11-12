namespace PasswordWallet.Crypto.Classes
{
    public class AlgoOperations
    {
        public bool IsValidPassword(string password, string passwordToCheck)
        {
            if (password == passwordToCheck)
                return true;

            return false;
        }

        public bool IsEnoughCharacters(string potentialPassword)
        {
            if (potentialPassword.Length > 8)
                return true;

            return false;
        }
    }
}