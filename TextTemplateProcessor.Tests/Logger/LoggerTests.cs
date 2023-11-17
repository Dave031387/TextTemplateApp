namespace TemplateProcessor.Logger
{
    public class LoggerTests
    {
        private static readonly LogEntry _expected;
        private static readonly int _lineNumber;
        private static readonly (string segmentName, int lineNumber) _location;
        private static readonly string _message;
        private static readonly string _segmentName;

        private readonly TestHelper _testHelper = new()
        {
            ResetLogger = true
        };

        static LoggerTests()
        {
            _segmentName = "Segment1";
            _lineNumber = 1;
            _location = (_segmentName, _lineNumber);
            _message = "This is an error message.";
            _expected = new(LogEntryType.Parsing, _segmentName, _lineNumber, _message);
        }

        public LoggerTests() => _testHelper.Reset();

        [Fact]
        public void Clear_ErrorsCollectionContainsItems_ClearsTheErrorsCollection()
        {
            // Arrange
            ConsoleLogger.Log(LogEntryType.Loading, "Message 1");
            ConsoleLogger.Log(LogEntryType.Parsing, _location, "Message 2");
            ConsoleLogger.Log(LogEntryType.Generating, _location, "Message 3");
            ConsoleLogger.LogEntries
                .Should()
                .HaveCount(3, "the Errors collection should have 3 items at the start of the test");

            // Act
            ConsoleLogger.Clear();

            // Assert
            ConsoleLogValidater.AssertLogContents(0);
        }

        [Fact]
        public void Log_MessageWithNoArguments_AddsMessageToErrorsCollection()
        {
            // Arrange
            List<LogEntry> expectedLogEntries = new()
            {
                _expected
            };

            // Act
            ConsoleLogger.Log(LogEntryType.Parsing, _location, _message);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
        }

        [Fact]
        public void Log_MessageWithOneArgument_FormatsMessageAndAddsToErrors()
        {
            // Arrange
            string msg = "This is an {0} message.";
            string arg = "error";
            List<LogEntry> expectedLogEntries = new()
            {
                _expected
            };

            // Act
            ConsoleLogger.Log(LogEntryType.Parsing, _location, msg, arg);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
        }

        [Fact]
        public void Log_MessageWithTwoArguments_FormatsMessageAndAddsToErrors()
        {
            // Arrange
            string msg = "This is an {0} {1}.";
            string arg1 = "error";
            string arg2 = "message";
            List<LogEntry> expectedLogEntries = new()
            {
                _expected
            };

            // Act
            ConsoleLogger.Log(LogEntryType.Parsing, _location, msg, arg1, arg2);

            // Assert
            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
        }

        [Theory]
        [InlineData(LogEntryType.Setup)]
        [InlineData(LogEntryType.Loading)]
        [InlineData(LogEntryType.Writing)]
        [InlineData(LogEntryType.Reset)]
        internal void Log_LogEntryTypeNotParsingOrGenerating_FormatsLogEntryCorrectly(LogEntryType type)
        {
            // Arrange
            List<LogEntry> expectedLogEntries = new()
            {
                new(type, string.Empty, 0, _message)
            };

            // Act
            ConsoleLogger.Log(type, _message);

            // Assert
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
        }

        [Theory]
        [InlineData(LogEntryType.Parsing)]
        [InlineData(LogEntryType.Generating)]
        internal void Log_LogEntryTypeParsingOrGenerating_FormatsLogEntryCorrectly(LogEntryType type)
        {
            // Arrange
            List<LogEntry> expectedLogEntries = new()
            {
                new(type, _segmentName, _lineNumber, _message)
            };

            // Act
            ConsoleLogger.Log(type, _location, _message);

            // Assert
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
        }
    }
}