namespace TemplateProcessor
{
    using TemplateProcessor.Logger;
    using static TemplateProcessor.Messages.Messages;

    /// <include file="docs.xml" path="docs/members[@name=&quot;textlineparser&quot;]/TextLineParser/*"/>
    internal static class TextLineParser
    {
        private const string Comment = "///";
        private const string IndentAbsolute = "@=";
        private const string IndentAbsoluteOneTime = "O=";
        private const string IndentLeftOneTime = "O-";
        private const string IndentLeftRelative = "@-";
        private const string IndentRightOneTime = "O+";
        private const string IndentRightRelative = "@+";
        private const string IndentUnchanged = "   ";
        private const string SegmentHeaderCode = "###";

        /// <include file="docs.xml" path="docs/members[@name=&quot;textlineparser&quot;]/IsCommentLine/*"/>
        internal static bool IsCommentLine(string text) => text[..3] == Comment;

        /// <include file="docs.xml" path="docs/members[@name=&quot;textlineparser&quot;]/IsSegmentHeader/*"/>
        internal static bool IsSegmentHeader(string text) => text[..3] == SegmentHeaderCode;

        /// <include file="docs.xml" path="docs/members[@name=&quot;textlineparser&quot;]/IsValidPrefix/*"/>
        internal static bool IsValidPrefix(string textLine)
        {
            if (textLine.Length < 3)
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgMinimumLineLengthInTemplateFileIs3);
                return false;
            }

            if (textLine.Length > 3 && textLine[3] != ' ')
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgFourthCharacterMustBeBlank);
                return false;
            }

            string controlCode = textLine[..3];

            return IsValidControlCode(controlCode);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;textlineparser&quot;]/ParseTextLine/*"/>
        internal static TextItem ParseTextLine(string templateLine)
        {
            string code = templateLine[..2].ToUpperInvariant();
            string indentString = templateLine[1..3].Replace('=', '+');
            int indent = 0;
            string text = templateLine.Length > 4 ? templateLine[4..] : string.Empty;
            bool isRelative = true;
            bool isOneTime = false;

            if (indentString != "  ")
            {
                _ = int.TryParse(indentString, out indent);
            }

            if (code is IndentAbsoluteOneTime
                     or IndentLeftOneTime
                     or IndentRightOneTime)
            {
                isOneTime = true;
            }

            if (code is IndentAbsolute
                     or IndentAbsoluteOneTime)
            {
                isRelative = false;
            }

            TokenProcessor.ExtractTokens(ref text);

            return new(indent, isRelative, isOneTime, text);
        }

        private static bool IsValidControlCode(string controlCode)
        {
            if (controlCode is SegmentHeaderCode
                            or IndentUnchanged
                            or Comment)
            {
                return true;
            }

            string prefix = controlCode[..2];
            string indent = controlCode[1..3].Replace('=', '+');

            if (prefix is IndentAbsolute
                       or IndentAbsoluteOneTime
                       or IndentLeftOneTime
                       or IndentLeftRelative
                       or IndentRightOneTime
                       or IndentRightRelative)
            {
                return IndentProcessor.IsValidIndentValue(indent, out int _);
            }

            ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgInvalidControlCode, controlCode);
            return false;
        }
    }
}