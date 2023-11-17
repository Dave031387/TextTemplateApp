# C# .NET Class Property Parser
## Introduction
The ___Class Property Parser___ is used to extract property info from class types. Typically this information would be used by a code generator class. For
example, the information might be used for generating wrapper classes for model classes in an MVVM application. The ___ClassPropertyParser___ class
implements the ___IClassPropertyParser___ interface.

Three types of properties are handled by the parser.

### Simple Properties
A simple property is any property that returns a value type or a string. The following are examples of simple properties:

```csharp
public int Age { get; set; }
public string Name { get; set; }
public bool IsModified { get; set; }
```

### Complex Properties
A complex property is any property that returns a reference type other than string. However, the property must not return a collection. For example:

```csharp
public Person Manager { get; set; }
public Address BusinessAddress { get; set; }
```

### Collection Properties
As the name implies, a collection property is any property that returns a collection of objects. The collection type must implement 
the ___IEnumerable___ interface. For example:

```csharp
public List<string> Departments { get; set; }
public ObservableCollection<Address> AddressList { get; set; }
```

## Constructors
The ___Class Property Parser___ has two constructors with the following signatures:

```csharp
public ClassPropertyParser()
public ClassPropertyParser(Type classType)
```

The first constructor (the default constructor) simply initializes all the internal lists to empty lists. (Refer to the __Properties__ section in this
document. Each ___ClassPropertyParser___ property corresponds to one of these lists.)

The second constructor takes a ___Type___ argument. It also initializes the internal lists to empty lists. After initializing the lists, the second
constructor makes a call to the ___GetClassTypes___ method to retrieve all the class types that reside in the same assembly as the class type supplied
to the constructor.

Example:

```csharp
public IClassPropertyParser parser = new(typeof(Models.Address));
````

> ___NOTE:___ It may make sense to organize your code such that all class types that you will be processing with the ___Class Property Parser___ reside in
> their own assembly separate from all other class types. This is optional, but it may make things easier since you can simply iterate over the entire list
> of class types without having to check whether or not a given class type is one that should be parsed.

## Properties
### ___ClassTypes___
The ___ClassTypes___ property returns an ___IEnumerable___ collection of ___Type___ objects corresponding to the class types found in a single assembly. This
collection is populated by either calling the ___ClassPropertyParser___ constructor that takes a ___Type___ argument, or by calling the
___GetClassTypes___ method.

Example:

```csharp
IEnumerable<Type> classTypes = parser.ClassTypes;
```

### ___CollectionProperties___
The ___CollectionProperties___ property returns an ___IEnumerable___ collection of ___PropertyInfo___ objects corresponding to the collection properties
found in a single class type. This collection is populated by calling either the ___GetAllProperties___ or the ___GetCollectionProperties___ methods.

Example:

```csharp
IEnumerable<PropertyInfo> collectionProperties = parser.CollectionProperties;
```

### ___ComplexProperties___
The ___ComplexProperties___ property returns an ___IEnumerable___ collection of ___PropertyInfo___ objects corresponding to the complex properties found in a
single class type. This collection is populated by calling either the ___GetAllProperties___ or the ___GetComplexProperties___ methods.

Example:

```csharp
IEnumerable<PropertyInfo> complexProperties = parser.ComplexProperties;
```

### ___SimpleProperties___
The ___SimpleProperties___ property returns an ___IEnumerable___ collection of ___PropertyInfo___ objects corresponding to the simple properties found in a
single class type. This collection is populated by calling either the ___GetAllProperties___ or the ___GetSimpleProperties___ methods.

Example:

```csharp
IEnumerable<PropertyInfo> simpleProperties = parser.SimpleProperties;
```

## Methods
### ___GetAllProperties___
The ___GetAllProperties___ method takes a ___Type___ argument which specifies the class type for which you want to extract all property info. This method
simply makes calls to the ___GetCollectionProperties___, ___GetComplexProperties___, and  ___GetSimpleProperties___ methods, passing in the given
___Type___ argument to each one. If the ___Type___ argument happens to be ___null___, then all property collections will be cleared and will be empty when
control returns back to the caller.

Example:

```csharp
parser.GetAllProperties(typeof(Models.CompanyInfo));
```

### ___GetClassTypes___
The ___GetClassTypes___ method uses reflection to retrieve ___Type___ information for all class types residing in a single .NET assembly. The method takes
a single ___Type___ argument which supplies a single class type from the assembly of interest. This method clears the ___ClassTypes___ property and then
populates it with ___Type___ objects corresponding to each class type that is found in the assembly. The method then returns the resulting collection to
the caller.

Example:

```csharp
IEnumerable<Type> classTypes = parser.GetClassTypes(typeof(Models.Person));
```

### ___GetCollectionProperties___
The ___GetCollectionProperties___ method uses reflection to retrieve ___PropertyInfo___ objects for all collection properties defined in a single class
type. The method takes a single ___Type___ argument which gives the class type of interest. The method clears the ___CollectionProperties___ property
and then populates it with ___PropertyInfo___ objects corresponding to each collection property that is defined in the given class type. The method
then returns the resulting collection to the caller.

Example:

```csharp
IEnumerable<PropertyInfo> collectionProperties = parser.GetCollectionProperties(typeof(Models.CompanyInfo));
```

### ___GetComplexProperties___
The ___GetComplexProperties___ method uses reflection to retrieve ___PropertyInfo___ objects for all complex properties defined in a single class type. The
method takes a single ___Type___ argument which gives the class type of interest. The method clears the ___ComplexProperties___ property and then populates
it with ___PropertyInfo___ objects corresponding to each complex property that is defined in the given class type. The method then returns the resulting
collection to the caller.

Example:

```csharp
IEnumerable<PropertyInfo> complexProperties = parser.GetComplexProperties(typeof(Models.CompanyInfo));
```

### ___GetSimpleProperties___
The ___GetSimpleProperties___ method uses reflection to retrieve ___PropertyInfo___ objects for all simple properties defined in a single class type. The
method takes a single ___Type___ argument which gives the class type of interest. The method clears the ___SimpleProperties___ property and then populates
it with ___PropertyInfo___ objects corresponding to each simple property that is defined in the given class type. The method then returns the resulting
collection to the caller.

Example:

```csharp
IEnumerable<PropertyInfo> simpleProperties = parser.GetSimpleProperties(typeof(Models.CompanyInfo));
```

### ___GetTypeName___
The ___GetTypeName___ method is a static method that takes a ___Type___ as an argument and then returns the full type name to the caller as a ___string___.
Generic types are formatted with the list of generic type arguments appearing as a comma-separated list between angle brackets after the generic type name.

Example:

```csharp
string fullTypeName = ClassPropertyParser.GetTypeName(typeof(Models.Person));
```

## ___IClassPropertyParser___ Interface
The ___IClassPropertyParser___ interface defines all of the methods and properties (with the exception of the ___GetTypeName___ static property)
described in the __Methods__ and __Properties__ sections above.

---
# Compatibility
The ___ClassPropertyParser___ class library was developed using ___C# 10.0___ and ___.NET 6.0___ for the ___Windows___ desktop targeting
___Windows 7___ and above.