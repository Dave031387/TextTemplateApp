namespace TemplateProcessor
{
    using System.Collections.Generic;

    /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/ITextTemplateProcessor/*"/>
    public interface ITextTemplateProcessor
    {
        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/CurrentIndent/*"/>
        int CurrentIndent { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/GeneratedText/*"/>
        IEnumerable<string> GeneratedText { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/IsOutputFileWritten/*"/>
        bool IsOutputFileWritten { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/IsTemplateLoaded/*"/>
        bool IsTemplateLoaded { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/TabSize/*"/>
        int TabSize { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/TemplateFilePath/*"/>
        string TemplateFilePath { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/GenerateSegment/*"/>
        void GenerateSegment(string segmentName, Dictionary<string, string>? tokenDictionary = null);

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/LoadTemplate1/*"/>
        void LoadTemplate();

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/LoadTemplate2/*"/>
        void LoadTemplate(string filePath);

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/ResetAll/*"/>
        void ResetAll();

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/ResetGeneratedText/*"/>
        void ResetGeneratedText();

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/ResetSegment/*"/>
        void ResetSegment(string segmentName);

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/SetTabSize/*"/>
        void SetTabSize(int tabSize);

        /// <include file="docs.xml" path="docs/members[@name=&quot;texttemplateprocessor&quot;]/WriteGeneratedTextToFile/*"/>
        void WriteGeneratedTextToFile(string filePath, bool resetGeneratedText = true);
    }
}