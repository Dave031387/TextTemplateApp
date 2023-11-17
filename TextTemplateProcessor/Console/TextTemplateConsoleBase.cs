namespace TemplateProcessor.Console
{
    using System;
    using TemplateProcessor;
    using TemplateProcessor.IO;
    using TemplateProcessor.Logger;
    using static TemplateProcessor.Messages.Messages;

    /// <include file="../docs.xml" path="docs/members[@name=&quot;texttemplateconsolebase&quot;]/TextTemplateConsoleBase/*"/>
    public abstract class TextTemplateConsoleBase : TextTemplateProcessor
    {
        private const string SolutionFileSearchPattern = "*.sln";
        private readonly char _directorySeparator = Path.DirectorySeparatorChar;

        /// <include file="../docs.xml" path="docs/members[@name=&quot;texttemplateconsolebase&quot;]/Constructor1/*"/>
        public TextTemplateConsoleBase() : base(new TextReader(), new TextWriter())
        {
            SolutionDirectory = GetSolutionDirectory();
            OutputDirectory = string.Empty;
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;texttemplateconsolebase&quot;]/Constructor2/*"/>
        public TextTemplateConsoleBase(string path) : base(path)
        {
            SolutionDirectory = GetSolutionDirectory();
            OutputDirectory = string.Empty;
            LoadTemplate(path);
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;texttemplateconsolebase&quot;]/OutputDirectory/*"/>
        public string OutputDirectory { get; private set; }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;texttemplateconsolebase&quot;]/SolutionDirectory/*"/>
        public string SolutionDirectory { get; private set; }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;texttemplateconsolebase&quot;]/ShowContinuationPrompt/*"/>
        public static string ShowContinuationPrompt(string message = MsgContinuationPrompt)
        {
            ConsoleWriter.WriteLogEntries();
            ConsoleWriter.WriteLine("\n" + message + "\n");
            return Console.ReadLine() ?? string.Empty;
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;texttemplateconsolebase&quot;]/ClearOutputDirectory/*"/>
        public void ClearOutputDirectory()
        {
            if (!string.IsNullOrWhiteSpace(OutputDirectory))
            {
                ConsoleWriter.WriteLine(MsgClearTheOutputDirectory, OutputDirectory);
                string response = ShowContinuationPrompt(MsgYesNoPrompt);

                if (response.ToUpper() == "Y")
                {
                    if (Directory.Exists(OutputDirectory))
                    {
                        DirectoryInfo directoryInfo = new(OutputDirectory);

                        foreach (FileInfo file in directoryInfo.GetFiles())
                        {
                            file.Delete();
                        }

                        ConsoleLogger.Log(LogEntryType.Setup, MsgOutputDirectoryCleared);
                    }
                }

                ConsoleWriter.WriteLogEntries();
            }
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;texttemplateconsolebase&quot;]/LoadTemplate/*"/>
        public new void LoadTemplate(string filePath)
        {
            try
            {
                string fullFilePath = GetRootedPath(filePath, true);
                (string templateDirectory, string templateFileName) = PathValidater.ValidatePath(fullFilePath, true, true);
                string templateFilePath = Path.Combine(templateDirectory, templateFileName);
                base.LoadTemplate(templateFilePath);
            }
            catch (Exception ex)
            {
                ConsoleLogger.Log(LogEntryType.Loading, MsgUnableToLoadTemplateFile, ex.Message);
                ResetAll();
            }

            ConsoleWriter.WriteLogEntries();
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;texttemplateconsolebase&quot;]/SetOutputDirectory/*"/>
        public void SetOutputDirectory(string directoryPath)
        {
            try
            {
                string fullDirectoryPath = GetRootedPath(directoryPath);
                (string outputDirectoryPath, string _) = PathValidater.ValidatePath(fullDirectoryPath, false, false);
                OutputDirectory = outputDirectoryPath;

                if (!Directory.Exists(OutputDirectory))
                {
                    Directory.CreateDirectory(OutputDirectory);
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.Log(LogEntryType.Setup, MsgUnableToSetOutputDirectory, ex.Message);
                OutputDirectory = string.Empty;
            }

            ConsoleWriter.WriteLogEntries();
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;texttemplateconsolebase&quot;]/WriteGeneratedTextToFile/*"/>
        public new void WriteGeneratedTextToFile(string fileName, bool resetGeneratedText = true)
        {
            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                ConsoleLogger.Log(LogEntryType.Writing, MsgOutputDirectoryNotSet);
            }
            else
            {
                string filePath = string.IsNullOrWhiteSpace(fileName)
                    ? OutputDirectory
                    : Path.Combine(OutputDirectory, fileName);

                base.WriteGeneratedTextToFile(filePath, resetGeneratedText);
            }

            ConsoleWriter.WriteLogEntries();
        }

        private string GetRootedPath(string path, bool isFilePath = false)
        {
            if (path is null)
            {
                string msg = isFilePath ? MsgNullFilePath : MsgNullDirectoryPath;
                throw new Exception(msg);
            }
            else if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }
            else if (Path.IsPathRooted(path))
            {
                return path;
            }
            else if (string.IsNullOrWhiteSpace(SolutionDirectory))
            {
                throw new Exception(MsgFullPathCannotBeDetermined);
            }

            return Path.Combine(SolutionDirectory, path);
        }

        private string GetSolutionDirectory()
        {
            string? path = Path.GetDirectoryName(GetType().Assembly.Location);
            int pathIndex;

            if (path is null)
            {
                ConsoleLogger.Log(LogEntryType.Setup, MsgUnableToLocateSolutionDirectory);
                return string.Empty;
            }

            while (true)
            {
                pathIndex = path.LastIndexOf(_directorySeparator);

                if (pathIndex < 0)
                {
                    ConsoleLogger.Log(LogEntryType.Setup, MsgUnableToLocateSolutionDirectory);
                    return string.Empty;
                }

                path = path[..pathIndex];

                string[] files = Directory.GetFiles(path, SolutionFileSearchPattern);

                if (files.Length > 0)
                {
                    ConsoleLogger.Log(LogEntryType.Setup, MsgFoundSolutionDirectoryPath, path);
                    return path;
                }
            }
        }
    }
}