# Prism

Prism is a framework for building loosely coupled, maintainable, and testable XAML applications in WPF, Windows 10 UWP, and Xamarin Forms. Separate releases are available for each platform and those will be developed on independent timelines. Prism provides an implementation of a collection of design patterns that are helpful in writing well-structured and maintainable XAML applications, including MVVM, dependency injection, commands, EventAggregator, and others. Prism's core functionality is a shared code base in a Portable Class Library targeting these platforms. Those things that need to be platform specific are implemented in the respective libraries for the target platform. Prism also provides great integration of these patterns with the target platform. For example, Prism for UWP and Xamarin Forms allows you to use an abstraction for navigation that is unit testable, but that layers on top of the platform concepts and APIs for navigation so that you can fully leverage what the platform itself has to offer, but done in the MVVM way.

Prism 6 is a fully open source version of the Prism guidance [originally produced by Microsoft patterns & practices](http://blogs.msdn.com/b/dotnet/archive/2015/03/19/prism-grows-up.aspx). The core team members were all part of the P&amp;P team that developed Prism 1 through 5, and the effort has now been turned over to the open source community to keep it alive and thriving to support the .NET community. There are thousands of companies who have adopted previous versions of Prism for WPF, Silverlight, and Windows Runtime, and we hope they will continue to move along with us as we continue to evolve and enhance the framework to keep pace with current platform capabilities and requirements.

At the current time, we have no plans to create new versions of the library for Silverlight or for Windows 8/8.1/WP8.1. For those you can still use the previous releases from Microsoft P&amp;P [here](https://msdn.microsoft.com/en-us/library/Gg430869%28v=PandP.40%29.aspx) and [here](http://prismwindowsruntime.codeplex.com/). If there is enough interest and contributors to do the work, we can consider it, but it is not on our roadmap for now.

## Plugins

There are certain things that cannot be added directly into Prism for various reasons. To handle these common tasks such as supporting PopupPage's in Xamarin Forms, there are Prism Plugins. You can find a number of Plugins available on NuGet from our maintainer @DanJSiegel.

- [Prism.Plugin.Popups](https://github.com/dansiegel/Prism.Plugin.Popups) (Forms Only)
- [Prism.Plugin.Logging](https://github.com/dansiegel/Prism.Plugin.Logging) (Works on all Platforms)
  - Adds support for Syslog, Loggly, and Graylog
- [Prism.Plugin.PageDialogs](https://github.com/dansiegel/Prism.Plugin.PageDialogs) (Forms Only)
- [Prism.MFractor.Config](https://nuget.org/packages/Prism.MFractor.Config)
  - Configures MFractor in Visual Studio for Mac to follow Prism Conventions

## Build Status

|          | Status |
| -------- | ------ |
| Prism | [![AppVeyor](https://img.shields.io/appveyor/ci/brianlagunas/prism/master.svg?label=appveyor)](https://ci.appveyor.com/project/brianlagunas/prism/branch/master) |
| Prism.Wpf | [![AppVeyor](https://img.shields.io/appveyor/ci/brianlagunas/prism-jy368/master.svg?label=appveyor)](https://ci.appveyor.com/project/brianlagunas/prism-jy368/branch/master) |
| Prism.Windows | [![AppVeyor](https://img.shields.io/appveyor/ci/brianlagunas/prism-gv1t3/master.svg?label=appveyor)](https://ci.appveyor.com/project/brianlagunas/prism-gv1t3/branch/master) |
| Prism.Forms | [![AppVeyor](https://img.shields.io/appveyor/ci/brianlagunas/prism-bvely/master.svg?label=appveyor)](https://ci.appveyor.com/project/brianlagunas/prism-bvely/branch/master) |

## Support

- Documentation is maintained in [the Prism-Documentation repo](https://github.com/PrismLibrary/Prism-Documentation) under /docs and can be found in a readable format on [the website](http://prismlibrary.github.io/docs/).
- For general questions and support, post your questions on [StackOverflow](http://stackoverflow.com/questions/tagged/prism).
- You can enter bugs and feature requests in our [Issues](https://github.com/PrismLibrary/Prism/issues).

## Help Support Prism

As most of you know, it takes a lot of time and effort for our small team to manage and maintain Prism in our spare time.  Even though Prism is open source and hosted on GitHub, there are a number of costs associated with maintaining a project such as Prism.  If you would like to help support us, the easiest thing you can do is become a Patron and watch our Pluralsight courses on Prism.

By becoming a [Patron and subscribing](https://www.patreon.com/prismlibrary) to the Prism Library, you will receive a number of benefits depending on your level of support.

**Basic** - $1+ per month
- Receive all Prism Library news and announcements, such as new release information and blogs posts. 
- Gives you access to our community Slack channel where you can ask questions and get help from the community and the Prism Library project maintainers (when available).

**Premium** - $5+ per month
- Gives you access to members only video tutorials.
- A Prism sticker

**Platinum** - $10+ per month
- Early access to Visual Studio productivity tools.
- Early access to video tutorials
- Receive priority response on all GitHub issues.
- Gives you access to a private Slack channel where you can ask questions and get help directly from Prism Library project maintainers.

By watching our courses, not only do you help support the project financially, but you might also learn something along the way.  We believe this is a win-win for everyone.

- [Introduction to Prism](https://app.pluralsight.com/library/courses/prism-introduction/table-of-contents)
- [What's New in Prism 5.0](https://app.pluralsight.com/library/courses/prism-50-whats-new/table-of-contents)
- [Prism Problems & Solutions: Showing Multiple Shells](https://app.pluralsight.com/library/courses/prism-showing-multiple-shells/table-of-contents)
- [Prism Problems & Solutions: Mastering TabControl](https://app.pluralsight.com/library/courses/prism-mastering-tabcontrol/table-of-contents)
- [Prism Problems & Solutions: Loading Modules Based on User Roles](https://app.pluralsight.com/library/courses/prism-loading-modules-user-roles/table-of-contents)
- [Prism Problems & Solutions: Loading Dependent Views](https://app.pluralsight.com/library/courses/prism-problems-solutions/table-of-contents)

We appreciate your support.

## NuGet Packages

Official Prism releases are available on NuGet. Prism also has a MyGet feed which will be updated with each merged PR. If you want to take advantage of a new feature as soon as it's merged into the code base, or if there is a critical bug you need fixed we invite you to try the packages on this feed. Our feed is a public feed in the MyGet Gallery.

Simply add `https://www.myget.org/F/prism/api/v3/index.json` as a package source to either Visual Studio or Visual Studio for Mac.

### Core Packages

These are the base packages for each platform, together with the Prism's Core assembly as a cross-platform PCL.

| Platform | Assembly | Package | NuGet | MyGet |
| -------- | -------- | ------- | ------- | ----- |
| PCL | Prism.dll | [Prism.Core][CoreNuGet] | [![CoreNuGetShield]][CoreNuGet] | [![CoreMyGetShield]][CoreMyGet] |
| WPF | Prism.Wpf.dll | [Prism.Wpf][WpfNuGet] | [![WpfNuGetShield]][WpfNuGet] | [![WpfMyGetShield]][WpfMyGet] |
| Xamarin.Forms | Prism.Forms.dll | [Prism.Forms][FormsNuGet] | [![FormsNuGetShield]][FormsNuGet] | [![FormsMyGetShield]][FormsMyGet] |
| Windows 10 UWP | Prism.Windows.dll | [Prism.Windows][UWPNuGet] | [![UWPNuGetShield]][UWPNuGet] | [![UWPMyGetShield]][UWPMyGet] |

### Container-specific packages

Each supported IoC container has its own package assisting in the setup and usage of that container together with Prism. The assembly is named using this convention: Prism.*Container.Platform*.dll, e.g. **Prism.Unity.Wpf.dll**. Starting with version 7.0, Prism is moving to separate packages for each platform. Be sure to install the package for the Container and the Platform of your choice.

**NOTE:** MEF is supported with WPF for compatibility with previous versions. It will not be added to Windows 10 UWP or Xamarin Forms.

#### WPF

| Package | NuGet | MyGet |
|---------|-------|-------|
| [Prism.Autofac][AutofacWpfNuGet] | [![AutofacWpfNuGetShield]][AutofacWpfNuGet] | [![AutofacWpfMyGetShield]][AutofacWpfMyGet] |
| [Prism.DryIoc][DryIocWpfNuGet] | [![DryIocWpfNuGetShield]][DryIocWpfNuGet] | [![DryIocWpfMyGetShield]][DryIocWpfMyGet] |
| [Prism.Mef][MefWpfNuGet] | [![MefWpfNuGetShield]][MefWpfNuGet] | [![MefMyGetShield]][MefWpfMyGet] |
| [Prism.Ninject][NinjectWpfNuGet] | [![NinjectWpfNuGetShield]][NinjectWpfNuGet] | [![NinjectWpfMyGetShield]][NinjectWpfMyGet] |
| [Prism.StructureMap][StructureMapWpfNuGet] | [![StructureMapWpfNuGetShield]][StructureMapWpfNuGet] | [![StructureMapWpfMyGetShield]][StructureMapWpfMyGet] |
| [Prism.Unity][UnityWpfNuGet] | [![UnityWpfNuGetShield]][UnityWpfNuGet] | [![UnityWpfMyGetShield]][UnityWpfMyGet] |

#### UWP

| Package | NuGet | MyGet |
|---------|-------|-------|
| [Prism.Autofac.Windows][AutofacUWPNuGet] | [![AutofacUWPNuGetShield]][AutofacUWPNuGet] | [![AutofacUWPMyGetShield]][AutofacUWPMyGet] |
| [Prism.SimpleInjector.Windows][SimpleInjectorUWPNuGet] | [![SimpleInjectorUWPNuGetShield]][SimpleInjectorUWPNuGet] | [![SimpleInjectorUWPMyGetShield]][SimpleInjectorUWPMyGet] |
| [Prism.Unity.Windows][UnityUWPNuGet] | [![UnityUWPNuGetShield]][UnityUWPNuGet] | [![UnityUWPMyGetShield]][UnityUWPMyGet] |

#### Xamarin Forms

| Package | NuGet | MyGet |
|---------|-------|-------|
| [Prism.Autofac.Forms][AutofacFormsNuGet] | [![AutofacFormsNuGetShield]][AutofacFormsNuGet] | [![AutofacFormsMyGetShield]][AutofacFormsMyGet] |
| [Prism.DryIoc.Forms][DryIocFormsNuGet] | [![DryIocFormsNuGetShield]][DryIocFormsNuGet] | [![DryIocFormsMyGetShield]][DryIocFormsMyGet] |
| [Prism.Ninject.Forms][NinjectFormsNuGet] | [![NinjectFormsNuGetShield]][NinjectFormsNuGet] | [![NinjectFormsMyGetShield]][NinjectFormsMyGet] |
| [Prism.Unity.Forms][UnityFormsNuGet] | [![UnityFormsNuGetShield]][UnityFormsNuGet] | [![UnityFormsMyGetShield]][UnityFormsMyGet] |

![NuGet package tree](images/NuGetPackageTree.png)

A detailed overview of each assembly per package is available [here](docs/Download-and-Setup-Prism.md#overview-of-assemblies).

## Prism Template Pack

Prism now integrates with Visual Studio and Xamarin Studio to enable a highly productive developer workflow for creating WPF, UWP, and native iOS and Android applications using Xamarin.Forms.  Jump start your Prism apps with code snippets, item templates, and project templates for your IDE of choice.

### Visual Studio Gallery

The Prism Template Pack is available on the [Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/e7b6bde2-ba59-43dd-9d14-58409940ffa0).  To install, just go to Visual Studio -> Tools -> Extensions and Updates... then search for **Prism** in the online gallery:

![Visual Studio Gallery](images/prism-visual-studio-gallery.jpg)

### Visual Studio for Mac Addin

Due to the deprecation of the Mono Addin site, Prism now offers a direct Addin feed for the Prism Template Pack. Simply go to `Visual Studio` -> `Extensions...` and add the Prism Addins feed, then you can simply search for **Prism** in the Gallery and install the Template Pack.

![Visual Studio for Mac Addin Manager](images/prism-vs4mac-addin-templatepack.png)

## Samples

We have both a development sandbox (frequently changing) and stable samples for using Prism with WPF, UWP and Xamarin Forms. An overview of the samples can be found [here](Sandbox/README.md).

## Contributing

We strongly encourage you to get involved and help us evolve the code base.

- You can see what our expectations are for pull requests [here](https://github.com/PrismLibrary/Prism/blob/master/.github/CONTRIBUTING.md).

### .NET Foundation

This project is part of the [.NET Foundation](http://www.dotnetfoundation.org/projects).

[CoreNuGet]: https://www.nuget.org/packages/Prism.Core/
[WpfNuGet]: https://www.nuget.org/packages/Prism.Wpf/
[FormsNuGet]: https://www.nuget.org/packages/Prism.Forms/
[UWPNuGet]: https://www.nuget.org/packages/Prism.Windows/

[AutofacWpfNuGet]: https://www.nuget.org/packages/Prism.Autofac/
[DryIocWpfNuGet]: https://www.nuget.org/packages/Prism.DryIoc/
[MefWpfNuGet]: https://www.nuget.org/packages/Prism.Mef/
[NinjectWpfNuGet]: https://www.nuget.org/packages/Prism.Ninject/
[StructureMapWpfNuGet]: https://www.nuget.org/packages/Prism.StructureMap/
[UnityWpfNuGet]: https://www.nuget.org/packages/Prism.Unity/

[AutofacUWPNuGet]: https://www.nuget.org/packages/Prism.Autofac.Windows/
[SimpleInjectorUWPNuGet]: https://www.nuget.org/packages/Prism.SimpleInjector.Windows/
[UnityUWPNuGet]: https://www.nuget.org/packages/Prism.Unity.Windows/

[UnityFormsNuGet]: https://www.nuget.org/packages/Prism.Unity.Forms/
[NinjectFormsNuGet]: https://www.nuget.org/packages/Prism.Ninject.Forms/
[AutofacFormsNuGet]: https://www.nuget.org/packages/Prism.Autofac.Forms/
[DryIocFormsNuGet]: https://www.nuget.org/packages/Prism.DryIoc.Forms/


[CoreNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Core.svg
[WpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Wpf.svg
[FormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Forms.svg
[UWPNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Windows.svg

[AutofacWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Autofac.svg
[DryIocWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.svg
[MefWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Mef.svg
[NinjectWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Ninject.svg
[StructureMapWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.StructureMap.svg
[UnityWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.svg

[AutofacUWPNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Autofac.Windows.svg
[SimpleInjectorUWPNuGetShield]: https://img.shields.io/nuget/vpre/Prism.SimpleInjector.Windows.svg
[UnityUWPNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.Windows.svg

[AutofacFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Autofac.Forms.svg
[DryIocFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.Forms.svg
[NinjectFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Ninject.Forms.svg
[UnityFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.Forms.svg


[CoreMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Core
[WpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Wpf/
[FormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Forms/
[UWPMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Windows/

[AutofacWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Autofac
[DryIocWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.DryIoc/
[MefWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Mef/
[NinjectWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Ninject/
[StructureMapWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.StructureMap/
[UnityWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Unity/

[AutofacUWPMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Autofac.Windows/
[SimpleInjectorUWPMyGet]: https://myget.org/feed/prism/package/nuget/Prism.SimpleInjector.Windows/
[UnityUWPMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Unity.Windows/

[UnityFormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Unity.Forms/
[NinjectFormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Ninject.Forms/
[AutofacFormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Autofac.Forms/
[DryIocFormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.DryIoc.Forms/

[CoreMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Core.svg
[WpfMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Wpf.svg
[UWPMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Windows.svg
[FormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Forms.svg

[AutofacWpfMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Autofac.svg
[DryIocWpfMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.DryIoc.svg
[MefMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Mef.svg
[NinjectWpfMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Ninject.svg
[StructureMapWpfMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.StructureMap.svg
[UnityWpfMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Unity.svg

[AutofacUWPMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Autofac.Windows.svg
[SimpleInjectorUWPMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.SimpleInjector.Windows.svg
[UnityUWPMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Unity.Windows.svg

[AutofacFormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Autofac.Forms.svg
[DryIocFormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.DryIoc.Forms.svg
[NinjectFormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Ninject.Forms.svg
[UnityFormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Unity.Forms.svg