namespace TestShared
{
    using FluentAssertions;
    using System.Collections.Generic;
    using TemplateProcessor.Logger;

    internal static class ConsoleLogValidater
    {
        internal static void AssertLogContents(int numberOfLogEntries, IEnumerable<LogEntry>? logEntries = null)
        {
            if (numberOfLogEntries == 0)
            {
                ConsoleLogger.LogEntries
                    .Should()
                    .BeEmpty();
            }
            else if (numberOfLogEntries > 0)
            {
                ConsoleLogger.LogEntries
                    .Should()
                    .HaveCount(numberOfLogEntries);
            }

            if (logEntries is not null)
            {
                ConsoleLogger.LogEntries
                    .Should()
                    .Contain(logEntries);
            }
        }
    }
}