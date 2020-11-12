using System;

namespace PasswordWallet.Crypto.Classes
{
    public class AlgoManagement
    {
        private AlgoUsage AlgoUsage;

        public string CheckAndReversePasswordIfPossible(string potentialPassword)
        {
            if (potentialPassword != null)
            {
                return AlgoUsage.ReversePasswordIfIsValid(potentialPassword);
            }

            throw new Exception("Given argument was wrong in CheckAndReversePasswordIfPossible.");
        }
    }
}