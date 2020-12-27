using System;
using System.Collections.Generic;
using System.Globalization;

namespace WebConfig
{

    internal sealed class Parser
    {
        public Parser(string input)
        {
            _lex = new Lexer(input.ToCharArray());
        }

        private UnsupportedDirective GetUnsupportedDirective(int number)
        {
            UnsupportedDirective unsupportedDirective = new UnsupportedDirective
            {
                StartPosition = _lex.CurrentToken.Position
            };
            for (int i = 0; i < number; i++)
            {
                if (_lex.Peek(1).TokenID == TokenID.StringLiteral)
                {
                    _lex.ReadNextToken();
                }
            }
            unsupportedDirective.EndPosition = _lex.CurrentToken.EndPosition;
            unsupportedDirective.ErrorMessage = string.Format(CultureInfo.InvariantCulture, "This directive was not converted because it is not supported by IIS: {0}.", new string(_lex.Code, unsupportedDirective.StartPosition, unsupportedDirective.EndPosition + 1 - unsupportedDirective.StartPosition));
            return unsupportedDirective;
        }

        internal IList<RawEntry> Parse()
        {
            List<RawEntry> list = new List<RawEntry>();
            while (_lex.ReadNextToken().TokenID != TokenID.EOF)
            {
                switch (_lex.CurrentToken.TokenID)
                {
                    case TokenID.RewriteBase:
                        list.Add(ParseRewriteBase());
                        break;
                    case TokenID.RewriteLock:
                        list.Add(ParseRewriteLock());
                        break;
                    case TokenID.RewriteLog:
                        list.Add(ParseRewriteLog());
                        break;
                    case TokenID.RewriteLogLevel:
                        list.Add(ParseRewriteLogLevel());
                        break;
                    case TokenID.RewriteOptions:
                        list.Add(ParseRewriteOptions());
                        break;
                    case TokenID.RewriteEngine:
                        list.Add(ParseRewriteEngine());
                        break;
                    case TokenID.RewriteRule:
                        list.Add(ParseRewriteRule());
                        break;
                    case TokenID.RewriteCond:
                        list.Add(ParseRewriteCond());
                        break;
                    case TokenID.Comment:
                        list.Add(ParseComment());
                        break;
                }
            }
            return list;
        }

        private RawComment ParseComment()
        {
            return new RawComment
            {
                StartPosition = _lex.CurrentToken.Position,
                Text = _lex.CurrentToken.Text,
                EndPosition = _lex.CurrentToken.EndPosition
            };
        }

        private RawEntry ParseRewriteBase()
        {
            return GetUnsupportedDirective(1);
        }

        private RawCondition ParseRewriteCond()
        {
            RawCondition rawCondition = new RawCondition
            {
                StartPosition = _lex.CurrentToken.Position
            };
            if (_lex.AdvanceIfTokenIs(TokenID.StringLiteral))
            {
                rawCondition.TestString = _lex.CurrentToken.Text;
                if (_lex.AdvanceIfTokenIs(TokenID.StringLiteral))
                {
                    rawCondition.Pattern = _lex.CurrentToken.Text;
                    Token token = _lex.Peek(1);
                    if (token.TokenID == TokenID.StringLiteral && token.Text.StartsWith("[", StringComparison.Ordinal))
                    {
                        _lex.ReadNextToken();
                        rawCondition.Flags = token.Text;
                    }
                }
            }
            rawCondition.EndPosition = _lex.CurrentToken.EndPosition;
            return rawCondition;
        }

        private RawRuleEngine ParseRewriteEngine()
        {
            RawRuleEngine rawRuleEngine = new RawRuleEngine
            {
                StartPosition = _lex.CurrentToken.Position
            };
            if (_lex.AdvanceIfTokenIs(TokenID.StringLiteral))
            {
                rawRuleEngine.Value = _lex.CurrentToken.Text;
            }
            rawRuleEngine.EndPosition = _lex.CurrentToken.EndPosition;
            return rawRuleEngine;
        }

        private RawEntry ParseRewriteLock()
        {
            return GetUnsupportedDirective(1);
        }

        private RawEntry ParseRewriteLog()
        {
            return GetUnsupportedDirective(1);
        }

        private RawEntry ParseRewriteLogLevel()
        {
            return GetUnsupportedDirective(1);
        }

        private RawEntry ParseRewriteOptions()
        {
            return GetUnsupportedDirective(1);
        }

        private RawRule ParseRewriteRule()
        {
            RawRule rawRule = new RawRule
            {
                StartPosition = _lex.CurrentToken.Position
            };
            if (_lex.AdvanceIfTokenIs(TokenID.StringLiteral))
            {
                rawRule.Pattern = _lex.CurrentToken.Text;
                if (_lex.AdvanceIfTokenIs(TokenID.StringLiteral))
                {
                    rawRule.Substitution = _lex.CurrentToken.Text;
                    Token token = _lex.Peek(1);
                    if (token.TokenID == TokenID.StringLiteral && token.Text.StartsWith("[", StringComparison.Ordinal))
                    {
                        _lex.ReadNextToken();
                        rawRule.Flags = token.Text;
                    }
                }
            }
            rawRule.EndPosition = _lex.CurrentToken.EndPosition;
            return rawRule;
        }

        private readonly Lexer _lex;

        internal class RawEntry
        {
            public int StartPosition { get; set; }

            public int EndPosition { get; set; }
        }

        internal class RawComment : RawEntry
        {
            public string Text { get; set; }
        }

        internal class RawRuleEngine : RawEntry
        {
            public string Value { get; set; }
        }

        internal class UnsupportedDirective : RawEntry
        {
            public string ErrorMessage { get; set; }
        }

        internal class RawCondition : RawEntry
        {
            public string TestString { get; set; }

            public string Pattern { get; set; }

            public string Flags
            {
                get
                {
                    return _flags;
                }
                set
                {
                    _flags = value;
                    if (_flags != null && !_flags.EndsWith("]", StringComparison.Ordinal))
                    {
                        _flags += "]";
                    }
                }
            }

            public override string ToString()
            {
                return string.Concat(new string[]
                {
                    "RewriteCond ",
                    TestString,
                    " ",
                    Pattern,
                    " ",
                    Flags
                });
            }

            private string _flags;
        }

        internal class RawRule : RawEntry
        {
            public string Pattern { get; set; }

            public string Substitution { get; set; }

            public string Flags
            {
                get
                {
                    return _flags;
                }
                set
                {
                    _flags = value;
                    if (_flags != null && !_flags.EndsWith("]", StringComparison.Ordinal))
                    {
                        _flags += "]";
                    }
                }
            }

            public override string ToString()
            {
                return string.Concat(new string[]
                {
                    "RewriteRule ",
                    Pattern,
                    " ",
                    Substitution,
                    " ",
                    Flags
                });
            }

            private string _flags;
        }

        private sealed class Lexer
        {
            public Lexer(char[] code)
            {
                LoadKeywords();
                _code = code;
                _atBOL = true;
            }

            public char[] Code
            {
                get
                {
                    return _code;
                }
            }

            public Token CurrentToken
            {
                get
                {
                    return _currentToken;
                }
            }

            public int Length
            {
                get
                {
                    return _code.Length;
                }
            }

            public bool AdvanceIfTokenIs(TokenID token)
            {
                if (Peek(1).TokenID == token)
                {
                    ReadNextToken();
                    return true;
                }
                return false;
            }

            private void LoadKeywords()
            {
                _keywords = new Dictionary<string, TokenID>(StringComparer.OrdinalIgnoreCase);
                for (int i = 1; i < 9; i++)
                {
                    TokenID tokenID = (TokenID)i;
                    string key = tokenID.ToString();
                    _keywords[key] = tokenID;
                }
            }

            public Token Peek(int numberOfTokens)
            {
                if (numberOfTokens <= 0)
                {
                    return _currentToken;
                }
                Token token = null;
                if (_lookAheadTokens.Count == 1)
                {
                    return _lookAheadTokens.Peek();
                }
                for (int i = 1; i <= numberOfTokens; i++)
                {
                    token = ScanToken();
                    _lookAheadTokens.Enqueue(token);
                }
                return token;
            }

            public Token ReadNextToken()
            {
                if (_lookAheadTokens.Count > 0)
                {
                    _currentToken = _lookAheadTokens.Dequeue();
                }
                else
                {
                    _currentToken = ScanToken();
                }
                return _currentToken;
            }

            private Token ScanToken()
            {
                Token token = new Token();
                while (token.TokenID == TokenID.Invalid)
                {
                    if (Length <= _curPos)
                    {
                        token.TokenID = TokenID.EOF;
                        break;
                    }
                    token.Position = _curPos;
                    char c = _code[_curPos++];
                    char c2 = c;
                    switch (c2)
                    {
                        case '\t':
                            break;
                        case '\n':
                            _atBOL = true;
                            continue;
                        case '\v':
                        case '\f':
                            goto IL_198;
                        case '\r':
                            if (_curPos < Length && _code[_curPos] == '\n')
                            {
                                _curPos++;
                            }
                            _atBOL = true;
                            continue;
                        default:
                            if (c2 != ' ')
                            {
                                if (c2 != '#')
                                {
                                    goto IL_198;
                                }
                                if (_atBOL)
                                {
                                    token.TokenID = TokenID.Comment;
                                    while (_curPos < Length && _code[_curPos] != '\n' && _code[_curPos] != '\r')
                                    {
                                        _curPos++;
                                    }
                                    int num = _curPos - token.Position;
                                    if (num < 0)
                                    {
                                        num = 0;
                                    }
                                    token.Text = new string(_code, token.Position, num);
                                    _atBOL = true;
                                    continue;
                                }
                                continue;
                            }
                            break;
                    }
                    while (_curPos < Length)
                    {
                        if (_code[_curPos] != ' ' && _code[_curPos] != '\t')
                        {
                            break;
                        }
                        _curPos++;
                    }
                    continue;
                IL_198:
                    int num2 = 1;
                    int startIndex = _curPos - 1;
                    while (_curPos < Length && _code[_curPos] != '\n' && _code[_curPos] != '\r' && _code[_curPos] != ' ' && _code[_curPos] != '\t' && _code[_curPos] != '#')
                    {
                        if (_code[_curPos] == '\\' && _curPos + 1 < Length && _code[_curPos + 1] == ' ')
                        {
                            _curPos++;
                            num2++;
                        }
                        _curPos++;
                        num2++;
                    }
                    string text = new string(_code, startIndex, num2);
                    token.Text = text;
                    if (_keywords.TryGetValue(text, out TokenID tokenID))
                    {
                        token.TokenID = tokenID;
                    }
                    else
                    {
                        token.TokenID = TokenID.StringLiteral;
                    }
                }
                token.EndPosition = _curPos - 1;
                return token;
            }

            private Dictionary<string, TokenID> _keywords;

            private int _curPos;

            private readonly char[] _code;

            private bool _atBOL;

            private readonly Queue<Token> _lookAheadTokens = new Queue<Token>();

            private Token _currentToken;
        }

        private sealed class Token
        {
            internal TokenID TokenID { get; set; }

            internal string Text { get; set; }

            internal int Position { get; set; }

            internal int EndPosition { get; set; }
        }

        private enum TokenID
        {
            Invalid,
            RewriteBase,
            RewriteLock,
            RewriteLog,
            RewriteLogLevel,
            RewriteOptions,
            RewriteEngine,
            RewriteRule,
            RewriteCond,
            StringLiteral,
            Comment,
            EOF
        }
    }

}
