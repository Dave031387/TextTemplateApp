namespace TemplateProcessor.Logger
{
    using System.Collections.Generic;

    /// <include file="../docs.xml" path="docs/members[@name=&quot;consolelogger&quot;]/ConsoleLogger/*"/>
    internal static class ConsoleLogger
    {
        private static readonly List<LogEntry> _logEntries;

        static ConsoleLogger() => _logEntries = new();

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolelogger&quot;]/LogEntries/*"/>
        internal static IEnumerable<LogEntry> LogEntries => _logEntries;

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolelogger&quot;]/Clear/*"/>
        internal static void Clear() => _logEntries.Clear();

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolelogger&quot;]/Log1/*"/>
        internal static void Log(LogEntryType type, string message) => _logEntries.Add(new(type, string.Empty, 0, message));

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolelogger&quot;]/Log2/*"/>
        internal static void Log(LogEntryType type, (string segmentName, int lineNumber) location, string message)
        {
            switch (type)
            {
                case LogEntryType.Setup:
                case LogEntryType.Loading:
                case LogEntryType.Writing:
                case LogEntryType.Reset:
                    Log(type, message);
                    break;

                case LogEntryType.Parsing:
                case LogEntryType.Generating:
                    _logEntries.Add(new(type, location.segmentName, location.lineNumber, message));
                    break;

                default:
                    break;
            }
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolelogger&quot;]/Log3/*"/>
        internal static void Log(LogEntryType type, string message, string arg)
        {
            string formattedMessage = string.Format(message, arg);
            Log(type, formattedMessage);
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolelogger&quot;]/Log4/*"/>
        internal static void Log(LogEntryType type, string message, string arg1, string arg2)
        {
            string formattedMessage = string.Format(message, arg1, arg2);
            Log(type, formattedMessage);
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolelogger&quot;]/Log5/*"/>
        internal static void Log(LogEntryType type, (string segmentName, int lineNumber) location, string message, string arg)
        {
            string formattedMessage = string.Format(message, arg);
            Log(type, location, formattedMessage);
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolelogger&quot;]/Log6/*"/>
        internal static void Log(LogEntryType type, (string segmentName, int lineNumber) location, string message, string arg1, string arg2)
        {
            string formattedMessage = string.Format(message, arg1, arg2);
            Log(type, location, formattedMessage);
        }
    }
}