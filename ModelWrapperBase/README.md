# _ModelWrapperBase_ Class Library
## Introduction
The ___ModelWrapperBase___ class library provides a number of classes and interfaces that facilitate the implementation of robust MVVM
applications. Specifically, the library provides a "wrapper" class that can be used to wrap any model class (a class made up primarily of only
public properties). The wrapped model class has the following features:

- Property change notification (implements the ___System.ComponentModel.INotifyPropertyChanged___ interface)
- Data validation (implements the ___System.ComponentModel.DataAnnotations.IValidatableObject___ interface)
- Error notification (implements the ___System.ComponentModel.INotifyDataErrorInfo___ interface)
- Revertible change tracking (implements the ___System.ComponentModel.IRevertibleChangeTracking___ interface)

The class library is comprised of the following components which are described in further detail in the remainder of this __README__ file:

- ___ChangeTrackingCollection___ base class
- ___IValidatingTrackingObject___ interface
- ___ModelWrapper___ abstract base class
- ___NotifyDataErrorInfoBase___ abstract base class
- ___ObservableCollection___ base class

## Model Class vs Model Wrapper Class
The remainder of this __README__ file will often refer to model classes and model wrapper classes. "Model wrapper class" will sometimes be
shortened to just "wrapper class". For the purposes of this __README__ file, a _"model class"_ is a ___POCO___ (Plain Old CLR Object) comprised
primarily of public properties and that has no dependencies on a particular framework or infrastructure, or on any third-party libraries, and
doesn't contain any business logic. A ___Person___ class that has properties for ___FirstName___, ___LastName___, and ___Age___, but that
doesn't have any methods and doesn't derive from another class would be a ___POCO___. If that same class derived from a third-party class
library, or had dependencies on a particular database or hardware, then it would no longer be a ___POCO___.

> __Disclaimer:__ _This is my definition of a ___POCO___ and it may not agree entirely with other definitions that can be found on the Internet._

As mentioned in the _Introduction_, a _"model wrapper class"_ (or _"wrapper class"_) wraps an ordinary model class to provide additional
functionality required for MVVM applications. The model wrapper class will contain the same properties as the model class, plus several additional
properties and some methods to support the additional functionality. All model wrapper classes derive from the ___ModelWrapper___ base class.

> __Note:__ In this document, a model class that is wrapped by a model wrapper class is often referred to as the _"wrapped model class"_. This
> should not be confused with the _"model wrapper class"_. The first is the model class that is being wrapped. The second is the wrapper class
> that is doing the wrapping.

## Supported Property Types
There are three basic property types that are supported by the ___ModelWrapper___ class library. These are:

- ___Simple___ properties - These are properties that return either a ___value___ type (___int___, ___bool___, ___DateTime___, etc.) or a ___string___.
- ___Complex___ properties - These are properties that return a ___reference___ type. The reference type returned must be a model class as defined above.
  For example, a ___Person___ class might have an ___Address___ property that returns an ___Address___ model class object having properties of ___Street___,
  ___City___, and ___State___.
- ___Collection___ properties - These are properties that return a collection of model class objects. For example, a ___Person___ class may have an
  ___EmailAddresses___ property that returns a collection of ___EmailAddress___ objects having properties of ___DisplayName___, ___IsPrimary___, and
  ___EmailAddress___.

Property types other than the ones mentioned above are not supported by the ___ModelWrapper___ class library. For example, properties returning a
collection of value type objects or a collection of string objects are not supported. However, you could encapsulate the value type or string type in
a simple model class and change the property to return the model class object instead.

## Sample Model Classes
The following are sample model classes. These classes will be used and referred to throughout the remainder of this __README__ file.

___Address___ model class:
```csharp
public class Address
{
    // These are all simple properties.
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
}
```

___EmailAddress___ model class:
```csharp
public class EmailAddress
{
    // These are all simple properties.
    public string DisplayName { get; set; }
    public bool IsPrimary { get; set; }
    public string EmailAddress { get; set; }
}
```

___Persoon___ model class:
```csharp
public class Person
{
    // Simple properties
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    // Complex property
    public Address HomeAddress { get; set; }

    // Collection property
    public List<EmailAddress> EmailAddresses { get; set; }
}
```

## Sample Model Wrapper Classes
The following model wrapper classes correspond to the sample model classes mentioned above. These also will be referred to throughout the remainder
of this __README__ file. Only the basic shell of each class is shown below as the details will be covered later.

___AddressWrapper___ class:
```csharp
public class AddressWrapper : ModelWrapper<Address>
{
    ...
}
```

___EmailAddressWrapper___ class:
```csharp
public class EmailAddressWrapper : ModelWrapper<EmailAddress>
{
    ...
}
```

___PersonWrapper___ class:
```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    ...
}
```

> __Note:__ As a general rule (but not a requirement) the model classes will be defined in a ___Models___ project and the model wrapper classes will
> be defined in a ___ModelWrappers___ (or ___Wrappers___) project. It is generally a bad idea to mix model classes and model wrapper classes in
> the same project since they serve very different functions.

# _ChangeTrackingCollection_ Base Class
The ___ChangeTrackingCollection___ base class is used to wrap collection properties in a wrapped model class. It extends the 
___System.Collections.ObjectModel.ObservableCollection\<T>___ class by implementing the ___System.ComponentModel.IRevertibleChangeTracking___
interface. The class tracks all additions and deletions of items to/from the collection. It also provides a mechanism for accepting or rejecting
all pending changes.

## Interfaces
The ___ChangeTrackingCollection___ class implements the ___IValidatingTrackingObject___ interface which inherits from the following interfaces:

- ___INotifyPropertyChanged___
- ___IRevertibleChangeTracking___

The class also inherits the following interfaces through the ___ObservableCollection\<T>___ base class:

- ___INotifyCollectionChanged___
- ___IList\<T>___ and ___IList___
- ___ICollection\<T>___ and ___ICollection___
- ___IReadOnlyList\<T>___
- ___IReadOnlyCollection\<T>___
- ___IEnumerable\<T>___ and ___IEnumerable___

## Constructor
The ___ChangeTrackingCollection___ class has a single constructor with the following signature:

```csharp
public ChangeTrackingCollection(IEnumerable<T> items)
```

The class is constructed with an ___IEnumerable\<T>___ collection of objects. The objects must be model class objects that implement the
___IValidatingTrackingObject___ interface (see later in this __README__ file for details). Typically these will be classes that are derived from the
___ModelWrapper___ class.

For example, the sample ___Person___ model class has a property named ___EmailAddresses___ which returns a collection of ___EmailAddress___ model
class objects.

```csharp
public List<EmailAddress> EmailAddresses { get; set; }
```

The ___PersonWrapper___ class would also have an ___EmailAddresses___ property, but this property would return a ___ChangeTrackingCollection___ of
___EmailAddressWrapper___ objects, like so:

```csharp
public ChangeTrackingCollection<EmailAddressWrapper> EmailAddresses { get; set; }
```

Using ___LINQ___, we can initialize the ___EmailAddresses___ property in the ___PersonWrapper___ class like so:

```csharp
EmailAddresses = new ChangeTrackingCollection<EmailAddressWrapper>(Model.EmailAddresses.Select(e => new EmailAddressWrapper(e)));
```

The above example uses the ___Model___ property of the ___ModelWrapper___ base class to get a reference to the ___Person___ model class object
that is wrapped by the ___PersonWrapper___ model wrapper class object.

## Properties
The ___ChangeTrackingCollection___ class has the following properties in addition to the properties supplied by the inherited
___ObservableCollection\<T>___ class.

### ___AddedItems___
The ___AddedItems___ property returns a ___ReadOnlyObservableCollection\<T>___ of objects that have been added to the
___ChangeTrackingCollection___ since the last successful ___AcceptChanges___ or ___RejectChanges___ call was made, or since the collection
was initialized if neither ___AcceptChanges___ nor ___RejectChanges___ have been called yet. The ___AddedItems___ collection is cleared once
the changes have either been accepted or rejected.

If an object is added to the ___ChangeTrackingCollection___ and then later modified before either ___AcceptChanges___ or ___RejectChanges___
is called, that object remains in the ___AddedItems___ collection. It is not moved to the ___ModifiedItems___ collection.

If an object is added to the ___ChangeTrackingCollection___ and then later removed before either ___AcceptChanges___ or ___RejectChanges___
is called, that object is removed from the ___AddedItems___ collection, but it is not moved to the ___RemovedItems___ collection.

> __Note:__ A "successful" ___AceptChanges___ call is one that takes place when both the ___IsChanged___ property is ___true___ and the
> ___IsValid___ property is ___true___. Similarly, a "successful" ___RejectChanges___ call is one that takes place when ___IsChanged___
> is ___true___.

Example (assumes there are no validation errors at any time):
```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    public ChangeTrackingCollection<EmailAddressWrapper> EmailAddresses { get; set; }
    ...
    EmailAddressWrapper item1 = new EmailAddressWrapper(new EmailAddress());
    EmailAddresses.Add(item1);
    // Returns a collection containing the item1 object:
    ReadOnlyObservableCollection<EmailAddressWrapper> items = EmailAddresses.AddedItems;
    EmailAddresses.AcceptChanges();
    // Returns an empty collection:
    items = EmailAddresses.AddedItems;
    EmailAddressWrapper item2 = new EmailAddressWrapper(new EmailAddress());
    EmailAddresses.Add(item2);
    // Returns a collection containing the item2 object:
    items = EmailAddresses.AddedItems;
    EmailAddresses.Remove(item2);
    // Returns an empty collection:
    items = EmailAddresses.AddedItems;
    // Returns an empty collection:
    items = EmailAddresses.RemovedItems;
}
```

### ___IsChanged___
The ___IsChanged___ property is a boolean property that returns ___true___ if any objects have been added, removed, or modified in the
___ChangeTrackingCollection___ since the last successful ___AcceptChanges___ or ___RejectChanges___ call was made, or since the collection
was initialized if neither ___AcceptChanges___ nor ___RejectChanges___ have been called yet. Otherwise, it returns ___false___.

Example:
```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    public ChangeTrackingCollection<EmailAddressWrapper> EmailAddresses { get; set; }
    ...
    EmailAddressWrapper item1 = new EmailAddressWrapper(new EmailAddress());
    item1.DisplayName = "Bob";
    item1.AcceptChanges();
    EmailAddresses.Add(item1);
    // Returns true:
    bool isChanged = EmailAddresses.IsChanged;
    EmailAddresses.AcceptChanges();
    // Returns false:
    isChanged = EmailAddresses.IsChanged;
    // Modify the DisplayName property.
    item1.DisplayName = "Sam";
    // Returns true:
    isChanged = EmailAddresses.IsChanged;
    // Change the DisplayName property back to its original value.
    item1.DisplayName = "Bob";
    // Returns false:
    isChanged = EmailAddresses.IsChanged;
}
```

### ___IsValid___
The ___IsValid___ property is a boolean property that returns ___true___ if all objects in the ___ChangeTrackingCollection___ are valid.
If even one object in the collection is not valid, this property returns ___false___. (See the section in this __README__ file that describes
the ___Validate___ method of the ___ModelWrapper___ class.)

Example (assumes there is validation defined for the ___EmailAddress___ property of the ___EmailAddressWrapper___ class):
```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    public ChangeTrackingCollection<EmailAddressWrapper> EmailAddresses { get; set; }
    ...
    EmailAddressWrapper item1 = new EmailAddressWrapper(new EmailAddress());
    item1.DisplayName = "Bob";
    // Set the EmailAddress property to an invalid value.
    item1.EmailAddress = "bob";
    EmailAddresses.Add(item1);
    // Returns false because item1 is invalid:
    bool isCollectionValid = EmailAddresses.IsValid;
    // The IsValid property of the PersonWrapper class also returns false:
    bool isPersonValid = IsValid;
    // Set the EmailAddress property to a valid value.
    item1.EmailAddress = "bob@yahoo.com";
    // Returns true:
    isCollectionValid = EmailAddresses.IsValid;
}
```

### ___ModifiedItems___
The ___ModifiedItems___ property returns a ___ReadOnlyObservableCollection\<T>___ of objects that have been modified in the
___ChangeTrackingCollection___ since the last successful ___AcceptChanges___ or ___RejectChanges___ call was made, or since the collection
was initialized if neither ___AcceptChanges___ nor ___RejectChanges___ have been called yet. The ___ModifiedItems___ collection is cleared
once the changes have either been accepted or rejected.

If an object is modified in the ___ChangeTrackingCollection___ and then later the object is changed back to its original state before
either ___AcceptChanges___ or ___RejectChanges___ is called, then that object is removed from the ___ModifiedItems___ collection.

If an object is modified in the ___ChangeTrackingCollection___ and then later that object is removed before either ___AcceptChanges___
or ___RejectChanges___ is called, then that object is removed from the ___ModifiedItems___ collection and the unmodified version of the
object is added to the ___RemovedItems___ collection.

Example (assumes there are no validation errors at any time):
```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    public ChangeTrackingCollection<EmailAddressWrapper> EmailAddresses { get; set; }
    ...
    EmailAddressWrapper item1 = new EmailAddressWrapper(new EmailAddress());
    item1.DisplayName = "Test1";
    EmailAddresses.Add(item1);
    EmailAddressWrapper item2 = new EmailAddressWrapper(new EmailAddress());
    item2.DisplayName = "Test2";
    EmailAddresses.Add(item2);
    EmailAddresses.AcceptChanges();
    item1.DisplayName = "Bob";
    // Returns a collection containing the modified item1 object:
    ReadOnlyObservableCollection<EmailAddressWrapper> items = EmailAddresses.ModifiedItems;
    EmailAddresses.AcceptChanges();
    // Returns an empty collection:
    items = EmailAddresses.ModifiedItems;
    item2.DisplayName = "Sam";
    // Returns a collection containing the modified item2 object:
    items = EmailAddresses.ModifiedItems;
    item2.DisplayName = "Test2";
    // Returns an empty collection:
    items = EmailAddresses.ModifiedItems;
}
```

### ___RemovedItems___

The ___RemovedItems___ property returns a ___ReadOnlyObservableCollection\<T>___ of objects that have been removed from the
___ChangeTrackingCollection___ since the last successful ___AcceptChanges___ or ___RejectChanges___ call was made, or since the collection
was initialized if neither ___AcceptChanges___ nor ___RejectChanges___ have been called yet. The ___RemovedItems___ collection is cleared
once the changes have either been accepted or rejected.

If an object is removed from the ___ChangeTrackingCollection___ and then the same object is added back to the collection before either
___AcceptChanges___ or ___RejectChanges___ is called, then that object will be removed from the ___RemovedItems___ collection. It will not
be added to the ___AddedItems___ collection.

If an object is removed from the ___ChangeTrackingCollection___ and then later a new object is added that is equivalent to the removed
object (i.e., has the same property values), the new object is added to the ___AddedItems___ collection and the removed object remains
in the ___RemovedItems___ collection.

Example (assumes there are no validation errors at any time):
```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    public ChangeTrackingCollection<EmailAddressWrapper> EmailAddresses { get; set; }
    ...
    // Add item1 and item2 to the EmailAddresses collection property.
    EmailAddressWrapper item1 = new EmailAddressWrapper(new EmailAddress());
    EmailAddresses.Add(item1);
    EmailAddressWrapper item2 = new EmailAddressWrapper(new EmailAddress());
    EmailAddresses.Add(item2);
    // Accept the pending changes.
    EmailAddresses.AcceptChanges();
    // Remove item1 from the collection.
    EmailAddresses.Remove(item1);
    // Returns a collection containing the removed item1 object:
    ReadOnlyObservableCollection<EmailAddressWrapper> items = EmailAddresses.RemovedItems;
    // Accept the changes.
    EmailAddresses.AcceptChanges();
    // Returns an empty collection:
    items = EmailAddresses.RemovedItems;
    // Remove the item2 object from the collection.
    EmailAddresses.Remove(item2);
    // Returns a collection containing the removed item2 object:
    items = EmailAddresses.RemovedItems;
    // Add the item2 object back to the collection.
    EmailAddresses.Add(item2);
    // Returns an empty collection:
    items = EmailAddresses.RemovedItems;
    // Returns an empty collection:
    items = EmailAddresses.AddedItems;
}
```

## Methods
The ___ChangeTrackingCollection___ class has the following methods in addition to the methods supplied by the inherited
___ObservableCollection\<T>___ class.

### ___AcceptChanges___
The ___AcceptChanges___ method has the following signature:

```csharp
public void AcceptChanges()
```

The ___AcceptChanges___ method first checks to see if the ___IsChanged___ and ___IsValid___ properties both return ___true___. It that is
the case, then all pending changes on the ___ChangeTrackingCollection___ are finalized, and the ___AddedItems___, ___ModifiedItems___,
and ___RemovedItems___ collections are cleared. The ___IsChanged___ property is then set to ___false___ and the ___PropertyChanged___
event is raised for the ___IsChanged___ property.

If either the ___IsChanged___ or the ___IsValid___ property returns ___false___ when ___AcceptChanges___ is called, the method will return
without doing anything.

Example (assumes there are no validation errors at any time):
```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    public ChangeTrackingCollection<EmailAddressWrapper> EmailAddresses { get; set; }
    ...
    // Does nothing since IsChanged is false at this point:
    EmailAddresses.AcceptChanges();
    // Add a couple items to the collection.
    EmailAddressWrapper item1 = new EmailAddressWrapper(new EmailAddress());
    EmailAddresses.Add(item1);
    EmailAddressWrapper item2 = new EmailAddressWrapper(new EmailAddress());
    EmailAddresses.Add(item2);
    // Accept the pending changes.
    EmailAddresses.AcceptChanges();
    // Add a third item to the collection.
    EmailAddresses.Remove(item1);
    // Modify an item.
    item2.DisplayName = "Bob";
    // Remove an item.
    EmailAddressWrapper item3 = new EmailAddressWrapper(new EmailAddress());
    EmailAddresses.Add(item3);
    // Returns a collection containing the removed item1 object:
    ReadOnlyObservableCollection<EmailAddressWrapper> removedItems = EmailAddresses.RemovedItems;
    // Returns a collection containing the modified item2 object:
    ReadOnlyObservableCollection<EmailAddressWrapper> modifiedItems = EmailAddresses.ModifiedItems;
    // Returns a collection containing the added item3 object:
    ReadOnlyObservableCollection<EmailAddressWrapper> addedItems = EmailAddresses.AddedItems;
    // At this point the EmailAddresses collection contains the
    // modified item2 object and the added item3 object.
    // Accept the pending changes.
    EmailAddresses.AcceptChanges();
    // At this point the EmailAddresses collection contains the
    // modified item2 object and the added item3 object.
    // All the following now return empty collections:
    removedItems = EmailAddresses.RemovedItems;
    modifiedItems = EmailAddresses.ModifiedItems;
    addedItems = EmailAddresses.AddedItems;
}
```

### ___RejectChanges___
The ___RejectChanges___ method first checks to see if the ___IsChanged___ property returns ___true___. If it does, then all pending changes
on the ___ChangeTrackingCollection___ are rejected and the collection is returned to its original state. The ___AddedItems___,
___ModifiedItems___, and ___RemovedItems___ collections are cleared. The ___IsChanged___ property is then set to ___false___ and the
___PropertyChanged___ event is raised for the ___IsChanged___ property.

If ___IsChanged___ returns ___false___ when ___RejectChanges___ is called, the method returns without doing anything.

Example (assumes there are no validation errors at any time):
```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    public ChangeTrackingCollection<EmailAddressWrapper> EmailAddresses { get; set; }
    ...
    // Does nothing since IsChanged is false at this point.
    EmailAddresses.RejectChanges();
    // Store a couple items.
    EmailAddressWrapper item1 = new EmailAddressWrapper(new EmailAddress());
    EmailAddresses.Add(item1);
    EmailAddressWrapper item2 = new EmailAddressWrapper(new EmailAddress());
    EmailAddresses.Add(item2);
    // Accept the pending changes.
    EmailAddresses.AcceptChanges();
    // Remove item1 from the collection.
    EmailAddresses.Remove(item1);
    // Modify item2.
    item2.DisplayName = "Bob";
    // Add item3 to the collection.
    EmailAddressWrapper item3 = new EmailAddressWrapper(new EmailAddress());
    EmailAddresses.Add(item3);
    // Returns a collection containing the removed item1 object:
    ReadOnlyObservableCollection<EmailAddressWrapper> removedItems = EmailAddresses.RemovedItems;
    // Returns a collection containing the modified item2 object:
    ReadOnlyObservableCollection<EmailAddressWrapper> modifiedItems = EmailAddresses.ModifiedItems;
    // Returns a collection containing the added item3 object:
    ReadOnlyObservableCollection<EmailAddressWrapper> addedItems = EmailAddresses.AddedItems;
    // At this point the EmailAddresses collection contains the
    // modified item2 object and the added item3 object.
    // Reject the pending changes.
    EmailAddresses.RejectChanges();
    // At this point the EmailAddresses collection contains the
    // original (unmodified) item1 and item2 objects.
    // All the following now return empty collections.
    removedItems = EmailAddresses.RemovedItems;
    modifiedItems = EmailAddresses.ModifiedItems;
    addedItems = EmailAddresses.AddedItems;
}
```

## Overrides
The ___ChangeTrackingCollection___ class overrides the ___OnCollectionChanged___ method of the ___ObservableCollection\<T>___ base class.
This method maintains the objects in the ___AddedItems___, ___ModifiedItems___, and ___RemovedItems___ collections as changes are made to
the ___ChangeTrackingCollection___. It also raises the ___PropertyChanged___ event on the ___IsChanged___ and ___IsValid___ properties.

The ___OnCollectionChanged___ method normally isn't called explicitly. It will get invoked automatically when changes are made to the
___ChangeTrackingCollection___.

# ___IValidatingTrackingObject___ Interface
The ___IValidatingTrackingObject___ interface inherits from the ___INotifyPropertyChanged___ and the ___IRevertibleChangeTracking___
interfaces and adds in the ___IsValid___ property. Thus, the interface provides for both property change notification and revertible
change tracking. The ___IValidatingTrackingObject___ interface is referenced by both the ___ChangeTrackingCollection___ class and the
___ModelWraper___ class.

# ___ModelWrapper___ Base Class
The ___ModelWrapper___ class is an abstract base class used for wrapping a model class. The wrapped class, as mentioned in the __Introduction__ above,
enhances the model class by implementing the following features:

- Property change notification
- Data validation
- Error notification
- Revertible change tracking

The model wrapper class is ideal for use in a view model class in applications that follow the __MVVM__ design pattern. It may have other uses as well,
but it is particularly well suited for ___WPF___ applications because it implements all the pieces needed to keep the model wrapper class and the user
interface (the ___XAML___) in sync with each other as changes are made by the user.

## Interfaces
The ___ModelWrapper___ class implements the following interfaces:

- ___IValidatableObject___
- ___IValidatingTrackingObject___ (described in the previous section in this __README__ file)

As mentioned above, the ___IValidatingTrackingObject___ interface inherits from the following classes:

- ___INotifyPropertyChanged___
- ___IRevertibleChangeTracking___

Finally, the ___ModelWrapper___ class inherits the following property through the ___NotifyDataErrorInfoBase___ class described in a later section in
this __README__ file):

- ___INotifyDataErrorInfo___

## Constructor
The ___ModelWrapper___ class has a single constructor with the following signature:

```csharp
public ModelWrapper(T model)
```

The generic type ___T___ of the argument passed into the constructor must be a class type. As mentioned above, that class type that is passed in
should be a ___POCO___ class type, and, as the argument name implies, it is usually a model class in an __MVVM__ application.

You can't create instances of the ___ModelWrapper___ class since it is defined as an abstract class. Instead, you must create a new class that
derives from the ___ModelWrapper___ class. For example, if we have a ___Person___ model class in our application, we would then define a new
___PersonWrapper___ class like so:

```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    ...
}
```

The ___PersonWrapper___ class would then be referenced in our application like so:

```csharp
public class MyClass
{
    public PersonWrapper Employee { get; set; }
    ...
    public void DoSomething()
    {
        Person employee = new Person();
        Employee = new PersonWrapper(employee);
        ...
    }
}
```

## Fields

### ___Errors___
The ___Errors___ field is a protected field that is inherited from the ___NotifyDataErrorInfoBase___ base class. Normally there is no need to access
this field directly since the public ___GetErrors___ method described later in this document should suffice. However, this field can be accessed
in wrapper classes derived from ___ModelWrapper___ if there is a need. The ___Errors___ field consists of a read-only collection of validation errors
that were detected on the model wrapper class object.

The ___Errors___ collection is a ___Dictionary\<string, List\<string>>___ object where the key is the name of a property of the model wrapper class,
and the value is a list of string objects that are the validation error messages associated with that property. The ___Errors___ collection is
maintained by the protected version of the ___Validate___ method described later in this document.

For comparison, the two statements in the example below return identical results. Note that ___Errors___ can only be accessed from within a class
derived from ___NotifyDataErrorInfoBase___, such as the ___ModelWrapper___ class, whereas ___GetErrors___ can be called from any class.

```csharp
public PersonWrapper : ModelWrapper<Person>
{
    public int Age
    {
        get => GetValue<int>();
        set => SetValue(value);
    }
    ...
    public void DoSomething()
    {
        List<string> errors;

        // The next two statements return identical results:
        errors = Errors[nameof(Age)];
        errors = GetErrors(nameof(Age) as List<string>;
    }
    ...
}
```

## Events
The are two primary events that can be raised in the ___ModelWrapper___ class and in derived classes.

### ___ErrorsChanged___
The ___ErrorsChanged___ event (inherited from the ___NotifyDataErrorInfoBase___ class) gets raised whenever the list of validation errors for a particular
property in the model wrapper class changes.

### ___PropertyChanged___
The ___PropertyChanged___ event (inherited from the ___Observable___ class) gets raised whenever the value changes for a property of the model wrapper class.

## Properties
The ___ModelWrapper___ class has the following properties:

### ___IsChanged___
The ___IsChanged___ property returns the "_is changed_" state of the ___ModelWrapper___ class object. This property will return ___true___ if any property of
the model wrapper class object has been changed since the last successful call to either ___AcceptChanges___ or ___RejectChanges___ (see the __Methods__
section below for details), or since the model wrapper class object was created if neither ___AcceptChanges___ nor ___RejectChanges___ was called.

Example (assumes there are no validation errors at any time):
```csharp
public class MyClass
{
    public PersonWrapper Employee {get; set; }
    ...
    public void DoSomething()
    {
        Employee = new PersonWrapper(new Person());
        // Returns false because no changes have been made yet:
        bool isChanged = Employee.IsChanged;
        Employee.FirstName = "Bob";
        // Returns true:
        isChanged = Employee.IsChanged;
        Employee.AcceptChanges();
        // Returns false since no changes made after calling AcceptChanges():
        isChanged = Employee.IsChanged;
    }
}
```

### ___HasErrors___
Inherited from the ___NotifyDataErrorInfo___ base class, the ___HasErrors___ property returns ___true___ if there are any validation errors on the
model wrapper class object.

Example (assumes FirstName and LastName are required as well as Street in the HomeAddress complex property):
```csharp
public class MyClass
{
    public PersonWrapper Employee {get; set; }
    ...
    public void DoSomething()
    {
        Employee = new PersonWrapper(new Person());
        // Returns true because FirstName and LastName are empty:
        bool hasErrors = Employee.HasErrors;
        Employee.FirstName = "Bob";
        // Returns true because LatName is still empty:
        hasErrors = Employee.HasErrors;
        Employee.LastName = "Smith";
        // Returns false even though Street is still empty:
        hasErrors = Employee.HasErrors;
        // Returns true because the Street property of the
        // AddressWrapper object is invalid:
        hasErrors = Employee.HomeAddress.HasErrors;
    }
}
```

> __Note:__ The ___HasErrors___ property only returns ___true___ if there are validation errors on the properties of the model wrapper class object itself.
> It will return ___false___ if the validation errors are only on objects contained within the model wrapper class object, such as properties on an
> object returned from a complex property, or properties on objects contained within the collection returned from a collection property.

### ___IsValid___
The ___IsValid___ property returns the _"is valid"_ state of the ___ModelWrapper___ class object. This property returns true only if all properties on the
wrapped model class object are valid. If the model class contains a _complex property_, then all properties on the object returned by the complex property must
be valid. If the model class contains a _collection property_, then every object in the collection must be valid.

> __Note:__ It is possible that the ___HasErrors___ property and the ___IsValid___ property could both be ___false___ at the same time. This would happen
> if there are no validation errors on the model wrapper class itself, but there are validation errors on objects returned from complex properties or
> collection properties contained within the model wrapper class object. However, it is not possible that ___HasErrors___ and ___IsValid___ will both be
> ___true___ at the same time. ___IsValid___ will always be ___false___ when ___HasErrors___ is ___true___.

Example (assumes FirstName and LastName are required as well as Street in the HomeAddress complex property):
```csharp
public class MyClass
{
    public PersonWrapper Employee {get; set; }
    ...
    public void DoSomething()
    {
        Employee = new PersonWrapper(new Person());
        // Returns false because FirstName and LastName are empty:
        bool isValid = Employee.IsValid;
        Employee.FirstName = "Bob";
        // Returns false because LatName is still empty:
        isValid = Employee.IsValid;
        Employee.LastName = "Smith";
        // Returns false because Street is still empty:
        isValid = Employee.IsValid;
        Employee.HomeAddress.Street = "123 First Street";
        // Returns true because there are no more validation errors
        // on the Employee object or on any object contained within
        // the Employee object:
        isValid = Employee.IsValid;
    }
}
```

### ___Model___
The ___Model___ property returns a reference to the model class object that is wrapped by the model wrapper class.

Example:
```csharp
public class MyClass
{
    public PersonWraper Employee { get; set; }
    ...
    public void DoSomething()
    {
        Person employee1 = new Person();
        employee1.FirstName = "Bob";
        Employee = new PersonWrapper(employee1);
        // Returns a reference to the employee1 object:
        Person employee2 = Employee.Model;
        // Returns "Bob":
        string employeeName = employee2.FirstName;
        // Returns true because employee1 and employee2 both
        // point to the same Person object:
        bool areTheSameObject = Object.ReferenceEquals(employee1, employee2);
    }
}
```

## Methods
The ___ModelWrapper___ class has the following methods:

### ___AcceptChanges___
The ___AcceptChanges___ method is called to accept (finalize) all pending changes on the wrapped model object. This method will return without doing
anything if either the ___IsChanged___ property or ___IsValid___ property is ___false___. If both of these properties are ___true___, then the pending
changes will be finalized.

This method will call the ___AcceptChanges___ method for modified objects returned from any complex properties the model wrapper class
may have. It will also call ___AcceptChanges___ on any modified objects contained in any collection property of the model wrapper class. It can do
this because both these types of objects must implement the ___IValidatingChangeTracking___ interface which, in turn, inherits from the
___IRevertibleChangeTracking___ interface which defines the ___AcceptChanges___ method.

Once all changes have been accepted, this method raises the ___PropertyChanged___ event for all properties on the model wrapper class. From this point
on, if you decide you didn't really want to accept the changes, your only option would be to manually reverse all the changes that were accepted and
call ___AcceptChanges___ again.

Example (assumes that there are no validation errors at any time):
```csharp
public class MyClass
{
    public PersonWrapper Employee { get; set; }
    ...
    public void DoSomething()
    {
        // Create a new PersonWrapper object.
        Person employee = new Person();
        Employee = new PersonWrapper(employee);
        // Returns false because nothing is changed yet:
        bool isChanged = Employee.IsChanged;
        // Doesn't do anything since nothing has changed since
        // the Employee object was created:
        Employee.AcceptChanges();
        // Modify the PersonWrapper object.
        Employee.FirstName = "Bob";
        // Returns true now that changes have been made:
        isChanged = Employee.IsChanged;
        // Accepts the pending changes:
        Employee.AcceptChanges();
    }
}
```

> __Note:__ It is the responsibility of the developer to handle persisting the changes once they have been accepted. The ___ModelWrapper___ class
> doesn't provide any persistence services. Therefore, the developer will need to provide the means for saving the changes to a file system,
> database, or some other persistence store once the changes have been accepted.

### ___ClearErrors___
The ___ClearErrors___ method is a protected method that is inherited from the ___NotifyDataErrorInfoBase___ class which is covered later in this
document. This method does two things:

- Removes each key/value pair from the ___Errors___ collection. (The key is the name of a property of a model wrapper class that is derived from
  ___ModelWrapper___, and the value is the ___List\<string>___ of error messages on that property.)
- Raises the ___ErrorsChanged___ event for each property that is removed from the ___Errors___ collection.

The ___ClearErrors___ method has the following signature:
```csharp
protected void ClearErrors()
```

The ___ClearErrors___ method is called from within the protected ___Validate___ method of the ___ModelWrapper___ class. In that method it removes
the old validation errors before kicking off the validation process again. There should be no need to call ___ClearErrors___ from any other location.

### ___GetErrors___
The ___GetErrors___ method is inherited from the ___NotifyDataErrorInfoBase___ base class. It is called to retrieve a collection of validation errors
on the given property. The method will return an empty list if there are no validation errors on the given property name, or if the property name is
___null___ or invalid.

The ___GetErrors___ method has the following signature:

```csharp
public IEnumerable GetErrors(string? propertyName)
```

The method takes a property name as an argument and it returns an ___IEnumerable___ list of ___string___ where each item in the list is an error
message corresponding to a validation rule that was violated by the current value of the given property. The error messages are ones that get
generated automatically when validating ___DataAnnotation___ attributes defined on properties, or they're error messages that are generated in
the overridden public virtual version of the ___Validate___ method (described later in this document).

Example (assumes the ___PersonWrapper___ class is set up similar to the example given for the public virtual version
of the ___Validate___ method later in this document):
```csharp
public MyClass
{
    public PersonWrapper Employee { get; set; }
    ...
    public void DoSomething()
    {
        Employee = new PersonWrapper(new Person());
        // Initialize the PersonWrapper object with valid data:
        Employee.FirstName = "Bob"; // Must not be null, empty, or whitespace
        Employee.Age = 32; // Must be between 18 and 65
        Employee.AcceptChanges();
        // Returns an empty list because there are no validation errors:
        List<string> errors = Employee.GetErrors(nameof(Employee.Age)) as List<string>;
        // Set Age to an invalid value:
        Employee.Age = 3;
        // Returns a list with a single string object containing the validation error message:
        // "Age is not within valid range."
        errors = Employee.GetErrors(nameof(Employee.Age)) as List<string>;
        // This does nothing because there are validation errors:
        Employee.AcceptChanges();
        // Reject the pending changes:
        Employee.RejectChanges();
        // Returns an empty list because there are no longer any validation errors:
        errors = Employee.GetErrors(nameof(Employee.Age)) as List<string>;
    }
}
```

> __Note:__ The errors collection is maintained by the protected version of the ___Validate___ method discussed later in this document. In the example
> above, ___Validate___ is called behind the scenes every time a property is modified. ___Validate___ clears the errors collection before performing
> the validation, and then repopulates the collection with the new validation errors if any are found.

### ___GetIsChanged___
The ___GetIsChanged___ method is a protected method that returns ___true___ if the current value of the specified property differs from its original
value. The original value is the value the property had after the last successful call to ___AcceptChanges___ on the model wrapper class, or it's the
value that the property had when the model class object was created if ___AcceptChanges___ hasn't been called yet.

The ___GetIsChanged___ method has the following signature:
```csharp
protected bool GetIsChanged(string propertyName)
```

The ___propertyName___ argument must be the name of a property that exists in both the model wrapper class and in the model class that is wrapped by
that class. The ___GetIsChanged___ method will return ___false___ if an unknown or invalid property name is passed into the method. No other
indication will be given that the property name is invalid, so the developer must ensure that only valid property names are used.

Example:
```csharp
public class MyClass
{
    public PersonWrapper Employee = { get; set; }
    ...
    public void DoSomething()
    {
        Employee = new PersonWrapper(new Person());
        // Returns false because nothing has been changed yet:
        bool isChanged = Employee.GetIsChanged(nameof(Employee.FirstName));
        Employee.FirstName = "Bob";
        // Returns true:
        isChanged = Employee.GetIsChanged(nameof(Employee.FirstName));
        Employee.AcceptChanges();
        // Returns false because all pending changes have been accepted:
        isChanged = Employee.GetIsChanged(nameof(Employee.FirstName));
    }
}
```

### ___GetOriginalValue___
The ___GetOriginalValue___ method is a protected method that returns the original value of the specified property of the model wrapper class. (See the
description of the ___GetIsChanged___ method above for a definition of _"original value"_.)

The ___GetOriginalValue___ method has the following signature:
```csharp
protected TValue? GetOriginalValue<TValue>(string propertyName)
```

The method has a generic type parameter ___TValue___ that is used to supply the return type of the specified property. The ___propertyName___
argument gives the name of the property for which we want to retrieve its original value. An ___ArgumentNullException___ is thrown if the
supplied property name is ___null___. An ___ArgumentException___ is thrown if the specified property name isn't found in the model wrapper
class.

Example:
```csharp
public class MyClass
{
    public PersonWrapper Employee = { get; set; }
    ...
    public void DoSomething()
    {
        Employee = new PersonWrapper(new Person());
        Employee.FirstName = "Bob";
        Employee.AcceptChanges();
        Employee.FirstName = "John";
        // Returns "Bob":
        string originalValue = Employee.GetOriginalValue<string>(nameof(Employee.FirstName));
        Employee.FirstName = "Gary";
        // Returns "Bob":
        string originalValue = Employee.GetOriginalValue<string>(nameof(Employee.FirstName));
        Employee.AcceptChanges();
        // Returns "Gary":
        string originalValue = Employee.GetOriginalValue<string>(nameof(Employee.FirstName));
    }
}
```

### ___GetValue___
The ___GetValue___ method is a protected method that should be invoked in the getters of all of the simple properties in your derived model wrapper class.
Collection properties and complex properties should not use the ___GetValue___ method.
This method retrieves the current value of the given property of the wrapped model class object. The method has the following signature:

```csharp
protected TValue? GetValue<TValue>([CallerMemberName] string? propertyName = null)
```

The method has a generic type parameter ___TValue___ that is used to supply the return type of the specified property.
It also takes an optional ___string___ argument that gives the name of the property for which we want to retrieve its current value. The argument
is decorated with the ___CallerMemberName___ attribute, and so it can be omitted when calling the ___GetValue___ method from within a property.

Example:
```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    ...
    public int Age
    {
        get => GetValue<int>();
        set => SetValue(value);
    }
    ...
}

public class MyClass
{
    public PersonWrapper { get; set; }
    ...
    public void DoSomething()
    {
        Employee = new PersonWrapper(new Person());
        Employee.Age = 25;
        // Calling GetValue via the getter of the Age property:
        int employeeAge = Employee.Age;
        // Calling GetValue directly (not usually done this way):
        employeeAge = GetValue<int>(nameof(Employee.Age));
    }
}
```

> __Note:__ In order for classes derived from ___ModelWrapper___ to function properly, all properties in the model class that is being wrapped must have a
> corresponding property in the model wrapper class. For example, if the ___Person___ model class has a property ___FirstName___ which returns a
> ___string___, the ___PersonWrapper___ class must also have a ___FirstName___ property that returns a ___string___. Also, the properties in both the
> model class and the wrapper class must not contain any business logic. All necessary logic for handling validation and property change notification is
> implemented in the ___SetValue___ method discussed later in this __README__ file. Also note that the wrapper class may contain additional properties
> beyond the ones contained in the wrapped model class.

### ___InitializeCollectionProperties___
The ___InitializeCollectionProperties___ method is a protected virtual method used for initializing collection properties in the model wrapper class
object. Any model class having one or more collection properties must override this method in the derived model wrapper class. The method has the
following signature:

```csharp
protected virtual void InitializeCollectionProperties()
```

The ___InitializeCollectionProperties___ method is called by the constructor of the ___ModelWrapper___ base class. The method is used to create
wrapped copies of each object that is in the collection property of the model class and add those wrapped copies to the corresponding collection
in the wrapper class. This is required to support change notification and revertible change tracking of the collection property and all its
contained objects.

Example:
```csharp
public class Person
{
    ...
    public List<Email> EmailAddresses { get; set; }
    ...
}

public class PersonWrapper : ModelWrapper<Person>
{
    ...
    public ChangeTrackingCollection<EmailWrapper> EmailAddresses { get; private set; }
    ...
    protected override void InitializeCollectionProperties()
    {
        if (Model.EmailAddresses is null)
        {
            throw new ArgumentException("EmailAddresses must not be null.");
        }
         
        EmailAddresses = new ChangeTrackingCollection<EmailWrapper>(Model.EmailAddresses.Select(e => new EmailWrapper(e)));
        RegisterCollection(EmailAddresses, Model.EmailAddresses);
    }
}
```

The example above uses the ___Model___ property of the ___ModelWrapper___ class to reference the model class object that is wrapped. If there are
more than one collection property in the model class, then the statements inside the ___InitializeCollectionProperties___ method would be
repeated for each collection property. (Note that the collection property on the model class must not be ___null___.) The ___RegisterCollection___
method (covered later) is required to register the collection property with the change tracking mechanism of the ___ModelWrapper___ class.

### ___InitializeComplexProperties___
The ___InitializeComplexProperties___ method is a protected virtual method used for initializing complex properties in the model wrapper class
object. Any model class having one or more complex properties must override this method in the derived model wrapper class. The method has the
following signature:

```csharp
protected virtual void InitializeComplexProperties()
```

The ___InitializeComplexProperties___ method is called by the constructor of the ___ModelWrapper___ base class. The method is used to create a
wrapped copy of a complex property object in the model class and save that copy to the corresponding complex property in the wrapper class.
This is required to support change notification and revertible change tracking of the complex property objects.

Example:
```csharp
public class Person
{
    ...
    public Address HomeAddress { get; set; }
    ...
}

public class PersonWrapper : ModelWrapper<Person>
{
    ...
    public AddressWrapper HomeAddress { get; private set; }
    ...
    protected virtual void InitializeComplexProperties()
    {
        if (Model.HomeAddress is null)
        {
            throw new ArgumentException("HomeAddress must not be null.");
        }

        HomeAddress = new AddressWrapper(Model.HomeAddress);
        RegisterComplex(HomeAddress);
    }
}
```

The example above uses the ___Model___ property of the ___ModelWrapper___ class to reference the model class object that is wrapped. If there are
more than one complex property in the model class, then the statements inside the ___InitializeComplexProperties___ method would be
repeated for each complex property. (Note that the complex property on the model class must not be ___null___.) The ___RegisterComplex___
method (covered later) is required to register the complex property with the change tracking mechanism of the ___ModelWrapper___ class.

### ___OnErrorsChanged___
The ___OnErrorsChanged___ method is a protected virtual method that is inherited from the ___NotifyDataErrorInfoBase___ class. This method is called
from the ___ClearErrors___ method described above, and from the protected ___Validate___ method described below. ___OnErrorsChanged___ raises the
___ErrorsChanged___ event for the specified property.

The ___OnErrorsChanged method has the following signature:
```csharp
protected virtual void OnErrorsChanged(string propertyName)
```

The single argument supplies the name of the property for which we want to raise the ___ErrorsChanged___ event. We normally don't have to be concerned
with raising this event on our own since the ___ModelWrapper___ base class does it for us. But the ___OnErrorsChanged___ method can be invoked in the
classes that derive from ___ModelWrapper___ if there is a specific need.

### ___OnPropertyChanged___
The ___OnPropertyChanged___ method is a protected virtual method that is inherited from the ___NotifyDataErrorInfoBase___ class which in turn inherits
it from the ___Observable___ class. This method is called from several different methods of the ___ModelWrapper___ base class. The method raises the
___PropertyChanged___ event on the specified property. Normally you won't need to call this method from the classes you derive from the ___ModelWrapper___
class, but the option is there if you need it.

The ___OnPropertyChanged___ method has the following signature:
```csharp
protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
```

### ___RegisterCollection___
The ___RegisterCollection___ method is a protected method used to register a collection property in the derived model wrapper class with the change
tracking mechanism of the ___ModelWrapper___ base class. The ___RegisterCollection___ method also sets up the ___CollectionChanged___ event handler
on the collection object. This method should only be invoked from within the overridden ___InitializeCollectionProperties___ method (covered earlier
in this document). The method must be invoked once for each collection property defined in the model wrapper class.

This is the signature of the ___RegisterCollection___ method:
```csharp
protected void RegisterCollection<TWrapper, TModel>(ChangeTrackingCollection<TWrapper> wrapperCollection, IList<TModel> modelCollection)
    where TWrapper : ModelWrapper<TModel>
    where TModel : class
```

Refer to the example in the earlier section above that describes the ___InitializeCollectionProperties___ method.

### ___RegisterComplex___
The ___RegisterComplex___ method is a protected method used to register a complex property in the derived model wrapper class with the change
tracking mechanism of the ___ModelWrapper___ base class. This method should only be invoked from within the overridden
___InitializeComplexProperties___ method (covered earlier in this document). The method must be invoked once for each complex property defined
in the model wrapper class.

This is the signature of the ___RegisterComplex___ method:
```csharp
protected void RegisterComplex<TModel>(ModelWrapper<TModel> wrapper)
    where TModel : class
```

Refer to the example in the earlier section above that describes the ___InitializeComplexProperties___ method.

### ___RejectChanges___
The ___RejectChanges___ method is called to reject all pending changes on the wrapped model object. If the ___IsChanged___ property is ___false___
then the method will return without doing anything.

This method will call the ___RejectChanges___ method for modified objects returned from any complex properties the wrapped model class
may have. It will also call ___RejectChanges___ on any modified objects contained in any collection property of the wrapped model class. It can do
this because both these types of objects must implement the ___IValidatingTrackingObject___ interface which, in turn, inherits from the
___IRevertibleChangeTracking___ interface which defines the ___RejectChanges___ method.

Once all changes have been rejected, this method raises the ___PropertyChanged___ event for all properties on the wrapped model class.

Example:
```csharp
public class MyClass
{
    public PersonWrapper Employee { get; set; }
    ...
    public void DoSomething()
    {
        Employee = new PersonWrapper(new Person());
        // Make a change and accept it:
        Employee.LastName = "Larson";
        Employee.AcceptChanges();
        // Doesn't do anything since no changes have been
        // made since the changes were accepted:
        Employee.RejectChanges();
        // Make a change:
        Employee.LastName = "Smith";
        // Returns "Smith":
        string employeeLastName = Employee.LastName;
        // Reject the changes:
        Employee.RejectChanges();
        // Returns "Larson":
        employeeLastName = Employee.LastName;
    }
}
```

### ___SetValue___
The ___SetValue___ method is a protected method that should be invoked from within the setters of all simple properties of your derived model wrapper
class. This method should not be invoked from within collection properties or complex properties. This method sets the current value of the property
to a new value. This method also keeps track of the original value of the property in case the current value needs to be reverted back to the
original value by the ___RejectChanges___ method (discussed earlier in this document).

If the new value specified in the call to ___SetValue___ differs from the original value of the property, then the protected version of the
___Validate___ method (discussed later in this document) is invoked after the change is made. Also, the ___PropertyChanged___ event is raised for
the property.

The ___SetValue___ method has the following signature:

```csharp
protected void SetValue<TValue>(TValue? newValue, [CallerMemberName] string? propertyName = null)
```

The method has a generic type parameter ___TValue___ that is used to supply the return type of the specified property. The ___newValue___
parameter gives the new value for the property. The method also takes an optional ___string___ argument that gives the name of the property for
which we want to change its current value. The argument is decorated with the ___CallerMemberName___ attribute, and so it can be omitted when
calling the ___SetValue___ method from within a property.

Example:
```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    ...
    public string FirstName
    {
        get => GetValue<string>();
        set => SetValue(value);
    }
    ...
}

public class MyClass
{
    public PersonWrapper { get; set; }
    ...
    public void DoSomething()
    {
        Employee = new PersonWrapper(new Person());
        // Calling SetValue via the setter of the FirstName property:
        Employee.FirstName = "Bob";
        // Calling SetValue directly (not usually done this way):
        SetValue("Larry", nameof(Employee.FirstName));
    }
}
```

> __Note:__ You may notice that the type parameter doesn't appear on the two calls to ___SetValue___ in the example above. This is because of a
> feature in C# .NET that is able to infer the type of the type parameter in certain cases. In the ___PersonWrapper___ class definition it knows
> the type of the ___value___ argument is ___string___, and so it infers the type argument for the ___SetValue___ method must be ___string___.
> Similarly, in the ___MyClass___ class it knows the literal value "Larry" is a ___string___, and so it infers the type parameter to be ___string___
> as well. However, it is not able to infer the type of the type parameter on the ___GetValue___ method, and so that type has to be explicitly stated.

### ___Validate___ (the public virtual version)
There are two different versions of the ___Validate___ method defined in the ___ModelWrapper___ class. The version covered here is a public virtual
implementation of the ___Validate___ method that is defined by the ___IValidatableObject___ interface that the ___ModelWrapper___ class derives from.
Since the ___ModelWrapper___ class implements the ___IValidatableObject___ interface, all derived classes inherit the ability to validate properties
based on data annotation attributes that are defined on the properties of the model wrapper class.
(Refer to the ___System.ComponentModel.DataAnnotations___ namespace for a list of supported attributes.)

This version of the ___Validate___ method must be overridden in the derived model wrapper class when validation is needed that goes beyond the
simple validation provided by the ___DataAnnotation___ attributes. For example, if the validity of one property depends on another, the logic
for validating this dependency can be added to the overridden ___Validate___ method.

The ___Validate___ method has the following signature:

```csharp
public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
```

A ___ValidationContext___ must be passed into the ___Validate___ method. This defines the context in which the validation check is performed. This
context is set for us by the ___ModelWrapper___ class, so we don't need to be concerned about it. (The context is actually set in the other version
of the ___Validate___ method described later in this document.)

The ___Validate___ method returns a collection of ___ValidationResult___ objects. Each ___ValidationResult___ corresponds to a validation error
that has been detected on the model wrapper class object. Each ___ValidationResult___ generated by the overridden ___Validate___ method must be
initialized with an error message string and a collection of property names that failed the validation. For example, assume the Age property failed
validation because it wasn't in the expected range. The ___ValidationResult___ object for this might be initialized as follows:

```csharp
ValidationResult validationResult = new ValidationResult("Age is outside valid range.", new[] { nameof(Age) };
```

Here is a sample overridden ___Validate___ method in a derived ___ModelWrapper___ class:

```csharp
public class PersonWrapper : ModelWrapper<Person>
{
    public int Age { get; set; }
    public string FirstName { get; set; }
    ...
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Age < 18 || Age > 65)
        {
            yield return new ValidationResult("Age is not within valid range.", new[] { nameof(Age) });
        }
        if (string.IsNullOrWhitspace(FirstName))
        {
            yield return new ValidationResult("First name is required.", new[] { nameof(FirstName) });
        }
    }
}
```

> __Note:__ Both of the validations in the example above could have been implemented just as easily with ___DataAnnotation___ attributes. The
> ___Range___ attribute could have been used for the ___Age___ property, and the ___Required___ attribute could have been used for the
> ___FirstName___ property. The example shows that you don't necessarily need to use attributes. You can put all of your validation logic
> in the ___Validate___ method. Conversely, if all your validation is implemented using attributes, you don't need to override the ___Validate___
> method in your derived model wrapper class.

### ___Validate___ (the protected version)
This is the second version of the ___Validate___ method implemented in the ___ModelWrapper___ base class. It is used to initiate the validation of
the properties of the model wrapper classes derived from the ___ModelWrapper___ base class. The ___ModelWrapper___ base class calls
this version of the ___Validate___ method from four locations:

- The ___ModelWrapper___ constructor calls ___Validate___ after calling the ___InitializeComplexProperties___ and ___InitializeCollectionProperties___
  classes.
- The ___RejectChanges___ method calls ___Validate___ after rejecting all pending changes.
- The ___SetValue___ method calls ___Validate___ after changing the current value of a simple property.
- ___Validate___ is also called from within the ___CollectionChanged___ event handler that gets attached to collection properties by the
  ___RegisterCollection___ method.

Normally you shouldn't need to call this version of ___Validate___ from any other location in the model wrapper classes that you derive from the
___ModelWrapper___ base class. However, this method was defined as ___protected___ so that you could call it from your derived classes if needed.

This version of the ___Validate___ method has this signature:

```csharp
protected void Validate()
```

The ___Validate___ method performs the following steps:

1. Clears the collection of errors (see the ___Errors___ property covered earlier in this document)
1. Sets the validation context to the derived model wrapper class
1. Calls ___Validate.TryValidateObject___ using the validation context
1. If there are any validation errors found:
    a. Gets a list of all properties having errors
    a. For each property, saves the list of validation errors to the ___Errors___ collection and then raises the ___ErrorsChanged___ event on the property
1. Raises the ___PropertyChanged___ event on the ___IsValid___ property of the model wrapper class

No example will be given for the protected version of the ___Validate___ method since it is quite easy to use. Simply insert the following statement into
your derived model wrapper class where you feel you need to manually trigger the validation process:

```csharp
Validate();
```

# ___NotifyDataErrorInfoBase___ Base Class

___NotifyDataErrorInfoBase___ is an abstract base class that provides validation support and error notification. The ___ModelWrapper___ base class
described above derives from this class. ___NotifyDataErrorInfoBase___ in turn derives from the ___Observable___ base class described later in this
document. This means that classes derived from ___NotifyDataErrorInfoBase___ also provide property change notification.

## Interfaces
The ___NotifyDataErrorInfoBase___ class implements the ___INotifyDataErrorInfo___ interface. It also indirectly implements the ___INotifyPropertyChanged___
interface through the ___Observable___ base class.

## Constructors
___NotifyDataErrorInfoBase___ has a single constructor with the following signature:

```csharp
protected NotifyDataErrorInfoBase()
```

Since the constructor is protected it can only be invoked from within classes that derive from ___DataErrorInfoBase___. In fact, this constructor never
needs to be called directly since it is a parameterless default constructor and will get invoked automatically as needed.

## Fields

### ___Errors___
___Errors___ is a protected field that can only be referenced from within classes that derive from ___NotifyDataErrorInfoBase___.
This field contains a ___Dictionary\<string, List\<string>>___ collection of validation errors. The key of this ___Dictionary___ object is a
___string___ containing the name of a property found in a class that derives from ___NotifyDataErrorInfoBase___. The value associated with the
key is a ___List\<string>___ containing any validation errors that were found on that property (or an empty list of no errors were found for
that property).

> __Note:__ The ___NotifyDataErrorInfoBase___ class doesn't actually do anything with the ___Errors___ field. It is up to derived classes
> to supply the functionality required to maintain the ___Errors___ collection. For example, it is the protected ___Validate___ method that
> provides this functionality in the ___ModelWrapper___ class. Refer to the description of that method earlier in this document.

The ___ModelWrapper___ class described earlier in this document also contains a description of the ___Errors___ field. Refer to that section
for more information and an example usage.

## Events
___NotifyDataErrorInfoBase___ implements the following event:

### ___ErrorsChanged___
The ___ErrorsChanged___ event is raised whenever the validity of a property in a derived class changes. It can also be raised explicitly after
performing some action that alters the validity of the derived class object. (See the ___OnErrorsChanged___ protected method described later.)

## Properties

### ___HasErrors___
___HasErrors___ is a property that returns ___true___ if there are any validation errors on the derived class object. To be more exact, this
property returns ___true___ if the ___Errors___ collection described above isn't empty.

The ___HasErrors___ property is also described in the section of this document dealing with the ___ModelWrapper___ class. Refer to that
section for more details and an example.

## Methods

### ___ClearErrors___
The ___ClearErrors___ method is a protected method that simply clears the contents of the ___Errors___ collection. It also raises the ___ErrorsChanged___
event for each property name that is found in the ___Errors___ collection. ___ClearErrors___ is something that normally would be called just before
kicking off validation on a class derived from ___NotifyDataErrorInfoBase___. This would be done in order to clear the old validation errors
before repopulating the ___Errors___ collection with the new errors after validation is completed.

The ___ClearErrors___ method has the following signature:
```csharp
protected void ClearErrors();
```

Example (this is part of the ___Validate___ method of the ___ModelWrapper___ class):
```csharp
protected void Validate()
{
    // Clear the Errors collection before initiating validation.
    ClearErrors();

    List<ValidationResult> results = new();
    ValidationContext context = new(this);
    // Kick off the validation.
    Validator.TryValidateObject(this, context, results);
    ...
}
```

### ___GetErrors___
The ___GetErrors___ method takes a single parameter that specifies the name of a property on a class that derives from the
___NotifyDataErrorInfoBase___ base class. It returns a collection of strings that are the validation error messages for any
validation errors that the property currently has (or an empty list if there are no validation errors currently for that
property). ___GetErrors___ retrieves these error messages from the ___Errors___ collection mentioned above.

Refer to the ___GetErrors___ property on the ___ModelWrapper___ class described earlier in the document for more details on the
___GetErrors___ property and for sample usage.

### ___OnErrorsChanged___
The ___OnErrorsChanged___ method is a protected virtual method that raises the ___ErrorsChanged___ event on the given property.

The ___OnErrorsChanged___ method has the following signature:
```csharp
protected virtual void OnErrorsChanged(string propertyName)
```

The method takes a single argument that supplies the name of the property on which we want to raise the ___ErrorsChanged___ event.
This method is called by the ___ClearErrors___ method mentioned above after clearing the ___Errors___ collection. It would also be called for any
property that has validation errors in a derived class after the validation process is run. (Refer to the protected ___Validate___ method
of the ___ModelWrapper___ base class which is described earlier in this document.)

# ___Observable___ Base Class
The ___Observable___ class is an abstract base class that provides property change notification for any class derived from it. This class is the
base class for the ___NotifyDataErrorInfoBase___ class that was described in the previous section of this document.

## Interfaces
The ___Observable___ base class implements the ___INotifyPropertyChange___ interface.

## Constructors
There are no explicit constructors for the ___Observable___ class. So the .NET runtime will provide a parameterless default constructor that will
be invoked automatically when the ___Observable___ class is instantiated.

## Events

### ___PropertyChanged___
The ___PropertyChanged___ event gets raised whenever the value of a property changes.

## Methods

### ___OnPropertyChanged___
The ___OnPropertyChanged___ method is a protected virtual method that can be called to raise the ___PropertyChanged___ event for the specified
property.

The ___OnPropertyChanged___ method has the following signature:
```csharp
protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
```

The single optional argument supplies the name of the property for which we want to raise the ___PropertyChanged___ event. The argument is decorated
with the ___CallerMemberName___ attribute so that the property name will automatically be passed into the ___OnPropertyChanged___ method without
having to supply the argument as long as the call to ___OnPropertyChanged___ is made from within the property itself (specifically from within the
setter of the property).

# Sample Implementation of a Wrapper Class
This section gives a sample implementation of a class that is derived from the ___ModelWrapper___ abstract base class. The following example gives
one possible implementation of the ___PersonWrapper___ class which is a wrapper class for the ___Person___ model class that was described towards
the beginning of this __README__ file. The sample wrapper class would be suitable for use as a view model class in an MVVM application since it
provides all the functionality required for property change notification, revertible change tracking, validation, and data error notification.

```csharp
public class PersonWrapper : ModelClass<Person>
{
    // The following constructor is required:
    public PersonWrapper(Person person) : base(person) {}

    // The following properties map to simple properties
    // on the wrapped Person class. Notice the use of
    // GetValue and SetValue for getting and setting
    // the value of each property.
    public string FirstName
    {
        get => GetValue<string>();
        set => SetValue();
    }
    public string LastName
    {
        get => GetValue<string>();
        set => SetValue();
    }
    public int Age
    {
        get => GetValue<int>();
        set => SetValue();
    }

    // The following properties are used for getting
    // the IsChanged status of the corresponding
    // simple property.
    public bool FirstNameIsChanged => GetIsChanged(nameof(FirstName));
    public bool LastNameIsChanged => GetIsChanged(nameof(LastName));
    public bool AgeIsChanged => GetIsChanged(nameof(Age));

    // The following properties are used for getting
    // the original value of the simple property
    public string FirstNameOriginalValue
        => GetOriginalValue<string>(nameof(FirstName));
    public string LastNameOriginalValue
        => GetOriginalValue<string>(nameof(LastName));
    public int AgeOriginalValue
        => GetOriginalValue<int>(nameof(Age));

    // The following property maps to the complex
    // property on the Person class. Notice that the
    // property returns the wrapped Address class
    // object instead of the Address class object
    // itself. Also, GetValue and SetValue aren't
    // used on complex properties.
    public AddressWrapper HomeAddress { get; private set; }

    // The following property maps to the collection
    // property on the Person class. Notice that it
    // returns a ChangeTrackingCollection of
    // EmailAddressWrapper objects instead of a List
    // of EmailAddress objects.
    public ChangeTrackingCollection<EmailAddressWrapper> EmailAddresses { get; private set; }

    // The following override is required to initialize
    // the HomeAddress complex property. Notice that
    // the HomeAddress property in the PersonWrapper
    // class is set to a wrapped version of the
    // HomeAddress property in the Person class. The
    // wrapped HomeAddress is then registered with the
    // change tracking mechanism.
    protected override void InitializeComplexProperties()
    {
        if (Model.HomeAddress is null)
        {
            throw new ArgumentException("HomeAddress must not be null.");
        }
         
        HomeAddress = new AddressWrapper(Model.HomeAddress);
        RegisterComplex(HomeAddress);
    }

    // The following override is required to initialize
    // the EmailAddresses collection property. Notice
    // that the ChangeTrackingCollection is populated
    // with wrapped versions of the EmailAddress
    // objects from the EmailAddresses collection in
    // the Person class. The wrapped collection is then
    // registered with the change tracking mechanism.
    protected override void InitializeCollectionProperties()
    {
        if (Model.EmailAddresses is null)
        {
            throw new ArgumentException("EmailAddresses must not be null.");
        }
         
        EmailAddresses = new ChangeTrackingCollection<EmailAddressWrapper>(Model.EmailAddresses.Select(e => new EmailAddressWrapper(e)));
        RegisterCollection(EmailAddresses, Model.EmailAddresses);
    }

    // The following override provides validation for
    // the PersonWrapper class. The last if statement
    // shows an example of a single validation rule
    // involving more than one property.
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            yield return new ValidationResult("FirstName is required",
                new[] { nameof(FirstName) });
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            yield return new ValidationResult("LastName is required",
                new[] { nameof(LastName) });
        }

        if (Age < 18 || Age > 65)
        {
            yield return new ValidationResult("Age must be between 18 and 65.",
                new[] { nameof(Age) });
        }

        if (FirstName == "Bob" && EmailAddresses.Count == 0)
        {
            yield return new ValidationResult("Bob must have an email address",
                new[] { nameof(FirstName), nameof(EmailAddresses) });
        }
    }
}
```