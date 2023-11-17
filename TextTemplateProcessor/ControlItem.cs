namespace TemplateProcessor
{
    /// <include file="docs.xml" path="docs/members[@name=&quot;controlitem&quot;]/ControlItem/*"/>
    internal class ControlItem
    {
        /// <include file="docs.xml" path="docs/members[@name=&quot;controlitem&quot;]/Constructor/*"/>
        internal ControlItem()
        {
            IsFirstTime = true;
            FirstTimeIndent = 0;
            PadSegment = null;
            TabSize = 0;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;controlitem&quot;]/FirstTimeIndent/*"/>
        internal int FirstTimeIndent { get; set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;controlitem&quot;]/IsFirstTime/*"/>
        internal bool IsFirstTime { get; set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;controlitem&quot;]/PadSegment/*"/>
        internal string? PadSegment { get; set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;controlitem&quot;]/TabSize/*"/>
        internal int TabSize { get; set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;controlitem&quot;]/ShouldGeneratePadSegment/*"/>
        internal bool ShouldGeneratePadSegment => !(string.IsNullOrEmpty(PadSegment) || IsFirstTime);

        /// <include file="docs.xml" path="docs/members[@name=&quot;controlitem&quot;]/Equals/*"/>
        public override bool Equals(object? obj)
        {
            return obj != null && (obj is ControlItem controlItem
                ? FirstTimeIndent == controlItem.FirstTimeIndent
                    && IsFirstTime == controlItem.IsFirstTime
                    && PadSegment == controlItem.PadSegment
                    && TabSize == controlItem.TabSize
                : base.Equals(obj));
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;controlitem&quot;]/GetHashCode/*"/>
        public override int GetHashCode()
            => FirstTimeIndent.GetHashCode()
            ^ IsFirstTime.GetHashCode()
            ^ PadSegment?.GetHashCode()
            ^ TabSize.GetHashCode()
            ?? 0;

        /// <include file="docs.xml" path="docs/members[@name=&quot;controlitem&quot;]/ToString/*"/>
        public override string ToString() => $"Is first time: {IsFirstTime} / FTI: {FirstTimeIndent} / PAD: {PadSegment} / TAB: {TabSize}";
    }
}