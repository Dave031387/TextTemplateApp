# Text Template Processor Class Library
## Introduction
The __Text Template Class Library__ provides functionality that can be used for processing text template files to produce generated text files.
For example, in an MVVM application you might have view model classes that wrap your model classes. These view model classes likely have a lot
of code in common. You could create a text template file that could be used to automatically generate your view model classes from the model
classes. Areas of the template that need to differ from one generated file to the next can be indicated by token placeholders in the template
file. The __Text Template Processor__ would then substitute the correct strings for these placeholders when the output file is generated.

There are two classes defined in the __Text Template Class Library__ that provide the tools you need to create your own templated text file
generator. These classes are:

- ___TextTemplateProcessor___ - This class provides all the functionality required for reading and parsing text template files, extracting token
  names and replacing tokens in the template with actual values, and for writing the generated text to an output file.

- ___TextTemplateConsoleBase___ - This is an abstract base class that you can derive from to create your own console class containing the logic
  needed to correctly process your template files and generate the desired text files. This class derives from the ___TextTemplateProcessor___
  class, so it contains all the basic functionality. You just add functionality specific to your particular needs.

# Template File Structure
A template file consists of a series of lines of text. The first three characters of each line is a prefix. The prefix identifies the line as being
a segment header (covered later), comment line, or a (possibly tokenized) line of text that will be written to the generated output file.

The fourth character of each line must be blank.

The remainder of the line depends on the type of line. For segment headers, there will be the segment name and possibly an optional segment parameter
or two. For comment lines it will be the comment text. For template text lines it will be the tokenized text to be written to the output file.

## Line Prefixes and Indentation
The first three characters (the prefix) of each line in the template file determine how the rest of the line is interpreted. The valid prefixes are
described below. The _"n"_ at the end of some prefixes represents an integer value that must be between 0 and 9.

- `###` - This prefix marks the line as a segment header. Each logical grouping of text lines in the template file must begin with a segment header.
- `///` - This prefix marks a comment line. Comments are an optional feature used for documenting your template file. Comments have absolutely no
  impact on the generated output file. They are ignored by the ___TextTemplateProcessor___ class.

The remaining prefixes all mark tokenized text lines that get written to the generated output file. The prefix describes how the line is indented
before being written to the file.

- `@=n` - The current text line will be indented exactly _n_ tab stops to the right starting from the left margin.
- `@+n` - The current text will be indented _n_ tab stops to the right starting from the indent value carried over from the previous text line.
- `@-n` - The current text will be indented _n_ tab stops to the left starting from the indent value carried over from the previous text line, but
   will not be moved any further left than the left margin.

The previous three prefixes alter the current indent value that gets carried over to the next text line. The next text line will be indented
relative to what this new current indent value is. The next three prefixes below mirror the previous three in that they affect the current text line in
exactly the same way as described above. However, they have no impact on the current indent value that gets carried over to the next text line.
These are called _"one-time"_ indent prefixes since they affect only the current line.

- `O=n` - The current text line will be indented exactly _n_ tab stops to the right starting from the left margin.
- `O+n` - The current text will be indented _n_ tab stops to the right starting from the indent value carried over from the previous text line.
- `O-n` - The current text will be indented _n_ tab stops to the left starting from the indent value carried over from the previous text line, but
   will not be moved any further left than the left margin.

There is one more valid prefix. It consists of just three spaces. When this prefix is used, the text line will be written to the output file
using the current indent value, and the current indent value remains unchanged. This would be the same as using any of these prefixes:
`@+0`, `@-0`, `O+0`, or `O-0`.

In a tokenized text line from a text template file, the prefix takes up the first three characters. This is followed by a mandatory space.
Everything starting from the fifth character position to the end of the line makes up the text that will get written to the generated
output file.

An example may give a better idea of how these prefixes affect the generated text. Each item in the following bulleted list starts with a sample
line from a template file. The bulleted list represents a contiguous series of lines from the same template file. It is assumed that before the first
line is processed the current indent value is set to its initial value of zero. Following each sample line there is a description of how the current
text line will be formatted and what impact, if any, there will be on the current indent value. The tab is assumed to be set at 2 spaces.

- `@+2 Line 1` - The prefix indicates that we should indent this line 2 tab stops to the right relative to the current indent value. This means the
  line will be indented 4 spaces (2 tab stops times 2 spaces per tab stop). Therefore, the text "Line 1" will be written to the output file after
  adding 4 spaces to the beginning of the line. The current indent value will then be changed from 0 to 4 since this isn't a one-time indent.

- `O-1 Line 2` - The prefix indicates that we should indent this line 1 tab stop to the left of the current indent value. The current indent is 4, so
  going left 1 tab stop would mean this line needs to be indented 2 spaces (current indent of 4 spaces minus 1 tab stop of 2 spaces). Therefore, the
  text "Line 2" will be written to the output file after adding 2 spaces to the beginning of the line. The current indent value of 4 will not change
  since this is a one-time indent.

- `@+1 Line 3` - The text "Line 3" would be written to the output file after adding 6 spaces to the beginning of the line. (Current indent of 4 spaces
  plus tab right one tab stop of 2 spaces equals 6.) The current indent will then be set to 6.

- `@=1 Line 4` - The prefix indicates that the line should be indented 1 tab stop from the left margin (1 tab stop is 2 spaces). The text "Line 3"
  will be written to the output file after adding 2 spaces to the beginning of the line. The current indent will then be set to 2 spaces.

- `@-2 Line 5` - The prefix indicates that the line should be indented 2 tab stops to the left of the current indent value. However, this would
  result in an indent value of -2 (current indent is 2, minus 2 tab stops of 2 spaces each equals -2). Negative indents aren't allowed. So the
  text "Line 5" will be written to the output file with no spaces inserted at the beginning of the line. The current indent will then be set to 0.

The resulting output file would look like this (periods are used to represent spaces that were inserted for the indentation):

```
....Line 1
..Line 2
......Line 3
..Line 4
Line 5
```

One thing should be apparent from the previous examples - the indent values will always be multiples of the tab size. If you wish to use some indent value
that isn't a multiple of the tab size then you will need to insert the appropriate number of spaces ahead of the text on the line in the template file.
For example, if you wanted "Line 2" in the example above to be indented 3 spaces instead of 2, then you would insert a space in front of "Line 2" so that
there are two spaces between the prefix and the text instead of just one. (Remember that the space immediately following the prefix isn't counted
as part of the text, but any spaces after that are counted as text.)

## Segments
A _segment_ in the text template file consists of a segment header line and one or more contiguous lines of text that make up a logical building block
of your template file. A segment header line starts with a segment prefix (`###`) followed by a space. A segment name must appear starting at the
fifth character position of the line. Segment names can contain upper- and lowercase letters, numbers, and the underscore character. They can be any
length. However, they must start with an upper- or lowercase letter and they must not contain embedded spaces.

Following the segment name there can be optional parameters of the form "parameter=value". There must not be any spaces on either side of the equals
sign. One or more spaces must appear between the segment name and the first parameter, and between each subsequent parameter. The following example
shows the proper format of a segment header containing two parameters:

```
### SegmentName PARAM1=value1 PARAM2=value2
```

### Segment Parameters
Currently there are only Three segment parameters supported by the __Text Template Class Library__:

- ___FTI___ - This is the _First Time Indent_ parameter.
- ___PAD___ - This is the _Pad Segment_ parameter.
- ___TAB___ - This is the _Tab Size_ parameter.

#### The ___FTI___ Option
The ___FTI___ (_First Time Indent_) parameter controls the indentation of the first text line of a segment block the first time that segment is
processed. The ___FTI___ parameter must be assigned an integer value between __-9__ and __9__, inclusive. The value overrides the indent amount
specified in the prefix of the first text line of the segment, and it is always relative to the current indent value at the time the segment is
processed.

Consider the following template file:

```
### Segment1
    Line 1.1
### Segment2 FTI=1
    Line 2.1
@+1 Line 2.2
@-1 Line 2.3
```

Assuming the current indent is 0 and the tab size is 2 spaces, if we were to process __Segment1__ once and then __Segment2__ twice, the output would
look like this:

```
Line 1.1
  Line 2.1
    Line 2.2
  Line 2.3
  Line 2.1
    Line 2.2
  Line 2.3
```

The first time _Line 2.1_ is processed it is indented one tab stop (___FTI=1___) to the right of the current indent value carried forward from _Line 1.1_.
The second time it is processed it uses the current indent value carried forward from _Line 2.3_ without modification (since _Line 2.1_ has a blank prefix).
As you can see, the ___FTI___ parameter is useful for indenting the text in one segment relative to the text in the preceding segment.

You might be wondering why the ___FTI___ parameter is even needed. Couldn't we just omit it and instead set the indent in the prefix  of the first text
line to the correct value? If we tried this, our template file from the previous example would look like this:

```
### Segment1
    Line 1.1
### Segment2
@+1 Line 2.1
@+1 Line 2.2
@-1 Line 2.3
```

If we then processed __Segment1__ once and then __Segment2__ twice as before, the output would look like this:

```
Line 1.1
  Line 2.1
    Line 2.2
  Line 2.3
    Line 2.1
      Line 2.2
    Line 2.3
```

You can clearly see that this is not the result that we wanted. So the ___FTI___ parameter does serve a useful function.

> __Note:__ Although zero is a valid indent value, you can't use it as an ___FTI___ option value. This is because the __Text Template Processor__ uses
> this value to signify that there is no ___FTI___ option specified for the current segment. If you use ___FTI=0___ on any segment header, an error
> message will be logged and the ___FTI___ option will be ignored.

#### The ___PAD___ Option
The ___PAD___ (_Pad Segment_) parameter is used for inserting the text lines from one segment (the _Pad Segment_) between multiple occurrences of the
current segment. The ___PAD___ parameter must be set to the name of a different segment that has been defined earlier in the template file. The first
time the current segment is processed nothing special happens. Every time after that, though, the _Pad Segment_ will automatically be processed
before the current segment is processed. The current segment name, line number, and current indent values will be saved prior to processing the _Pad
Segment_ and these values will be restored before the current segment is processed.

Consider the following template file:

```
### Segment0
@+1 Line 0.1
### Segment1
    Line 1.1
### Segment2 PAD=Segment0 FTI=1
    Line 2.1
    Line 2.2
```

Assuming the current indent is 0 and the tab size is 2 spaces, if we were to process __Segment1__ once and then __Segment2__ three times, the output
would look like this:

```
Line 1.1
  Line 2.1
  Line 2.2
    Line 0.1
  Line 2.1
  Line 2.2
    Line 0.1
  Line 2.1
  Line 2.2
```
The first time _Line 2.1_ is processed it is indented 1 tab stop to the right of the current indent because of the ___FTI=1___ option. However, the
line is output directly after _Line 1.1_ since the ___PAD___ option is ignored the first time _Line 2.1_ is processed.

When _Line 2.1_ is processed the second time, the ___FTI=1___ option is ignored, but the ___PAD=Segment0___ option is processed. This causes _Line 0.1_
to be processed ahead of _Line 2.1_. Notice that _Line 0.1_ is indented one tab stop to the right of the current indent value. Normally, this would
mean that _Line 2.1_ would be indented the same amount as _Line 0.1_. However, the current indent is saved before __Segment0__ is processed, and it is
restored afterwards. Therefore, the second time _Line 2.1_ is processed it will be indented the same amount as the last line processed before the
pad segment was processed, which is _Line 2.2_ in this example.

The ___PAD___ option is typically used to insert a blank line or some other separator between consecutive occurrences of the same segment. But, as the
example above demonstrates, it is not limited to that.

> __Note:__ Although a segment that is used as a __Pad Segment__ can itself specify the ___PAD___ option, it probably is not a good idea to do so
> because the output will likely not be what you expected.

> __Note:__ An error will be written to the log and the ___PAD___ option value will be ignored if the value isn't a valid segment name.

#### The ___TAB___ Option
The ___TAB___ option is perhaps the easiest of the three segment header options to understand. It simply specifies a new value to be used for the
tab size. Every time a segment is processed, if it specifies the ___TAB___ option, the current tab size will be set to the value of the ___TAB___
option before processing the text lines in the segment. The new tab size value will remain in effect until either another segment is processed
that specifies a different tab size, or the ___SetTabSize___ method is called on the ___TextTemplateProcessor___ class object.

Specifying a ___TAB___ option is not required in any segment header of a template file. If no ___TAB___ option is specified on any segment, then the
default tab size of 4 will be used. The default value can also be overridden through the ___SetTabSize___ method of the ___TextTemplateProcessor___
class described later in this __README__ file. If you do specify the ___TAB___ option on a segment header, it must be assigned a value between 1 and
9, inclusive.

There are a couple of things to be aware of when changing the tab size value. First of all, if you start out with one tab size, and then change
the tab size to a new value after you have processed some segments, the current indent value may no longer be a multiple of the tab size value.
For example, suppose the current indent was 4 after processing a segment. If the next segment to be processed specifies the option ___TAB=3___
then the current indent will no longer be a multiple of the tab size. This can possibly lead to some strange alignment issues in your output if
you're not careful.

The second thing to be aware of is the potential impact of specifying the ___TAB___ option on some segments, but not on others, especially if
the ___TAB___ options specify different tab sizes. Imagine a segment (call it _Segment1_) is defined without the ___TAB___ option. A second
segment (_Segment2_) is defined with ___TAB=3___. Now imagine that _Segment1_ gets processed when the current tab size is set to 2. Then
_Segment2_ gets processed at some point after _Segment1_ and changes the tab size to 3. Now if _Segment1_ happens to be processed again
immediately after _Segment2_ it will be processed with a tab size of 3 - different than the first time it was processed. Again, this can lead
to some strange alignment issues.

The safest thing to do is to use the same tab size for everything in the template file. You can do this one of three ways:

- Don't specify the ___TAB___ option on any segment header. Instead, use the ___SetTabSize___ method of the ___TextTemplateProcessor___ class
  to set the tab size (or let it default to 4).

- Specify the ___TAB___ option on every segment header and give it the same value everywhere.

- Specify the ___TAB___ option only on the segment that will be the first segment processed in the template file.

If you must use different tab sizes, keep in mind the potential issues mentioned above, and plan accordingly.

Here's a sample template that demonstrates the use of the ___TAB___ option:

```
### Segment1
@=0 Line 1.1
@+1 Line 1.2
@+1 Line 1.3
### Segment2 TAB=2
@=0 Line 2.1
@+1 Line 2.2
@+1 Line 2.3
```

Assume __Segment1__ is processed first, then __Segment2__, and then __Segment1__ again. The first time __Segment1__ is processed it will use the
default tab size of 4 since it doesn't specify a ___TAB___ option. When __Segment2__ is processed the tab size will be set to 2. Finally, when
__Segment1__ is processed again the tab size will still be set to 2. The generated output will look like this:

```
Line 1.1
    Line 1.2
        Line 1.3
Line 2.1
  Line 2.2
    Line 2.3
Line 1.1
  Line 1.2
    Line 1.3
```

### Default Segment Names
There are certain situations where the ___TextTemplateProcessor___ class will choose to use a default segment name. The default name will be
the string _"DefaultSegment"_ followed by an integer value. Default names will be assigned in sequence. So the first default segment name will
be _"DefaultSegment1"_, the second one will be _"DefaultSegment2"_, and so on.

Default segment names will be generated under the following situations:

- If the first non-comment line in a text template file isn't a segment header, then a default segment header will be created with a default
  segment name and no optional segment parameters.

- If a segment name is found that is a duplicate of a previous segment name in the same text template file, a default segment name will be
  used instead of the duplicate name for that segment.

- A default segment name will be used if no name follows the segment header prefix (`###`) or if the segment name doesn't begin in the fifth
  character position of the segment header line.

- Finally, if the supplied segment name isn't a valid name as described above, then a default segment name will be used instead.

## Token Placeholders
The final important feature of text template files is the token placeholder. Tokens are placed in text lines at locations where the generated
output will vary each time the segment containing the text line is processed. A token must begin with the token start delimiter characters `<#=`
followed by one or more optional spaces, then the token name, then one or more optional spaces, and ending with the token end delimiter characters
`#>`.

The token names must follow the same rules as for segment names. That is, they must start with an upper- or lowercase letter and can contain
upper- or lowercase letters, the digits 0 through 9, or the underscore character.

As an example, let's say we want to generate text lines for an auto-property in C#. The segment definition in the text template file might look
like this:

```
### Property
    public <#=TypeName#> <#=PropertyName#> { get; set; }
```

The segment named "__Property__" contains one text line with two token placeholders -- one named "_TypeName_", and another named "_PropertyName_".
When the __Property__ segment is processed, the user would need to supply the values to substitute for the placeholders. (Refer to the description
of the ___GenerateSegment___ method of the ___TextTemplateProcessor___ class for details.) Let's say that the first time we process this segment
we supply the values "int" for _TypeName_ and "Age" for _PropertyName_. The second time we supply "string" and "Name" for the same tokens. The
resulting generated output would then look like this:

```
public int Age { get; set; }
public string Name { get; set; }
```

The same token name can be used multiple times in the same segment, or even on the same text line if necessary. Each occurrance of the token will
be replaced with whatever value was supplied when ___GenerateSegment___ is called.

If for some reason you need to be able to output the characters `<#=` to the generated output file, you will need to escape them with a backslash
character. For example:

```
### Segment1
    The value of \<#=Token1#> is "<#=Token1#>".
```

If we were to process __Segment1__ passing in the value "123" for _Token1_, we would get the following output:

```
The value of <#=Token1#> is "123".
```

Notice that the first appearance of `<#=Token1#>` in the text line is output to the generated text as-is since it is preceded by a backslash
character. The second occurrance, however, is replaced with the characters `123` since the token isn't escaped. Also notice that the backslash
character is not output to the generated text.

> __Note:__ Only backslash characters immediately preceding the token start delimiter characters will be omitted from the generated output. All other
> backslash characters will be output to the generated text as they appear on the text lines.

> __Note:__ If a token is invalid in some way (missing the token end delimiter characters, or the token name is invalid or missing) then the
> token will be output to the generated text as-is even if the token start delimiter characters aren't preceded by a backslash character. An
> error message will also be written to the log.

# ___ITextTemplateProcessor___ Interface
The ___ITextTemplateProcessor___ interface defines the public properties and methods needed for implementing a __Text Template Processor__. These
properties and methods are described in detail in the section that covers the ___TextTemplateProcessor___ class.

## Properties
The ___ITextTemplateProcessor___ interface defines the following public properties:

- ___CurrentIndent___ - gets the current indent value
- ___GeneratedText___ - gets a reference to the generated text buffer
- ___IsOutputFileWritten___ - gets a boolean value that indicates whether or not the generated text buffer has been written to the output file
- ___IsTemplateLoaded___ - gets a boolean value that indicates whether or not a text template file has been loaded
- ___TabSize___ - gets the current tab size value
- ___TemplateFilePath___ - gets the file path of the text template file

## Methods
The ___ITextTemplateProcessor___ interface defines the following public methods:

- ___GenerateSegment___ - creates the generated text for the given segment and adds it to the generated text buffer
- ___LoadTemplate___ - parses a text template and builds the necessary control structures
- ___ResetAll___ - resets the operating environment of the text template processor
- ___ResetGeneratedText___ - clears the generated text buffer and sets the current indent value to 0
- ___ResetSegment___ - resets the _"is first time"_ flag for the given segment to _true_
- ___SetTabSize___ - sets the tab size to the given value
- ___WriteGeneratedTextToFile___ - writes the contents of the generated text buffer to the specified output file

# ___TextTemplateProcessor___ Class

## Introduction
The ___TextTemplateProcessor___ class is the workhorse of the __Text Template Processor Class Library__. It is responsible for the following tasks:

- Reading the text template file into memory
- Parsing the text template and extracting tokens and control information
- Formatting generated text lines, which entails:
    - Adding spaces to the beginning of each text line to maintain proper indentation
    - Replacing token placeholders with the appropriate string values
    - Adding padding between blocks of text as needed
- Writing the generated text to the output file

Internally, the ___TextTemplateProcessor___ class maintains four control structures that are used during the text generation process:

- The _Control Dictionary_ contains the control information for each segment in the template file. This includes:
    - The _First Time Indent_ segment header option value, if specified
    - The _Pad Segment_ option value, if specified
    - The _Tab Size_ option value, if specified
    - A flag indicating whether or not the given segment has been processed at least once
- The _Segment Dictionary_ contains control information for all of the text lines in each segment. This includes:
    - The indent value for the text line (or 0 if none is specified)
    - A flag indicating whether the indent value is relative to the current indent value (a _"relative"_ indent) or relative to the left margin
      (an _"absolute"_ indent)
    - A flag indicating whether or not the indent value affects only the current text line (a _"one time"_ indent)
    - The text, including token placeholders, of each text line in the template file
- The _Token Dictionary_ maintains a list of all valid token names found in the text template file and their corresponding string values, if given.
- The _Generated Text Buffer_ holds all of the generated text lines until they have been written to the output file.

> __Note:__ The ___TextTemplateProcessor___ class makes use of several static classes, properties, and methods. Because of this, you should avoid
> having more than one instance of the ___TextTemplateProcessor___ class active at any given time since changes made in one instance will impact
> static properties that may get referenced by the other instance, thus yielding unpredictable results.

## Interfaces
The ___TextTemplateProcessor___ class implements the ___ITextTemplateProcessor___ class mentioned earlier in this document.

## Constructor
The ___TextTemplateProcessor___ class has one constructor with the following signature:

```csharp
public TextTemplateProcessor(string filePath)
```

The ___filePath___ argument gives the file path to a text template file. This must point to an existing file. An error will be written to the log
if the supplied value is null, empty, contains invalid file path characters, or is a non-existent file.

## Properties

### ___CurrentIndent___
The ___CurrentIndent___ property gets the current indent value. This value is used in calculating the number of spaces to add to the beginning of each
generated text line. The ___CurrentIndent___ property will be set to the new calculated value unless the text line specifies a _"one time"_ indent, in
which case the ___CurrentIndent___ value will remain unchanged.

Example:

```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
int currentIndent = processor.CurrentIndent;
```

### ___GeneratedText___
The ___GeneratedText___ property gets a reference to the generated text buffer.

```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
...
// Process some text segments here to generate some text.
...
IEnumerable<string> generatedText = processor.GeneratedText;
```

### ___IsOutputFileWritten___
The ___IsOutputFileWritten___ property gets a boolean value that will be _true_ if the generated text buffer has been written to the output file
since the last time that the ___GenerateSegment___ method was called. Otherwise, it will be _false_.

Example
```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
...
// Process some text segments here to generate some text.
...
bool isOutputFileWritten = processor.IsOutputFileWritten;
```

### ___IsTemplateLoaded___
The ___IsTemplateLoaded___ property gets a boolean value that will be _true_ if a template file has been successfully loaded into memory. Generally,
the only time this will return _false_ is if the ___TemplateFilePath___ property is invalid or doesn't point to an existing file, or if the
___ResetAll___ method was called.

Example:
```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
bool isTemplateFileLoaded = processor.IsTempalteFileLoaded;
```

### ___TabSize___
The ___TabSize___ property gets the current tab size value. This will be an integer value between 1 and 9, inclusive, and represents the number of
space characters that should be added to the beginning of a generated text line for each tab stop that the line is indented. The tab size value
can be changed by the ___TAB___ option on a segment header in the text template file, or by calling the ___SetTabSize___ method.

Example:
```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
int tabSize = processor.TabSize;
```

### ___TemplateFilePath___
The ___TemplateFilePath___ property gets the full file path of the text template file that is being processed. If a relative file path was
supplied to the constructor of the ___TextTemplateProcessor___ class, or to the ___LoadTemplate___ method, that relative file path will be
translated into the full file path and ___TemplateFilePath___ will be set to that value.

Example:
```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
string templateFilePath = processor.TemplateFilePath;
```

## Methods

### ___GenerateSegment___
The ___GenerateSegment___ method is used for generating text for the given segment. The method has the following signature:

```csharp
public void GenerateSegment(string segmentName, Dictionary<string, string>? tokenDictionary = null)
```

The ___segmentName___ parameter specifies the name of the segment whose text lines are to be generated. The name must be a valid segment name
that exists in the text template file.

The ___tokenDictionary___ parameter is an optional parameter used for supplying values for the token placeholders that are in the given segment.
The key of the ___tokenDictionary___ is the token name, and the value is the substitution value for that token name. Wherever the given token
is used in the text lines of the segment, it will be replaced with the substitution value. If no ___tokenDictionary___ is given when
___GenerateSegment___ is called, any token values that were supplied on previous calls to ___GenerateSegment___ will remain in effect.

> __Note:__ The ___tokenDictionary___ can contain "token name/substitution value" pairs for any token placeholder found anywhere in the text
> template file, even if they're not found in the segment given when ___GenerateSegment___ is called.

> __Note:__ Once a substitution value has been assigned to a token, that value remains in effect until it is modified by the ___tokenDictionary___
> supplied to another call to ___GenerateSegment___, or until the ___ResetAll___ method or ___LoadTemplate___ method is called.

Example (assumes _Segment1_ exists and contains tokens named "FirstName" and "LastName"):
```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
Dictionary<string, string> tokenDictionary = new();
tokenDictionary.Add("FirstName", "Bob");
tokenDictionary.Add("LastName", "Smith");
GenerateSegment("Segment1", tokenDictionary);
```

### ___LoadTemplate___
The ___LoadTemplate___ method is used for loading a text template file into memory and parsing it to create the control structures needed for
processing the template file. There are two versions of this method having the following signatures:

```csharp
public void LoadTemplate()
public void LoadTemplate(string filePath)
```

The version of ___LoadTemplate___ that doesn't have any parameters first checks to see if the value of the ___TemplateFilePath___ property is
a valid file path that points to an existing text template file. It also checks to make sure that the file hasn't already been loaded. If both
of these checks pass, the text template file will be read into memory and parsed. Otherwise, an appropriate error message will be written to
the log and the method returns without doing anything.

The other version of ___LoadTemplate___ takes a single parameter that must be a valid file path pointing to an existing text template file.
An error will be written to the log and the method will return without doing anything if either the file path isn't valid, or the file path
matches the ___TemplateFilePath___ property and has already been loaded. Otherwise, the ___TemplateFilePath___ will be set to the new file
path and the text template file will be read into memory and parsed. A message will also be written to the log if the generated output from
the previous text template file wasn't written to the output file before loading the new text template file.

Example:
```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path1");
...
// Process the template file and write the generated
// text to the output file.
...
processor.LoadTemplate("template.file.path2");
```

### ___ResetAll___
The ___ResetAll___ method is used to reset the operating environment of the ___TextTemplateProcessor___ class object. The method has the
following signature:

```csharp
public void ResetAll()
```

The ___ResetAll___ method performs the following actions:

- Clears the generated text buffer
- Clears the control dictionary
- Clears the segment dictionary
- Clears the token dictionary
- Sets the current indent value to 0
- Sets the tab size to the default value of 4
- Sets the "is template loaded" flag to _false_
- Sets the "is output file written" flag to _false_
- The default segment name generator will be reset (i.e., the next default segment name will start over with _"DefaultSegment1"_)

Example:
```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
...
// Process the template file, etc.
...
processor.ResetAll();
// Now we can reuse the same template file a second time...
// Note that we first need to reload the template file
// since it got cleared by ResetAll.
processor.LoadTemplate();
...
// Process the template file again with different
// values for the tokens, etc.
...
```

> __Note:__ The ___ResetAll___ method calls the ___ResetGeneratedText___ method to perform some of its actions. Therefore, there is no need to
> make a separate call to ___ResetGeneratedText___ after calling ___ResetAll___.

> __Note:__ The ___ResetAll___ method is automatically called by the ___LoadTemplate___ method just before loading a new template file.

### ___ResetGeneratedText___
The ___ResetGeneratedText___ method is used to clear the generated text buffer. The method has the following signature:

```csharp
public void ResetGeneratedText()
```

The ___ResetGeneratedText___ method performs the following actions:

- Clears the generated text buffer
- Sets the current indent value to 0
- Sets the tab size to the default value of 4
- Resets the _"is first time"_ flag for all segments in the control dictionary to _true_

As you can see, this is a subset of the actions performed by the ___ResetAll___ method described above. The ___ResetGeneratedText___ method is
generally called after you have finished processing a template file and have written the generated text to the output file so that you can use
the same template to generate another file with different token values. In fact, the ___ResetGeneratedText___ method is called by default by
the ___WriteGeneratedTextToFile___ method after the output file is written to.

Example:
```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
...
// Process the template file, etc.
...
processor.ResetGeneratedText();
// Now we can reuse the same template file a second time...
...
// Process the template file again with different
// values for the tokens, etc. (Note that we don't need
// to reload the template file as we did when using
// the ResetAll method.)
...
```

### ___ResetSegment___
The ___ResetSegment___ method is used to reset the _"is first time"_ flag for the given segment to _true_. The method has the following signature:

```csharp
public void ResetSegment(string segmentName)
```

The ___segmentName___ parameter must specify a valid segment name from the text template file. An error will be written to the log if the supplied
segment name is invalid or unknown.

The ___ResetSegment___ method is useful when you have a segment that specifies the ___FTI___ (First Time Indent) option and/or the ___PAD___ (Pad
Segment) option and you want to process that segment multiple times in two different places in the generated text file. The ___ResetSegment___
method would be called before starting the second group so that the ___FTI___ and ___PAD___ options would be handled correctly.

For example, assume we have the following text template file:
```
### BlankLine
    
### TopSegment
    Top Line
### BottomSegment
@-1 Bottom Line
### MiddleSegment FTI=1 PAD=BlankLine
    Middle Line
```

This is a rather simple example for illustration purposes. Suppose we want to process __TopSegment__ once, __MiddleSegment__ twice, and then
__BottomSegment__ once. Then we want to repeat that same sequence. The necessary code for doing this would be:

```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
// Process the first group.
processor.GenerateSegment("TopSegment");
processor.GenerateSegment("MiddleSegment");
processor.GenerateSegment("MiddleSegment");
processor.GenerateSegment("BottomSegment");
// Reset the MiddleSegment.
processor.ResetSegment("MiddleSegment");
// Process the second group.
processor.GenerateSegment("TopSegment");
processor.GenerateSegment("MiddleSegment");
processor.GenerateSegment("MiddleSegment");
processor.GenerateSegment("BottomSegment");
```

The generated text would look like this (assuming the default tab size of 4):

```
Top Line
    Middle Line

    Middle Line
Bottom Line
Top Line
    Middle Line

    Middle Line
Bottom Line
```

If we hadn't used the ___ResetSegment___ method between the two groups, the output would have looked like this:

```
Top Line
    Middle Line

    Middle Line
Bottom Line
Top Line

Middle Line

Middle Line
Bottom Line
```

As you can see, the output isn't the desired output if the call to ___ResetSegment___ is omitted between the two groups of segments.

### ___SetTabSize___
As the name implies, the ___SetTabSize___ method is used for setting the tab size of the ___TextTemplateProcessor___ class object. It has the following
signature:

```csharp
public void SetTabSize(int tabSize)
```

The method takes a single ___tabSize___ parameter which must be an integer value between 1 and 9, inclusive. An error will be written to the log if an
invalid value is supplied for this parameter. If the supplied value is less than 1, then the ___TabSize___ property will be set to 1. If the supplied
value is greater than 9, then ___TabSize___ will be set to 9. Otherwise, if the supplied value is within the valid range, the ___TabSize___ property
will be set to the value of the ___tabSize___ parameter.

Example:
```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
processor.SetTabSize(2);
```

The above example will change the ___TabSize___ property value from the default value of 4 to the new value of 2.

> __Note:__ Calling the ___SetTabSize___ method before calling ___GenerateSegment___ on a segment that doesn't specify a ___TAB___ option behaves
> the same as if the segment had the ___TAB___ option and ___SetTabSize___ was never called. The two features are interchangeable, assuming they
> specify the same tab size value. However, if ___SetTabSize___ is called before calling ___GenerateSegment___ on a segment that does specify the
> ___TAB___ option, the value specified on the ___TAB___ option takes precedence.

### ___WriteGeneratedTextToFile___
The ___WriteGeneratedTextToFile___ method is used to write the contents of the generated text buffer to the output file. The method has the
following signature:

```csharp
public void WriteGeneratedTextToFile(string filePath, bool resetGeneratedText = true)
```

This method takes one required parameter and one optional parameter. The required ___filePath___ parameter specifies the file path of the
output file. If the file doesn't exist, it will be created. If the file does exist, it will be overwritten. If the specified file path is
invalid, an error will be written to the log and the method will return without creating any file.

The optional ___resetGeneratedText___ parameter indicates whether or not the ___ResetGeneratedText___ method should be called after the
output file has been successfully created. This parameter defaults to _true_, meaning that the generated text buffer will be cleared after
the output file is created. This is normally what you would want to happen. However, you can override this value if you do not wish to clear
the generated text buffer for some reason.

If the output file is successfully created, the _"is output file written"_ flag is set to _true_.

Example:
```csharp
ITextTemplateProcessor processor = new TextTemplateProcessor("template.file.path");
...
// Generate one or more segments here...
...
// Write the generated text to the output file and
// clear the generated text buffer if successful.
processor.WriteGeneratedTextToFile("some.output.file.path");
```

# ___TextTemplateConsoleBase___ Class
The ___TextTemplateConsoleBase___ class is an abstract base class that extends the ___TextTemplateProcessor___ class. This is the class from which you
would derive your own custom text template processors. Typically you would add a console project to your business solution and would define your
custom text template processors in this console project. You would then call these custom processors from within the ___Program___ class of the
console project.

The ___TextTemplateConsoleBase___ class derives from the ___TextTemplateProcessor___ class, so it has all the properties and methods described
above, plus the ones mentioned below.

## Constructors
The ___TextTemplateConsoleBase___ class has two constructors having the following signatures:

```csharp
public TextTemplateConsoleBase()
public TextTemplateConsoleBase(string path)
```

The first constructor has no parameters. It creates an instance without a text template file. If you use this constructor in your derived class you
will need to call the ___LoadTemplate___ method with a valid text template file path before trying to process any segments.

The second constructor takes a single parameter that specifies the file path of an existing text template file. The specified text template file
will be loaded by the constructor so that you will be ready to start processing segments right away.

Both constructors will set the ___SolutionDirectory___ property to the directory path that contains your solution file. They also will both set the
___OutputDirectory___ property to an empty string.

## Properties

### ___OutputDirectory___
The ___OutputDirectory___ property gets the full path of the directory where the generated text files are to be written. This property will
return an empty string if the ___SetOutputDirectory___ method hasn't been called yet.

Example:
```csharp
using TemplateProcessor.Console;
public class MyTemplateProcessor : TextTemplateConsoleBase
{
    public void DoSomething()
    {
        ...
        string outputDirectory = OutputDirectory;
        ...
    }
}
```

### ___SolutionDirectory___
The ___SolutionDirectory___ property gets the full path of the directory containing the solution file. This property gets set in the constructor
and remains unchanged for the life of the ___TextTemplateConsoleBase___-derived object. The search for the solution directory begins in the
directory where the derived class is defined. The search proceeds up the directory tree until it finds a directory containing a file with a
file extension of _".sln"_. The ___SolutionDirectory___ property will be set to the first directory found that contains a solution file.
An error will be written to the log if the root directory is reached without locating any solution files.

Example:
```csharp
using TemplateProcessor.Console;
public class MyTemplateProcessor : TextTemplateConsoleBase
{
    public void DoSomething()
    {
        ...
        string solutionDirectory = SolutionDirectory;
        ...
    }
}
```

## Methods

### ___ShowContinuationPrompt___
The ___ShowContinuationPrompt___ method is used for displaying a prompt message on the console and reading the user's response. The method has the
following signature:

```csharp
public static string ShowContinuationPrompt(string message = "\nPress [ENTER] to continue...\n")
```

The method takes one optional parameter that supplies the text to be displayed for the prompt. If the parameter is omitted, the string _"Press
[ENTER] to continue..."_ will be written to the console. The method will wait for the user to press the ENTER key after displaying the prompt
message. Any characters the user types before pressing ENTER will be returned to the caller.

Example:
```csharp
using TemplateProcessor.Console;
public class MyTemplateProcessor : TextTemplateConsoleBase
{
    public void DoSomething()
    {
        ...
        string response = ShowContinuationPrompt("Enter Y (yes) or N (no)...");
        if (response.ToUpper() == "Y") { ... }
        ...
    }
}
```

This method is useful if you need input from the user when generating the text from a template. For example, maybe your text template file can
be used for generating two different versions of the output file depending on the user's response.

> __Note:__ The ___ShowContinuationPrompt___ method outputs a newline character before and after the outputting the specified prompt text. So
> there is no need to include newline characters in the prompt text you specify (unless you want more than one blank line before the prompt).

> __Note:__ If the user presses ENTER at the prompt without typing anything else, an empty string will be returned to the caller. A null value
> will never be returned from this method.

### ___ClearOutputDirectory___
The ___ClearOutputDirectory___ method is used for clearing the contents of the output directory. The method has the following signature:

```csharp
public void ClearOutputDirectory()
```

The ___ClearOutputDirectory___ method only deletes the files contained within the directory pointed to by the ___OutputDirectory___ property. It does
not delete the output directory or any subdirectories contained within the output directory.

Example:
```csharp
using TemplateProcessor.Console;
public class MyTemplateProcessor : TextTemplateConsoleBase
{
    public void DoSomething()
    {
        ...
        ClearOutputDirectory();
        ...
    }
}
```

This method is useful primarily during development when you may be rerunning your text generator multiple times as you make changes and
corrections to your text template file and/or to your ___TextTemplateConsoleBase___-derived class. Calling ___ClearOutputDirectory___ before
processing any segments from the template will ensure that the output directory starts out clean.

### ___LoadTemplate___
The ___LoadTemplate___ method of the ___TextTemplateConsoleBase___ class replaces the method having the same name and signature in the
___TextTemplateProcessor___ base class. The method has the following signature:

```csharp
public void LoadTemplate(string filePath)
```

The method takes a single parameter that specifies the file path to an existing text template file. This can be either an absolute or
relative file path. Relative file paths are always assumed to be relative to the directory pointed to by the ___SolutionDirectory___
property.

If the specified file path is an absolute path, the path will be passed directly to the ___LoadTemplate___ method of the
___TextTemplateProcessor___ base class which will then load the text template file into memory. If a relative path is specified, then
the path pointed to by the ___SolutionDirectory___ property will be combined with the specified file path to make the full file path,
and the full file path will be passed to the ___LoadTemplate___ method of the ___TextTemplateProcessor___ base class.

Example:
```csharp
using TemplateProcessor.Console;
public class MyTemplateProcessor : TextTemplateConsoleBase
{
    public void DoSomething()
    {
        ...
        LoadTemplate(@"templates\mytemplate.txt");
        ...
    }
}
```

The above example will look for the text template file named _"mytemplate.txt"_ in the _"templates"_ subdirectory under the solution directory. An
error will be logged if the solution directory doesn't contain a subdirectory named _"templates"_ or if the _"mytemplate.txt"_ file isn't found
in the _"templates"_ subdirectory.

> __Note:__ If the specified file path is invalid, or if the file can't be successfully loaded for any reason, an error will be written to the log
> and the ___ResetAll___ method will be called to reset the ___TextTemplateProcessor___ environment.

> __Note:__ An error will be written to the log if the ___filePath___ parameter specifies a relative path but the ___SolutionDirectory___
> property is an empty string. This would only be the case if the class constructor was not able to locate the solution directory for some reason.

### ___SetOutputDirectory___
The ___SetOutputDirectory___ method sets the ___OutputDirectory___ property to the specified directory path. The method has the following
signature:

```csharp
public void SetOutputDirectory(string directoryPath)
```

The method takes a single parameter that specifies the directory path where the generated text files will be written. This can be either an
absolute or relative directory path. Relative directory paths are always assumed to be relative to the directory pointed to by the
___SolutionDirectory___ property. The directory path will be created if it doesn't already exist.

If the specified directory path is an absolute path, the ___OutputDirectory___ property will be set to that path. If a relative path is
specified, then the path pointed to by the ___SolutionDirectory___ property will be combined with the specified directory path to make
the full directory path, and the ___OutputDirectory___ property will be set to the full directory path.

Example:
```csharp
using TemplateProcessor.Console;
public class MyTemplateProcessor : TextTemplateConsoleBase
{
    public void DoSomething()
    {
        ...
        SetOutputDirectory("generated");
        ...
    }
}
```

The above example will look for a subdirectory named _"generated"_ located in the directory pointed at by the ___SolutionDirectory___ property.
The subdirectory will be created if it doesn't already exist.

> __Note:__ If the specified directory path is invalid, or if the directory can't be created for any reason, an error will be written to the log
> and the ___OutputDirectory___ property will be set to an empty string.

> __Note:__ An error will be written to the log if the ___directoryPath___ parameter specifies a relative path but the ___SolutionDirectory___
> property is an empty string. This would only be the case if the class constructor was not able to locate the solution directory for some reason.

### ___WriteGeneratedTextToFile___
The ___WriteGeneratedTextToFile___ method replaces the method having the same name in the ___TextTemplateProcessor___ base class. The method has the
following signature:

```csharp
public void WriteGeneratedTextToFile(string fileName, bool resetGeneratedText = true)
```

The method takes one required and one optional parameter. The required ___fileName___ parameter specifies the name of the output file to be written.
If the file already exists it will be overwritten.

The optional ___resetGeneratedText___ parameter specifies whether or not the generated text buffer should be cleared after the output file is
successfully written to. If omitted, this parameter defaults to _true_, meaning that the generated text buffer will be cleared.

The ___OutputDirectory___ property must contain a valid directory path value before the ___WriteGeneratedTextToFile___ method is called. An error
will be written to the log if this is not the case, and the method will return without writing to any file. Otherwise, the path pointed to by the
___OutputDirectory___ property will be combined with the specified value of the ___fileName___ parameter to generate the full file path. This full
file path will then be passed into the ___WriteGeneratedTextToFile___ method of the ___TextTemplateProcessor___ base class along with the value of
the ___resetGeneratedText___ parameter.

Example:
```csharp
using TemplateProcessor.Console;
public class MyTemplateProcessor : TextTemplateConsoleBase
{
    public void DoSomething()
    {
        SetOutputDirectory("generated");
        ...
        // Process some segments to generate some text.
        ...
        WriteGeneratedTextToFile("mygeneratedtext.txt");
}
```

The example above will create the file _"mygeneratedtext.txt"_ in the _"generated"_ subdirectory located under the solution directory. The file will
contain the contents of the generated text buffer. The generated text buffer will be cleared after the file is created.