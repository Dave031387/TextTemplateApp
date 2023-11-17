namespace TemplateProcessor
{
    using System.Collections.Generic;
    using TemplateProcessor.Logger;
    using static TemplateProcessor.Messages.Messages;

    /// <include file="docs.xml" path="docs/members[@name=&quot;templateloader&quot;]/TemplateLoader/*"/>
    internal class TemplateLoader
    {
        private readonly TextTemplateProcessor _templateProcessor;
        private int _textLineCount = 0;

        /// <include file="docs.xml" path="docs/members[@name=&quot;templateloader&quot;]/Constructor/*"/>
        internal TemplateLoader(TextTemplateProcessor templateProcessor) => _templateProcessor = templateProcessor;

        private Dictionary<string, ControlItem> ControlDictionary => _templateProcessor.ControlDictionary;

        private Dictionary<string, List<TextItem>> SegmentDictionary => _templateProcessor.SegmentDictionary;

        /// <include file="docs.xml" path="docs/members[@name=&quot;templateloader&quot;]/LoadTemplate/*"/>
        internal void LoadTemplate(IEnumerable<string> templateLines)
        {
            Locater.LineNumber = 0;

            foreach (string templateLine in templateLines)
            {
                Locater.LineNumber++;

                if (TextLineParser.IsValidPrefix(templateLine))
                {
                    ParseTemplateLine(templateLine);
                }
                else
                {
                    CheckForMissingSegmentHeader();
                }
            }

            CheckForEmptySegment();
        }

        private void AddSegmentToControlDictionary(string segmentName, ControlItem controlItem)
        {
            if (ControlDictionary.ContainsKey(segmentName))
            {
                Locater.CurrentSegment = DefaultSegmentNameGenerator.Next;
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgFoundDuplicateSegmentName, segmentName, Locater.CurrentSegment);
            }

            if (IsPadSegmentInvalid(segmentName, controlItem.PadSegment))
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgPadSegmentsMustBeDefinedEarlier, segmentName, controlItem.PadSegment!);
                controlItem.PadSegment = null;
            }

            ControlDictionary[Locater.CurrentSegment] = controlItem;
            _textLineCount = 0;
            ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgSegmentHasBeenAdded);
        }

        private void AddTextItemToSegmentDictionary(TextItem textItem)
        {
            if (SegmentDictionary.ContainsKey(Locater.CurrentSegment))
            {
                SegmentDictionary[Locater.CurrentSegment].Add(textItem);
            }
            else
            {
                SegmentDictionary.Add(Locater.CurrentSegment, new List<TextItem>() { textItem });
            }

            _textLineCount++;
        }

        private void CheckForEmptySegment()
        {
            if (Locater.IsValidSegmentHeader && _textLineCount == 0)
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgNoTextLinesFollowingSegmentHeader, Locater.CurrentSegment);
            }
        }

        private void CheckForMissingSegmentHeader()
        {
            if (Locater.IsMissingSegmentHeader)
            {
                CreateDefaultSegment();
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgMissingInitialSegmentHeader, Locater.CurrentSegment);
            }
        }

        private void CreateDefaultSegment()
        {
            Locater.CurrentSegment = DefaultSegmentNameGenerator.Next;
            ControlDictionary[Locater.CurrentSegment] = new();
            _textLineCount = 0;
        }

        private bool IsPadSegmentInvalid(string segmentName, string? padSegment)
            => !string.IsNullOrEmpty(padSegment) && (!ControlDictionary.ContainsKey(padSegment) || padSegment == segmentName);

        private void ParseTemplateLine(string templateLine)
        {
            if (TextLineParser.IsCommentLine(templateLine))
            {
                return;
            }
            else if (TextLineParser.IsSegmentHeader(templateLine))
            {
                CheckForEmptySegment();

                ControlItem controlItem = SegmentHeaderParser.ParseSegmentHeader(templateLine);
                AddSegmentToControlDictionary(Locater.CurrentSegment, controlItem);
            }
            else
            {
                CheckForMissingSegmentHeader();

                TextItem textItem = TextLineParser.ParseTextLine(templateLine);
                AddTextItemToSegmentDictionary(textItem);
            }
        }
    }
}