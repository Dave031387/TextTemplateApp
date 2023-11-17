namespace ModelWrapperBase
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <include file="docs.xml" path="docs/members[@name=&quot;observable&quot;]/Observable/*"/>
    public abstract class Observable : INotifyPropertyChanged
    {
        /// <include file="docs.xml" path="docs/members[@name=&quot;observable&quot;]/PropertyChanged/*"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <include file="docs.xml" path="docs/members[@name=&quot;observable&quot;]/OnPropertyChanged/*"/>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}