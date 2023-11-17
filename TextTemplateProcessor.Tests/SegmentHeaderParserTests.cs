namespace TemplateProcessor
{
    using static Globals;
    using static TemplateProcessor.Messages.Messages;

    public class SegmentHeaderParserTests
    {
        private const int ExpectedLineNumber = 0;
        private const string ExpectedSegmentName = "Segment1";
        private const string SegmentCode = "###";

        private readonly ControlItem _defaultControlItem = new()
        {
            FirstTimeIndent = 0,
            IsFirstTime = true,
            PadSegment = null
        };

        private readonly TestHelper _testHelper = new()
        {
            ResetLocater = true,
            ResetLogger = true,
            ResetNameGenerator = true
        };

        public SegmentHeaderParserTests() => _testHelper.Reset();

        [Theory]
        [InlineData("one")]
        [InlineData("2.3")]
        [InlineData(".")]
        [InlineData("1five")]
        public void ParseSegmentHeader_FirstTimeIndentIsNotAValidNumber_LogsAnErrorAndIgnoresTheOption(string indentValue)
        {
            // Arrange
            string text = $"{SegmentCode} {ExpectedSegmentName} FTI={indentValue}";
            string expectedMsg = string.Format(MsgIndentValueMustBeValidNumber, indentValue);
            LogEntry expectedLogEntry = FormatLogEntry(ExpectedSegmentName, expectedMsg);

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                ExpectedSegmentName,
                expectedLogEntry);
        }

        [Theory]
        [InlineData("-11")]
        [InlineData("-10")]
        [InlineData("10")]
        [InlineData("11")]
        public void ParseSegmentHeader_FirstTimeIndentIsOutOfRange_LogsAnErrorAndIgnoresTheOption(string indentValue)
        {
            // Arrange
            string text = $"{SegmentCode} {ExpectedSegmentName} FTI={indentValue}";
            string expectedMsg = string.Format(MsgIndentValueOutOfRange, indentValue);
            LogEntry expectedLogEntry = FormatLogEntry(ExpectedSegmentName, expectedMsg);

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                ExpectedSegmentName,
                expectedLogEntry);
        }

        [Fact]
        public void ParseSegmentHeader_FirstTimeIndentSetToZero_LogsAnErrorButSavesTheValue()
        {
            // Arrange
            string text = $"{SegmentCode} {ExpectedSegmentName} FTI=0";
            string expectedMsg = MsgFirstTimeIndentSetToZero;
            LogEntry expectedLogEntry = FormatLogEntry(ExpectedSegmentName, expectedMsg);

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                ExpectedSegmentName,
                expectedLogEntry);
        }

        [Theory]
        [InlineData("FTI=2 PAD=Segment2 FTI=1", "FTI", 2, "Segment2")]
        [InlineData("PAD=Segment0 FTI=1 PAD=Segment2", "PAD", 1, "Segment0")]
        public void ParseSegmentHeader_HeaderContainsDuplicateOptions_LogsAnErrorAndIgnoresDuplicates(string options, string optionName, int firstTimeIndent, string? padValue)
        {
            // Arrange
            string text = $"{SegmentCode} {ExpectedSegmentName} {options}";
            string expectedMsg = string.Format(MsgFoundDuplicateOptionNameOnHeaderLine, ExpectedSegmentName, optionName);
            LogEntry expectedLogEntry = FormatLogEntry(ExpectedSegmentName, expectedMsg);
            ControlItem expectedControlItem = new()
            {
                FirstTimeIndent = firstTimeIndent,
                IsFirstTime = true,
                PadSegment = padValue
            };

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                expectedControlItem,
                actualControlItem,
                ExpectedSegmentName,
                expectedLogEntry);
        }

        [Theory]
        [InlineData("FTI=1", 1, null)]
        [InlineData("PAD=Segment2", 0, "Segment2")]
        [InlineData("FTI=-1 PAD=Segment2", -1, "Segment2")]
        [InlineData("PAD=Segment2 FTI=9", 9, "Segment2")]
        public void ParseSegmentHeader_HeaderContainsValidOptions_SavesOptionValuesInControlItem(string options, int firstTimeIndent, string? padValue)
        {
            // Arrange
            string text = $"{SegmentCode} {ExpectedSegmentName} {options}";
            ControlItem expectedControlItem = new()
            {
                FirstTimeIndent = firstTimeIndent,
                IsFirstTime = true,
                PadSegment = padValue
            };

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                expectedControlItem,
                actualControlItem,
                ExpectedSegmentName);
        }

        [Fact]
        public void ParseSegmentHeader_HeaderLineLengthIsLessThanFive_LogsAnErrorAndSetsDefaultName()
        {
            // Arrange
            string text = $"{SegmentCode} ";
            string expectedSegmentName = DefaultSegmentName1;
            string expectedMsg = string.Format(MsgSegmentNameMustStartInColumn5, DefaultSegmentName1);
            LogEntry expectedLogEntry = FormatLogEntry(expectedSegmentName, expectedMsg);

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            //Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                expectedSegmentName,
                expectedLogEntry);
        }

        [Theory]
        [InlineData("1Segment")]
        [InlineData("_Segment")]
        [InlineData("Segment\t1")]
        [InlineData("Segment?1")]
        [InlineData("Segment.1")]
        [InlineData("Segment-1")]
        public void ParseSegmentHeader_InvalidSegmentNameAndNoOptions_LogsAnErrorAndSetsDefaultName(string segmentName)
        {
            // Arrange
            string text = $"{SegmentCode} {segmentName}";
            string expectedSegmentName = DefaultSegmentName1;
            string expectedMsg = string.Format(MsgInvalidSegmentName, segmentName, DefaultSegmentName1);
            LogEntry expectedLogEntry = FormatLogEntry(expectedSegmentName, expectedMsg);

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                expectedSegmentName,
                expectedLogEntry);
        }

        [Fact]
        public void ParseSegmentHeader_OptionNameMissingFromSegmentOption_LogsAnError()
        {
            // Arrange
            string text = $"{SegmentCode} {ExpectedSegmentName} =value";
            string expectedMsg = string.Format(MsgOptionNameMustPrecedeEqualsSign, ExpectedSegmentName);
            LogEntry expectedLogEntry = FormatLogEntry(ExpectedSegmentName, expectedMsg);

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                ExpectedSegmentName,
                expectedLogEntry);
        }

        [Fact]
        public void ParseSegmentHeader_SegmentNameStartsAfterFifthColumn_LogsAnErrorAndSetsDefaultName()
        {
            // Arrange
            string text = $"{SegmentCode}  Segment1";
            string expectedMsg = string.Format(MsgSegmentNameMustStartInColumn5, DefaultSegmentName1);
            LogEntry expectedLogEntry = FormatLogEntry(DefaultSegmentName1, expectedMsg);

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                DefaultSegmentName1,
                expectedLogEntry);
        }

        [Fact]
        public void ParseSegmentHeader_SegmentOptionHasNoValue_LogsAnErrorAndIgnoresTheOption()
        {
            // Arrange
            string optionName = "FTI";
            string text = $"{SegmentCode} {ExpectedSegmentName} {optionName}=";
            string expectedMsg = string.Format(MsgOptionValueMustFollowEqualsSign, ExpectedSegmentName, optionName);
            LogEntry expectedLogEntry = FormatLogEntry(ExpectedSegmentName, expectedMsg);

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                ExpectedSegmentName,
                expectedLogEntry);
        }

        [Theory]
        [InlineData("FTI")]
        [InlineData("PAD")]
        public void ParseSegmentHeader_SegmentOptionNotFollowedByEqualSign_LogsAnError(string optionName)
        {
            // Arrange
            string text = $"{SegmentCode} {ExpectedSegmentName} {optionName}";
            string expectedMsg = string.Format(MsgInvalidFormOfOption, optionName);
            LogEntry expectedLogEntry = FormatLogEntry(ExpectedSegmentName, expectedMsg);

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                ExpectedSegmentName,
                expectedLogEntry);
        }

        [Fact]
        public void ParseSegmentHeader_UnknownSegmentOption_LogsAnErrorAndIgnoresOption()
        {
            // Arrange
            string unknownOption = "FIT=value";
            string text = $"{SegmentCode} {ExpectedSegmentName} {unknownOption}";
            string expectedMsg = string.Format(MsgUnknownSegmentOptionFound, ExpectedSegmentName, unknownOption);
            LogEntry expectedLogEntry = FormatLogEntry(ExpectedSegmentName, expectedMsg);

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                ExpectedSegmentName,
                expectedLogEntry);
        }

        [Fact]
        public void ParseSegmentHeader_ValidSegmentNameAndNoSegmentOptions_SetsLocater()
        {
            // Arrange
            string text = $"{SegmentCode} {ExpectedSegmentName}";

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                _defaultControlItem,
                actualControlItem,
                ExpectedSegmentName);
        }

        [Theory]
        [InlineData("FTI=1", 1, null)]
        [InlineData("PAD=Segment2", 0, "Segment2")]
        public void ParseSegmentHeader_ValidSegmentOptionFollowsBadOption_LogsAnErrorAndAcceptsTheValidOption(string validOption, int firstTimeIndent, string? padValue)
        {
            // Arrange
            string badOption = "BadOption=value";
            string text = $"{SegmentCode} {ExpectedSegmentName} {badOption} {validOption}";
            string expectedMsg = string.Format(MsgUnknownSegmentOptionFound, ExpectedSegmentName, badOption);
            LogEntry expectedLogEntry = FormatLogEntry(ExpectedSegmentName, expectedMsg);
            ControlItem expectedControlItem = new()
            {
                FirstTimeIndent = firstTimeIndent,
                IsFirstTime = true,
                PadSegment = padValue
            };

            // Act
            ControlItem actualControlItem = SegmentHeaderParser.ParseSegmentHeader(text);

            // Assert
            AssertExpectedResults(
                expectedControlItem,
                actualControlItem,
                ExpectedSegmentName,
                expectedLogEntry);
        }

        private static void AssertExpectedResults(
            ControlItem expectedControlItem,
            ControlItem actualControlItem,
            string expectedSegmentName,
            LogEntry? expectedLogEntry = null)
        {
            if (expectedLogEntry is null)
            {
                ConsoleLogValidater.AssertLogContents(0);
            }
            else
            {
                List<LogEntry> expectedLogEnetries = new()
                {
                    expectedLogEntry
                };
                ConsoleLogValidater.AssertLogContents(1, expectedLogEnetries);
            }

            actualControlItem
                .Should()
                .BeEquivalentTo(expectedControlItem);
            Locater.CurrentSegment
                .Should()
                .Be(expectedSegmentName);
            Locater.LineNumber
                .Should()
                .Be(ExpectedLineNumber);
        }

        private static LogEntry FormatLogEntry(string expectedSegmentName, string expectedMsg)
            => new(LogEntryType.Parsing, expectedSegmentName, ExpectedLineNumber, expectedMsg);
    }
}