# Prism
<img src="https://ci.appveyor.com/api/projects/status/pn4fcaghmlwueu52?svg=true" width="300"/>

Prism is a framework for building loosely coupled, maintainable, and testable XAML applications in WPF, Windows 10 UWP, and Xamarin Forms. Separate releases are available for each platform and those will be developed on independent timelines. Prism provides an implementation of a collection of design patterns that are helpful in writing well-structured and maintainable XAML applications, including MVVM, dependency injection, commands, EventAggregator, and others. Prism's core functionality is a shared code base in a Portable Class Library targeting these platforms. Those things that need to be platform specific are implemented in the respective libraries for the target platform. Prism also provides great integration of these patterns with the target platform. For example, Prism for UWP and Xamarin Forms allows you to use an abstraction for navigation that is unit testable, but that layers on top of the platform concepts and APIs for navigation so that you can fully leverage what the platform itself has to offer, but done in the MVVM way.

Prism 6 in the works - we plan to have Prism 6 for WPF and Prism 6 for UWP out in the September timeframe. Xamarin Forms will follow, but hopefully not by far.

Prism 6 is a fully open source version of the Prism guidance [originally produced by Microsoft patterns & practices](http://blogs.msdn.com/b/dotnet/archive/2015/03/19/prism-grows-up.aspx). The core team members were all part of the p&p team that developed Prism 1 through 5, and the effort has now been turned over to the open source community to keep it alive and thriving to support the .NET community. There are thousands of companies who have adopted previous versions of Prism for WPF, Silverlight, and Windows Runtime, and we hope they will continue to move along with us as we continue to evolve and enhance the framework to keep pace with current platform capabilities and requirements.

At the current time we have no plans to create new versions of the library for Silverlight or for Windows 8/8.1/WP8.1. For those you can still use the previous releases from Microsoft p&p [here](https://msdn.microsoft.com/en-us/library/Gg430869%28v=PandP.40%29.aspx) and [here](http://prismwindowsruntime.codeplex.com/). If there is enough interest and contributors to do the work, we can consider it, but it is not on our roadmap for now.

#NuGet Packages
######Prism PCL
| Assembly | Package |
| -------- | ------- |
| Prism.dll | [Prism.Core](https://www.nuget.org/packages/Prism.Core/) |

######WPF
| Assembly | Package |
| -------- | ------- |
| Prism.Wpf.dll | [Prism.Wpf](https://www.nuget.org/packages/Prism.Wpf/) |
| Prism.Unity.Wpf.dll | [Prism.Unity](https://www.nuget.org/packages/Prism.Unity/) |
| Prism.Mef.Wpf.dll | [Prism.Mef](https://www.nuget.org/packages/Prism.Mef/) |
| Prism.Autofac.Wpf.dll | [Prism.Autofac](https://www.nuget.org/packages/Prism.Autofac/) |
| Prism.StructureMap.Wpf.dll | [Prism.StructureMap](https://www.nuget.org/packages/Prism.StructureMap/) |
| Prism.Ninject.Wpf.dll | [Prism.Ninject](https://www.nuget.org/packages/Prism.Ninject/) |

######Xamarin.Forms
| Assembly | Package |
| -------- | ------- |
| Prism.dll, Prism.Forms.dll, Prism.Unity.Forms.dll | [Prism.Forms](https://www.nuget.org/packages/Prism.Forms/) |

######Universal Windows Platform
| Assembly | Package |
| -------- | ------- |
| TBD | TBD |

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

[Release Notes](https://github.com/PrismLibrary/Prism/wiki/Release-Notes---6.0.0)

##Breaking Changes
- Removed all types that were marked as "Obsolete" in Prism 5
- Changed namespaces to remove Microsoft namespaces
- Moved a number of types around to better organize and to get as much into a single Portable Class Library as possible
- ViewModelLocator naming convention changes: [Name]View now requires [Name]ViewModel.  No longer [Name]ViewViewModel
- NavigationParameters now derives from Dictionary which will break various scenarios such as duplicate params, and non-existent params throwing an exception now instead of null.

###Prism for UWP Preview
- Prism for UWP is a port of the Prism for Windows Runtime 2.0 release
- Removed SettingsPane functionality from PrismApplication because it is deprecated in UWP
- Visual State management parts of VisualStateAwarePage were removed and it is now renamed to SessionStateAwarePage. 

#Prism for Xamarin.Forms Preview
Check out the new Prism for Xamarin.Forms Preview: http://brianlagunas.com/first-look-at-the-prism-for-xamarin-forms-preview/

### .NET Foundation

This project is part of the [.NET Foundation](http://www.dotnetfoundation.org/projects).

