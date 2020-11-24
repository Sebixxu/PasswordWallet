namespace PasswordWallet.Crypto.Classes
{
    public class AlgoOperations
    {
        public virtual bool IsValidPassword(string password, string passwordToCheck)
        {
            if (password == passwordToCheck)
                return true;

            return false;
        }

        public virtual bool IsEnoughCharacters(string potentialPassword)
        {
            if (potentialPassword.Length > 8)
                return true;

            return false;
        }
    }
}