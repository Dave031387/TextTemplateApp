// Ignore Spelling: Validater

namespace TemplateProcessor
{
    using System.Text.RegularExpressions;

    /// <include file="docs.xml" path="docs/members[@name=&quot;namevalidater&quot;]/NameValidater/*"/>
    internal static class NameValidater
    {
        private static readonly Regex _valid = new("^([A-Z]|[a-z])+([A-Z]|[a-z]|[0-9]|_)*$");

        /// <include file="docs.xml" path="docs/members[@name=&quot;namevalidater&quot;]/IsValidName/*"/>
        internal static bool IsValidName(string? identifier) => identifier is not null && _valid.IsMatch(identifier);
    }
}