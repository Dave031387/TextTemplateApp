namespace TemplateProcessor
{
    /// <include file="docs.xml" path="docs/members[@name=&quot;locater&quot;]/Locater/*"/>
    internal static class Locater
    {
        /// <include file="docs.xml" path="docs/members[@name=&quot;locater&quot;]/Constructor/*"/>
        static Locater()
        {
            CurrentSegment = string.Empty;
            LineNumber = 0;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;locater&quot;]/CurrentSegment/*"/>
        internal static string CurrentSegment { get; set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;locater&quot;]/IsMissingSegmentHeader/*"/>
        internal static bool IsMissingSegmentHeader => string.IsNullOrWhiteSpace(CurrentSegment);

        /// <include file="docs.xml" path="docs/members[@name=&quot;locater&quot;]/IsValidSegmentHeader/*"/>
        internal static bool IsValidSegmentHeader => !string.IsNullOrWhiteSpace(CurrentSegment);

        /// <include file="docs.xml" path="docs/members[@name=&quot;locater&quot;]/LineNumber/*"/>
        internal static int LineNumber { get; set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;locater&quot;]/Location/*"/>
        internal static (string currentSegment, int lineNumber) Location => (CurrentSegment, LineNumber);

        /// <include file="docs.xml" path="docs/members[@name=&quot;locater&quot;]/Reset/*"/>
        internal static void Reset()
        {
            CurrentSegment = string.Empty;
            LineNumber = 0;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;locater&quot;]/ToString/*"/>
        internal new static string ToString() => $"{CurrentSegment}[{LineNumber}]";
    }
}