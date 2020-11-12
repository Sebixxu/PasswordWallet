using System;

namespace PasswordWallet.Crypto.Classes
{
    public class AlgoUsage : IAlgoUsage
    {
        AlgoOperations AlgoOperations;

        public AlgoUsage()
        {
            AlgoOperations = new AlgoOperations();
        }

        public AlgoUsage(AlgoOperations algoOperations)
        {
            AlgoOperations = algoOperations;
        }

        public string ReversePasswordIfIsValid(string password)
        {
            if (AlgoOperations.IsEnoughCharacters(password))
            {
                char[] charArray = password.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }
            else
            {
                return null;
            }
        }
    }

    public interface IAlgoUsage
    {
        string ReversePasswordIfIsValid(string password);
    }
}