namespace TemplateProcessor
{
    using static TemplateProcessor.Messages.Messages;

    public class TextLineParserTests
    {
        private static readonly int _lineNumber;
        private static readonly string _segmentName;

        private readonly TestHelper _testHelper = new()
        {
            ResetLogger = true,
            ResetNameGenerator = true,
            ResetTokenProcessor = true
        };

        static TextLineParserTests()
        {
            _segmentName = "Segment1";
            _lineNumber = 1;
            Locater.CurrentSegment = _segmentName;
            Locater.LineNumber = _lineNumber;
        }

        public TextLineParserTests() => _testHelper.Reset();

        [Theory]
        [InlineData("##  Segment1")]
        [InlineData(" ### Segment1")]
        [InlineData("##1 Segment1")]
        public void IsSegmentHeader_LineDoesNotStartWithSegmentCode_ReturnsFalse(string text)
        {
            // Act
            bool actual = TextLineParser.IsSegmentHeader(text);

            // Assert
            AssertExpectedResults(false, actual);
        }

        [Fact]
        public void IsSegmentHeader_LineStartsWithSegmentCode_ReturnsTrue()
        {
            // Act
            bool actual = TextLineParser.IsSegmentHeader("### Segment1");

            // Assert
            AssertExpectedResults(true, actual);
        }

        [Fact]
        public void IsValidPrefix_FourthCharacterIsNotBlank_LogsAnErrorAndReturnsFalse()
        {
            // Arrange
            string expectedMsg = MsgFourthCharacterMustBeBlank;
            LogEntry expectedLogEntry = new(LogEntryType.Parsing, _segmentName, _lineNumber, expectedMsg);

            // Act
            bool actual = TextLineParser.IsValidPrefix("   X");

            // Assert
            AssertExpectedResults(false, actual, expectedLogEntry);
        }

        [Theory]
        [InlineData(".")]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("O")]
        public void IsValidPrefix_IndentValueIsNotValidNumber_LogsAnErrorAndReturnsFalse(string value)
        {
            // Arrange
            string text = $"@+{value} Line 1";
            string expectedMsg = string.Format(MsgIndentValueMustBeValidNumber, $"+{value}");
            LogEntry expectedLogEntry = new(LogEntryType.Parsing, _segmentName, _lineNumber, expectedMsg);

            // Act
            bool actual = TextLineParser.IsValidPrefix(text);

            // Assert
            AssertExpectedResults(false, actual, expectedLogEntry);
        }

        [Theory]
        [InlineData(" +1")]
        [InlineData(" -1")]
        [InlineData(" =1")]
        [InlineData("## ")]
        [InlineData("@ 1")]
        [InlineData("O 1")]
        public void IsValidPrefix_LineHasInvalidControlCode_LogsAnErrorAndReturnsFalse(string controlCode)
        {
            // Arrange
            string text = $"{controlCode} Line 1";
            string expectedMsg = string.Format(MsgInvalidControlCode, controlCode);
            LogEntry expectedLogEntry = new(LogEntryType.Parsing, _segmentName, _lineNumber, expectedMsg);

            // Act
            bool actual = TextLineParser.IsValidPrefix(text);

            // Assert
            AssertExpectedResults(false, actual, expectedLogEntry);
        }

        [Theory]
        [InlineData("###", " Segment1")]
        [InlineData("   ", " Line 1")]
        [InlineData("@=1", "")]
        [InlineData("O=1", " ")]
        [InlineData("O-2", " Line 1")]
        [InlineData("@-2", "")]
        [InlineData("O+3", " ")]
        [InlineData("@+3", " Line 1")]
        public void IsValidPrefix_LineHasValidPrefix_ReturnsTrue(string prefix, string suffix)
        {
            // Arrange
            string text = $"{prefix}{suffix}";

            // Act
            bool actual = TextLineParser.IsValidPrefix(text);

            // Assert
            AssertExpectedResults(true, actual);
        }

        [Fact]
        public void IsValidPrefix_LineLengthIsLessThanThree_LogsAnErrorAndReturnsFalse()
        {
            // Arrange
            string expectedMsg = MsgMinimumLineLengthInTemplateFileIs3;
            LogEntry expectedLogEntry = new(LogEntryType.Parsing, _segmentName, _lineNumber, expectedMsg);

            // Act
            bool actual = TextLineParser.IsValidPrefix("  ");

            // Assert
            AssertExpectedResults(false, actual, expectedLogEntry);
        }

        [Fact]
        public void ParseTextLine_LineContainsTokens_ExtractsTokensAndReturnsTextItem()
        {
            // Arrange
            string token1 = "Token1";
            string token2 = "Token2";
            string text = $"    Line 1 <#={token1}#> and <#={token2}#>.";
            string textLine = $"    {text}";
            TextItem expectedTextItem = new(0, true, false, text);
            Dictionary<string, string> expectedTokens = new()
            {
                { token1, string.Empty },
                { token2, string.Empty }
            };

            // Act
            TextItem actualTextItem = TextLineParser.ParseTextLine(textLine);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            TokenProcessor._tokenDictionary
                .Should()
                .BeEquivalentTo(expectedTokens);
            actualTextItem
                .Should()
                .Be(expectedTextItem);
        }

        [Theory]
        [InlineData("    ", "Line 1", 0, true, false)]
        [InlineData("@+1 ", "", 1, true, false)]
        [InlineData("O+1 ", "Line 1", 1, true, true)]
        [InlineData("@-1 ", "Line 1", -1, true, false)]
        [InlineData("O-1", "", -1, true, true)]
        [InlineData("@=1 ", "Line 1", 1, false, false)]
        [InlineData("O=1 ", "", 1, false, true)]
        public void ParseTextLine_LineHasValidPrefix_ReturnsTextItem(string prefix, string text, int indent, bool isRelative, bool isOneTime)
        {
            // Arrange
            string textLine = $"{prefix}{text}";
            TextItem expectedTextItem = new(indent, isRelative, isOneTime, text);

            // Act
            TextItem actualTextItem = TextLineParser.ParseTextLine(textLine);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            actualTextItem
                .Should()
                .Be(expectedTextItem);
        }

        private static void AssertExpectedResults(bool expected, bool actual, LogEntry? expectedLogEntry = null)
        {
            if (expectedLogEntry is null)
            {
                ConsoleLogValidater.AssertLogContents(0);
            }
            else
            {
                List<LogEntry> expectedLogEntries = new()
                {
                    expectedLogEntry
                };
                ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            }

            actual
                .Should()
                .Be(expected);
        }
    }
}