# Text Template Processor Sample Application
## Overview
The ___TextTemplateApp___ project contains a sample ___ModelWrapperGenerator___ class that is derived from the
___TextTemplateConsoleBase___ class. This class is instantiated in the ___Program___ class. The ___Program___ class is quite simple.
It performs the following actions:

- Instantiates a copy of the ___ClassPropertyParser___ class and uses it to obtain all the model class types contained in the ___TestModels___ project.

- Instantiates a copy of the ___ModelWrapperGenerator___ class passing in the text template file path, the generated output directory path, and the
  name that should be used for the generated model wrapper class namespace.

- Clears the contents of the output directory by calling the ___ClearOutputDirectory___ method of the ___ModelWrapperGenerator___ class object.

- Calls the ___Generate___ method of the ___ModelWrapperGenerator___ class passing in a reference to the ___ClassPropertyParser___ class object that
  was instantiated in the first step.

## Objective
This sample application attempts to show:

- How to correctly use the ___ClassPropertyParser___ class to retrieve class type information and property information from an assembly

- How to derive your own text generator class from the ___TextTemplateConsoleBase___ class

- What a typical text template file looks like (the sample can be found under the ___Wrappers\Template___ subdirectory within the ___TextTemplateApp___
  project directory)

- How to use the text generator class to generate model wrappers based on the ___ModelWrapper___ class (wrappers are generated for each model class
  found in the ___TestModels___ project)

When the ___ProgramClass___ object is finished processing, you should see the generated model wrapper class definitions in the ___Wrappers\Generated___
subdirectory within the ___TextTemplateApp___ project directory.

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

All projects in the solution were written in C# 10.0 and .NET 6.0