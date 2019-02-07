# Prism

Prism is a framework for building loosely coupled, maintainable, and testable XAML applications in WPF, Windows 10 UWP, and Xamarin Forms. Separate releases are available for each platform and those will be developed on independent timelines. Prism provides an implementation of a collection of design patterns that are helpful in writing well-structured and maintainable XAML applications, including MVVM, dependency injection, commands, EventAggregator, and others. Prism's core functionality is a shared code base in a Portable Class Library targeting these platforms. Those things that need to be platform specific are implemented in the respective libraries for the target platform. Prism also provides great integration of these patterns with the target platform. For example, Prism for UWP and Xamarin Forms allows you to use an abstraction for navigation that is unit testable, but that layers on top of the platform concepts and APIs for navigation so that you can fully leverage what the platform itself has to offer, but done in the MVVM way.

Prism 6 is a fully open source version of the Prism guidance [originally produced by Microsoft patterns & practices](http://blogs.msdn.com/b/dotnet/archive/2015/03/19/prism-grows-up.aspx). The core team members were all part of the P&amp;P team that developed Prism 1 through 5, and the effort has now been turned over to the open source community to keep it alive and thriving to support the .NET community. There are thousands of companies who have adopted previous versions of Prism for WPF, Silverlight, and Windows Runtime, and we hope they will continue to move along with us as we continue to evolve and enhance the framework to keep pace with current platform capabilities and requirements.

At the current time, we have no plans to create new versions of the library for Silverlight or for Windows 8/8.1/WP8.1. For those you can still use the previous releases from Microsoft P&amp;P [here](https://msdn.microsoft.com/en-us/library/Gg430869%28v=PandP.40%29.aspx) and [here](http://prismwindowsruntime.codeplex.com/). If there is enough interest and contributors to do the work, we can consider it, but it is not on our roadmap for now.

## Build Status

|          | Status |
| -------- | ------ |
| Full Build | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9) |
| Prism.Core | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism.Core-CI)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=10) |
| Prism.Wpf | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism.WPF-CI)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=12) |
| Prism.Windows | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism.UWP-CI)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=13) |
| Prism.Forms | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism.Forms-CI)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=11) |

## Support

- Documentation is maintained in [the Prism-Documentation repo](https://github.com/PrismLibrary/Prism-Documentation) under /docs and can be found in a readable format on [the website](http://prismlibrary.github.io/docs/).
- For general questions and support, post your questions on [StackOverflow](http://stackoverflow.com/questions/tagged/prism).
- You can enter bugs and feature requests in our [Issues](https://github.com/PrismLibrary/Prism/issues).

## Help Support Prism

As most of you know, it takes a lot of time and effort for our small team to manage and maintain Prism in our spare time.  Even though Prism is open source and hosted on GitHub, there are a number of costs associated with maintaining a project such as Prism.  If you would like to help support us, the easiest thing you can do is become a Patron and watch our Pluralsight courses on Prism.

By becoming a [Patron and subscribing](https://www.patreon.com/prismlibrary) to the Prism Library, you will receive a number of benefits depending on your level of support.

**Supporter** - $5+ per month
- Receive all Prism Library news and announcements, such as new release information and blogs posts.
- Gives you access to our community Slack channel where you can ask questions and get help from the community and the Prism Library project maintainers (when available).

**Backer** - $10+ per month
Everything in the Supporter plan plus:
- A Prism sticker

**Generous Backer** - $25+ per month
Everything in the Backer plan plus:
- Video Tutorial request priortiy (topic acceptance not guaranteed)
- Bragging rights!

**Bronze Sponsor** - $100+ per month
Everything in the Generous Backer plan plus:
- Your name or company logo (small) will be put in sponsors.md in the Prism repository.

**Silver Sponsor** - $250+ per month
Everything in the Generous Backer plan plus:
- Your name or company logo (medium) will be put in the sponsors.md in the Prism repository.
- Your name or company logo (medium) will be put on the repository ReadMe.md

**Gold Sponsor** - $500+ per month
Everything in the Generous Backer plan plus:
- Your name or company logo (large) will be put in the sponsors.md in the Prism repository.
- Your name or company logo (large) will be put on the repository ReadMe.md
- Your name or company logo (large) on the homepage of PrismLibrary.com
- 1 hour VIP support per month

**Platinum Sponsor** - $1,000+ per month
Everything in the Generous Backer plan plus:
- Your name or company logo (large) will be put at the top of the sponsors.md in the Prism repository.
- Your name or company logo (large) will be put on the repository ReadMe.md
- Your name or company logo on the homepage of PrismLibrary.com
- 1 Sponsored Video Tutorial per month
- 2 hours VIP support per month

**Corporate Sponsor** - $10,000+ per month
Everything in the Platinum Sponsor plan plus:
- 3 days onsite training for your company (every six months)
- Dedicated VIP Support

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

#### WPF

| Package | NuGet | MyGet |
|---------|-------|-------|
| [Prism.Autofac][AutofacWpfNuGet]* | [![AutofacWpfNuGetShield]][AutofacWpfNuGet] | see notes |
| [Prism.DryIoc][DryIocWpfNuGet] | [![DryIocWpfNuGetShield]][DryIocWpfNuGet] | [![DryIocWpfMyGetShield]][DryIocWpfMyGet] |
| [Prism.Mef][MefWpfNuGet]* | [![MefWpfNuGetShield]][MefWpfNuGet] | see notes |
| [Prism.Ninject][NinjectWpfNuGet] | [![NinjectWpfNuGetShield]][NinjectWpfNuGet] | [![NinjectWpfMyGetShield]][NinjectWpfMyGet] |
| [Prism.StructureMap][StructureMapWpfNuGet] | [![StructureMapWpfNuGetShield]][StructureMapWpfNuGet] | see notes |
| [Prism.Unity][UnityWpfNuGet] | [![UnityWpfNuGetShield]][UnityWpfNuGet] | [![UnityWpfMyGetShield]][UnityWpfMyGet] |

#### UWP

| Package | NuGet | MyGet |
|---------|-------|-------|
| [Prism.DryIoc.Windows][DryIocUWPNuGet] | [![DryIocUWPNuGetShield]][DryIocUWPNuGet] | [![DryIocUWPMyGetShield]][DryIocUWPMyGet] |
| [Prism.Unity.Windows][UnityUWPNuGet] | [![UnityUWPNuGetShield]][UnityUWPNuGet] | [![UnityUWPMyGetShield]][UnityUWPMyGet] |

#### Xamarin Forms

| Package | NuGet | MyGet |
|---------|-------|-------|
| [Prism.Autofac.Forms][AutofacFormsNuGet]* | [![AutofacFormsNuGetShield]][AutofacFormsNuGet] | see notes |
| [Prism.DryIoc.Forms][DryIocFormsNuGet] | [![DryIocFormsNuGetShield]][DryIocFormsNuGet] | [![DryIocFormsMyGetShield]][DryIocFormsMyGet] |
| [Prism.Ninject.Forms][NinjectFormsNuGet]* | [![NinjectFormsNuGetShield]][NinjectFormsNuGet] | see notes |
| [Prism.Unity.Forms][UnityFormsNuGet] | [![UnityFormsNuGetShield]][UnityFormsNuGet] | [![UnityFormsMyGetShield]][UnityFormsMyGet] |

#### Package Notices

- Autofac will be removed following the 7.1 release due to it's inability to support Prism Modularity.
- MEF is no longer supported in Prism 7 as it is not truly a DI Container and lacks the performance that developers deserve.
- Ninject for Xamarin Forms has been discontinued due in Prism 7 to an incompatibility of the container with Xamarin Targets.
- StructureMap has reached EOL as a container. As a result the Prism team will no longer be continuing to provide updates to the StructureMap package moving forward.
- For developers using Unity with Prism 6, take note that the new Unity maintainer has made major breaking changes. This includes changing namespaces and the package structure. These changes were NOT made by the Prism team nor do we have any control over it. When upgrading to Prism 7 you will need to uninstall the existing Unity package as we now reference the Unity.Container NuGet.

![NuGet package tree](images/NuGetPackageTree.png)

A detailed overview of each assembly per package is available [here](docs/Download-and-Setup-Prism.md#overview-of-assemblies).

## Prism Template Pack

Prism now integrates with Visual Studio and Xamarin Studio to enable a highly productive developer workflow for creating WPF, UWP, and native iOS and Android applications using Xamarin.Forms.  Jump start your Prism apps with code snippets, item templates, and project templates for your IDE of choice.

### Visual Studio Gallery

The Prism Template Pack is available on the [Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/e7b6bde2-ba59-43dd-9d14-58409940ffa0).  To install, just go to Visual Studio -> Tools -> Extensions and Updates... then search for **Prism** in the online gallery:

![Visual Studio Gallery](images/prism-visual-studio-gallery.jpg)

### Visual Studio for Mac Addin

The Prism Template Studio and Developer Toolkit is available from the Visual Studio Mac Extensions Gallery.

## Plugins

There are certain things that cannot be added directly into Prism for various reasons. To handle these common tasks such as supporting PopupPage's in Xamarin Forms, there are Prism Plugins. You can find a number of Plugins available on NuGet from our maintainer @DanJSiegel.

- [Prism.Plugin.Popups](https://github.com/dansiegel/Prism.Plugin.Popups) (Forms Only)
- [Prism.Plugin.Logging](https://github.com/dansiegel/Prism.Plugin.Logging) (Works on all Platforms)
  - Adds support for Syslog, Loggly, and Graylog
- [Prism.Plugin.PageDialogs](https://github.com/dansiegel/Prism.Plugin.PageDialogs) (Forms Only)
- [Prism.MFractor.Config](https://nuget.org/packages/Prism.MFractor.Config)
  - Configures MFractor in Visual Studio for Mac to follow Prism Conventions

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

[DryIocUWPNuGet]: https://www.nuget.org/packages/Prism.DryIoc.Windows/
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

[DryIocUWPNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.Windows.svg
[UnityUWPNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.Windows.svg

[AutofacFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Autofac.Forms.svg
[DryIocFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.Forms.svg
[NinjectFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Ninject.Forms.svg
[UnityFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.Forms.svg


[CoreMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Core
[WpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Wpf
[FormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Forms
[UWPMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Windows

[AutofacWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Autofac
[DryIocWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.DryIoc
[MefWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Mef
[NinjectWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Ninject
[StructureMapWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.StructureMap
[UnityWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Unity

[DryIocUWPMyGet]: https://myget.org/feed/prism/package/nuget/Prism.DryIoc.Windows
[UnityUWPMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Unity.Windows

[UnityFormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Unity.Forms
[NinjectFormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Ninject.Forms
[AutofacFormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Autofac.Forms
[DryIocFormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.DryIoc.Forms

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

[DryIocUWPMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.DryIoc.Windows.svg
[UnityUWPMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Unity.Windows.svg

[AutofacFormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Autofac.Forms.svg
[DryIocFormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.DryIoc.Forms.svg
[NinjectFormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Ninject.Forms.svg
[UnityFormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Unity.Forms.svg
