namespace TemplateProcessor
{
    using TemplateProcessor.Logger;
    using static TemplateProcessor.Messages.Messages;

    public class IndentProcessorTests
    {
        private static readonly int _lineNumber;
        private static readonly string _segmentName;

        private readonly TestHelper _testHelper = new()
        {
            ResetIndentProcessor = true,
            ResetLogger = true
        };

        static IndentProcessorTests()
        {
            _segmentName = "Segment1";
            _lineNumber = 1;
            Locater.CurrentSegment = _segmentName;
            Locater.LineNumber = _lineNumber;
        }

        public IndentProcessorTests() => _testHelper.Reset();

        [Fact]
        public void GetFirstTimeIndent_CalculatedValueIsLessThanZero_LogsAnErrorAndSetsIndentToZero()
        {
            // Arrange
            TextItem initial = new(2, true, false, "");
            TextItem textItem = new(1, true, false, "");
            IndentProcessor.SetTabSize(2);
            _ = IndentProcessor.GetIndent(initial);
            string msg = string.Format(MsgFirstTimeIndentHasBeenTruncated, _segmentName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, _segmentName, _lineNumber, msg)
            };

            // Act
            int actual = IndentProcessor.GetFirstTimeIndent(-3, textItem);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            actual
                .Should()
                .Be(0);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(0);
        }

        [Theory]
        [InlineData(-2, 0)]
        [InlineData(-1, 2)]
        [InlineData(1, 6)]
        [InlineData(2, 8)]
        public void GetFirstTimeIndent_FirstTimeIndentIsNotZero_ReturnsCorrectIndentValue(int firstTimeIndent, int expected)
        {
            // Arrange
            TextItem initial = new(2, true, false, "");
            TextItem textItem = new(1, true, false, "");
            IndentProcessor.SetTabSize(2);
            _ = IndentProcessor.GetIndent(initial);

            // Act
            int actual = IndentProcessor.GetFirstTimeIndent(firstTimeIndent, textItem);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            actual
                .Should()
                .Be(expected);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(expected);
        }

        [Fact]
        public void GetFirstTimeIndent_FirstTimeIndentIsZero_CallsGetIndentAndReturnsIndentValue()
        {
            // Arrange
            TextItem initial = new(2, true, false, "");
            TextItem textItem = new(1, true, false, "");
            IndentProcessor.SetTabSize(2);
            _ = IndentProcessor.GetIndent(initial);

            // Act
            int actual = IndentProcessor.GetFirstTimeIndent(0, textItem);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            actual
                .Should()
                .Be(6);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(6);
        }

        [Theory]
        [InlineData(false, false, 0)]
        [InlineData(true, false, 0)]
        [InlineData(false, true, 4)]
        [InlineData(true, true, 4)]
        public void GetIndent_CalculatedIndentIsNegative_LogsAnErrorAndSetsIndentToZero(bool isRelative, bool isOneTime, int expected)
        {
            // Arrange
            TextItem initial = new(2, true, false, "");
            TextItem textItem = new(-3, isRelative, isOneTime, "");
            IndentProcessor.SetTabSize(2);
            _ = IndentProcessor.GetIndent(initial);
            string msg = string.Format(MsgLeftIndentHasBeenTruncated, _segmentName);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Generating, _segmentName, _lineNumber, msg)
            };

            // Act
            int actual = IndentProcessor.GetIndent(textItem);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            actual
                .Should()
                .Be(0);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(expected);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        [InlineData(2, 4)]
        public void GetIndent_IsNotRelativeAndIsOneTime_ReturnsCorrectIndentValue(int indent, int expected)
        {
            // Arrange
            TextItem initial = new(2, true, false, "");
            TextItem textItem = new(indent, false, true, "");
            IndentProcessor.SetTabSize(2);
            _ = IndentProcessor.GetIndent(initial);

            // Act
            int actual = IndentProcessor.GetIndent(textItem);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            actual
                .Should()
                .Be(expected);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(4);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        [InlineData(2, 4)]
        public void GetIndent_IsNotRelativeAndNotOneTime_ReturnsCorrectIndentValue(int indent, int expected)
        {
            // Arrange
            TextItem initial = new(2, true, false, "");
            TextItem textItem = new(indent, false, false, "");
            IndentProcessor.SetTabSize(2);
            _ = IndentProcessor.GetIndent(initial);

            // Act
            int actual = IndentProcessor.GetIndent(textItem);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            actual
                .Should()
                .Be(expected);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(expected);
        }

        [Theory]
        [InlineData(-2, 0)]
        [InlineData(-1, 2)]
        [InlineData(0, 4)]
        [InlineData(1, 6)]
        [InlineData(2, 8)]
        public void GetIndent_IsRelativeAndIsOneTime_ReturnsCorrectIndentValue(int indent, int expected)
        {
            // Arrange
            TextItem initial = new(2, true, false, "");
            TextItem textItem = new(indent, true, true, "");
            IndentProcessor.SetTabSize(2);
            _ = IndentProcessor.GetIndent(initial);

            // Act
            int actual = IndentProcessor.GetIndent(textItem);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            actual
                .Should()
                .Be(expected);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(4);
        }

        [Theory]
        [InlineData(-2, 0)]
        [InlineData(-1, 2)]
        [InlineData(0, 4)]
        [InlineData(1, 6)]
        [InlineData(2, 8)]
        public void GetIndent_IsRelativeAndNotOneTime_ReturnsCorrectIndentValue(int indent, int expected)
        {
            // Arrange
            TextItem initial = new(2, true, false, "");
            TextItem textItem = new(indent, true, false, "");
            IndentProcessor.SetTabSize(2);
            _ = IndentProcessor.GetIndent(initial);

            // Act
            int actual = IndentProcessor.GetIndent(textItem);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            actual
                .Should()
                .Be(expected);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(expected);
        }

        [Theory]
        [InlineData("-9", -9)]
        [InlineData("-8", -8)]
        [InlineData("0", 0)]
        [InlineData("+8", 8)]
        [InlineData("9", 9)]
        public void IsValidIndentValue_ValueIsInsideOfRange_ReturnsTrueAndTheIntegerValue(string value, int expected)
        {
            // Act
            bool actual = IndentProcessor.IsValidIndentValue(value, out int indent);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            actual
                .Should()
                .BeTrue();
            indent
                .Should()
                .Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("-")]
        [InlineData("+")]
        [InlineData(".")]
        [InlineData("1.0")]
        [InlineData("ABC")]
        [InlineData("1X")]
        [InlineData(null)]
        public void IsValidIndentValue_ValueIsNotAValidInteger_LogsAnErrorAndReturnsFalse(string value)
        {
            // Arrange
            string msg = string.Format(MsgIndentValueMustBeValidNumber, value);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, _segmentName, _lineNumber, msg)
            };

            // Act
            bool actual = IndentProcessor.IsValidIndentValue(value, out int indent);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            actual
                .Should()
                .BeFalse();
            indent
                .Should()
                .Be(0);
        }

        [Theory]
        [InlineData("-11")]
        [InlineData("-10")]
        [InlineData("10")]
        [InlineData("11")]
        public void IsValidIndentValue_ValueIsOutsideOfRange_LogsAnErrorAndReturnsFalse(string value)
        {
            // Arrange
            string msg = string.Format(MsgIndentValueOutOfRange, value);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Parsing, _segmentName, _lineNumber, msg)
            };

            // Act
            bool actual = IndentProcessor.IsValidIndentValue(value, out int indent);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            actual
                .Should()
                .BeFalse();
            indent
                .Should()
                .Be(0);
        }

        [Fact]
        public void Reset_WhenCalled_SetsCurrentIndentToZero()
        {
            // Arrange
            TextItem initial = new(2, true, false, "");
            IndentProcessor.SetTabSize(2);
            _ = IndentProcessor.GetIndent(initial);

            // Act
            IndentProcessor.Reset();

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            IndentProcessor.CurrentIndent
                .Should()
                .Be(0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(9)]
        [InlineData(5)]
        public void SetTabSize_ValidTabSize_SetsTheTabSizeToTheGivenValue(int tabSize)
        {
            // Act
            IndentProcessor.SetTabSize(tabSize);

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
            IndentProcessor.TabSize
                .Should()
                .Be(tabSize);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SetTabSize_ValueGivenIsLessThanMinimum_LogsAnErrorAndSetsTabSizeTo_1(int tabSize)
        {
            // Arrange
            string expectedMsg = string.Format(MsgTabSizeTooSmall, 1);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Setup, string.Empty, 0, expectedMsg)
            };

            // Act
            IndentProcessor.SetTabSize(tabSize);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            IndentProcessor.TabSize
                .Should()
                .Be(1);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(11)]
        public void SetTabSize_ValueGivenIsMoreThanMaximum_LogsAnErrorAndSetsTabSizeTo_9(int tabSize)
        {
            // Arrange
            string expectedMsg = string.Format(MsgTabSizeTooLarge, 9);
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Setup, string.Empty, 0, expectedMsg)
            };

            // Act
            IndentProcessor.SetTabSize(tabSize);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
            IndentProcessor.TabSize
                .Should()
                .Be(9);
        }
    }
}