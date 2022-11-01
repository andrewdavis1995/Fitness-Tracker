
using System.Windows.Controls;

namespace MoneyMatters.Helpers
{
    internal static class BankNameHelper
    {
        static string[] _supportedBanks = new string[] {
            "YBS", "Yorkshire Building Society", "RBS", "Royal Bank of Scotland", "Tesco", "Santander", "Atom", "Chase",
            "HSBC", "Barclays", "Natwest", "Lloyds"
        };
    
        /// <summary>
        /// Formats the supplied bank name to the output format (to allow the correct image to be shown)
        /// </summary>
        /// <param name="input">The name of the bank to look for</param>
        public static string GetBankName(string input)
        {
            // remove spaces and make lower case
            string strippedInput = input.Trim().ToLower();
            
            // remove the word bank - generalise
            strippedInput = input.Replace("bank", "");
        
            string output = string.Empty;
            
            // check for exact match
            bool satisfied = CheckExactMatch(strippedInput, ref output);
            if(!satisfied)
            {
                satisfied = CheckPartialMatch(strippedInput, ref output);
            }
            
            // if found a match, return it. Otherwise return the input (unformatted)
            return satisfied ? output : input;
        }
        
        /// <summary>
        /// Checks the list for an exact match
        /// </summary>
        /// <param name="input">The name of the bank to look for</param>
        /// <param name="output">The resulting name to return</param>
        static bool CheckExactMatch(string input, ref string output)
        {
            bool satisfied = false;
            foreach(var b in _supportedBanks)
            {
                // exact match, return
                if(b.ToLower() == input)
                {
                    output = b;
                    satisfied = true;
                    break;
                }
            }
            return satisfied;
        }
        
        /// <summary>
        /// Checks the list for a partial match
        /// </summary>
        /// <param name="input">The name of the bank to look for</param>
        /// <param name="output">The resulting name to return</param>
        static bool CheckPartialMatch(string input, ref string output)
        {
            bool satisfied = false;
            foreach(var b in _supportedBanks)
            {
                // if either word contains the other, return
                if(b.ToLower().Contains(input) || input.Contains(b.ToLower()))
                {
                    output = b;
                    satisfied = true;
                    break;
                }
            }
            return satisfied;
        }
    }
}
