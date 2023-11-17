// Ignore Spelling: Validater

namespace TemplateProcessor.IO
{
    using static TemplateProcessor.Messages.Messages;
    using static TestShared.Globals;

    public class PathValidaterTests
    {
        [Theory]
        [MemberData(nameof(TestData.InvalidPathCharacters), MemberType = typeof(TestData))]
        public void ValidatePath_DirectoryPathContainsInvalidPathCharacters_ThrowsException(string invalidChar)
        {
            // Arrange
            string directoryPath = $@"C:\x{invalidChar}x";

            // Act/Assert
            AssertException(directoryPath, MsgInvalidDirectoryCharacters);
        }

        [Theory]
        [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
        public void ValidatePath_DirectoryPathIsEmptyOrWhitespace_ThrowsException(string directoryPath)
        {
            // Act/Assert
            AssertException(directoryPath, MsgDirectoryPathIsEmptyOrWhitespace);
        }

        [Theory]
        [MemberData(nameof(TestData.InvalidFileNameCharacters), MemberType = typeof(TestData))]
        public void ValidatePath_FileNameContainsInvalidFileNameCharacters_ThrowsException(string invalidChar)
        {
            // Arrange
            string filePath = $@"C:\test\x{invalidChar}x.test";

            // Act/Assert
            AssertException(filePath, MsgInvalidFileNameCharacters, true);
        }

        [Fact]
        public void ValidatePath_FileNameMissingFromFilePath_ThrowsException()
        {
            // Arrange
            string filePath = PathWithMissingFileName;

            // Act/Assert
            AssertException(filePath, MsgMissingFileName, true);
        }

        [Theory]
        [MemberData(nameof(TestData.Whitespace), MemberType = typeof(TestData))]
        public void ValidatePath_FilePathIsEmptyOrWhitespace_ThrowsException(string filePath)
        {
            // Act/Assert
            AssertException(filePath, MsgFilePathIsEmptyOrWhitespace, true);
        }

        [Fact]
        public void ValidatePath_MissingDirectoryPath_ThrowsException()
        {
            // Arrange
            string filePath = FileNameWithoutDirectoryPath;

            // Act/Assert
            AssertException(filePath, MsgMissingDirectoryPath, true);
        }

        [Fact]
        public void ValidatePath_NullDirectoryPath_ThrowsException()
        {
            // Act/Assert
            AssertException(null, MsgNullDirectoryPath);
        }

        [Fact]
        public void ValidatePath_NullFilePath_ThrowsException()
        {
            // Act/Assert
            AssertException(null, MsgNullFilePath, true);
        }

        [Fact]
        public void ValidatePath_OkayIfDirectoryNotFound_ReturnsDirectory()
        {
            // Act
            (string directoryPath, string fileName) = PathValidater.ValidatePath(NonexistentDirectory);

            // Assert
            directoryPath
                .Should()
                .Be(NonexistentDirectory);
            fileName
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void ValidatePath_OkayIfFileNotFound_ReturnsDirectoryAndFileName()
        {
            // Arrange
            string expectedFileName = NonexistentFileName;
            string filePath = Path.Combine(TemplateDirectoryPathString, expectedFileName);

            // Act
            (string directoryPath, string fileName) = PathValidater.ValidatePath(filePath, true);

            // Assert
            directoryPath
                .Should()
                .Be(TemplateDirectoryPathString);
            fileName
                .Should()
                .Be(expectedFileName);
        }

        [Fact]
        public void ValidatePath_RequiredDirectoryFound_ReturnsDirectory()
        {
            // Arrange
            CreateTestFiles(TemplateDirectoryPathString, true);

            // Act
            (string directoryPath, string fileName) = PathValidater.ValidatePath(TemplateDirectoryPathString, false, true);

            // Assert
            directoryPath
                .Should()
                .Be(TemplateDirectoryPathString);
            fileName
                .Should()
                .BeEmpty();

            // Cleanup
            DeleteTestFiles();
        }

        [Fact]
        public void ValidatePath_RequiredDirectoryNotFound_ThrowsException()
        {
            // Act/Assert
            AssertException(NonexistentDirectory, MsgDirectoryNotFound + NonexistentDirectory, false, true);
        }

        [Fact]
        public void ValidatePath_RequiredFileFound_ReturnsDirectoryAndFileName()
        {
            // Arrange
            CreateTestFiles();

            // Act
            (string directoryPath, string fileName) = PathValidater.ValidatePath(TemplateFilePathString1, true, true);

            // Assert
            directoryPath
                .Should()
                .Be(TemplateDirectoryPathString);
            fileName
                .Should()
                .Be(TemplateFileNameString1);

            // Cleanup
            DeleteTestFiles();
        }

        [Fact]
        public void ValidatePath_RequiredFileNotFound_ThrowsException()
        {
            // Arrange
            string filePath = Path.Combine(TemplateDirectoryPathString, NonexistentFileName);

            // Act/Assert
            AssertException(filePath, MsgFileNotFound + filePath, true, true);
        }

        private static void AssertException(string? filePath, string expectedMsg, bool isFilePath = false, bool shouldExist = false)
        {
            try
            {
                _ = PathValidater.ValidatePath(filePath, isFilePath, shouldExist);
                Assert.Fail(MsgExpectedExceptionNotThrown);
            }
            catch (Exception ex)
            {
                ex
                    .Should()
                    .BeOfType<FilePathException>();
                ex.Message
                    .Should()
                    .Be(expectedMsg);
            }
        }
    }
}