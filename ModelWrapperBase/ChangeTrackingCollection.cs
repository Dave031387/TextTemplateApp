namespace ModelWrapperBase
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <include file="docs.xml" path="docs/members[@name=&quot;changetrackingcollection&quot;]/ChangeTrackingCollection/*"/>
    public class ChangeTrackingCollection<T> : ObservableCollection<T>, IValidatingTrackingObject
        where T : class, IValidatingTrackingObject
    {
        private readonly ObservableCollection<T> _addedItems;
        private readonly ObservableCollection<T> _modifiedItems;
        private readonly ObservableCollection<T> _removedItems;
        private List<T> _originalCollection;

        /// <include file="docs.xml" path="docs/members[@name=&quot;changetrackingcollection&quot;]/Constructor/*"/>
        public ChangeTrackingCollection(IEnumerable<T> items) : base(items)
        {
            _originalCollection = this.ToList();

            AttachItemPropertyChangedHandler(_originalCollection);

            _addedItems = new ObservableCollection<T>();
            _removedItems = new ObservableCollection<T>();
            _modifiedItems = new ObservableCollection<T>();

            AddedItems = new ReadOnlyObservableCollection<T>(_addedItems);
            RemovedItems = new ReadOnlyObservableCollection<T>(_removedItems);
            ModifiedItems = new ReadOnlyObservableCollection<T>(_modifiedItems);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;changetrackingcollection&quot;]/AddedItems/*"/>
        public ReadOnlyObservableCollection<T> AddedItems { get; private set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;changetrackingcollection&quot;]/IsChanged/*"/>
        public bool IsChanged => AddedItems.Count > 0 || RemovedItems.Count > 0 || ModifiedItems.Count > 0;

        /// <include file="docs.xml" path="docs/members[@name=&quot;changetrackingcollection&quot;]/IsValid/*"/>
        public bool IsValid => this.All(r => r.IsValid);

        /// <include file="docs.xml" path="docs/members[@name=&quot;changetrackingcollection&quot;]/ModifiedItems/*"/>
        public ReadOnlyObservableCollection<T> ModifiedItems { get; private set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;changetrackingcollection&quot;]/RemovedItems/*"/>
        public ReadOnlyObservableCollection<T> RemovedItems { get; private set; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;changetrackingcollection&quot;]/AcceptChanges/*"/>
        public void AcceptChanges()
        {
            if (IsChanged && IsValid)
            {
                foreach (T item in this)
                {
                    item.AcceptChanges();
                }

                _originalCollection = this.ToList();
                ClearTrackingItems();
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;changetrackingcollection&quot;]/RejectChanges/*"/>
        public void RejectChanges()
        {
            if (IsChanged)
            {
                foreach (T addedItem in _addedItems.ToList())
                {
                    _ = Remove(addedItem);
                }

                foreach (T removedItem in _removedItems.ToList())
                {
                    Add(removedItem);
                }

                foreach (T modifiedItem in _modifiedItems.ToList())
                {
                    modifiedItem.RejectChanges();
                }

                ClearTrackingItems();
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;changetrackingcollection&quot;]/OnCollectionChanged/*"/>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            IEnumerable<T> added = this.Where(current => _originalCollection.All(orig => orig != current));
            IEnumerable<T> removed = _originalCollection.Where(orig => this.All(current => current != orig));
            IEnumerable<T> modified = this.Except(added).Except(removed).Where(item => item.IsChanged);

            AttachItemPropertyChangedHandler(added);
            DetachItemPropertyChangedHandler(removed);

            UpdateChangeTrackingCollection(_addedItems, added);
            UpdateChangeTrackingCollection(_removedItems, removed);
            UpdateChangeTrackingCollection(_modifiedItems, modified);

            base.OnCollectionChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsValid)));
        }

        private static void UpdateChangeTrackingCollection(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        private void AttachItemPropertyChangedHandler(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
        }

        private void ClearTrackingItems()
        {
            _addedItems.Clear();
            _modifiedItems.Clear();
            _removedItems.Clear();
        }

        private void DetachItemPropertyChangedHandler(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                item.PropertyChanged -= ItemPropertyChanged;
            }
        }

        private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsValid))
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsValid)));
            }
            else if (sender is T item)
            {
                if (_addedItems.Contains(item))
                {
                    return;
                }

                if (item.IsChanged)
                {
                    if (!_modifiedItems.Contains(item))
                    {
                        _modifiedItems.Add(item);
                    }
                }
                else
                {
                    if (_modifiedItems.Contains(item))
                    {
                        _ = _modifiedItems.Remove(item);
                    }
                }

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            }
        }
    }
}