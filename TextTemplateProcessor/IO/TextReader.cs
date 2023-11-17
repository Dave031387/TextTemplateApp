namespace TemplateProcessor.IO
{
    using TemplateProcessor.Logger;
    using static TemplateProcessor.Messages.Messages;

    /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/TextReader/*"/>
    internal class TextReader : ITextReader
    {
        private bool _isFilePathSet = false;

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/Constructor1/*"/>
        internal TextReader() => InitializeProperties();

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/Constructor2/*"/>
        internal TextReader(string filePath) => SetFilePath(filePath);

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/DirectoryPath/*"/>
        public string DirectoryPath { get; private set; } = string.Empty;

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/FileName/*"/>
        public string FileName { get; private set; } = string.Empty;

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/FullFilePath/*"/>
        public string FullFilePath { get; private set; } = string.Empty;

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/ReadTextFile/*"/>
        public IEnumerable<string> ReadTextFile()
        {
            List<string> textLines = new();

            if (!_isFilePathSet)
            {
                ConsoleLogger.Log(LogEntryType.Loading, MsgUnableToSetTemplateFilePath, MsgFilePathNotSet);
                return textLines;
            }

            try
            {
                ConsoleLogger.Log(LogEntryType.Loading, MsgAttemptingToReadFile, FullFilePath);
                using StreamReader reader = new(FullFilePath);
                while (!reader.EndOfStream)
                {
                    string? textLine = reader.ReadLine();

                    if (textLine is not null)
                    {
                        textLines.Add(textLine);
                    }
                }
                ConsoleLogger.Log(LogEntryType.Loading, MsgFileSuccessfullyRead);
            }
            catch (Exception ex)
            {
                ConsoleLogger.Log(LogEntryType.Loading, MsgErrorWhileReadingFile, ex.Message);
            }

            return textLines;
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/SetFilePath/*"/>
        public void SetFilePath(string filePath)
        {
            InitializeProperties();

            try
            {
                (string directoryPath, string fileName) = PathValidater.ValidatePath(filePath, true, true);
                DirectoryPath = directoryPath;
                FileName = fileName;
                FullFilePath = Path.Combine(directoryPath, fileName);
                _isFilePathSet = true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.Log(LogEntryType.Loading, MsgUnableToSetTemplateFilePath, ex.Message);
            }
        }

        private void InitializeProperties()
        {
            _isFilePathSet = false;
            DirectoryPath = string.Empty;
            FileName = string.Empty;
            FullFilePath = string.Empty;
        }
    }
}