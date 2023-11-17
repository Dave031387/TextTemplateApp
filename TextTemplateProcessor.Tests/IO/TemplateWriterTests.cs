namespace TemplateProcessor.IO
{
    using System.Linq;
    using static Globals;
    using static TemplateProcessor.Messages.Messages;

    public class TemplateWriterTests
    {
        private readonly TestHelper _testHelper = new()
        {
            ResetLogger = true
        };

        private readonly List<string> _textLines = new()
        {
            "Line 1",
            "Line 2",
            "Line 3"
        };

        public TemplateWriterTests() => _testHelper.Reset();

        [Theory]
        [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
        public void CreateOutputFile_DirectoryPathContainsInvalidPathCharacters_LogsAnError(string invalidChar)
        {
            // Arrange
            TextWriter writer = new();
            string filePath = $@"C:\x{invalidChar}x\test.file";
            string expectedMessage = string.Format(MsgUnableToWriteFile, MsgInvalidDirectoryCharacters);

            // Act
            writer.CreateOutputFile(filePath, _textLines);

            // Assert
            AssertTestResults(expectedMessage);
        }

        [Theory]
        [MemberData(nameof(TestData.InvalidFileNameCharacters), MemberType = typeof(TestData))]
        public void CreateOutputFile_FileNameContainsInvalidFileNameCharacters_LogsAnError(string invalidChar)
        {
            // Arrange
            TextWriter writer = new();
            string filePath = $@"C:\test\x{invalidChar}x.test";
            string expectedMessage = string.Format(MsgUnableToWriteFile, MsgInvalidFileNameCharacters);

            // Act
            writer.CreateOutputFile(filePath, _textLines);

            // Assert
            AssertTestResults(expectedMessage);
        }

        [Fact]
        public void CreateOutputFile_FileNameMissingFromFilePath_LogsAnError()
        {
            // Arrange
            TextWriter writer = new();
            string filePath = PathWithMissingFileName;
            string expectedMessage = string.Format(MsgUnableToWriteFile, MsgMissingFileName);

            // Act
            writer.CreateOutputFile(filePath, _textLines);

            // Assert
            AssertTestResults(expectedMessage);
        }

        [Theory]
        [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
        public void CreateOutputFile_FilePathIsEmptyOrWhitespace_LogsAnError(string filePath)
        {
            // Arrange
            TextWriter writer = new();
            string expectedMessage = string.Format(MsgUnableToWriteFile, MsgFilePathIsEmptyOrWhitespace);

            // Act
            writer.CreateOutputFile(filePath, _textLines);

            // Assert
            AssertTestResults(expectedMessage);
        }

        [Fact]
        public void CreateOutputFile_GeneratedTextIsEmpty_LogsAnError()
        {
            // Arrange
            DeleteTestFiles();
            TextWriter writer = new();
            string filePath = TemplateFilePathString1;
            string expectedMsg = MsgGeneratedTextIsEmpty;

            // Act
            writer.CreateOutputFile(filePath, new List<string>());

            // Assert
            AssertTestResults(expectedMsg, true);
        }

        [Fact]
        public void CreateOutputFile_GeneratedTextIsNull_LogsAnError()
        {
            // Arrange
            DeleteTestFiles();
            TextWriter writer = new();
            string filePath = TemplateFilePathString1;
            string expectedMsg = MsgGeneratedTextIsNull;

            // Act
            writer.CreateOutputFile(filePath, null);

            // Assert
            AssertTestResults(expectedMsg, true);
        }

        [Fact]
        public void CreateOutputFile_MissingDirectoryPath_LogsAnError()
        {
            // Arrange
            TextWriter writer = new();
            string filePath = FileNameWithoutDirectoryPath;
            string expectedMessage = string.Format(MsgUnableToWriteFile, MsgMissingDirectoryPath);

            // Act
            writer.CreateOutputFile(filePath, _textLines);

            // Assert
            AssertTestResults(expectedMessage);
        }

        [Fact]
        public void CreateOutputFile_NullFilePath_LogsAnError()
        {
            // Arrange
            TextWriter writer = new();
            string expectedMessage = string.Format(MsgUnableToWriteFile, MsgNullFilePath);

            // Act
            writer.CreateOutputFile(null!, _textLines);

            // Assert
            AssertTestResults(expectedMessage);
        }

        [Fact]
        public void CreateOutputFile_ValidFilePathAndGeneratedText_WritesGeneratedTextToFile()
        {
            // Arrange
            DeleteTestFiles();
            TextWriter writer = new();
            string filePath = TemplateFilePathString1;
            string expectedMsg = string.Format(MsgWritingTextFile, TemplateFileNameString1);

            // Act
            writer.CreateOutputFile(filePath, _textLines);

            // Assert
            string[] textLines = File.ReadAllLines(filePath);
            AssertTestResults(expectedMsg, true, true, textLines);

            // Cleanup
            DeleteTestFiles();
        }

        private void AssertTestResults(string expectedMsg, bool checkOutputFile = false, bool outputFileShouldExist = false, string[]? textLines = null)
        {
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Writing, string.Empty, 0, expectedMsg)
            };

            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);

            if (checkOutputFile)
            {
                File.Exists(TemplateFilePathString1)
                    .Should()
                    .Be(outputFileShouldExist);
                textLines
                    ?.Should()
                    .NotBeNull();
                textLines
                    ?.Should()
                    .ContainInConsecutiveOrder(_textLines);
            }
        }
    }
}