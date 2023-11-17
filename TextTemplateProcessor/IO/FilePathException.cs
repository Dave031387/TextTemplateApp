namespace TemplateProcessor.IO
{
    using System;

    /// <include file="../docs.xml" path="docs/members[@name=&quot;filepathexception&quot;]/FilePathException/*"/>
    [Serializable]
    public class FilePathException : Exception
    {
        /// <include file="../docs.xml" path="docs/members[@name=&quot;filepathexception&quot;]/Constructor1/*"/>
        public FilePathException()
        { }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;filepathexception&quot;]/Constructor2/*"/>
        public FilePathException(string message) : base(message)
        {
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;filepathexception&quot;]/Constructor3/*"/>
        public FilePathException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <include file="../docs.xml" path="docs/members[@name=&quot;filepathexception&quot;]/Constructor4/*"/>
        protected FilePathException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}