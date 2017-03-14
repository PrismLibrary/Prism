using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Prism.Wpf")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Prism")]
[assembly: AssemblyCopyright("Copyright ©  2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en")]

// -----  Legacy -----
[assembly: XmlnsDefinition("http://www.codeplex.com/prism", "Prism.Regions")]
[assembly: XmlnsDefinition("http://www.codeplex.com/prism", "Prism.Regions.Behaviors")]
[assembly: XmlnsDefinition("http://www.codeplex.com/prism", "Prism.Mvvm")]
[assembly: XmlnsDefinition("http://www.codeplex.com/prism", "Prism.Interactivity")]
[assembly: XmlnsDefinition("http://www.codeplex.com/prism", "Prism.Interactivity.InteractionRequest")]
// -----  Legacy -----

[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Regions")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Regions.Behaviors")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Mvvm")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Interactivity")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Interactivity.InteractionRequest")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("6.3")]
[assembly: AssemblyFileVersion("6.3.0")]
[assembly: AssemblyInformationalVersion("6.3.0-pre2")]
