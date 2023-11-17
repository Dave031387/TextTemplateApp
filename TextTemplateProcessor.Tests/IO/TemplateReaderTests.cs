namespace TemplateProcessor.IO
{
    using System;
    using System.IO;
    using static Globals;
    using static TemplateProcessor.Messages.Messages;

    public class TemplateReaderTests
    {
        private readonly TestHelper _testHelper = new()
        {
            ResetLogger = true
        };

        public TemplateReaderTests() => _testHelper.Reset();

        [Fact]
        public void ReadTextFile_FileIsEmpty_ReturnsEmptyList()
        {
            // Arrange
            CreateTestFiles();
            TextReader reader = new(TemplateFilePathString1);

            // Act
            List<string> actual = reader.ReadTextFile().ToList();

            // Assert
            actual
                .Should()
                .BeEmpty();

            // Cleanup
            DeleteTestFiles();
        }

        [Fact]
        public void ReadTextFile_FileIsNotEmpty_ReturnsFileContents()
        {
            // Arrange
            string[] textLines = new[]
            {
                "Line1",
                "   Line2",
                " Line3  "
            };
            CreateTestFiles(textLines);
            TextReader reader = new(TemplateFilePathString1);

            // Act
            List<string> actual = reader.ReadTextFile().ToList();

            // Assert
            actual
                .Should()
                .HaveCount(3);
            actual
                .Should()
                .BeEquivalentTo(textLines);

            // Cleanup
            DeleteTestFiles();
        }

        [Fact]
        public void ReadTextFile_FilePathNotSet_LogsAnError()
        {
            // Arrange
            string filePath = Path.Combine(TemplateDirectoryPathString, NonexistentFileName);
            TextReader reader = new(filePath);
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgFilePathNotSet);
            ConsoleLogger.Clear();

            // Act
            List<string> actualTextLines = reader.ReadTextFile().ToList();

            // Assert
            AssertLogEntries(expectedMsg);
            actualTextLines
                .Should()
                .BeEmpty();
        }

        [Theory]
        [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
        public void SetFilePath_DirectoryPathContainsInvalidPathCharacters_LogsAnError(string invalidChar)
        {
            // Arrange
            string filePath = $@"C:\x{invalidChar}x\test.file";
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgInvalidDirectoryCharacters);

            // Act/Assert
            SetInvalidFilePath(filePath, expectedMsg);
        }

        [Theory]
        [MemberData(nameof(TestData.InvalidFileNameCharacters), MemberType = typeof(TestData))]
        public void SetFilePath_FileNameContainsInvalidFileNameCharacters_LogsAnError(string invalidChar)
        {
            // Arrange
            string filePath = $@"C:\test\x{invalidChar}x.test";
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgInvalidFileNameCharacters);

            // Act/Assert
            SetInvalidFilePath(filePath, expectedMsg);
        }

        [Fact]
        public void SetFilePath_FileNameMissingFromFilePath_LogsAnError()
        {
            // Arrange
            string filePath = PathWithMissingFileName;
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgMissingFileName);

            // Act/Assert
            SetInvalidFilePath(filePath, expectedMsg);
        }

        [Fact]
        public void SetFilePath_FileNotFound_LogsAnError()
        {
            // Arrange
            string filePath = Path.Combine(TemplateDirectoryPathString, NonexistentFileName);
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgFileNotFound + filePath);

            // Act/Assert
            SetInvalidFilePath(filePath, expectedMsg);
        }

        [Theory]
        [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
        public void SetFilePath_FilePathIsEmptyOrWhitespace_LogsAnError(string filePath)
        {
            // Arrange
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgFilePathIsEmptyOrWhitespace);

            // Act/Assert
            SetInvalidFilePath(filePath, expectedMsg);
        }

        [Fact]
        public void SetFilePath_MissingDirectoryPath_LogsAnError()
        {
            // Arrange
            string filePath = FileNameWithoutDirectoryPath;
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgMissingDirectoryPath);

            // Act/Assert
            SetInvalidFilePath(filePath, expectedMsg);
        }

        [Fact]
        public void SetFilePath_NullFilePath_LogsAnError()
        {
            // Arrange
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgNullFilePath);

            // Act/Assert
            SetInvalidFilePath(null, expectedMsg);
        }

        [Fact]
        public void SetFilePath_ValidFilePath_SetsTheFilePath()
        {
            // Arrange
            CreateTestFiles(Array.Empty<string>(), true);
            TextReader reader = new(TemplateFilePathString2);
            string filePath = TemplateFilePathString1;

            // Act
            reader.SetFilePath(filePath);

            // Assert
            AssertFilePath(reader);

            // Cleanup
            DeleteTestFiles();
        }

        [Theory]
        [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
        public void TemplateReader_DirectoryPathContainsInvalidPathCharacters_LogsAnError(string invalidChar)
        {
            // Arrange
            string filePath = $@"C:\x{invalidChar}x\test.file";
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgInvalidDirectoryCharacters);

            // Act/Assert
            CreateInvalidTextReader(filePath, expectedMsg);
        }

        [Theory]
        [MemberData(nameof(TestData.InvalidFileNameCharacters), MemberType = typeof(TestData))]
        public void TemplateReader_FileNameContainsInvalidFileNameCharacters_LogsAnError(string invalidChar)
        {
            // Arrange
            string filePath = $@"C:\test\x{invalidChar}x.test";
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgInvalidFileNameCharacters);

            // Act/Assert
            CreateInvalidTextReader(filePath, expectedMsg);
        }

        [Fact]
        public void TemplateReader_FileNameMissingFromFilePath_LogsAnError()
        {
            // Arrange
            string filePath = PathWithMissingFileName;
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgMissingFileName);

            // Act/Assert
            CreateInvalidTextReader(filePath, expectedMsg);
        }

        [Fact]
        public void TemplateReader_FileNotFound_LogsAnError()
        {
            // Arrange
            string filePath = Path.Combine(TemplateDirectoryPathString, NonexistentFileName);
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgFileNotFound + filePath);

            // Act/Assert
            CreateInvalidTextReader(filePath, expectedMsg);
        }

        [Theory]
        [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
        public void TemplateReader_FilePathIsEmptyOrWhitespace_LogsAnError(string filePath)
        {
            // Arrange
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgFilePathIsEmptyOrWhitespace);

            // Act/Assert
            CreateInvalidTextReader(filePath, expectedMsg);
        }

        [Fact]
        public void TemplateReader_MissingDirectoryPath_LogsAnError()
        {
            // Arrange
            string filePath = FileNameWithoutDirectoryPath;
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgMissingDirectoryPath);

            // Act/Assert
            CreateInvalidTextReader(filePath, expectedMsg);
        }

        [Fact]
        public void TemplateReader_NoFilePathGiven_SetsFilePathToEmptyString()
        {
            // Act
            TextReader reader = new();

            // Assert
            AssertFilePath(reader, true);
        }

        [Fact]
        public void TemplateReader_NullFilePath_LogsAnError()
        {
            // Arrange
            string expectedMsg = string.Format(MsgUnableToSetTemplateFilePath, MsgNullFilePath);

            // Act/Assert
            CreateInvalidTextReader(null, expectedMsg);
        }

        [Fact]
        public void TemplateReader_ValidFilePath_ExtractsDirectoryPathAndFileNameFromFilePath()
        {
            // Arrange
            string[] textLines = new[]
            {
                "This is a test"
            };
            CreateTestFiles(textLines);
            string filePath = TemplateFilePathString1;

            // Act
            TextReader reader = new(filePath);

            // Assert
            AssertFilePath(reader);

            // Cleanup
            DeleteTestFiles();
        }

        private static void AssertFilePath(TextReader reader, bool shouldBeEmpty = false)
        {
            if (shouldBeEmpty)
            {
                reader.DirectoryPath
                    .Should()
                    .Be(string.Empty);
                reader.FileName
                    .Should()
                    .Be(string.Empty);
                reader.FullFilePath
                    .Should()
                    .Be(string.Empty);
            }
            else
            {
                reader.DirectoryPath
                    .Should()
                    .Be(TemplateDirectoryPathString);
                reader.FileName
                    .Should()
                    .Be(TemplateFileNameString1);
                reader.FullFilePath
                    .Should()
                    .Be(TemplateFilePathString1);
            }
        }

        private static void AssertLogEntries(string expectedMsg)
        {
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Loading, string.Empty, 0, expectedMsg)
            };

            ConsoleLogValidater.AssertLogContents(1, expectedLogEntries);
        }

        private static void CreateInvalidTextReader(string? filePath, string expectedMsg)
        {
            // Act
            TextReader reader = new(filePath!);

            // Assert
            AssertLogEntries(expectedMsg);
            AssertFilePath(reader, true);
        }

        private static void SetInvalidFilePath(string? filePath, string expectedMsg)
        {
            // Arrange
            CreateTestFiles();
            TextReader reader = new(TemplateFilePathString1);

            // Act
            reader.SetFilePath(filePath!);

            // Assert
            AssertLogEntries(expectedMsg);
            AssertFilePath(reader, true);

            // Cleanup
            DeleteTestFiles();
        }
    }
}