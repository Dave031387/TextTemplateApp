namespace TemplateProcessor.IO
{
    using System;
    using System.Collections.Generic;
    using TemplateProcessor.Logger;
    using static TemplateProcessor.Messages.Messages;

    /// <include file="../docs.xml" path="docs/members[@name=&quot;textwriter&quot;]/TextWriter/*"/>
    internal class TextWriter : ITextWriter
    {
        private string _directoryPath = string.Empty;
        private string _fileName = string.Empty;
        private string _filePath = string.Empty;

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textwriter&quot;]/CreateOutputFile/*"/>
        public bool CreateOutputFile(string filePath, IEnumerable<string>? textLines)
        {
            bool isValid = ValidTextLines(textLines);

            if (isValid)
            {
                try
                {
                    ValidateOutputFilePath(filePath);
                    CreateOutputDirectory();
                    WriteGeneratedTextToFile(textLines!);
                }
                catch (Exception ex)
                {
                    ConsoleLogger.Log(LogEntryType.Writing, MsgUnableToWriteFile, ex.Message);
                    isValid = false;
                }
            }

            return isValid;
        }

        private static bool ValidTextLines(IEnumerable<string>? textLines)
        {
            if (textLines is null)
            {
                ConsoleLogger.Log(LogEntryType.Writing, MsgGeneratedTextIsNull);
                return false;
            }

            if (!textLines.Any())
            {
                ConsoleLogger.Log(LogEntryType.Writing, MsgGeneratedTextIsEmpty);
                return false;
            }

            return true;
        }

        private void CreateOutputDirectory()
        {
            if (string.IsNullOrWhiteSpace(_directoryPath) || Directory.Exists(_directoryPath))
            {
                return;
            }

            Directory.CreateDirectory(_directoryPath);
        }

        private void ValidateOutputFilePath(string filePath)
        {
            (string directoryPath, string fileName) = PathValidater.ValidatePath(filePath, true);
            _directoryPath = directoryPath;
            _fileName = fileName;
            _filePath = Path.Combine(directoryPath, fileName);
        }

        private void WriteGeneratedTextToFile(IEnumerable<string> textLines)
        {
            ConsoleLogger.Log(LogEntryType.Writing, MsgWritingTextFile, _fileName);

            using StreamWriter writer = new(_filePath);
            foreach (string textLine in textLines)
            {
                writer.WriteLine(textLine);
            }
        }
    }
}