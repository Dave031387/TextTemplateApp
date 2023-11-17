namespace TemplateProcessor.IO
{
    using System.Collections.Generic;

    /// <include file="../docs.xml" path="docs/members[@name=&quot;textwriter&quot;]/ITextWriter/*"/>
    internal interface ITextWriter
    {
        /// <include file="../docs.xml" path="docs/members[@name=&quot;textwriter&quot;]/CreateOutputFile/*"/>
        bool CreateOutputFile(string filePath, IEnumerable<string> textLines);
    }
}