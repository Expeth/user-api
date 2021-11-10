using System;
using System.Text.RegularExpressions;

namespace UserAPI.Application.Common.Helper
{
    public static class RegEx
    {
        public static bool IsMatchSafely(string str, Regex expression)
        {
            try
            {
                return expression.IsMatch(str);
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
        }
    }
}