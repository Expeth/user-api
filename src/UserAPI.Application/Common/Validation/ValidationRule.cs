using System;
using System.Text.RegularExpressions;
using UserAPI.Application.Common.Helper;

namespace UserAPI.Application.Common.Validation
{
    public static class ValidationRule
    {
        private static Regex EmailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",
            RegexOptions.None, TimeSpan.FromMilliseconds(100));
        
        private static Regex UsernameRegex = new Regex(@"^[a-zA-Z0-9]+$",
            RegexOptions.None, TimeSpan.FromMilliseconds(100));
        
        private static Regex PasswordRegex = new Regex(@"^(.{0,7}|[^0-9]*|[^A-Z])$",
            RegexOptions.None, TimeSpan.FromMilliseconds(100));

        public static Func<string, bool> Email = str => RegEx.IsMatchSafely(str, EmailRegex);
        public static Func<string, bool> Username = str => RegEx.IsMatchSafely(str, UsernameRegex);
        public static Func<string, bool> Password = str => RegEx.IsMatchSafely(str, PasswordRegex);
    }
}