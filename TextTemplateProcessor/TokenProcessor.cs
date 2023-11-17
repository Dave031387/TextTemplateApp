namespace TemplateProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TemplateProcessor.Logger;
    using static TemplateProcessor.Messages.Messages;

    /// <include file="docs.xml" path="docs/members[@name=&quot;tokenprocessor&quot;]/TokenProcessor/*"/>
    internal static class TokenProcessor
    {
        /// <include file="docs.xml" path="docs/members[@name=&quot;tokenprocessor&quot;]/_tokenDictionary/*"/>
        internal static readonly Dictionary<string, string> _tokenDictionary;

        private const string TokenEnd = "#>";
        private const string TokenStart = "<#=";

        static TokenProcessor() => _tokenDictionary = new();

        /// <include file="docs.xml" path="docs/members[@name=&quot;tokenprocessor&quot;]/ClearTokens/*"/>
        internal static void ClearTokens() => _tokenDictionary.Clear();

        /// <include file="docs.xml" path="docs/members[@name=&quot;tokenprocessor&quot;]/ExtractTokens/*"/>
        internal static void ExtractTokens(ref string text)
        {
            int startIndex = 0;

            while (startIndex < text.Length - 1)
            {
                (string token, string tokenName) = FindToken(ref startIndex, ref text);

                if (string.IsNullOrEmpty(token))
                {
                    continue;
                }

                if (!_tokenDictionary.ContainsKey(tokenName))
                {
                    _tokenDictionary.Add(tokenName, string.Empty);
                }
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;tokenprocessor&quot;]/LoadTokenValues/*"/>
        internal static void LoadTokenValues(Dictionary<string, string> tokenDictionary)
        {
            if (tokenDictionary is not null)
            {
                if (tokenDictionary.Count > 0)
                {
                    foreach (string tokenName in tokenDictionary.Keys)
                    {
                        string tokenValue = tokenDictionary[tokenName];

                        UpdateTokenDictionary(tokenName, tokenValue);
                    }
                }
                else
                {
                    ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgTokenDictionaryIsEmpty, Locater.CurrentSegment);
                }
            }
            else
            {
                ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgTokenDictionaryIsNull, Locater.CurrentSegment);
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;tokenprocessor&quot;]/ReplaceTokens/*"/>
        internal static string ReplaceTokens(string text)
        {
            StringBuilder builder = new(text);
            int startIndex = 0;

            while (startIndex < text.Length)
            {
                (string token, string tokenName) = FindToken(ref startIndex, ref text);

                if (string.IsNullOrEmpty(tokenName))
                {
                    break;
                }

                if (string.IsNullOrEmpty(_tokenDictionary[tokenName]))
                {
                    ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgTokenValueIsEmpty, Locater.CurrentSegment, tokenName);
                }

                _ = builder.Replace(token, _tokenDictionary[tokenName]);
            }

            builder = builder.Replace($@"\{TokenStart}", TokenStart);
            return builder.ToString();
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;tokenprocessor&quot;]/ResetTokens/*"/>
        internal static void ResetTokens()
        {
            foreach (string tokenName in _tokenDictionary.Keys)
            {
                _tokenDictionary[tokenName] = string.Empty;
            }
        }

        private static (string token, string tokenName) ExtractToken(int tokenStart, ref int tokenEnd, ref string text)
        {
            int tokenNameStart = tokenStart + TokenStart.Length;
            int tokenNameEnd = tokenEnd;
            tokenEnd += TokenEnd.Length;
            string token = text[tokenStart..tokenEnd];
            string tokenName = text[tokenNameStart..tokenNameEnd].Trim();
            bool isValidToken = true;

            if (string.IsNullOrWhiteSpace(tokenName))
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgMissingTokenName);
                isValidToken = false;
            }
            else if (!NameValidater.IsValidName(tokenName))
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgTokenHasInvalidName, tokenName);
                isValidToken = false;
            }

            if (!isValidToken)
            {
                text = InsertEscapeCharacter(tokenStart, text);
                tokenEnd++;
                token = string.Empty;
                tokenName = string.Empty;
            }

            return (token, tokenName);
        }

        private static (string token, string tokenName) FindToken(ref int startIndex, ref string text)
        {
            (string token, string tokenName) result = (string.Empty, string.Empty);

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            while (startIndex < text.Length
                && string.IsNullOrEmpty(result.token))
            {
                (bool isValidTokenStart, int tokenStart) = LocateTokenStartDelimiter(startIndex, text);

                if (!isValidTokenStart)
                {
                    startIndex = tokenStart;
                    continue;
                }

                (bool isValidTokenEnd, int tokenEnd) = LocateTokenEndDelimiter(tokenStart, ref text);

                if (!isValidTokenEnd)
                {
                    startIndex = tokenEnd;
                    break;
                }

                result = ExtractToken(tokenStart, ref tokenEnd, ref text);
                startIndex = tokenEnd;
            }

            return result;
        }

        private static string InsertEscapeCharacter(int tokenStart, string text) => text.Insert(tokenStart, @"\");

        private static (bool isValidTokenEnd, int tokenEnd) LocateTokenEndDelimiter(int tokenStart, ref string text)
        {
            int tokenEnd = text.IndexOf(TokenEnd, tokenStart, StringComparison.Ordinal);

            if (tokenEnd < 0)
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgTokenMissingEndDelimiter);
                text = InsertEscapeCharacter(tokenStart, text);
                return (false, text.Length);
            }

            return (true, tokenEnd);
        }

        private static (bool isValidTokenStart, int newIndexValue) LocateTokenStartDelimiter(int startIndex, string text)
        {
            int tokenStart = text.IndexOf(TokenStart, startIndex, StringComparison.Ordinal);

            return tokenStart < 0
                ? ((bool isValidTokenStart, int newIndexValue))(false, text.Length)
                : tokenStart > 0 && text[tokenStart - 1] == '\\' ? ((bool isValidTokenStart, int newIndexValue))(false, tokenStart + TokenStart.Length) : ((bool isValidTokenStart, int newIndexValue))(true, tokenStart);
        }

        private static void UpdateTokenDictionary(string tokenName, string tokenValue)
        {
            if (NameValidater.IsValidName(tokenName))
            {
                if (_tokenDictionary.ContainsKey(tokenName))
                {
                    if (tokenValue is null)
                    {
                        ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgTokenWithNullValue, Locater.CurrentSegment, tokenName);
                        tokenValue = string.Empty;
                    }
                    else if (string.IsNullOrEmpty(tokenValue))
                    {
                        ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgTokenWithEmptyValue, Locater.CurrentSegment, tokenName);
                    }

                    _tokenDictionary[tokenName] = tokenValue;
                }
                else
                {
                    ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgUnknownTokenName, Locater.CurrentSegment, tokenName);
                }
            }
            else
            {
                ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgTokenDictionaryContainsInvalidTokenName, Locater.CurrentSegment, tokenName);
            }
        }
    }
}