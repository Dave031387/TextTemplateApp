namespace TemplateProcessor
{
    /// <include file="docs.xml" path="docs/members[@name=&quot;defaultsegmentnamegenerator&quot;]/DefaultSegmentNameGenerator/*"/>
    internal static class DefaultSegmentNameGenerator
    {
        private const string DefaultSegmentNamePrefix = "DefaultSegment";
        private static int _defaultSegmentCounter = 0;

        /// <include file="docs.xml" path="docs/members[@name=&quot;defaultsegmentnamegenerator&quot;]/Next/*"/>
        internal static string Next => $"{DefaultSegmentNamePrefix}{++_defaultSegmentCounter}";

        /// <include file="docs.xml" path="docs/members[@name=&quot;defaultsegmentnamegenerator&quot;]/Reset/*"/>
        internal static void Reset() => _defaultSegmentCounter = 0;
    }
}