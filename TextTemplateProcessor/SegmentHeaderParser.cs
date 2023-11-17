namespace TemplateProcessor
{
    using System;
    using TemplateProcessor.Logger;
    using static TemplateProcessor.Messages.Messages;

    /// <include file="docs.xml" path="docs/members[@name=&quot;segmentheaderparser&quot;]/SegmentHeaderParser/*"/>
    internal static class SegmentHeaderParser
    {
        private const string FirstTimeIndentOption = "FTI";
        private const string PadSegmentNameOption = "PAD";
        private const string TabSizeOption = "TAB";

        /// <include file="docs.xml" path="docs/members[@name=&quot;segmentheaderparser&quot;]/ParseSegmentHeader/*"/>
        internal static ControlItem ParseSegmentHeader(string textLine)
        {
            ControlItem controlItem = new();

            if (textLine.Length < 5 || textLine[4] == ' ')
            {
                Locater.CurrentSegment = DefaultSegmentNameGenerator.Next;
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgSegmentNameMustStartInColumn5, Locater.CurrentSegment);
                return controlItem;
            }

            string[] args = textLine.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string segmentName = args[1];

            if (NameValidater.IsValidName(segmentName))
            {
                Locater.CurrentSegment = segmentName;
            }
            else
            {
                Locater.CurrentSegment = DefaultSegmentNameGenerator.Next;
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgInvalidSegmentName, segmentName, Locater.CurrentSegment);
            }

            if (args.Length > 2)
            {
                controlItem = ParseSegmentOptions(args);
            }

            return controlItem;
        }

        private static (string optionName, string optionValue) ParseSegmentOption(string arg)
        {
            (string optionName, string optionValue) errorResult = (string.Empty, string.Empty);

            int optionIndex;

            if (arg.Contains('='))
            {
                optionIndex = arg.IndexOf('=');
            }
            else
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgInvalidFormOfOption, arg);
                return errorResult;
            }

            if (optionIndex < 1)
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgOptionNameMustPrecedeEqualsSign, Locater.CurrentSegment);
                return errorResult;
            }

            string optionName = arg[..optionIndex].ToUpperInvariant();

            if (optionName is not FirstTimeIndentOption and not PadSegmentNameOption and not TabSizeOption)
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgUnknownSegmentOptionFound, Locater.CurrentSegment, arg);
                return errorResult;
            }

            optionIndex++;

            if (optionIndex == arg.Length)
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgOptionValueMustFollowEqualsSign, Locater.CurrentSegment, optionName);
                return errorResult;
            }

            string optionValue = arg[optionIndex..];

            return (optionName, optionValue);
        }

        private static ControlItem ParseSegmentOptions(string[] args)
        {
            ControlItem controlItem = new();
            bool firstTimeIndentOptionFound = false;
            bool padSegmentOptionFound = false;
            bool tabOptionFound = false;

            for (int i = 2; i < args.Length; i++)
            {
                (string optionName, string optionValue) = ParseSegmentOption(args[i]);

                if (string.IsNullOrEmpty(optionName))
                {
                    continue;
                }

                if ((optionName == FirstTimeIndentOption && firstTimeIndentOptionFound)
                    || (optionName == PadSegmentNameOption && padSegmentOptionFound)
                    || (optionName == TabSizeOption && tabOptionFound))
                {
                    ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgFoundDuplicateOptionNameOnHeaderLine, Locater.CurrentSegment, optionName);
                    continue;
                }

                switch (optionName)
                {
                    case FirstTimeIndentOption:
                        SetFirstTimeIndentOption(controlItem, optionValue);
                        firstTimeIndentOptionFound = true;
                        break;

                    case PadSegmentNameOption:
                        SetPadSegmentOption(controlItem, optionValue);
                        padSegmentOptionFound = true;
                        break;

                    case TabSizeOption:
                        SetTabSizeOption(controlItem, optionValue);
                        tabOptionFound = true;
                        break;

                    default:
                        break;
                }
            }

            return controlItem;
        }

        private static void SetFirstTimeIndentOption(ControlItem controlItem, string optionValue)
        {
            if (IndentProcessor.IsValidIndentValue(optionValue, out int indentValue))
            {
                if (indentValue == 0)
                {
                    ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgFirstTimeIndentSetToZero);
                }

                controlItem.FirstTimeIndent = indentValue;
            }
        }

        private static void SetPadSegmentOption(ControlItem controlItem, string optionValue)
        {
            if (NameValidater.IsValidName(optionValue))
            {
                controlItem.PadSegment = optionValue;
            }
            else
            {
                ConsoleLogger.Log(LogEntryType.Parsing, Locater.Location, MsgInvalidPadSegmentName, optionValue, Locater.CurrentSegment);
            }
        }

        private static void SetTabSizeOption(ControlItem controlItem, string optionValue)
        {
            if (IndentProcessor.IsValidTabSizeValue(optionValue, out int tabValue))
            {
                controlItem.FirstTimeIndent = tabValue;
            }
        }
    }
}