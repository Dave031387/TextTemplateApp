namespace ClassPropertyParser
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/IClassPropertyParser/*"/>
    public interface IClassPropertyParser
    {
        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/ClassTypes/*"/>
        IEnumerable<Type> ClassTypes { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/CollectionProperties/*"/>
        IEnumerable<PropertyInfo> CollectionProperties { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/ComplexProperties/*"/>
        IEnumerable<PropertyInfo> ComplexProperties { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/SimpleProperties/*"/>
        IEnumerable<PropertyInfo> SimpleProperties { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetAllProperties/*"/>
        void GetAllProperties(Type classType);

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetClassTypes/*"/>
        IEnumerable<Type> GetClassTypes(Type classType);

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetCollectionProperties/*"/>
        IEnumerable<PropertyInfo> GetCollectionProperties(Type classType);

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetComplexProperties/*"/>
        IEnumerable<PropertyInfo> GetComplexProperties(Type classType);

        /// <include file="docs.xml" path="docs/members[@name=&quot;classpropertyparser&quot;]/GetSimpleProperties/*"/>
        IEnumerable<PropertyInfo> GetSimpleProperties(Type classType);
    }
}