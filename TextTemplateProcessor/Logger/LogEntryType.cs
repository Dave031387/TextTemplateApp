namespace TemplateProcessor.Logger
{
    /// <include file="../docs.xml" path="docs/members[@name=&quot;logentrytype&quot;]/LogEntryType/*"/>
    internal enum LogEntryType
    {
        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentrytype&quot;]/Setup/*"/>
        Setup,

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentrytype&quot;]/Loading/*"/>
        Loading,

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentrytype&quot;]/Parsing/*"/>
        Parsing,

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentrytype&quot;]/Generating/*"/>
        Generating,

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentrytype&quot;]/Writing/*"/>
        Writing,

        /// <include file="../docs.xml" path="docs/members[@name=&quot;logentrytype&quot;]/Reset/*"/>
        Reset
    }
}