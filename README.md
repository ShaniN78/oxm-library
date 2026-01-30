# OXM-Library
Object To XML Mapping and parsing XSD/WSDL into classes

Free XSD Code Generator of C# / VB classes from XSD files. **Targets .NET 8.**

## Building
Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet build OxmLibrary.sln
```

To run the GUI (OxmStylizer):
```bash
dotnet run --project OxmLibrary.GUI
```

---
I need your feedback.
Leave a review or a suggestion, with ideas and requests, I will do my best to implement them

Main Features:
Fast parsing - Small XMLs with XElement, big XMLS with XmlReader
Support for Xsd:import and Xsd:include
Can use Plug-Ins to extend features
Included is a Plug-In to Generate Contract and Client classes for Web Services
Namespace aware
Merging of similar Classes Based on automatic rules, or manually
Parsing Of Regular expression validation rules as metadata
Parsing of Default values from the attribute value=X and assigning it in the constructor
Parsing Of Annotations per class/type
Generate Enumerations for types restricted with enum (With annotations)
Force Multiplicity on properties when XSD states otherwise (You can fix XSD also, lol)
Does not rely on serialization or de-serialization - instead uses Linq To XML and reflection.
Enable Data Binding Via The INotifyPropertyChanged Interface
Uses List Or Array As Collections
Assign Default values from XSD
Incremental Inference of XML from several XML Files
Extensible to any programming language

This project helps create a set of classes from an XSD file/s or by inferring from XML files.
And then serialize the XML files to the entities. Works mostly like the utility XSD.exe , but with
a little more flexibility.
It allows creation of .GEN files (an XML file that tracks the generator created) that saves the configuration
and keeps track of the generated
structure, and allow you to modify classes properties and add some of your own.

Included is a reasonable GUI to help perform all the common actions such as XSD Parsing and XML Infering.

This project is in development phases, and probably needs alot of clean up.
The generated class files are usable and the serializing process is pretty fast!
well, it's not slow... :)
It's now working on two production servers in a medium sized web application.

Also supplied is an ElementBaseWriter class to produce XML strings (or files) from Classes.
Supported also through the ToString Method of every class derived from ElementBase Class

A few Examples:
Example Page


