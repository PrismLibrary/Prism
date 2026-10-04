using System.Windows.Markup;

// Canonical Prism schemas (MAUI and WPF respectively). These attributes are only
// compiled for Uno targets; Microsoft WinUI uses its own XAML compiler.
[assembly: XmlnsDefinition("http://prismlibrary.com", "Prism")]
[assembly: XmlnsDefinition("http://prismlibrary.com", "Prism.Navigation.Regions")]
[assembly: XmlnsDefinition("http://prismlibrary.com", "Prism.Navigation.Regions.Behaviors")]
[assembly: XmlnsDefinition("http://prismlibrary.com", "Prism.Mvvm")]
[assembly: XmlnsDefinition("http://prismlibrary.com", "Prism.Interactivity")]
[assembly: XmlnsDefinition("http://prismlibrary.com", "Prism.Dialogs")]
[assembly: XmlnsDefinition("http://prismlibrary.com", "Prism.Ioc")]

[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Navigation.Regions")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Navigation.Regions.Behaviors")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Mvvm")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Interactivity")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Dialogs")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Ioc")]

// Uno 6.6 makes registered library types available without a per-file prefix.
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/global", "Prism")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/global", "Prism.Navigation.Regions")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/global", "Prism.Navigation.Regions.Behaviors")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/global", "Prism.Mvvm")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/global", "Prism.Interactivity")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/global", "Prism.Dialogs")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation/global", "Prism.Ioc")]
