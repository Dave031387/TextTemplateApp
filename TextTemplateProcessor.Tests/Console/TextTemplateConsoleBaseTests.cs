namespace TemplateProcessor.Console
{
    using TemplateProcessor.Logger;
    using static Globals;
    using static TemplateProcessor.Messages.Messages;

    public class TextTemplateConsoleBaseTests
    {
        private readonly string[] _templateText = new[]
                {
                    "### Segment1",
                    "    Text line 1"
                };

        private readonly TestHelper _testHelper = new()
        {
            ResetLogger = true
        };

        public TextTemplateConsoleBaseTests() => _testHelper.Reset();

        [Theory]
        [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
        public void LoadTemplate_DirectoryPathContainsInvalidPathCharacters_LogsAnError(string invalidChar)
        {
            // Arrange
            string filePath = $@"C:\x{invalidChar}x\test.file";
            string expectedMessage = string.Format(MsgUnableToLoadTemplateFile, MsgInvalidDirectoryCharacters);

            // Act/Assert
            ValidateLoadTemplateLogsAnError(filePath, expectedMessage);
        }

        [Fact]
        public void LoadTemplate_FileExistsAndPathIsRooted_SetsTheFilePathAndLoadsTheTemplate()
        {
            // Arrange
            SampleTextTemplateConsole consoleBase = new();
            string expectedTemplateFilePath = $@"{TemplateDirectoryPathString}\{TemplateFileNameString1}";

            // Act/Assert
            TestValidTemplateFilePath(TemplateDirectoryPathString, expectedTemplateFilePath, consoleBase);
        }

        [Fact]
        public void LoadTemplate_FileExistsAndPathNotRooted_SetsTheFilePathAndLoadsTheTemplate()
        {
            // Arrange
            SampleTextTemplateConsole consoleBase = new();
            string directoryPath = $@"{nameof(TestModels)}\Templates";
            string expectedTemplateFilePath = $@"{SolutionDirectory}\{directoryPath}\{TemplateFileNameString1}";

            // Act/Assert
            TestValidTemplateFilePath(directoryPath, expectedTemplateFilePath, consoleBase);
        }

        [Theory]
        [MemberData(nameof(TestData.InvalidFileNameCharacters), MemberType = typeof(TestData))]
        public void LoadTemplate_FileNameContainsInvalidFileNameCharacters_LogsAnError(string invalidChar)
        {
            // Arrange
            string filePath = $@"C:\test\x{invalidChar}x.test";
            string expectedMessage = string.Format(MsgUnableToLoadTemplateFile, MsgInvalidFileNameCharacters);

            // Act/Assert
            ValidateLoadTemplateLogsAnError(filePath, expectedMessage);
        }

        [Fact]
        public void LoadTemplate_FileNameMissingFromFilePath_LogsAnError()
        {
            // Arrange
            string filePath = PathWithMissingFileName;
            string expectedMessage = string.Format(MsgUnableToLoadTemplateFile, MsgMissingFileName);

            // Act/Assert
            ValidateLoadTemplateLogsAnError(filePath, expectedMessage);
        }

        [Theory]
        [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
        public void LoadTemplate_FilePathIsEmptyOrWhitespace_LogsAnError(string filePath)
        {
            // Arrange
            string expectedMessage = string.Format(MsgUnableToLoadTemplateFile, MsgFilePathIsEmptyOrWhitespace);

            // Act/Assert
            ValidateLoadTemplateLogsAnError(filePath, expectedMessage);
        }

        [Fact]
        public void LoadTemplate_NullFilePath_LogsAnError()
        {
            // Arrange
            string? filePath = null;
            string expectedMessage = string.Format(MsgUnableToLoadTemplateFile, MsgNullFilePath);

            // Act/Assert
            ValidateLoadTemplateLogsAnError(filePath, expectedMessage);
        }

        [Fact]
        public void LoadTemplate_TemplateFileNotFound_LogsAnError()
        {
            // Arrange
            string filePath = Path.Combine(TemplateDirectoryPathString, NonexistentFileName);
            string expectedMessage = string.Format(MsgUnableToLoadTemplateFile, MsgFileNotFound + filePath);

            // Act/Assert
            ValidateLoadTemplateLogsAnError(filePath, expectedMessage);
        }

        [Fact]
        public void SetOutputDirectory_DirectoryDoesNotExistAndPathIsNotRooted_CreatesTheOutputDirectory()
        {
            // Arrange
            SampleTextTemplateConsole consoleBase = new();
            string directoryPath = $@"{nameof(TestModels)}\Templates";
            string expectedOutputDirectory = $@"{SolutionDirectory}\{directoryPath}";
            DeleteTestFiles(directoryPath);

            // Act/Assert
            TestValidOutputDirectory(directoryPath, expectedOutputDirectory, consoleBase, false);
        }

        [Fact]
        public void SetOutputDirectory_DirectoryDoesNotExistAndPathIsRooted_CreatesTheOutputDirectory()
        {
            // Arrange
            SampleTextTemplateConsole consoleBase = new();
            DeleteTestFiles(TemplateDirectoryPathString);

            // Act/Assert
            TestValidOutputDirectory(TemplateDirectoryPathString, TemplateDirectoryPathString, consoleBase, false);
        }

        [Fact]
        public void SetOutputDirectory_DirectoryExistsAndPathIsNotRooted_SetsTheOutputDirectory()
        {
            // Arrange
            SampleTextTemplateConsole consoleBase = new();
            string directoryPath = $@"{nameof(TestModels)}\Templates";
            string expectedOutputDirectory = $@"{SolutionDirectory}\{directoryPath}";

            // Act/Assert
            TestValidOutputDirectory(directoryPath, expectedOutputDirectory, consoleBase, true);
        }

        [Fact]
        public void SetOutputDirectory_DirectoryExistsAndPathIsRooted_SetsTheOutputDirectory()
        {
            // Arrange
            SampleTextTemplateConsole consoleBase = new();

            // Act/Assert
            TestValidOutputDirectory(TemplateDirectoryPathString, TemplateDirectoryPathString, consoleBase, true);
        }

        [Theory]
        [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
        public void SetOutputDirectory_DirectoryPathContainsInvalidPathCharacters_LogsAnError(string invalidChar)
        {
            // Arrange
            string directoryPath = $@"C:\x{invalidChar}x";
            string expectedMessage = string.Format(MsgUnableToSetOutputDirectory, MsgInvalidDirectoryCharacters);

            // Act/Assert
            ValidateSetOutputDirectoryLogsAnError(directoryPath, expectedMessage);
        }

        [Theory]
        [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
        public void SetOutputDirectory_DirectoryPathIsEmptyOrWhitespace_LogsAnError(string directoryPath)
        {
            // Arrange
            string expectedMessage = string.Format(MsgUnableToSetOutputDirectory, MsgDirectoryPathIsEmptyOrWhitespace);

            // Act/Assert
            ValidateSetOutputDirectoryLogsAnError(directoryPath, expectedMessage);
        }

        [Fact]
        public void SetOutputDirectory_NullDirectoryPath_LogsAnError()
        {
            // Arrange
            string expectedMessage = string.Format(MsgUnableToSetOutputDirectory, MsgNullDirectoryPath);

            // Act/Assert
            ValidateSetOutputDirectoryLogsAnError(null, expectedMessage);
        }

        [Fact]
        public void TextTemplateConsoleBase_ConstructUsingFilePath_LoadsTemplateFile()
        {
            // Arrange
            CreateTestFiles(_templateText);

            // Act
            SampleTextTemplateConsole consoleBase = new(TemplateFilePathString1);

            // Assert
            consoleBase.SolutionDirectory
                .Should()
                .Be(SolutionDirectory);
            consoleBase.TemplateFilePath
                .Should()
                .Be(TemplateFilePathString1);
            consoleBase.OutputDirectory
                .Should()
                .BeEmpty();
            consoleBase.IsTemplateLoaded
                .Should()
                .BeTrue();

            // Cleanup
            DeleteTestFiles();
        }

        [Fact]
        public void TextTemplateConsoleBase_ConstructUsingTemplateProcessor_SetsInitialPropertyValues()
        {
            // Act
            SampleTextTemplateConsole consoleBase = new();

            // Assert
            consoleBase.SolutionDirectory
                .Should()
                .Be(SolutionDirectory);
            consoleBase.TemplateFilePath
                .Should()
                .BeEmpty();
            consoleBase.OutputDirectory
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void TextTemplateConsoleBase_UsingDefaultConstructor_SetsInitialPropertyValues()
        {
            // Act
            SampleTextTemplateConsole consoleBase = new();

            // Assert
            consoleBase.SolutionDirectory
                .Should()
                .Be(SolutionDirectory);
            consoleBase.TemplateFilePath
                .Should()
                .BeEmpty();
            consoleBase.OutputDirectory
                .Should()
                .BeEmpty();
        }

        private static void TestValidOutputDirectory(string directoryPath, string expectedOutputDirectory, SampleTextTemplateConsole consoleBase, bool createDirectory = false)
        {
            // Arrange
            if (createDirectory)
            {
                CreateTestFiles(directoryPath, true);
            }
            else
            {
                Directory.Exists(expectedOutputDirectory)
                    .Should()
                    .BeFalse("The output directory shouldn't exist at the beginning of the test.");
            }

            // Act
            consoleBase.SetOutputDirectory(directoryPath);

            // Assert
            consoleBase.OutputDirectory
                .Should()
                .Be(expectedOutputDirectory);
            Directory.Exists(expectedOutputDirectory)
                .Should()
                .BeTrue();

            // Cleanup
            DeleteTestFiles(directoryPath);
        }

        private static void TestValidTemplateFilePath(string directoryPath, string expectedTemplateFilePath, SampleTextTemplateConsole consoleBase)
        {
            // Arrange
            string templateFilePath = $@"{directoryPath}\{TemplateFileNameString1}";
            CreateTestFiles(directoryPath);

            // Act
            consoleBase.LoadTemplate(templateFilePath);

            // Assert
            consoleBase.TemplateFilePath
                .Should()
                .Be(expectedTemplateFilePath);

            // Cleanup
            DeleteTestFiles(directoryPath);
        }

        private static void ValidateLoadTemplateLogsAnError(string? filePath, string expectedMessage)
        {
            // Arrange
            SampleTextTemplateConsole consoleBase = new();
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Loading, string.Empty, 0, expectedMessage)
            };

            // Act
            consoleBase.LoadTemplate(filePath!);

            // Assert
            consoleBase.TemplateFilePath
                .Should()
                .BeEmpty();
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
        }

        private static void ValidateSetOutputDirectoryLogsAnError(string? directoryPath, string expectedMessage)
        {
            // Arrange
            SampleTextTemplateConsole consoleBase = new();
            List<LogEntry> expectedLogEntries = new()
            {
                new(LogEntryType.Setup, string.Empty, 0, expectedMessage)
            };

            // Act
            consoleBase.SetOutputDirectory(directoryPath!);

            // Assert
            consoleBase.OutputDirectory
                .Should()
                .BeEmpty();
            ConsoleLogValidater.AssertLogContents(-1, expectedLogEntries);
        }
    }
}