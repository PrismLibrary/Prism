# Prism
Prism is a framework for building loosely coupled, maintainable, and testable XAML applications in WPF, Windows 10 UWP, and Xamarin Forms. Separate releases are available for each platform and those will be developed on independent timelines. Prism provides an implementation of a collection of design patterns that are helpful in writing well-structured and maintainable XAML applications, including MVVM, dependency injection, commands, EventAggregator, and others. Prism's core functionality is a shared code base in a Portable Class Library targeting these platforms. Those things that need to be platform specific are implemented in the respective libraries for the target platform. Prism also provides great integration of these patterns with the target platform. For example, Prism for UWP and Xamarin Forms allows you to use an abstraction for navigation that is unit testable, but that layers on top of the platform concepts and APIs for navigation so that you can fully leverage what the platform itself has to offer, but done in the MVVM way.

Prism 6 is a fully open source version of the Prism guidance [originally produced by Microsoft patterns & practices](http://blogs.msdn.com/b/dotnet/archive/2015/03/19/prism-grows-up.aspx). The core team members were all part of the p&p team that developed Prism 1 through 5, and the effort has now been turned over to the open source community to keep it alive and thriving to support the .NET community. There are thousands of companies who have adopted previous versions of Prism for WPF, Silverlight, and Windows Runtime, and we hope they will continue to move along with us as we continue to evolve and enhance the framework to keep pace with current platform capabilities and requirements.

At the current time we have no plans to create new versions of the library for Silverlight or for Windows 8/8.1/WP8.1. For those you can still use the previous releases from Microsoft p&p [here](https://msdn.microsoft.com/en-us/library/Gg430869%28v=PandP.40%29.aspx) and [here](http://prismwindowsruntime.codeplex.com/). If there is enough interest and contributors to do the work, we can consider it, but it is not on our roadmap for now.

#Build Status

|          | Status |
| -------- | ------ |
| Prism.Core | <img src="https://ci.appveyor.com/api/projects/status/pn4fcaghmlwueu52/branch/master?svg=true"/> |
| Prism.Wpf | <img src="https://ci.appveyor.com/api/projects/status/4lt3n2wf5m2efms7/branch/master?svg=true" /> |
| Prism.Windows | <img src="https://ci.appveyor.com/api/projects/status/j04r6a45fi2f9pv4/branch/master?svg=true" /> |
| Prism.Forms | <img src="https://ci.appveyor.com/api/projects/status/6ly53jgvwx62bm9u/branch/master?svg=true" /> |

#NuGet Packages
### Core Packages

These are the base packages for each platform, together with the Prism's Core assembly as a cross-platform PCL.

| Platform | Assembly | Package |
| -------- | -------- | ------- |
| PCL | Prism.dll | [Prism.Core][1] |
| WPF | Prism.Wpf.dll | [Prism.Wpf][2] |
| Xamarin.Forms | Prism.Forms.dll | [Prism.Forms][3] |
| Windows 10 UWP | Prism.Windows.dll | [Prism.Windows][4] |

### Container-specific packages

Each supported IoC container has its own package assisting in the setup and usage of that container together with Prism. The assembly is named using this convention: Prism.*Container.Platform*.dll, e.g. **Prism.Unity.Wpf.dll**. 

Following matrix shows the platform specific support currently available.

| Package               | WPF | Win10 UWP | Xamarin.Forms |
|-----------------------|:---:|:---:|:---:|
| [Prism.Unity][5]      |  X  |  X  |  X  |
| [Prism.Mef][6]        |  X  |     |     |
| [Prism.Autofac][7]    |  X  |  X  |     |
| [Prism.StructureMap][8]| X  |     |     |
| [Prism.Ninject][9]    |  X  |     |  X  |

Note that adding the container-specific package to your project, will also pull in the correct platform-specific package and the core PCL library. E.g. when you'd like to use Unity in a WPF project, add the Prism.Unity package and the rest will be pulled in as well.

![NuGet package tree](Documentation/images/NuGetPackageTree.png)

A detailed overview of each assembly per package is available [here](Documentation/DownloadandSetupPrism.md#overview-of-assemblies).

# Prism Template Pack
Get the latest snippets, item templates, and projects templates for dveloping WPF, UWP, and Xamarin.Forms applications with Prism from the [Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/e7b6bde2-ba59-43dd-9d14-58409940ffa0).

# Samples
We have both a development sandbox (frequently changing) and stable samples for using Prism with WPF, UWP and Xamarin Forms. An overview of the samples can be found [here](Sandbox/README.md).

#Roadmap/Milestones
You can check out our milestones for coming releases [here](https://github.com/PrismLibrary/Prism/milestones).

# Support
- For general questions and support, post your questions on [StackOverflow](http://stackoverflow.com/questions/tagged/prism)
- You can enter bugs and feature requests in our [Issues](https://github.com/PrismLibrary/Prism/issues).

#Contributing
We strongly encourage you to get involved and help us evolve the code base. 
- You can see what our expectations are for pull requests [here](https://github.com/PrismLibrary/Prism/blob/master/CONTRIBUTE.md).

#Moving to Prism 6 from Previous Releases
As part of taking over the code base from Microsoft and moving towards Prism 6, there are a number of breaking changes users of Prism 5 or Prism for Windows Runtime will have to deal with. Those changes are summarized below.

[Current Release Notes](https://github.com/PrismLibrary/Prism/wiki/Release-Notes---6.1.0)

##Breaking Changes
- Removed all types that were marked as "Obsolete" in Prism 5
- Removed IView interface
- Changed namespaces to remove Microsoft namespaces
- Moved a number of types around to better organize and to get as much into a single Portable Class Library as possible
- ViewModelLocator naming convention changes: [Name]View now requires [Name]ViewModel.  No longer [Name]ViewViewModel

###Prism for UWP Preview
- Prism for UWP is a port of the Prism for Windows Runtime 2.0 release
- Removed SettingsPane functionality from PrismApplication because it is deprecated in UWP
- Visual State management parts of VisualStateAwarePage were removed and it is now renamed to SessionStateAwarePage. 

#Prism for Xamarin.Forms Preview
Check out the new Prism for Xamarin.Forms Preview:
* Prism.Forms 5.7.0 Preview - http://brianlagunas.com/first-look-at-the-prism-for-xamarin-forms-preview/
* Prism.Forms 6.2.0 Preview - http://brianlagunas.com/prism-for-xamarin-forms-6-2-0-preview/

### .NET Foundation

This project is part of the [.NET Foundation](http://www.dotnetfoundation.org/projects).


[1]: https://www.nuget.org/packages/Prism.Core/
[2]: https://www.nuget.org/packages/Prism.Wpf/
[3]: https://www.nuget.org/packages/Prism.Forms/
[4]: https://www.nuget.org/packages/Prism.Windows/
[5]: https://www.nuget.org/packages/Prism.Unity/
[6]: https://www.nuget.org/packages/Prism.Mef/
[7]: https://www.nuget.org/packages/Prism.Autofac/
[8]: https://www.nuget.org/packages/Prism.StructureMap/
[9]: https://www.nuget.org/packages/Prism.Ninject/
