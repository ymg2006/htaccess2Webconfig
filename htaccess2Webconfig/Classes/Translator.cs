using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WebConfig
{
    public static class Translator
    {

        private static void AddConditionsToRule(List<ConditionEntry> conditions, RuleEntry rule)
        {
            string text = null;
            int num = 0;
            foreach (ConditionEntry conditionEntry in conditions)
            {
                if (conditionEntry.ErrorInfo != null)
                {
                    text = conditionEntry.ErrorInfo.Message;
                }
                if (conditionEntry.IsOr)
                {
                    num++;
                }
            }
            if (num > 0)
            {
                if (num == conditions.Count || (num == conditions.Count - 1 && !conditions[conditions.Count - 1].IsOr))
                {
                    rule.ConditionsLogicalGrouping = LogicalGrouping.MatchAny;
                }
                else
                {
                    rule.ErrorInfo = new ErrorInfo("This rule was not converted because only some of the conditions are using the OR flag.");
                }
            }
            if (text != null)
            {
                rule.HasErrorInConditions = true;
            }
            if (conditions != null)
            {
                foreach (ConditionEntry item in conditions)
                {
                    rule.Conditions.Add(item);
                }
            }
        }

        public static RewriteEntry Translate(string input, bool isServerLevel, int baseNumber)
        {
            Parser parser = new Parser(input);
            IList<Parser.RawEntry> list = parser.Parse();
            RewriteEntry rewriteEntry = new RewriteEntry(null, isServerLevel);
            List<CommentEntry> list2 = null;
            bool flag = false;
            List<ConditionEntry> list3 = null;
            for (int i = 0; i < list.Count; i++)
            {
                Parser.RawEntry rawEntry = list[i];
                if (rawEntry is Parser.RawCondition)
                {
                    if (list3 == null)
                    {
                        list3 = new List<ConditionEntry>();
                    }
                    ConditionEntry conditionEntry = ConditionLogic.Create((Parser.RawCondition)rawEntry);
                    if (list2 != null)
                    {
                        conditionEntry.Comments.AddRange(list2);
                        list2 = null;
                    }
                    list3.Add(conditionEntry);
                }
                else if (rawEntry is Parser.RawRule)
                {
                    RuleEntry ruleEntry = RuleLogic.Create(rewriteEntry, (Parser.RawRule)rawEntry, isServerLevel);
                    ruleEntry.Name = string.Format(CultureInfo.InvariantCulture, "Imported Rule {0}", rewriteEntry.Rules.Count + baseNumber + 1);
                    if (list3 != null)
                    {
                        AddConditionsToRule(list3, ruleEntry);
                    }
                    list3 = null;
                    if (flag || ruleEntry.IsChained)
                    {
                        ruleEntry.ErrorInfo = new ErrorInfo("This rule was not converted because it is chained to other rules.");
                        flag = ruleEntry.IsChained;
                    }
                    if (list2 != null)
                    {
                        ruleEntry.Comments.AddRange(list2);
                        list2 = null;
                    }
                    rewriteEntry.Rules.Add(ruleEntry);
                }
                else if (rawEntry is Parser.RawComment)
                {
                    if (list2 == null)
                    {
                        list2 = new List<CommentEntry>();
                    }
                    list2.Add(new CommentEntry((Parser.RawComment)rawEntry));
                }
                else if (rawEntry is Parser.RawRuleEngine)
                {
                    TranslateRuleEngine(rewriteEntry, rawEntry);
                }
                else if (rawEntry is Parser.UnsupportedDirective)
                {
                    rewriteEntry.UnsupportedDirectives.Add(new UnsupportedDirectiveEntry((Parser.UnsupportedDirective)rawEntry));
                }
            }
            return rewriteEntry;
        }

        private static void TranslateRuleEngine(RewriteEntry result, Parser.RawEntry entry)
        {
            Parser.RawRuleEngine rawRuleEngine = (Parser.RawRuleEngine)entry;
            if (string.Equals(rawRuleEngine.Value, "On", StringComparison.OrdinalIgnoreCase))
            {
                result.Enabled = true;
            }
            else if (string.Equals(rawRuleEngine.Value, "Off", StringComparison.OrdinalIgnoreCase))
            {
                result.Enabled = false;
            }
            result.Raw = rawRuleEngine;
        }

        internal static class RuleLogic
        {
            internal static RuleEntry Create(RewriteEntry rewrite, Parser.RawRule rawRule, bool isServerLevel)
            {
                RuleEntry ruleEntry = new RuleEntry(rawRule);
                if (rewrite.EnabledSet && !rewrite.Enabled)
                {
                    ruleEntry.Enabled = false;
                }
                ErrorInfo errorInfo = null;
                if (!string.IsNullOrEmpty(rawRule.Pattern))
                {
                    ruleEntry.MatchNegate = rawRule.Pattern.StartsWith("!", StringComparison.Ordinal);
                    if (!ruleEntry.MatchNegate)
                    {
                        ruleEntry.MatchUrl = RuleLogic.SanitizePattern(rawRule.Pattern, isServerLevel, ref errorInfo);
                    }
                    else
                    {
                        ruleEntry.MatchUrl = RuleLogic.SanitizePattern(rawRule.Pattern.Substring(1), isServerLevel, ref errorInfo);
                    }
                }
                ruleEntry.ActionType = ActionType.Rewrite;
                ruleEntry.ActionUrl = RuleLogic.SanitizeActionUrl(rawRule.Substitution, ref errorInfo);
                bool flag = false;
                if (!string.IsNullOrEmpty(rawRule.Flags))
                {
                    string text = rawRule.Flags.Substring(1, rawRule.Flags.Length - 2);
                    string[] array = text.Split(new char[]
                    {
                        ','
                    });
                    for (int i = 0; i < array.Length; i++)
                    {
                        string text2 = array[i];
                        int num = text2.IndexOf('=');
                        if (num != -1)
                        {
                            text2 = text2.Substring(0, num).Trim();
                        }
                        if (RuleLogic.Is(text2, "F") || RuleLogic.Is(text2, "forbidden"))
                        {
                            ruleEntry.ActionType = ActionType.CustomResponse;
                            ruleEntry.StatusCode = 403L;
                            ruleEntry.SubStatusCode = 0L;
                            ruleEntry.StatusReason = "Forbidden";
                            ruleEntry.StatusDescription = "Forbidden";
                        }
                        else if (RuleLogic.Is(text2, "Gone") || RuleLogic.Is(text2, "G"))
                        {
                            ruleEntry.ActionType = ActionType.CustomResponse;
                            ruleEntry.StatusCode = 410L;
                            ruleEntry.SubStatusCode = 0L;
                            ruleEntry.StatusReason = "Gone";
                            ruleEntry.StatusDescription = "The URL is gone";
                        }
                        else if (RuleLogic.Is(text2, "L") || RuleLogic.Is(text2, "last"))
                        {
                            ruleEntry.StopProcessing = true;
                        }
                        else if (RuleLogic.Is(text2, "nocase") || RuleLogic.Is(text2, "nc") || RuleLogic.Is(text2, "I"))
                        {
                            ruleEntry.MatchIgnoreCase = true;
                        }
                        else if (RuleLogic.Is(text2, "Proxy") || RuleLogic.Is(text2, "P"))
                        {
                            flag = true;
                            ruleEntry.ActionType = ActionType.Rewrite;
                            ruleEntry.StopProcessing = true;
                            if (string.IsNullOrEmpty(ruleEntry.ActionUrl))
                            {
                                ruleEntry.ActionUrl = "{R:0}";
                            }
                        }
                        else if (RuleLogic.Is(text2, "QSA") || RuleLogic.Is(text2, "qsappend"))
                        {
                            ruleEntry.ActionAppendQueryString = true;
                        }
                        else if (RuleLogic.Is(text2, "redirect") || RuleLogic.Is(text2, "r") || RuleLogic.Is(text2, "RP"))
                        {
                            flag = true;
                            ruleEntry.ActionType = ActionType.Redirect;
                            if (num != -1)
                            {
                                string value = array[i].Substring(num + 1).Trim();
                                if (RuleLogic.Is(value, "permanent") || RuleLogic.Is(value, "301"))
                                {
                                    ruleEntry.ActionRedirectType = RedirectType.Permanent;
                                }
                                else if (RuleLogic.Is(value, "302") || RuleLogic.Is(value, "Found"))
                                {
                                    ruleEntry.ActionRedirectType = RedirectType.Found;
                                }
                                else if (RuleLogic.Is(value, "303") || RuleLogic.Is(value, "seeother"))
                                {
                                    ruleEntry.ActionRedirectType = RedirectType.SeeOther;
                                }
                                else
                                {
                                    ruleEntry.ActionRedirectType = RedirectType.Temporary;
                                }
                            }
                            if (string.IsNullOrEmpty(ruleEntry.ActionUrl))
                            {
                                ruleEntry.ActionUrl = "{R:0}";
                            }
                        }
                        else if (RuleLogic.Is(text2, "cookie") || RuleLogic.Is(text2, "co") || RuleLogic.Is(text2, "env") || RuleLogic.Is(text2, "e") || RuleLogic.Is(text2, "noescape") || RuleLogic.Is(text2, "ne") || RuleLogic.Is(text2, "nosubreq") || RuleLogic.Is(text2, "ns") || RuleLogic.Is(text2, "type") || RuleLogic.Is(text2, "t"))
                        {
                            errorInfo = new ErrorInfo(string.Format(CultureInfo.InvariantCulture, "The rule cannot be converted into an equivalent IIS format because of unsupported flags: {0}", text2));
                        }
                        else if (RuleLogic.Is(text2, "chain") || RuleLogic.Is(text2, "c"))
                        {
                            ruleEntry.IsChained = true;
                        }
                        else
                        {
                            if (RuleLogic.Is(text2, "skip") || RuleLogic.Is(text2, "s") || RuleLogic.Is(text2, "next") || RuleLogic.Is(text2, "n"))
                            {
                                throw new NotSupportedException("The rule set cannot be converted into an equivalent IIS format because control flow flags (C, S, N) are not supported.");
                            }
                            if (!RuleLogic.Is(text2, "passthrough") && !RuleLogic.Is(text2, "PT"))
                            {
                                errorInfo = new ErrorInfo(string.Format(CultureInfo.InvariantCulture, "The rule contains a control flag that is not recognized: {0}.", text2), true);
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(ruleEntry.ActionUrl) && ruleEntry.ActionType == ActionType.Rewrite)
                {
                    ruleEntry.ActionType = ActionType.None;
                }
                if (!ruleEntry.ActionAppendQueryString && ruleEntry.ActionUrl != null && ruleEntry.ActionUrl.IndexOf('?') != -1)
                {
                    ruleEntry.ActionAppendQueryString = false;
                }
                if (!flag && !string.IsNullOrEmpty(ruleEntry.ActionUrl) && (ruleEntry.ActionUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || ruleEntry.ActionUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                {
                    ruleEntry.ActionType = ActionType.Redirect;
                    ruleEntry.ActionRedirectType = RedirectType.Found;
                }
                if (errorInfo != null)
                {
                    ruleEntry.ErrorInfo = errorInfo;
                }
                return ruleEntry;
            }

            private static bool Is(string value, string compareTo)
            {
                return string.Equals(value, compareTo, StringComparison.OrdinalIgnoreCase);
            }

            internal static string SanitizeActionUrl(string actionUrl, ref ErrorInfo errorInfo)
            {
                if (string.IsNullOrEmpty(actionUrl) || string.Equals(actionUrl, "-", StringComparison.Ordinal))
                {
                    return null;
                }
                string text = RuleLogic.TranslateKnownHeader(actionUrl);
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < actionUrl.Length; i++)
                {
                    char c = actionUrl[i];
                    char c2 = c;
                    switch (c2)
                    {
                        case '$':
                            {
                                if (Helper.TryMatchMapValue(actionUrl, ref i, out string text2, out string text3))
                                {
                                    errorInfo = new ErrorInfo(string.Format(CultureInfo.InvariantCulture, "This directive was not converted because map references are not supported: {0}.", text2));
                                    return null;
                                }
                                if (i + 1 < actionUrl.Length)
                                {
                                    char c3 = actionUrl[i + 1];
                                    if (char.IsDigit(c3))
                                    {
                                        stringBuilder.Append("{R:" + c3 + "}");
                                        i++;
                                    }
                                }
                                break;
                            }
                        case '%':
                            {
                                if (Helper.TryMatchServerVariable(actionUrl, ref i, out string text4, out string text5))
                                {
                                    if (string.Equals(text5, "HTTP:", StringComparison.OrdinalIgnoreCase))
                                    {
                                        stringBuilder.Append("{HTTP_" + text4.Replace('-', '_').Trim().ToUpper(CultureInfo.InvariantCulture) + "}");
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(text5))
                                        {
                                            errorInfo = new ErrorInfo(string.Format(CultureInfo.InvariantCulture, "This rule was not converted because it contains references that are not supported: '{0}:{1}'", text5, text4));
                                            return null;
                                        }
                                        if (string.Compare(text4, "REQUEST_URI", StringComparison.OrdinalIgnoreCase) == 0)
                                        {
                                            text4 = "URL";
                                        }
                                        stringBuilder.Append("{" + text4 + "}");
                                    }
                                }
                                else if (i + 1 < actionUrl.Length && char.IsDigit(actionUrl[i + 1]))
                                {
                                    stringBuilder.Append("{C:" + actionUrl[i + 1] + "}");
                                    i++;
                                }
                                else if (Helper.TryMatchServerVariable2(actionUrl, ref i, out text4))
                                {
                                    stringBuilder.Append("{" + text4 + "}");
                                }
                                else
                                {
                                    stringBuilder.Append(c);
                                }
                                break;
                            }
                        default:
                            if (c2 != '\\')
                            {
                                stringBuilder.Append(c);
                            }
                            else
                            {
                                stringBuilder.Append(c);
                                i++;
                                stringBuilder.Append(actionUrl[i]);
                            }
                            break;
                    }
                }
                return stringBuilder.ToString();
            }

            private static string SanitizePattern(string pattern, bool isServerLevel, ref ErrorInfo errorInfo)
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    return pattern;
                }
                if (isServerLevel)
                {
                    return pattern;
                }
                if (pattern.Length > 1 && pattern[0] == '^' && pattern[1] == '/')
                {
                    if (pattern.Length <= 2)
                    {
                        return "^";
                    }
                    char c = pattern[2];
                    if (c == '?' || c == '*')
                    {
                        return "^" + pattern.Substring(3);
                    }
                    if (char.IsLetterOrDigit(c) || c == '%' || c == '$' || c == '_' || c == '-' || c == '(' || c == ')' || c == '[' || c == ']' || c == '\\')
                    {
                        return '^' + pattern.Substring(2);
                    }
                    errorInfo = new ErrorInfo(string.Format(CultureInfo.InvariantCulture, "This rule contains a pattern that might not work: {0}", pattern), true);
                }
                return pattern;
            }

            private static string TranslateKnownHeader(string actionUrl)
            {
                string[] array = new string[]
                {
                    "Accept-Charset:",
                    "Accept-Encoding:",
                    "Accept-Language:",
                    "Accept:",
                    "Any-Custom-Header:",
                    "Authorization:",
                    "Cookie:",
                    "From:",
                    "Host:",
                    "If-Match:",
                    "If-Modified-Since:",
                    "If-None-Match:",
                    "If-Range:",
                    "If-Unmodified-Since:",
                    "Max-Forwards:",
                    "Proxy-Authorization:",
                    "Range:",
                    "Referer:",
                    "User-Agent:"
                };
                int num = Array.BinarySearch<string>(array, actionUrl, StringComparer.OrdinalIgnoreCase);
                if (num >= 0)
                {
                    string text = array[num].Replace(' ', '_').Replace('-', '_').ToUpperInvariant();
                    text = text.Remove(text.Length - 1);
                    return "{HTTP_" + text + "}";
                }
                if (string.Equals(actionUrl, "URL", StringComparison.OrdinalIgnoreCase))
                {
                    return "{REQUEST_URI}";
                }
                if (string.Equals(actionUrl, "METHOD", StringComparison.OrdinalIgnoreCase))
                {
                    return "{HTTP_METHOD}";
                }
                if (string.Equals(actionUrl, "VERSION", StringComparison.OrdinalIgnoreCase))
                {
                    return "{HTTP_VERSION}";
                }
                return null;
            }
        }

        internal static class ConditionLogic
        {
            internal static ConditionEntry Create(Parser.RawCondition rawCondition)
            {
                ConditionEntry conditionEntry = new ConditionEntry(rawCondition);
                ErrorInfo errorInfo = null;
                conditionEntry.Input = RuleLogic.SanitizeActionUrl(rawCondition.TestString, ref errorInfo);
                if (!string.IsNullOrEmpty(rawCondition.Pattern) && rawCondition.Pattern.Length >= 2)
                {
                    if (rawCondition.Pattern[0] == '!' && rawCondition.Pattern[1] != '=')
                    {
                        conditionEntry.Negate = rawCondition.Pattern.StartsWith("!", StringComparison.Ordinal);
                    }
                    if (!conditionEntry.Negate)
                    {
                        conditionEntry.Pattern = rawCondition.Pattern;
                    }
                    else
                    {
                        conditionEntry.Pattern = rawCondition.Pattern.Substring(1);
                    }
                    if (string.Equals(conditionEntry.Pattern, "-f", StringComparison.Ordinal))
                    {
                        conditionEntry.MatchType = MatchType.IsFile;
                        conditionEntry.Pattern = null;
                    }
                    else if (string.Equals(conditionEntry.Pattern, "-d", StringComparison.Ordinal))
                    {
                        conditionEntry.MatchType = MatchType.IsDirectory;
                        conditionEntry.Pattern = null;
                    }
                    else if (conditionEntry.Pattern.StartsWith("<", StringComparison.Ordinal) || conditionEntry.Pattern.StartsWith(">", StringComparison.Ordinal) || string.Equals(conditionEntry.Pattern, "-s", StringComparison.Ordinal) || string.Equals(conditionEntry.Pattern, "-l", StringComparison.Ordinal) || string.Equals(conditionEntry.Pattern, "-F", StringComparison.Ordinal) || string.Equals(conditionEntry.Pattern, "-U", StringComparison.Ordinal))
                    {
                        errorInfo = new ErrorInfo(string.Format(CultureInfo.InvariantCulture, "The condition pattern is not supported: {0}.", conditionEntry.Pattern));
                    }
                    else if (conditionEntry.Pattern[0] == '!' || conditionEntry.Pattern[0] == '=')
                    {
                        bool flag = conditionEntry.Pattern[0] == '!';
                        if (flag)
                        {
                            conditionEntry.Pattern = '^' + conditionEntry.Pattern.Substring(2) + '$';
                            conditionEntry.Negate = true;
                        }
                        else
                        {
                            conditionEntry.Pattern = '^' + conditionEntry.Pattern.Substring(1) + '$';
                        }
                    }
                }
                if (!string.IsNullOrEmpty(rawCondition.Flags))
                {
                    string text = Helper.SafeSubstring(rawCondition.Flags, 1, rawCondition.Flags.Length - 2);
                    string[] array = text.Split(new char[]
                    {
                        ','
                    });
                    foreach (string a in array)
                    {
                        if (string.Equals(a, "nocase", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "nc", StringComparison.OrdinalIgnoreCase))
                        {
                            conditionEntry.IgnoreCase = true;
                        }
                        else if (string.Equals(a, "or", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "ornext", StringComparison.OrdinalIgnoreCase))
                        {
                            conditionEntry.IsOr = true;
                        }
                    }
                }
                if (errorInfo != null)
                {
                    conditionEntry.ErrorInfo = errorInfo;
                }
                return conditionEntry;
            }
        }
    }

}
