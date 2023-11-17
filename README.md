# TextTemplateApp
## Overview
The ___TextTemplateApp___ solution is made up of four major components:

1. The ___ClassPropertyParser___ class library that allows you to extract type information for all classes contained within an assembly, or extract
   property information for all properties defined on a particular class.

2. The ___ModelWrapperBase___ class library provides an abstract base class which can be used to "wrap" simple model classes. The library
   extends the model class by adding the following features:
   a. Property change notification
   b. Revertible change tracking
   c. Validation
   d. Data error notification

3. The ___TextTemplateProcessor___ class library that provides tools for creating your own automatic text generator based on tokenized
   text templates.

4. The ___TextTemplateApp___ console application which demonstrates the use of each of the previous three class libraries.

Each of the four major components has its own detailed README file located in its project directory.

## Solution Contents
The ___TextTemplateApp___ solution contains nine projects:

- ___ClassPropertyParser___ - The Class Property Parser class library (refer to the ___README.md___ file in the project folder)

- ___ClassPropertyParser.Tests___ - Unit tests for the ___ClassPropertyParser___ class library

- ___ModelWrapperBase___ - The Model Wrapper Base class library (refer to the ___README.md___ file in the project folder)

- ___ModelWrapperBase.Tests___ - Unit tests for the ___ModelWrapperBase___ class library

- ___TestModels___ - Contains four model classes used by the sample app in the ___TextTemplateApp___ project

- ___TestShared___ - Contains classes and constants that are shared amongst the three unit test projects

- ___TextTemplateApp___ - A project containing a sample console application that uses the ___ClassPropertyParser___, ___ModelWrapperBase___, and
  ___TextTemplateProcessor___ class libraries

- ___TextTemplateProcessor___ - The Text Template Processor class library (refer to the ___README.md___ file in the project folder)

- ___TextTemplateProcessor.Tests___ - Unit tests for the ___TextTemplateProcessor___ class library

The ___xUnit___ and ___FluentAssertions___ NuGet packages are used in all three unit test projects. The ___Moq___ package is used in the
___TextTemplateProcessor.Tests___ unit test project.

---
# Compatibility
All projects in the ___TextTemplateApp___ solution were developed using ___C# 10.0___ and ___.NET 6.0___ for the ___Windows___ desktop targeting
___Windows 7___ and above.