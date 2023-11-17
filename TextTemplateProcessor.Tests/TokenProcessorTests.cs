namespace TemplateProcessor
{
    using static TemplateProcessor.Messages.Messages;

    public class TokenProcessorTests
    {
        private static readonly int _lineNumber;
        private static readonly string _segmentName;

        private readonly TestHelper _testHelper = new()
        {
            ResetLogger = true,
            ResetTokenProcessor = true
        };

        static TokenProcessorTests()
        {
            _segmentName = "Segment1";
            _lineNumber = 1;
            Locater.CurrentSegment = _segmentName;
            Locater.LineNumber = _lineNumber;
        }

        public TokenProcessorTests() => _testHelper.Reset();

        [Fact]
        public void ExtractTokens_LineContainsEscapedToken_TokenDictionaryShouldBeEmpty()
        {
            // Arrange
            string text = $@"Line1 \{Token("Token")}";

            // Act
            TokenProcessor.ExtractTokens(ref text);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTokenNames();
        }

        [Fact]
        public void ExtractTokens_LineContainsNormalTokenAfterEscapedToken_NormalTokenGetsAddedToDictionary()
        {
            // Arrange
            string tokenName = "Normal";
            string text = $@"Line 1 \{Token("Escaped")}{Token(tokenName)} end";

            // Act
            TokenProcessor.ExtractTokens(ref text);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTokenNames(tokenName);
        }

        [Fact]
        public void ExtractTokens_LineContainsNoTokens_TokenDictionaryShouldBeEmpty()
        {
            // Arrange
            string text = "Line1";

            // Act
            TokenProcessor.ExtractTokens(ref text);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTokenNames();
        }

        [Theory]
        [InlineData("Before", "After")]
        [InlineData("Text", "")]
        [InlineData("", "Text")]
        [InlineData("", "")]
        public void ExtractTokens_LineContainsToken_TokenGetsAddedToDictionary(string prefix, string suffix)
        {
            // Arrange
            string tokenName = "Token1";
            string text = $@"{prefix}{Token(tokenName)}{suffix}";

            // Act
            TokenProcessor.ExtractTokens(ref text);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTokenNames(tokenName);
        }

        [Theory]
        [InlineData("A ", " B ", " C")]
        [InlineData("A ", " B ", "")]
        [InlineData("A ", "", " C")]
        [InlineData("A ", "", "")]
        [InlineData("", " B ", " C")]
        [InlineData("", " B ", "")]
        [InlineData("", "", " C")]
        [InlineData("", "", "")]
        public void ExtractTokens_LineHasMoreThanOneToken_AllTokensAreAddedToDictionary(string part1, string part2, string part3)
        {
            // Arrange
            string tokenName1 = "Token1";
            string tokenName2 = "Token2";
            string text = $"{part1}{Token(tokenName1)}{part2}{Token(tokenName2)}{part3}";

            // Act
            TokenProcessor.ExtractTokens(ref text);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTokenNames(tokenName1, tokenName2);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("1Token")]
        public void ExtractTokens_LineHasValidTokenAfterInvalidToken_ValidTokenIsAddedToDictionary(string badToken)
        {
            // Arrange
            string goodToken = "Token1";
            string actualText = $"Line 1 {Token(badToken)}{Token(goodToken)}";
            string expectedText = $@"Line 1 \{Token(badToken)}{Token(goodToken)}";

            // Act
            TokenProcessor.ExtractTokens(ref actualText);

            // Assert
            ConsoleLogValidater.AssertLogContents(1);
            AssertTextLine(expectedText, actualText);
            AssertTokenNames(goodToken);
        }

        [Theory]
        [InlineData("", " ")]
        [InlineData("", "  ")]
        [InlineData(" ", "")]
        [InlineData("  ", "")]
        [InlineData(" ", " ")]
        public void ExtractTokens_TokenContainsEmbeddedWhitespace_TrimsWhitespaceFromToken(string before, string after)
        {
            // Arrange
            string tokenName = "Token";
            string paddedName = $"{before}{tokenName}{after}";
            string text = $@"Line 1 {Token(paddedName)}";

            // Act
            TokenProcessor.ExtractTokens(ref text);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTokenNames(tokenName);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("")]
        public void ExtractTokens_TokenFoundWithNoName_LogsAnErrorAndEscapesTheToken(string value)
        {
            // Arrange
            string actualText = $"Line 1 {Token(value)}.";
            string expectedText = $@"Line 1 \{Token(value)}.";
            string expectedMsg = MsgMissingTokenName;
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, _segmentName, _lineNumber, expectedMsg)
            };

            // Act
            TokenProcessor.ExtractTokens(ref actualText);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertTextLine(expectedText, actualText);
            AssertTokenNames();
        }

        [Theory]
        [InlineData("1Token")]
        [InlineData("Token 1")]
        [InlineData("_token")]
        public void ExtractTokens_TokenHasInvalidName_LogsAnErrorAndEscapesTheToken(string tokenName)
        {
            // Arrange
            string actualText = $"Line 1 {Token(tokenName)} end.";
            string expectedText = $@"Line 1 \{Token(tokenName)} end.";
            string expectedMsg = string.Format(MsgTokenHasInvalidName, tokenName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, _segmentName, _lineNumber, expectedMsg)
            };

            // Act
            TokenProcessor.ExtractTokens(ref actualText);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertTextLine(expectedText, actualText);
            AssertTokenNames();
        }

        [Fact]
        public void ExtractTokens_TokenIsMissingEndDelimiter_LogsAnErrorAndEscapesTheToken()
        {
            // Arrange
            string token = "<#=Token";
            string actualText = $"Line1 {token} ";
            string expectedText = $@"Line1 \{token} ";
            string expectedMsg = MsgTokenMissingEndDelimiter;
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, _segmentName, _lineNumber, expectedMsg)
            };

            // Act
            TokenProcessor.ExtractTokens(ref actualText);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertTextLine(expectedText, actualText);
            AssertTokenNames();
        }

        [Fact]
        public void LoadTokenValues_DictionaryContainsSeveralTokens_LoadsAllTokenValues()
        {
            // Arrange
            string tokenName1 = "Token1";
            string tokenValue1 = "old value 1";
            string expectedValue1 = "new value 1";
            string tokenName2 = "Token2";
            string tokenValue2 = "old value 2";
            string expectedValue2 = "new value 2";
            string tokenName3 = "Token3";
            string tokenValue3 = "old value 3";
            string expectedValue3 = "new value 3";
            Dictionary<string, string> tokenDictionary = new()
            {
                { tokenName1, expectedValue1 },
                { tokenName2, expectedValue2 },
                { tokenName3, expectedValue3 }
            };
            LoadTokenValues(
                (tokenName1, tokenValue1),
                (tokenName2, tokenValue2),
                (tokenName3, tokenValue3));

            // Act
            TokenProcessor.LoadTokenValues(tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTokenValues(
                (tokenName1, expectedValue1),
                (tokenName2, expectedValue2),
                (tokenName3, expectedValue3));
        }

        [Fact]
        public void LoadTokenValues_TokenDictionaryContainsInvalidName_LogsAnError()
        {
            // Arrange
            string badTokenName = "1Token";
            Dictionary<string, string> tokenDictionary = new()
            {
                { badTokenName, "value1" }
            };
            string expectedMsg = string.Format(MsgTokenDictionaryContainsInvalidTokenName, _segmentName, badTokenName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, _segmentName, _lineNumber, expectedMsg)
            };

            // Act
            TokenProcessor.LoadTokenValues(tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
        }

        [Fact]
        public void LoadTokenValues_TokenDictionaryContainsTokenWithEmptyValue_LogsAnErrorAndSetsValue()
        {
            // Arrange
            string tokenName = "Token1";
            string tokenValue = "value";
            string expectedValue = string.Empty;
            Dictionary<string, string> tokenDictionary = new()
            {
                { tokenName, string.Empty }
            };
            string expectedMsg = string.Format(MsgTokenWithEmptyValue, _segmentName, tokenName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, _segmentName, _lineNumber, expectedMsg)
            };
            LoadTokenValues((tokenName, tokenValue));

            // Act
            TokenProcessor.LoadTokenValues(tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertTokenValues((tokenName, expectedValue));
        }

        [Fact]
        public void LoadTokenValues_TokenDictionaryContainsTokenWithNullValue_LogsAnErrorAndSetsValueToEmpty()
        {
            // Arrange
            string tokenName = "Token1";
            string tokenValue = "value";
            string expectedValue = string.Empty;
            Dictionary<string, string> tokenDictionary = new()
            {
                { tokenName, null! }
            };
            string expectedMsg = string.Format(MsgTokenWithNullValue, _segmentName, tokenName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, _segmentName, _lineNumber, expectedMsg)
            };
            LoadTokenValues((tokenName, tokenValue));

            // Act
            TokenProcessor.LoadTokenValues(tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertTokenValues((tokenName, expectedValue));
        }

        [Fact]
        public void LoadTokenValues_TokenDictionaryContainsUnknownName_LogsAnError()
        {
            // Arrange
            string tokenName = "Token1";
            Dictionary<string, string> tokenDictionary = new()
            {
                { tokenName, "value" }
            };
            string expectedMsg = string.Format(MsgUnknownTokenName, _segmentName, tokenName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, _segmentName, _lineNumber, expectedMsg)
            };

            // Act
            TokenProcessor.LoadTokenValues(tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertTokenNames();
        }

        [Fact]
        public void LoadTokenValues_TokenDictionaryIsEmpty_LogsAnError()
        {
            // Arrange
            string expectedMsg = string.Format(MsgTokenDictionaryIsEmpty, _segmentName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, _segmentName, _lineNumber, expectedMsg)
            };

            // Act
            TokenProcessor.LoadTokenValues(new Dictionary<string, string>());

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
        }

        [Fact]
        public void LoadTokenValues_TokenDictionaryIsNull_LogsAnError()
        {
            // Arrange
            string expectedMsg = string.Format(MsgTokenDictionaryIsNull, _segmentName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, _segmentName, _lineNumber, expectedMsg)
            };

            // Act
            TokenProcessor.LoadTokenValues(null!);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
        }

        [Fact]
        public void LoadTokenValues_ValidTokenDictionary_LoadsTokenValues()
        {
            // Arrange
            string tokenName = "Token1";
            string tokenValue = "oldValue";
            string expectedValue = "new value";
            Dictionary<string, string> tokenDictionary = new()
            {
                { tokenName, expectedValue }
            };
            LoadTokenValues((tokenName, tokenValue));

            // Act
            TokenProcessor.LoadTokenValues(tokenDictionary);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTokenValues((tokenName, expectedValue));
        }

        [Fact]
        public void ReplaceTokens_LineContainsEscapedTokenAndValidToken_RemovesEscapeCharacterAndReplacesValidTokenWithValue()
        {
            // Arrange
            string escapedTokenName = "Escaped";
            string tokenName = "Valid";
            string tokenValue = "good";
            string originalText = $@"\{Token(escapedTokenName)} Line {Token(tokenName)}";
            string expectedText = $"{Token(escapedTokenName)} Line {tokenValue}";
            LoadTokenValues((tokenName, tokenValue));

            // Act
            string actualText = TokenProcessor.ReplaceTokens(originalText);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTextLine(expectedText, actualText);
        }

        [Fact]
        public void ReplaceTokens_LineContainsSeveralTokens_ReplacesAllTokensWithTokenValues()
        {
            // Arrange
            string tokenName1 = "Token1";
            string tokenValue1 = "Hello";
            string tokenName2 = "Token2";
            string tokenValue2 = "World";
            string tokenName3 = "Token3";
            string tokenValue3 = "!";
            string originalText = $"{Token(tokenName1)} {Token(tokenName2)}{Token(tokenName3)}";
            string expectedText = $"{tokenValue1} {tokenValue2}{tokenValue3}";
            LoadTokenValues(
                (tokenName1, tokenValue1),
                (tokenName2, tokenValue2),
                (tokenName3, tokenValue3));

            // Act
            string actualText = TokenProcessor.ReplaceTokens(originalText);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTextLine(expectedText, actualText);
        }

        [Fact]
        public void ReplaceTokens_LineContainsValidToken_ReplacesTokenWithTokenValue()
        {
            // Arrange
            string tokenName = "Token1";
            string tokenValue = "World";
            string originalText = $"Hello {Token(tokenName)}!";
            string expectedText = $"Hello {tokenValue}!";
            LoadTokenValues((tokenName, tokenValue));

            // Act
            string actualText = TokenProcessor.ReplaceTokens(originalText);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTextLine(expectedText, actualText);
        }

        [Theory]
        [InlineData(@"Line 1 \<#=Token1#>", "Line 1 <#=Token1#>")]
        [InlineData(@"Line 2 \<#=Token1#>\<#=Token2#>", "Line 2 <#=Token1#><#=Token2#>")]
        [InlineData(@"Line 3 \<#= \<#=Token1#>", "Line 3 <#= <#=Token1#>")]
        public void ReplaceTokens_LineHasEscapedTokens_RemovesEscapeCharacters(string originalText, string expectedText)
        {
            // Act
            string actualText = TokenProcessor.ReplaceTokens(originalText);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTextLine(expectedText, actualText);
        }

        [Fact]
        public void ReplaceTokens_LineHasNoTokens_ReturnsLineUnchanged()
        {
            // Arrange
            string expectedText = "Line 1";

            // Act
            string actualText = TokenProcessor.ReplaceTokens(expectedText);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            AssertTextLine(expectedText, actualText);
        }

        [Fact]
        public void ReplaceTokens_TokenValueIsEmptyString_LogsAnErrorAndReplacesTokenWithEmptyString()
        {
            // Arrange
            string tokenName1 = "Token1";
            string tokenValue1 = "Hello";
            string tokenName2 = "Token2";
            string tokenValue2 = "";
            string tokenName3 = "Token3";
            string tokenValue3 = "!";
            string originalText = $"{Token(tokenName1)} {Token(tokenName2)}{Token(tokenName3)}";
            string expectedText = $"{tokenValue1} {tokenValue3}";
            LoadTokenValues(
                (tokenName1, tokenValue1),
                (tokenName2, tokenValue2),
                (tokenName3, tokenValue3));
            string expectedMsg = string.Format(MsgTokenValueIsEmpty, _segmentName, tokenName2);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, _segmentName, _lineNumber, expectedMsg)
            };

            // Act
            string actualText = TokenProcessor.ReplaceTokens(originalText);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            AssertTextLine(expectedText, actualText);
        }

        private static void AssertTextLine(string expectedText, string actualText) => actualText.Should().Be(expectedText);

        private static void AssertTokenNames(params string[]? tokenNames)
        {
            if (tokenNames is null)
            {
                TokenProcessor._tokenDictionary
                    .Should()
                    .BeEmpty();
            }
            else
            {
                foreach (string tokenName in tokenNames)
                {
                    TokenProcessor._tokenDictionary
                        .Should()
                        .HaveCount(tokenNames.Length);
                    TokenProcessor._tokenDictionary
                        .Should()
                        .ContainKey(tokenName);
                }
            }
        }

        private static void AssertTokenValues(params (string tokenName, string expectedValue)[] tokenValueList)
        {
            if (tokenValueList is null)
            {
                Assert.Fail("Token value assertion failed because no token values were given.");
            }
            else
            {
                foreach ((string tokenName, string expectedValue) in tokenValueList)
                {
                    TokenProcessor._tokenDictionary[tokenName]
                        .Should()
                        .Be(expectedValue);
                }
            }
        }

        private static void LoadTokenValues(params (string tokenName, string tokenValue)[] tokenValueList)
        {
            if (tokenValueList == null)
            {
                Assert.Fail("There were no token name/value pairs passed into the LoadTokenValues method.");
            }
            else
            {
                foreach ((string tokenName, string tokenValue) in tokenValueList)
                {
                    TokenProcessor._tokenDictionary[tokenName] = tokenValue;
                }
            }
        }

        private static string Token(string tokenName) => $"<#={tokenName}#>";
    }
}