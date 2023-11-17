namespace TemplateProcessor
{
    using TemplateProcessor.Logger;
    using static TemplateProcessor.Messages.Messages;

    /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/IndentProcessor/*"/>
    internal static class IndentProcessor
    {
        private const int MaxIndentValue = 9;
        private const int MaxTabSize = 9;
        private const int MinIndentValue = -9;
        private const int MinTabSize = 1;
        private static bool _isCurrentIndentSaved = false;
        private static int _saveCurrentIndent = 0;
        private static string _saveCurrentSegment = string.Empty;
        private static int _saveLineNumber = 0;
        private static int _saveTabSize = 0;

        /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/CurrentIndent/*"/>
        internal static int CurrentIndent { get; private set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/TabSize/*"/>
        internal static int TabSize { get; private set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/GetFirstTimeIndent/*"/>
        internal static int GetFirstTimeIndent(int firstTimeIndent, TextItem textItem)
        {
            int indent = CurrentIndent;

            if (firstTimeIndent != 0)
            {
                indent += firstTimeIndent * TabSize;

                if (indent < 0)
                {
                    ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgFirstTimeIndentHasBeenTruncated, Locater.CurrentSegment);
                    indent = 0;
                }

                CurrentIndent = indent;
            }
            else
            {
                indent = GetIndent(textItem);
            }

            return indent;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/GetIndent/*"/>
        internal static int GetIndent(TextItem textItem)
        {
            int indent = CurrentIndent;

            if (textItem.IsRelative)
            {
                indent += textItem.Indent * TabSize;
            }
            else
            {
                indent = textItem.Indent * TabSize;
            }

            if (indent < 0)
            {
                ConsoleLogger.Log(LogEntryType.Generating, Locater.Location, MsgLeftIndentHasBeenTruncated, Locater.CurrentSegment);
                indent = 0;
            }

            if (!textItem.IsOneTime)
            {
                CurrentIndent = indent;
            }

            return indent;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/IsValidIndentValue/*"/>
        internal static bool IsValidIndentValue(string stringValue, out int indent)
        {
            if (int.TryParse(stringValue, out int indentValue))
            {
                if (indentValue is < MinIndentValue or > MaxIndentValue)
                {
                    ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgIndentValueOutOfRange, indentValue.ToString());
                }
                else
                {
                    indent = indentValue;
                    return true;
                }
            }
            else
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgIndentValueMustBeValidNumber, stringValue);
            }

            indent = 0;
            return false;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/IsValidTabSizeValue/*"/>
        internal static bool IsValidTabSizeValue(string stringValue, out int tabSize)
        {
            if (int.TryParse(stringValue, out int tabValue))
            {
                if (tabValue is < MinTabSize or > MaxTabSize)
                {
                    ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgTabSizeValueOutOfRange, tabValue.ToString());
                }
                else
                {
                    tabSize = tabValue;
                    return true;
                }
            }
            else
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgTabSizeValueMustBeValidNumber, stringValue);
            }

            tabSize = 0;
            return false;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/Reset/*"/>
        internal static void Reset() => CurrentIndent = 0;

        /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/RestoreCurrentIndentLocation/*"/>
        internal static void RestoreCurrentIndentLocation()
        {
            if (_isCurrentIndentSaved)
            {
                CurrentIndent = _saveCurrentIndent;
                TabSize = _saveTabSize;
                Locater.CurrentSegment = _saveCurrentSegment;
                Locater.LineNumber = _saveLineNumber;
                _isCurrentIndentSaved = false;
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/SaveCurrentIndentLocation/*"/>
        internal static void SaveCurrentIndentLocation()
        {
            _saveCurrentIndent = CurrentIndent;
            _saveTabSize = TabSize;
            _saveCurrentSegment = Locater.CurrentSegment;
            _saveLineNumber = Locater.LineNumber;
            _isCurrentIndentSaved = true;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;indentprocessor&quot;]/SetTabSize/*"/>
        internal static void SetTabSize(int tabSize)
        {
            if (tabSize < MinTabSize)
            {
                ConsoleLogger.Log(LogEntryType.Setup, MsgTabSizeTooSmall, MinTabSize.ToString());
                TabSize = MinTabSize;
            }
            else if (tabSize > MaxTabSize)
            {
                ConsoleLogger.Log(LogEntryType.Setup, MsgTabSizeTooLarge, MaxTabSize.ToString());
                TabSize = MaxTabSize;
            }
            else
            {
                TabSize = tabSize;
            }
        }
    }
}