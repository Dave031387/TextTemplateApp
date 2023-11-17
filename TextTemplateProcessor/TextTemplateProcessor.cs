namespace TemplateProcessor
{
    using TemplateProcessor.IO;
    using TemplateProcessor.Logger;
    using static TemplateProcessor.Messages.Messages;

    /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/TextTemplateProcessor/*"/>
    public class TextTemplateProcessor : ITextTemplateProcessor
    {
        private const int DefaultTabSize = 4;
        private readonly Dictionary<string, ControlItem> _controlDictionary;
        private readonly List<string> _generatedText;
        private readonly Dictionary<string, List<TextItem>> _segmentDictionary;
        private readonly ITextReader _textReader;
        private readonly ITextWriter _textWriter;

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/Constructor1/*"/>
        public TextTemplateProcessor(string filePath) : this(new TextReader(filePath), new TextWriter())
        {
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/Constructor2/*"/>
        internal TextTemplateProcessor(ITextReader templateReader, ITextWriter templateWriter)
        {
            _textReader = templateReader;
            _textWriter = templateWriter;
            _segmentDictionary = new();
            _controlDictionary = new();
            _generatedText = new();
            IndentProcessor.SetTabSize(DefaultTabSize);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/CurrentIndent/*"/>
        public int CurrentIndent => IndentProcessor.CurrentIndent;

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/GeneratedText/*"/>
        public IEnumerable<string> GeneratedText => _generatedText;

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/IsOutputFileWritten/*"/>
        public bool IsOutputFileWritten { get; private set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/IsTemplateLoaded/*"/>
        public bool IsTemplateLoaded { get; private set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/TabSize/*"/>
        public int TabSize => IndentProcessor.TabSize;

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/TemplateFilePath/*"/>
        public string TemplateFilePath => _textReader.FullFilePath;

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/CurrentSegment/*"/>
        internal static string CurrentSegment
        {
            get => Locater.CurrentSegment;
            private set => Locater.CurrentSegment = value;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/LineNumber/*"/>
        internal static int LineNumber
        {
            get => Locater.LineNumber;
            private set => Locater.LineNumber = value;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/ControlDictionary/*"/>
        internal Dictionary<string, ControlItem> ControlDictionary => _controlDictionary;

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/SegmentDictionary/*"/>
        internal Dictionary<string, List<TextItem>> SegmentDictionary => _segmentDictionary;

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/GenerateSegment/*"/>
        public void GenerateSegment(string segmentName, Dictionary<string, string>? tokenDictionary = null)
        {
            CurrentSegment = segmentName is null ? string.Empty : segmentName;
            LineNumber = 0;

            if (SegmentCanBeGenerated(CurrentSegment))
            {
                ControlItem controlItem = ControlDictionary[CurrentSegment];

                if (tokenDictionary is not null)
                {
                    TokenProcessor.LoadTokenValues(tokenDictionary);
                }

                if (controlItem.ShouldGeneratePadSegment)
                {
                    IndentProcessor.SaveCurrentIndentLocation();
                    GenerateSegment(controlItem.PadSegment!);
                    IndentProcessor.RestoreCurrentIndentLocation();
                }

                if (controlItem.TabSize > 0)
                {
                    SetTabSize(controlItem.TabSize);
                }

                foreach (TextItem textItem in SegmentDictionary[CurrentSegment])
                {
                    LineNumber++;
                    GenerateTextLine(controlItem, textItem);
                }

                IsOutputFileWritten = false;
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/LoadTemplate1/*"/>
        public void LoadTemplate()
        {
            if (IsTemplateLoaded)
            {
                ConsoleLogger.Log(LogEntryType.Loading, MsgAttemptToLoadMoreThanOnce, _textReader.FileName);
            }
            else if (IsValidTemplateFilePath())
            {
                ResetAll(false);
                LoadTemplateLines();
            }
            else
            {
                ConsoleLogger.Log(LogEntryType.Loading, MsgUnableToLoadTemplate);
                IsTemplateLoaded = false;
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/LoadTemplate2/*"/>
        public void LoadTemplate(string filePath)
        {
            string lastFileName = _textReader.FileName;
            string lastFilePath = TemplateFilePath;

            if (IsValidTemplateFilePath(filePath))
            {
                if (IsTemplateLoaded && TemplateFilePath == lastFilePath)
                {
                    ConsoleLogger.Log(LogEntryType.Loading, MsgAttemptToLoadMoreThanOnce, lastFileName);
                    return;
                }

                bool isOutputFileWritten = IsOutputFileWritten;
                ResetAll(false);

                if (!(isOutputFileWritten || string.IsNullOrEmpty(lastFileName)))
                {
                    ConsoleLogger.Log(LogEntryType.Loading, MsgNextLoadRequestBeforeFirstIsWritten, _textReader.FileName, lastFileName);
                }

                LoadTemplateLines();
            }
            else
            {
                IsTemplateLoaded = false;
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/ResetAll/*"/>
        public void ResetAll()
        {
            ResetAll(true);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/ResetGeneratedText/*"/>
        public void ResetGeneratedText()
        {
            ResetGeneratedText(true);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/ResetSegment/*"/>
        public void ResetSegment(string segmentName)
        {
            CurrentSegment = segmentName is null ? string.Empty : segmentName;
            LineNumber = 0;

            if (segmentName is not null && ControlDictionary.ContainsKey(CurrentSegment))
            {
                ControlDictionary[CurrentSegment].IsFirstTime = true;
            }
            else
            {
                ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgUnableToResetSegment, CurrentSegment);
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/SetTabSize/*"/>
        public void SetTabSize(int tabSize) => IndentProcessor.SetTabSize(tabSize);

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/WriteGeneratedTextToFile/*"/>
        public void WriteGeneratedTextToFile(string filePath, bool resetGeneratedText = true)
        {
            if (_textWriter.CreateOutputFile(filePath, _generatedText))
            {
                if (resetGeneratedText)
                {
                    ResetGeneratedText();
                }

                IsOutputFileWritten = true;
            }
        }

        private static bool IsEmptyTemplateFile(IEnumerable<string> textLines)
            => !textLines.Any() || (textLines.Count() == 1 && string.IsNullOrWhiteSpace(textLines.FirstOrDefault()));

        private void GenerateTextLine(ControlItem controlItem, TextItem textItem)
        {
            int indent;

            if (controlItem.IsFirstTime)
            {
                indent = IndentProcessor.GetFirstTimeIndent(controlItem.FirstTimeIndent, textItem);
                controlItem.IsFirstTime = false;
            }
            else
            {
                indent = IndentProcessor.GetIndent(textItem);
            }

            string pad = new(' ', indent);
            string text = TokenProcessor.ReplaceTokens(textItem.Text);
            _generatedText.Add(pad + text);
        }

        private bool IsValidTemplateFilePath() => !string.IsNullOrEmpty(_textReader.FullFilePath);

        private bool IsValidTemplateFilePath(string filePath)
        {
            _textReader.SetFilePath(filePath);
            return IsValidTemplateFilePath();
        }

        private void LoadTemplateLines()
        {
            List<string> templateLines = _textReader.ReadTextFile().ToList();

            if (IsEmptyTemplateFile(templateLines))
            {
                ConsoleLogger.Log(LogEntryType.Loading, MsgTemplateFileIsEmpty);
                IsTemplateLoaded = false;
            }
            else
            {
                TemplateLoader loader = new(this);

                ConsoleLogger.Log(LogEntryType.Loading, MsgLoadingTemplateFile, _textReader.FileName);
                loader.LoadTemplate(templateLines);
                IsTemplateLoaded = true;
                IsOutputFileWritten = false;
            }
        }

        private void ResetAll(bool shouldDisplayMessage)
        {
            ResetGeneratedText(false);
            _segmentDictionary.Clear();
            _controlDictionary.Clear();
            IsTemplateLoaded = false;
            IsOutputFileWritten = false;
            DefaultSegmentNameGenerator.Reset();
            TokenProcessor.ClearTokens();

            if (shouldDisplayMessage)
            {
                ConsoleLogger.Log(LogEntryType.Reset, MsgTemplateHasBeenReset, _textReader.FileName);
            }
        }

        private void ResetGeneratedText(bool shouldDisplayMessage)
        {
            _generatedText.Clear();
            Locater.Reset();
            IndentProcessor.Reset();
            IndentProcessor.SetTabSize(DefaultTabSize);

            foreach (string segmentName in _controlDictionary.Keys)
            {
                _controlDictionary[segmentName].IsFirstTime = true;
            }

            if (shouldDisplayMessage)
            {
                ConsoleLogger.Log(LogEntryType.Reset, MsgGeneratedTextHasBeenReset, _textReader.FileName);
            }
        }

        private bool SegmentCanBeGenerated(string segmentName)
        {
            bool result = false;

            if (IsTemplateLoaded)
            {
                if (ControlDictionary.ContainsKey(segmentName))
                {
                    if (SegmentDictionary.ContainsKey(segmentName))
                    {
                        ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgProcessingSegment);
                        result = true;
                    }
                    else
                    {
                        ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgSegmentHasNoTextLines, segmentName);
                    }
                }
                else
                {
                    ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgUnknownSegmentName, segmentName);
                }
            }
            else
            {
                ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgAttemptToGenerateSegmentBeforeItWasLoaded, segmentName);
            }

            return result;
        }
    }
}