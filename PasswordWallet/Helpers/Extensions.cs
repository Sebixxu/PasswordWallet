using System;
using System.Runtime.InteropServices;
using System.Security;

namespace PasswordWallet.Helpers
{
    public static class Extensions
    {
        public static SecureString StringToSecureString(this string input) //TODO To helper
        {
            SecureString output = new SecureString();

            int l = input.Length;
            char[] s = input.ToCharArray(0, l);

            foreach (char c in s)
            {
                output.AppendChar(c);
            }

            return output;
        }

        public static string SecureStringToString(this SecureString value) //TODO Do Extension methods
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}