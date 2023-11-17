namespace TemplateProcessor.IO
{
    using System.Collections.Generic;

    /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/ITextReader/*"/>
    internal interface ITextReader
    {
        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/DirectoryPath/*"/>
        string DirectoryPath { get; }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/FileName/*"/>
        string FileName { get; }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/FullFilePath/*"/>
        string FullFilePath { get; }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/ReadTextFile/*"/>
        IEnumerable<string> ReadTextFile();

        /// <include file="../docs.xml" path="docs/members[@name=&quot;textreader&quot;]/SetFilePath/*"/>
        void SetFilePath(string filePath);
    }
}