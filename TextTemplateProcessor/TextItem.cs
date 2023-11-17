namespace TemplateProcessor
{
#pragma warning disable CS1572 // XML comment has a param tag, but there is no parameter by that name
#pragma warning disable CS1734 // XML comment has a paramref tag, but there is no parameter by that name
    /// <include file="docs.xml" path="docs/members[@name=&quot;textitem&quot;]/TextItem/*"/>
    internal record TextItem(int Indent, bool IsRelative, bool IsOneTime, string Text);
#pragma warning restore CS1734 // XML comment has a paramref tag, but there is no parameter by that name
#pragma warning restore CS1572 // XML comment has a param tag, but there is no parameter by that name
}