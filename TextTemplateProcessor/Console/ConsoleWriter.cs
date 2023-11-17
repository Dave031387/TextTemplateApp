namespace TemplateProcessor.Console
{
    using System;
    using TemplateProcessor.Logger;

    /// <include file="../docs.xml" path="docs/members[@name=&quot;consolewriter&quot;]/ConsoleWriter/*"/>
    internal static class ConsoleWriter
    {
        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolewriter&quot;]/WriteLine1/*"/>
        internal static void WriteLine(string message) => Console.WriteLine(message);

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolewriter&quot;]/WriteLine2/*"/>
        internal static void WriteLine(string message, string arg) => WriteLine(string.Format(message, arg));

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolewriter&quot;]/WriteLine3/*"/>
        internal static void WriteLine(string message, string arg1, string arg2) => WriteLine(string.Format(message, arg1, arg2));

        /// <include file="../docs.xml" path="docs/members[@name=&quot;consolewriter&quot;]/WriteLogEntries/*"/>
        internal static void WriteLogEntries()
        {
#if WRITE_LOG
            foreach (LogEntry logEntry in ConsoleLogger.LogEntries)
            {
                WriteLine(logEntry.ToString());
            }

            ConsoleLogger.Clear();
#endif
        }
    }
}