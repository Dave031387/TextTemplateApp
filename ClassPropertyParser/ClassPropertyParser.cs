namespace ClassPropertyParser
{
    using System.Collections;
    using System.Reflection;

    /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/ClassPropertyParser/*"/>
    public class ClassPropertyParser : IClassPropertyParser
    {
        private const char GenericTypeNameDelimiter = '`';
        private readonly List<Type> _classTypes;
        private readonly List<PropertyInfo> _collectionProperties;
        private readonly List<PropertyInfo> _complexProperties;
        private readonly List<PropertyInfo> _simpleProperties;

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/Constructor1/*"/>
        public ClassPropertyParser()
        {
            _classTypes = new();
            _collectionProperties = new();
            _complexProperties = new();
            _simpleProperties = new();
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/Constructor2/*"/>
        public ClassPropertyParser(Type classType) : this() => GetClassTypes(classType);

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/ClassTypes/*"/>
        public IEnumerable<Type> ClassTypes => _classTypes;

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/CollectionProperties/*"/>
        public IEnumerable<PropertyInfo> CollectionProperties => _collectionProperties;

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/ComplexProperties/*"/>
        public IEnumerable<PropertyInfo> ComplexProperties => _complexProperties;

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/SimpleProperties/*"/>
        public IEnumerable<PropertyInfo> SimpleProperties => _simpleProperties;

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetTypeName/*"/>
        public static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                string[] genericArguments = type.GetGenericArguments().Select(t => GetTypeName(t)).ToArray();
                string? propertyTypeFullName = type.GetGenericTypeDefinition().FullName;

                ArgumentNullException.ThrowIfNull(propertyTypeFullName, nameof(propertyTypeFullName));

                int typeIndex = propertyTypeFullName.IndexOf(GenericTypeNameDelimiter);

                if (typeIndex < 1)
                {
                    throw new InvalidOperationException($"Couldn't determine generic type name from \"{propertyTypeFullName}\"");
                }

                string genericTypeName = propertyTypeFullName[..typeIndex];

                return $"{genericTypeName}<{string.Join(",", genericArguments)}>";
            }

            ArgumentNullException.ThrowIfNull(type.FullName, nameof(type.FullName));

            return type.FullName;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetAllProperties/*"/>
        public void GetAllProperties(Type classType)
        {
            _ = GetCollectionProperties(classType);
            _ = GetComplexProperties(classType);
            _ = GetSimpleProperties(classType);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetClassTypes/*"/>
        public IEnumerable<Type> GetClassTypes(Type classType)
        {
            _classTypes.Clear();
            _collectionProperties.Clear();
            _complexProperties.Clear();
            _simpleProperties.Clear();
            if (classType is not null)
            {
                _classTypes.AddRange(classType.Assembly.GetTypes().Where(t => t.IsClass && t.IsVisible));
            }
            return ClassTypes;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetCollectionProperties/*"/>
        public IEnumerable<PropertyInfo> GetCollectionProperties(Type classType)
        {
            _collectionProperties.Clear();
            if (classType is not null)
            {
                _collectionProperties.AddRange(classType.GetProperties()
                    .Where(p =>
                        p.PropertyType.IsGenericType
                        && typeof(IEnumerable).IsAssignableFrom(p.PropertyType)));
            }
            return CollectionProperties;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetComplexProperties/*"/>
        public IEnumerable<PropertyInfo> GetComplexProperties(Type classType)
        {
            _complexProperties.Clear();
            if (classType is not null)
            {
                _complexProperties.AddRange(classType.GetProperties()
                    .Where(p =>
                        !(p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                        && p.PropertyType.IsClass
                        && !typeof(IEnumerable).IsAssignableFrom(p.PropertyType)));
            }
            return ComplexProperties;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetSimpleProperties/*"/>
        public IEnumerable<PropertyInfo> GetSimpleProperties(Type classType)
        {
            _simpleProperties.Clear();
            if (classType is not null)
            {
                _simpleProperties.AddRange(classType.GetProperties()
                    .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string)));
            }
            return SimpleProperties;
        }
    }
}