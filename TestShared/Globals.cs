namespace TestShared
{
    public static class Globals
    {
        public const string DefaultSegmentName1 = "DefaultSegment1";
        public const string DefaultSegmentName2 = "DefaultSegment2";
        public const string DirectoryPathWithInvalidCharacters = $@"{TestDirectoryRoot}|dir\ABC.tst";
        public const string FileNameWithInvalidCharacters = $@"{TestDirectoryRoot}\ABC:123*.tst";
        public const string FileNameWithoutDirectoryPath = "test.file";
        public const string InvalidDirectoryPath = "This is an invalid directory path.";
        public const string MsgExpectedExceptionNotThrown = "Expected exception not thrown.";
        public const string NonexistentDirectory = @"C:\missing";
        public const string NonexistentFileName = "NotFound.txt";
        public const string OutputFilePathString = $@"{TestDirectoryRoot}\generated.txt";
        public const string PathWithMissingFileName = $@"{TestDirectoryRoot}\Missing\";
        public const string TemplateDirectoryPathString = $@"{TestDirectoryRoot}\Templates";
        public const string TemplateFileNameString1 = "test_template1.txt";
        public const string TemplateFileNameString2 = "test_template2.txt";
        public const string TemplateFilePathString1 = $@"{TemplateDirectoryPathString}\{TemplateFileNameString1}";
        public const string TemplateFilePathString2 = $@"{TemplateDirectoryPathString}\{TemplateFileNameString2}";
        public const string TestDirectoryRoot = @"C:\Test";
        public const string Whitespace = "\t\n\v\f\r \u0085\u00a0\u2002\u2003\u2028\u2029";

        static Globals()
        {
            string path = Directory.GetCurrentDirectory();
            string currentDirectory = path;
            int pathIndex;

            while (true)
            {
                pathIndex = path.LastIndexOf(Path.DirectorySeparatorChar);

                if (pathIndex < 0)
                {
                    throw new DirectoryNotFoundException($"Unable to locate the solution directory in the current directory path.\n Current directory: {currentDirectory}");
                }

                path = path[..pathIndex];
                string[] files = Directory.GetFiles(path, "*.sln");

                if (files.Length > 0)
                {
                    SolutionDirectory = path;
                    break;
                }
            }
        }

        public static string SolutionDirectory { get; private set; }

        public static void CreateTestFiles(string[]? text = null, bool createOtherFile = false)
        {
            DeleteTestFiles();
            text ??= Array.Empty<string>();
            Directory.CreateDirectory(TemplateDirectoryPathString);
            File.WriteAllLines(TemplateFilePathString1, text);

            if (createOtherFile)
            {
                File.WriteAllLines(TemplateFilePathString2, Array.Empty<string>());
            }
        }

        public static void CreateTestFiles(string path, bool directoryOnly = false)
        {
            DeleteTestFiles(path);
            string directoryPath = GetRootedPath(path);
            string fullFilePath = Path.Combine(directoryPath, TemplateFileNameString1);
            Directory.CreateDirectory(directoryPath);
            if (!directoryOnly)
            {
                File.WriteAllLines(fullFilePath, Array.Empty<string>());
            }
        }

        public static void DeleteTestFiles(string path = TestDirectoryRoot)
        {
            string directoryPath = GetRootedPath(path);

            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        public static string GetRootedPath(string path) => Path.IsPathRooted(path) ? path : Path.Combine(SolutionDirectory, path);
    }
}