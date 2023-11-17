namespace TemplateProcessor.Logger
{
    /// <include file="../docs.xml" path="docs/members[@name=&quot;logentry&quot;]/LogEntry/*"/>
    internal class LogEntry
    {
        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentry&quot;]/Constructor/*"/>
        internal LogEntry(LogEntryType logEntryType, string segmentName, int lineNumber, string message)
        {
            LogEntryType = logEntryType;
            SegmentName = segmentName;
            LineNumber = lineNumber;
            Message = message;
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentry&quot;]/LineNumber/*"/>
        internal int LineNumber { get; }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentry&quot;]/LogEntryType/*"/>
        internal LogEntryType LogEntryType { get; }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentry&quot;]/Message/*"/>
        internal string Message { get; }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentry&quot;]/SegmentName/*"/>
        internal string SegmentName { get; }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentry&quot;]/Equals/*"/>
        public override bool Equals(object? obj)
        {
            return obj is not null
                && (obj is LogEntry entry
                ? LineNumber == entry.LineNumber
                    && LogEntryType == entry.LogEntryType
                    && Message == entry.Message
                    && SegmentName == entry.SegmentName
                : base.Equals(obj));
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentry&quot;]/GetHashCode/*"/>
        public override int GetHashCode()
            => LineNumber.GetHashCode()
            ^ LogEntryType.GetHashCode()
            ^ Message.GetHashCode()
            ^ SegmentName.GetHashCode();

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentry&quot;]/ToString/*"/>
        public override string ToString() => string.IsNullOrEmpty(SegmentName)
            ? $"<{LogEntryType}> {Message}"
            : $"<{LogEntryType}> {SegmentName}[{LineNumber}] : {Message}";
    }
}