namespace ModelWrapperBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/ModelWrapper/*"/>
    public abstract class ModelWrapper<T> : NotifyDataErrorInfoBase, IValidatingTrackingObject, IValidatableObject
        where T : class
    {
        private const string IsChangedSuffix = "IsChanged";
        private readonly Dictionary<string, object?> _originalValues;
        private readonly List<IValidatingTrackingObject> _trackingObjects;

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/Constructor/*"/>
        public ModelWrapper(T model)
        {
            ArgumentNullException.ThrowIfNull(model);
            Model = model;
            _originalValues = new Dictionary<string, object?>();
            _trackingObjects = new List<IValidatingTrackingObject>();
            InitializeComplexProperties();
            InitializeCollectionProperties();
            Validate();
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/IsChanged/*"/>
        public bool IsChanged => _originalValues.Count > 0 || _trackingObjects.Any(t => t.IsChanged);

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/IsValid/*"/>
        public bool IsValid => !HasErrors && _trackingObjects.All(r => r.IsValid);

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/Model/*"/>
        public T Model { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/AcceptChanges/*"/>
        public void AcceptChanges()
        {
            if (IsChanged && IsValid)
            {
                _originalValues.Clear();

                foreach (IRevertibleChangeTracking trackingObject in _trackingObjects)
                {
                    trackingObject.AcceptChanges();
                }

                OnPropertyChanged(string.Empty);
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/RejectChanges/*"/>
        public void RejectChanges()
        {
            if (IsChanged)
            {
                foreach (KeyValuePair<string, object?> originalValueEntry in _originalValues)
                {
                    PropertyInfo? property = typeof(T).GetProperty(originalValueEntry.Key);
                    property?.SetValue(Model, originalValueEntry.Value);
                }

                _originalValues.Clear();

                foreach (IRevertibleChangeTracking trackingObject in _trackingObjects)
                {
                    trackingObject.RejectChanges();
                }

                Validate();
                OnPropertyChanged(string.Empty);
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/Validate1/*"/>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/GetIsChanged/*"/>
        protected bool GetIsChanged(string propertyName) => _originalValues.ContainsKey(propertyName);

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/GetOriginalValue/*"/>
        protected TValue? GetOriginalValue<TValue>(string propertyName)
        {
            if (_originalValues.ContainsKey(propertyName))
            {
                TValue? value = (TValue?)_originalValues[propertyName];
                return value is null ? default : value;
            }

            return GetValue<TValue>(propertyName);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/GetValue/*"/>
        protected TValue? GetValue<TValue>([CallerMemberName] string? propertyName = null)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
            TValue? value = (TValue?)propertyInfo.GetValue(Model);
            return value is null ? default : value;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/InitializeCollectionProperties/*"/>
        protected virtual void InitializeCollectionProperties()
        {
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/InitializeComplexProperties/*"/>
        protected virtual void InitializeComplexProperties()
        {
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/RegisterCollection/*"/>
        protected void RegisterCollection<TWrapper, TModel>(ChangeTrackingCollection<TWrapper> wrapperCollection, IList<TModel> modelCollection)
            where TWrapper : ModelWrapper<TModel>
            where TModel : class
        {
            wrapperCollection.CollectionChanged += (s, e) =>
            {
                modelCollection.Clear();
                foreach (TWrapper wrapper in wrapperCollection)
                {
                    modelCollection.Add(wrapper.Model);
                }

                Validate();
            };
            RegisterTrackingObject(wrapperCollection);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/RegisterComplex/*"/>
        protected void RegisterComplex<TModel>(ModelWrapper<TModel> wrapper)
            where TModel : class
        {
            RegisterTrackingObject(wrapper);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/SetValue/*"/>
        protected void SetValue<TValue>(TValue? newValue, [CallerMemberName] string? propertyName = null)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
            object? currentValue = propertyInfo.GetValue(Model);
            if (!Equals(currentValue, newValue))
            {
                UpdateOriginalValue(currentValue, newValue, propertyName!);
                propertyInfo.SetValue(Model, newValue);
                Validate();
                OnPropertyChanged(propertyName);
                OnPropertyChanged(propertyName + IsChangedSuffix);
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapper&quot;]/Validate2/*"/>
        protected void Validate()
        {
            ClearErrors();

            List<ValidationResult> results = new();
            ValidationContext context = new(this);
            Validator.TryValidateObject(this, context, results);

            if (results.Any())
            {
                List<string> propertyNames = results.SelectMany(r => r.MemberNames).Distinct().ToList();

                foreach (string propertyName in propertyNames)
                {
                    Errors[propertyName] = results
                        .Where(r => r.MemberNames.Contains(propertyName))
                        .Select(r => r.ErrorMessage)
                        .Distinct()
                        .ToList()!;
                    OnErrorsChanged(propertyName);
                }
            }

            OnPropertyChanged(nameof(IsValid));
        }

        private PropertyInfo GetPropertyInfo(string? propertyName)
        {
            ArgumentNullException.ThrowIfNull(propertyName);
            ArgumentNullException.ThrowIfNull(Model);
            PropertyInfo? propertyInfo = Model.GetType().GetProperty(propertyName);
            if (propertyInfo is null)
            {
                string msg = $"The property \"{propertyName}\" is not a member of \"{Model.GetType().Name}\".";
                throw new ArgumentException(msg);
            }
            return propertyInfo;
        }

        private void RegisterTrackingObject(IValidatingTrackingObject trackingObject)
        {
            if (!_trackingObjects.Contains(trackingObject))
            {
                _trackingObjects.Add(trackingObject);
                trackingObject.PropertyChanged += TrackingObjectPropertyChanged;
            }
        }

        private void TrackingObjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsChanged))
            {
                OnPropertyChanged(nameof(IsChanged));
            }
            else if (e.PropertyName == nameof(IsValid))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private void UpdateOriginalValue(object? currentValue, object? newValue, string propertyName)
        {
            if (!_originalValues.ContainsKey(propertyName))
            {
                _originalValues.Add(propertyName, currentValue);
                OnPropertyChanged(nameof(IsChanged));
            }
            else if (Equals(newValue, _originalValues[propertyName]))
            {
                _ = _originalValues.Remove(propertyName);
                OnPropertyChanged(nameof(IsChanged));
            }
        }
    }
}