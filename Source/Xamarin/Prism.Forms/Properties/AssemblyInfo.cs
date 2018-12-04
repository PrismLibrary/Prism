using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Prism.Forms.Tests")]
[assembly: InternalsVisibleTo("Prism.Autofac.Forms.Tests")]
[assembly: InternalsVisibleTo("Prism.DryIoc.Forms.Tests")]
[assembly: InternalsVisibleTo("Prism.Unity.Forms.Tests")]

[assembly: Xamarin.Forms.Internals.Preserve(AllMembers = true)]

#if NETSTANDARD2_0
[assembly: Xamarin.Forms.XmlnsDefinition("http://prismlibrary.com", "Prism.Behaviors")]
[assembly: Xamarin.Forms.XmlnsDefinition("http://prismlibrary.com", "Prism.Ioc")]
[assembly: Xamarin.Forms.XmlnsDefinition("http://prismlibrary.com", "Prism.Modularity")]
[assembly: Xamarin.Forms.XmlnsDefinition("http://prismlibrary.com", "Prism.Mvvm")]
[assembly: Xamarin.Forms.XmlnsDefinition("http://prismlibrary.com", "Prism.Navigation.Xaml")]
#endif