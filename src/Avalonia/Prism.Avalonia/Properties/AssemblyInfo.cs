using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Metadata;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]

[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Navigation.Regions")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Navigation.Regions.Behaviors")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Mvvm")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Interactivity")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Dialogs")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Ioc")]

[assembly: InternalsVisibleTo("Prism.Avalonia.Tests")]
