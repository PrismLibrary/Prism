# Prism

Prism is a framework for building loosely coupled, maintainable, and testable XAML applications in WPF, Xamarin Forms, Uno Platform and WinUI. Separate releases are available for each platform and those will be developed on independent timelines. Prism provides an implementation of a collection of design patterns that are helpful in writing well-structured and maintainable XAML applications, including MVVM, dependency injection, commands, EventAggregator, and others. Prism's core functionality is a shared code base supported in .NET Standard 2.0, .Net Core 3, and .NET Framework 4.5. Those things that need to be platform specific are implemented in the respective libraries for the target platform. Prism also provides great integration of these patterns with the target platform. For example, Prism for Xamarin Forms allows you to use an abstraction for navigation that is unit testable, but that layers on top of the platform concepts and APIs for navigation so that you can fully leverage what the platform itself has to offer, but done in the MVVM way.

Prism 7 is a fully open source version of the Prism guidance [originally produced by Microsoft patterns & practices](https://devblogs.microsoft.com/dotnet/prism-grows-up/). The core team members were all part of the P&amp;P team that developed Prism 1 through 5, and the effort has now been turned over to the open source community to keep it alive and thriving to support the .NET community. There are thousands of companies who have adopted previous versions of Prism, and we hope they will continue to move along with us as we continue to evolve and enhance the framework to keep pace with current platform capabilities and requirements.

## Help Support Prism

As most of you know, it takes a lot of time and effort for our small team to manage and maintain Prism in our spare time. Even though Prism is open source and hosted on GitHub, there are a number of costs associated with maintaining a project such as Prism.  Please be sure to Star the Prism repo and help sponsor Dan and Brian on GitHub.

- Sponsor [Brian Lagunas](https://xam.dev/sponsor-prism-brian)
- Sponsor [Dan Siegel](https://xam.dev/sponsor-prism-dan)

## Build Status

|          | Status |
| -------- | ------ |
| Full Build | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=Build%20Prism%20Library)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |
| Prism.Core | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=Build%20%26%20Test&jobName=Prism.Core)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |
| Prism.Wpf | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=Build%20%26%20Test&jobName=Prism.Wpf)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |
| Prism.Forms | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=Build%20%26%20Test&jobName=Prism.Forms)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |
| Prism.Uno | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=Build%20%26%20Test&jobName=Prism.Uno)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |

### E2E Build Status

| E2E App | Status |
|---------|:------:|
| WPF | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=End%20to%20End&jobName=E2E%20WPF%20App)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |
| Xamarin iOS | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=End%20to%20End&jobName=E2E%20iOS%20App)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |
| Xamarin Android | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=End%20to%20End&jobName=E2E%20Android%20App)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |

## Support

- Documentation is maintained in [the Prism-Documentation repo](https://github.com/PrismLibrary/Prism-Documentation) under /docs and can be found in a readable format on [the website](http://prismlibrary.com/docs/).
- For general questions and support, post your questions on [StackOverflow](http://stackoverflow.com/questions/tagged/prism).
- You can enter bugs and feature requests in our [Issues](https://github.com/PrismLibrary/Prism/issues/new/choose).
- Paid [Enterprise Support](https://avantipoint.com/contact?utm_source=github&utm_medium=prism-readme) is available exclusively from AvantiPoint, and helps to support this project.

## Videos &amp; Training

By watching our courses, not only do you help support the project financially, but you might also learn something along the way.  We believe this is a win-win for everyone.

- [Introduction to Prism for WPF (NEW)](https://pluralsight.pxf.io/bE3rB)
- [Introduction to Prism (Legacy)](https://pluralsight.pxf.io/W1Dz3)
- [What's New in Prism 5.0](https://pluralsight.pxf.io/z7avm)
- [Prism Problems & Solutions: Showing Multiple Shells](https://pluralsight.pxf.io/XVxR5)
- [Prism Problems & Solutions: Mastering TabControl](https://pluralsight.pxf.io/B6X99)
- [Prism Problems & Solutions: Loading Modules Based on User Roles](https://pluralsight.pxf.io/GvjkE)
- [Prism Problems & Solutions: Loading Dependent Views](https://pluralsight.pxf.io/a01zj)

We appreciate your support.

### Live & Recorded Video Content

Both Brian and Dan stream live on Twitch and host recorded content on their YouTube Channels. Be sure to Subscribe and Ring that bell for notifications when they go live or post new content.

| | Twitch | YouTube |
|:-:|:--:|:--:|
| Brian Lagunas | [Twitch](https://twitch.tv/brianlagunas) | [YouTube](https://youtube.com/brianlagunas) |
| Dan Siegel | [Twitch](https://twitch.tv/dansiegel) | [YouTube](https://youtube.com/dansiegel) |

## NuGet Packages

Official Prism releases are available on NuGet. Prism also has a MyGet feed which will be updated with each merged PR. If you want to take advantage of a new feature as soon as it's merged into the code base, or if there is a critical bug you need fixed we invite you to try the packages on this feed. Our feed is a public feed in the MyGet Gallery.

Simply add `https://www.myget.org/F/prism/api/v3/index.json` as a package source to either Visual Studio or Visual Studio for Mac.

### Core Packages

These are the base packages for each platform, together with the Prism's Core assembly as a cross-platform PCL.

| Platform | Package | NuGet | MyGet |
| -------- | ------- | ------- | ----- |
| Cross Platform | [Prism.Core][CoreNuGet] | [![CoreNuGetShield]][CoreNuGet] | [![CoreMyGetShield]][CoreMyGet] |
| WPF | [Prism.Wpf][WpfNuGet] | [![WpfNuGetShield]][WpfNuGet] | [![WpfMyGetShield]][WpfMyGet] |
| Xamarin.Forms | [Prism.Forms][FormsNuGet] | [![FormsNuGetShield]][FormsNuGet] | [![FormsMyGetShield]][FormsMyGet] |
| Uno Platform and WinUI | [Prism.Uno][UnoNuGet] | [![UnoNuGetShield]][UnoNuGet] | [![UnoMyGetShield]][UnoMyGet] |

### Container-specific packages

Each supported IoC container has its own package assisting in the setup and usage of that container together with Prism. The assembly is named using this convention: Prism.*Container.Platform*.dll, e.g. **Prism.Unity.Wpf.dll**. Starting with version 7.0, Prism is moving to separate packages for each platform. Be sure to install the package for the Container and the Platform of your choice.

#### WPF

| Package | NuGet | MyGet |
|---------|-------|-------|
| [Prism.DryIoc][DryIocWpfNuGet] | [![DryIocWpfNuGetShield]][DryIocWpfNuGet] | [![DryIocWpfMyGetShield]][DryIocWpfMyGet] |
| [Prism.Unity][UnityWpfNuGet] | [![UnityWpfNuGetShield]][UnityWpfNuGet] | [![UnityWpfMyGetShield]][UnityWpfMyGet] |

#### Xamarin Forms

| Package | NuGet | MyGet |
|---------|-------|-------|
| [Prism.DryIoc.Forms][DryIocFormsNuGet] | [![DryIocFormsNuGetShield]][DryIocFormsNuGet] | [![DryIocFormsMyGetShield]][DryIocFormsMyGet] |
| [Prism.Unity.Forms][UnityFormsNuGet] | [![UnityFormsNuGetShield]][UnityFormsNuGet] | [![UnityFormsMyGetShield]][UnityFormsMyGet] |

#### Package Notices

- For developers using Unity with Prism 6, take note that the new Unity maintainer has made major breaking changes. This includes changing namespaces and the package structure. These changes were NOT made by the Prism team nor do we have any control over it. When upgrading to Prism 7 you will need to uninstall the existing Unity package as we now reference the Unity.Container NuGet.

![NuGet package tree](images/NuGetPackageTree.png)

A detailed overview of each assembly per package is available [here](http://prismlibrary.github.io/docs/getting-started/NuGet-Packages.html).

## Prism Template Pack

Prism integrates with Visual Studio to enable a highly productive developer workflow for creating WPF, and native iOS and Android applications using Xamarin.Forms.  Jump start your Prism apps with code snippets, item templates, and project templates for your IDE of choice.

> **NOTE**
> The Prism Templates are open source and available at
> https://github.com/PrismLibrary/Prism.Templates

### Visual Studio Gallery

The Prism Template Pack is available on the [Visual Studio Gallery](https://marketplace.visualstudio.com/items?itemName=BrianLagunas.PrismTemplatePack).  To install, just go to Visual Studio -> Tools -> Extensions and Updates... then search for **Prism** in the online gallery:

![Visual Studio Gallery](images/prism-visual-studio-gallery.jpg)

## Plugins

There are certain things that cannot be added directly into Prism for various reasons. To handle these common tasks such as supporting PopupPage's in Xamarin Forms, there are Prism Plugins. You can find a number of Plugins available on NuGet from our maintainer [@DanJSiegel](https://twitter.com/DanJSiegel).

- [Prism.Plugin.Popups](https://github.com/dansiegel/Prism.Plugin.Popups) (Forms Only)
- [Prism.Plugin.Logging](https://github.com/dansiegel/Prism.Plugin.Logging) (Works on all Platforms)
  - Adds support for Syslog, Loggly, Graylog, Application Insights, &amp; App Center
- [Prism.Container.Extensions](https://github.com/dansiegel/Prism.Container.Extensions)
  - Adds advanced Container Registration abstractions
  - Adds DryIoc & Unity ContainerExtension with support for Microsoft.DependencyInjection.Extensions. Uses a singleton pattern to allow initialization from a native platform
  - Provides an extended PrismApplication with additional error handling and platform specifics support for Prism.Forms
- [Prism.Magician](https://sponsorconnect.dev/nuget/package/prism.magician) (Works with ALL Platforms)
  - The Magician works to reduce the amount of code you need to write with a collection of intelligent code generators that evaluate your codebase and references
  - It additionally provides a series of Roslyn Analyzers to help prevent you from making common mistakes
  - **NOTE:** This package is only available to Dan's [GitHub Sponsors](https://xam.dev/sponsor-prism-dan) and [Enterprise Support](https://avantipoint.com/contact) customers.

## Samples

For stable samples be sure to check out the samples repo for the platform you are most interested in.

- [Prism for WPF Samples](https://github.com/PrismLibrary/Prism-Samples-Wpf)
- [Prism for Xamarim.Forms](https://github.com/PrismLibrary/Prism-Samples-Forms)

## Contributing

We strongly encourage you to get involved and help us evolve the code base.

- You can see what our expectations are for pull requests [here](https://github.com/PrismLibrary/Prism/blob/master/.github/CONTRIBUTING.md).

### .NET Foundation

This project is part of the [.NET Foundation](http://www.dotnetfoundation.org/projects).

[CoreNuGet]: https://www.nuget.org/packages/Prism.Core/
[WpfNuGet]: https://www.nuget.org/packages/Prism.Wpf/
[FormsNuGet]: https://www.nuget.org/packages/Prism.Forms/
[UnoNuGet]: https://www.nuget.org/packages/Prism.Uno/

[DryIocWpfNuGet]: https://www.nuget.org/packages/Prism.DryIoc/
[UnityWpfNuGet]: https://www.nuget.org/packages/Prism.Unity/

[UnityFormsNuGet]: https://www.nuget.org/packages/Prism.Unity.Forms/
[DryIocFormsNuGet]: https://www.nuget.org/packages/Prism.DryIoc.Forms/

[DryIocUnoNuGet]: https://www.nuget.org/packages/Prism.DryIoc.Uno/
[UnityUnoNuGet]: https://www.nuget.org/packages/Prism.Unity.Uno/

[CoreNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Core.svg
[WpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Wpf.svg
[FormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Forms.svg
[UnoNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Uno.svg

[DryIocWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.svg
[UnityWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.svg

[DryIocFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.Forms.svg
[UnityFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.Forms.svg

[DryIocUnoNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.Uno.svg
[UnityUnoNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.Uno.svg

[CoreMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Core
[WpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Wpf
[FormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Forms
[UnoMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Uno

[DryIocWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.DryIoc
[UnityWpfMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Unity

[UnityFormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.Unity.Forms
[DryIocFormsMyGet]: https://myget.org/feed/prism/package/nuget/Prism.DryIoc.Forms

[CoreMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Core.svg
[WpfMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Wpf.svg
[FormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Forms.svg
[UnoMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Uno.svg

[DryIocWpfMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.DryIoc.svg
[UnityWpfMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Unity.svg

[DryIocFormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.DryIoc.Forms.svg
[UnityFormsMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Unity.Forms.svg

[DryIocUnoMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.DryIoc.Uno.svg
[UnityUnoMyGetShield]: https://img.shields.io/myget/prism/vpre/Prism.Unity.Uno.svg
