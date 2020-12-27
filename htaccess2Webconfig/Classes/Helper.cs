using System;
using System.Text.RegularExpressions;

namespace WebConfig
{

    internal static class Helper
    {
        internal static string SafeSubstring(string data, int startPos, int length)
        {
            int length2 = data.Length;
            if (startPos + length <= length2)
            {
                return data.Substring(startPos, length);
            }
            return data.Substring(startPos);
        }

        internal static bool TryMatchMapValue(string input, ref int i, out string serverVar, out string prefix)
        {
            Match match = MapVariableRegex.Match(input, i);
            if (match.Success)
            {
                serverVar = match.Groups["name"].Value;
                prefix = match.Groups["prefix"].Value;
                i += match.Length - 1;
                return true;
            }
            serverVar = null;
            prefix = null;
            return false;
        }

        internal static bool TryMatchServerVariable(string input, ref int i, out string serverVar, out string prefix)
        {
            Match match = ServerVariableRegex.Match(input, i);
            if (match.Success)
            {
                serverVar = match.Groups["name"].Value;
                prefix = match.Groups["prefix"].Value;
                i += match.Length - 1;
                return true;
            }
            serverVar = null;
            prefix = null;
            return false;
        }

        internal static bool TryMatchServerVariable2(string input, ref int i, out string serverVar)
        {
            Match match = ServerVariable2Regex.Match(input, i);
            if (match.Success)
            {
                serverVar = match.Groups["name"].Value;
                i += match.Length - 1;
                return true;
            }
            serverVar = null;
            return false;
        }

        private readonly static Regex ServerVariableRegex = new Regex("\\G\\%\\{(?<prefix>[A-Za-z0-9_\\-]+:)?(?<name>[A-Za-z0-9_\\-]+)\\}", RegexOptions.Multiline | RegexOptions.Singleline);

        private readonly static Regex ServerVariable2Regex = new Regex("\\G\\%(?<name>[A-Za-z0-9_\\-]+)", RegexOptions.Multiline | RegexOptions.Singleline);

        private readonly static Regex MapVariableRegex = new Regex("\\G\\$\\{(?<prefix>[A-Za-z0-9_\\-]+):(?<name>[A-Za-z0-9_\\-]+)\\}", RegexOptions.Multiline | RegexOptions.Singleline);
    }


}
