using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: CLSCompliant(true)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page, 
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page, 
                                              // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Unity")]
[assembly: InternalsVisibleTo("Prism.Unity.Wpf.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001008f34b619d7a39e44cebe5ccbd5607eaa0784c9c124431ba336a14e4fecd874f151b57163961505e76943910c7cabea9c7229edc3553dfc33ac7b269087e5cef9404bdb491907ffd9f9b26d737fa2c359620a2cbf2802f54118471d7c0ead3b95c916783dd4b99b9b1a0cd2785e1b5d614d3d9140a60c8c64c217e1c2b0ec8296")]
