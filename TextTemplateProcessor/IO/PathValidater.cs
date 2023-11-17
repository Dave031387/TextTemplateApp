// Ignore Spelling: Validater

namespace TemplateProcessor.IO
{
    using static TemplateProcessor.Messages.Messages;

    /// <include file="../docs.xml" path="docs/members[@name=&quot;pathvalidator&quot;]/PathValidater/*"/>
    internal static class PathValidater
    {
        private static readonly char[] _invalidFileNameChars;
        private static readonly char[] _invalidPathChars;

        static PathValidater()
        {
            _invalidPathChars = Path.GetInvalidPathChars();
            _invalidFileNameChars = Path.GetInvalidFileNameChars();
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;pathvalidator&quot;]/ValidatePath/*"/>
        internal static (string directoryPath, string fileName) ValidatePath(string? path, bool isFilePath = false, bool shouldExist = false)
        {
            path = CheckForNullOrEmpty(path, isFilePath);

            string directoryPath = ValidateDirectoryPath(path, isFilePath);

            string fileName;
            string fullPath;

            if (isFilePath)
            {
                fileName = ValidateFileName(path);

                fullPath = Path.GetFullPath(Path.Combine(directoryPath, fileName));
                string? directoryName = Path.GetDirectoryName(fullPath);
                directoryPath = directoryName is null ? string.Empty : directoryName;
            }
            else
            {
                directoryPath = Path.GetFullPath(directoryPath);
                fileName = string.Empty;
                fullPath = directoryPath;
            }

            if (shouldExist)
            {
                VerifyExists(fullPath, isFilePath);
            }

            return (directoryPath, fileName);
        }

        private static string CheckForNullOrEmpty(string? path, bool isFilePath)
        {
            if (path is null)
            {
                string msg = isFilePath ? MsgNullFilePath : MsgNullDirectoryPath;
                throw new FilePathException(msg);
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                string msg = isFilePath ? MsgFilePathIsEmptyOrWhitespace : MsgDirectoryPathIsEmptyOrWhitespace;
                throw new FilePathException(msg);
            }

            return path.Trim();
        }

        private static string ValidateDirectoryPath(string path, bool isFilePath)
        {
            string? directoryPath = isFilePath ? Path.GetDirectoryName(path) : path;

            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new FilePathException(MsgMissingDirectoryPath);
            }

            int errorIndex;

            errorIndex = directoryPath.IndexOfAny(_invalidPathChars);

            return errorIndex >= 0 ? throw new FilePathException(MsgInvalidDirectoryCharacters) : directoryPath;
        }

        private static string ValidateFileName(string path)
        {
            string? fileName = Path.GetFileName(path);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new FilePathException(MsgMissingFileName);
            }

            int errorIndex;

            errorIndex = fileName.IndexOfAny(_invalidFileNameChars);

            return errorIndex >= 0 ? throw new FilePathException(MsgInvalidFileNameCharacters) : fileName;
        }

        private static void VerifyExists(string path, bool isFilePath)
        {
            if (isFilePath)
            {
                if (!File.Exists(path))
                {
                    throw new FilePathException(MsgFileNotFound + path);
                }
            }
            else
            {
                if (!Directory.Exists(path))
                {
                    throw new FilePathException(MsgDirectoryNotFound + path);
                }
            }
        }
    }
}