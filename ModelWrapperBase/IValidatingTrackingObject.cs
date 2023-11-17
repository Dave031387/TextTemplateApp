namespace ModelWrapperBase
{
    using System.ComponentModel;

    /// <include file="docs.xml" path="docs/members[@name=&quot;validatingtrackingobject&quot;]/IValidatingTrackingObject/*"/>
    public interface IValidatingTrackingObject : IRevertibleChangeTracking, INotifyPropertyChanged
    {
        /// <include file="docs.xml" path="docs/members[@name=&quot;validatingtrackingobject&quot;]/IsValid/*"/>
        bool IsValid { get; }
    }
}