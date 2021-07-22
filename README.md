# Prism

Prism is a framework for building loosely coupled, maintainable, and testable XAML applications in WPF, Xamarin Forms, Uno Platform and WinUI. Separate releases are available for each platform and those will be developed on independent timelines. Prism provides an implementation of a collection of design patterns that are helpful in writing well-structured and maintainable XAML applications, including MVVM, dependency injection, commands, EventAggregator, and others. Prism's core functionality is a shared code base supported in .NET Standard 2.0, .NET Framework 4.5 / 4.7. Those things that need to be platform specific are implemented in the respective libraries for the target platform. Prism also provides great integration of these patterns with the target platform. For example, Prism for Xamarin Forms allows you to use an abstraction for navigation that is unit testable, but that layers on top of the platform concepts and APIs for navigation so that you can fully leverage what the platform itself has to offer, but done in the MVVM way.

## Help Support Prism

As most of you know, it takes a lot of time and effort for our small team to manage and maintain Prism in our spare time. Even though Prism is open source and hosted on GitHub, there are a number of costs associated with maintaining a project such as Prism.  Please be sure to Star the Prism repo and help sponsor Dan and Brian on GitHub. As a bonus GitHub sponsors get access to Sponsor Connect where you can access exclusive training content, all Prism CI builds, and a Sponsor Only Discord with Brian and Dan!

Don't forget both Brian and Dan have content on YouTube and stream there from time to time. Be sure to subscribe to their channels and turn on notifications so you know when they do a Live Stream!

| | Sponsor | Twitter | YouTube |
|:-:|:--:|:--:|:--:|
| Brian Lagunas | [![GitHub][OctoSponsor]](https://xam.dev/sponsor-prism-brian) | [![Twitter][TwitterLogo]](https://twitter.com/brianlagunas)<br /><span style="font-size:9px">Follow</span> | [![YouTube][YouTubeLogo]](https://youtube.com/brianlagunas)<br /><span style="font-size:9px">Subcribe & Ring the Bell</span>
| Dan Siegel | [![GitHub][OctoSponsor]](https://xam.dev/sponsor-prism-dan) | [![Twitter][TwitterLogo]](https://twitter.com/DanJSiegel)<br /><span style="font-size:9px">Follow</span> | [![YouTube][YouTubeLogo]](https://youtube.com/dansiegel)<br /><span style="font-size:9px">Subscribe & Ring the Bell</span>

## Build Status

|          | Status |
| -------- | ------ |
| Full Build | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=Build%20Prism%20Library)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |
| Prism.Core | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=Build%20%26%20Test&jobName=Prism.Core)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |
| Prism.Wpf | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=Build%20%26%20Test&jobName=Prism.Wpf)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |
| Prism.Forms | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=Build%20%26%20Test&jobName=Prism.Forms)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |
| Prism.Uno | [![Build Status](https://dev.azure.com/prismlibrary/Prism/_apis/build/status/Prism-CI?branchName=master&stageName=Build%20%26%20Test&jobName=Prism.Uno)](https://dev.azure.com/prismlibrary/Prism/_build/latest?definitionId=9&branchName=master) |

## Support

- Documentation is maintained in [the Prism-Documentation repo](https://github.com/PrismLibrary/Prism-Documentation) under /docs and can be found in a readable format on [the website](http://prismlibrary.com/docs/).
- For general questions and support, post your questions on [StackOverflow](http://stackoverflow.com/questions/tagged/prism).
- You can enter bugs and feature requests in our [Issues](https://github.com/PrismLibrary/Prism/issues/new/choose).
- [Enterprise Support](https://avantipoint.com/contact?utm_source=github&utm_medium=prism-readme) is available exclusively from AvantiPoint, and helps to support this project.

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

## NuGet Packages

Official Prism releases are available on NuGet. Prism packages are also available on the SponsorConnect feed which will be updated with each merged PR. If you want to take advantage of a new feature as soon as it's merged into the code base, or if there is a critical bug you need fixed we invite you to try the packages on this feed. The SponsorConnect package feed is available to Sponsors only.

### Core Packages

These are the base packages for each platform, together with the Prism's Core assembly as a cross-platform PCL.

| Platform | Package | NuGet | SponsorConnect |
| -------- | ------- | ------- | ----- |
| Cross Platform | [Prism.Core][CoreNuGet] | [![CoreNuGetShield]][CoreNuGet] | [![CoreSponsorConnectShield]][CoreSponsorConnect] |
| WPF | [Prism.Wpf][WpfNuGet] | [![WpfNuGetShield]][WpfNuGet] | [![WpfSponsorConnectShield]][WpfSponsorConnect] |
| Xamarin.Forms | [Prism.Forms][FormsNuGet] | [![FormsNuGetShield]][FormsNuGet] | [![FormsSponsorConnectShield]][FormsSponsorConnect] |
| Uno Platform and WinUI | [Prism.Uno][UnoNuGet] | [![UnoNuGetShield]][UnoNuGet] | [![UnoSponsorConnectShield]][UnoSponsorConnect] |

### Container-specific packages

Each supported IoC container has its own package assisting in the setup and usage of that container together with Prism. The assembly is named using this convention: Prism.*Container.Platform*.dll, e.g. **Prism.Unity.Wpf.dll**. Starting with version 7.0, Prism is moving to separate packages for each platform. Be sure to install the package for the Container and the Platform of your choice.

#### WPF

| Package | NuGet | SponsorConnect |
|---------|-------|-------|
| [Prism.DryIoc][DryIocWpfNuGet] | [![DryIocWpfNuGetShield]][DryIocWpfNuGet] | [![DryIocWpfSponsorConnectShield]][DryIocWpfSponsorConnect] |
| [Prism.Unity][UnityWpfNuGet] | [![UnityWpfNuGetShield]][UnityWpfNuGet] | [![UnityWpfSponsorConnectShield]][UnityWpfSponsorConnect] |

#### Xamarin Forms

| Package | NuGet | SponsorConnect |
|---------|-------|-------|
| [Prism.DryIoc.Forms][DryIocFormsNuGet] | [![DryIocFormsNuGetShield]][DryIocFormsNuGet] | [![DryIocFormsSponsorConnectShield]][DryIocFormsSponsorConnect] |
| [Prism.Unity.Forms][UnityFormsNuGet] | [![UnityFormsNuGetShield]][UnityFormsNuGet] | [![UnityFormsSponsorConnectShield]][UnityFormsSponsorConnect] |
| [Prism.Forms.Regions][PrismFormsRegionsNuget] | [![PrismFormsRegionsNuGetShield]][PrismFormsRegionsNuGet] | [![PrismFormsRegionsSponsorConnectShield]][PrismFormsRegionsSponsorConnect] |

#### Uno Platform

| Package | NuGet | SponsorConnect |
|---------|-------|-------|
| [Prism.DryIoc.Uno][DryIocUnoPlatformNuGet] | [![DryIocUnoPlatformNuGetShield]][DryIocUnoPlatformNuGet] | [![DryIocUnoPlatformSponsorConnectShield]][DryIocUnoPlatformSponsorConnect] |
| [Prism.Unity.Uno][UnityUnoPlatformNuGet] | [![UnityUnoPlatformNuGetShield]][UnityUnoPlatformNuGet] | [![UnityUnoPlatformSponsorConnectShield]][UnityUnoPlatformSponsorConnect] |

![NuGet package tree](images/NuGetPackageTree.png)

A detailed overview of each assembly per package is available [here](http://prismlibrary.github.io/docs/getting-started/NuGet-Packages.html).

## Prism Template Pack

Prism integrates with Visual Studio to enable a highly productive developer workflow for creating WPF, and native iOS and Android applications using Xamarin.Forms.  Jump start your Prism apps with code snippets, item templates, and project templates for your IDE of choice.

> **NOTE**
>
> The Prism Templates are open source and available at
>
> https://github.com/PrismLibrary/Prism.Templates

### Visual Studio Gallery

The Prism Template Pack is available on the [Visual Studio Gallery](https://marketplace.visualstudio.com/items?itemName=BrianLagunas.PrismTemplatePack).  To install, just go to Visual Studio -> Tools -> Extensions and Updates... then search for **Prism** in the online gallery:

![Visual Studio Gallery](images/prism-visual-studio-gallery.jpg)

## Plugins

There are certain things that cannot be added directly into Prism for various reasons. To handle these common tasks such as supporting PopupPage's in Xamarin Forms, there are Prism Plugins. You can find a number of Plugins available on NuGet from our maintainer [@DanJSiegel](https://twitter.com/DanJSiegel).

- [Prism.Plugin.Popups](https://github.com/dansiegel/Prism.Plugin.Popups) (Forms Only)
- [Prism.Popups.XCT](https://github.com/FileOnQ/Prism.Popups.XCT) (Forms Only)
  - Adds support for native popups using Xamarin Community Toolkits Popup API
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
- [Prism for Uno Platform](#) (Coming soon)

## Contributing

We strongly encourage you to get involved and help us evolve the code base.

- You can see what our expectations are for pull requests [here](https://github.com/PrismLibrary/Prism/blob/master/.github/CONTRIBUTING.md).

### .NET Foundation

This project is part of the [.NET Foundation](http://www.dotnetfoundation.org/projects).

[CoreNuGet]: https://www.nuget.org/packages/Prism.Core/
[WpfNuGet]: https://www.nuget.org/packages/Prism.Wpf/
[FormsNuGet]: https://www.nuget.org/packages/Prism.Forms/
[UnoNuGet]: https://www.nuget.org/packages/Prism.Uno/

[PrismFormsRegionsNuGet]: https://www.nuget.org/packages/Prism.Forms.Regions/
[PrismFormsRegionsSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.Forms.Regions
[PrismFormsRegionsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Forms.Regions.svg
[PrismFormsRegionsSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.Forms.Regions%2Fvpre

[DryIocWpfNuGet]: https://www.nuget.org/packages/Prism.DryIoc/
[UnityWpfNuGet]: https://www.nuget.org/packages/Prism.Unity/

[UnityFormsNuGet]: https://www.nuget.org/packages/Prism.Unity.Forms/
[DryIocFormsNuGet]: https://www.nuget.org/packages/Prism.DryIoc.Forms/

[DryIocUnoPlatformNuGet]: https://www.nuget.org/packages/Prism.DryIoc.Uno/
[UnityUnoPlatformNuGet]: https://www.nuget.org/packages/Prism.Unity.Uno/

[CoreNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Core.svg
[WpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Wpf.svg
[FormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Forms.svg
[UnoNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Uno.svg

[DryIocWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.svg
[UnityWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.svg

[DryIocFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.Forms.svg
[UnityFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.Forms.svg

[DryIocUnoPlatformNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.Uno.svg
[UnityUnoPlatformNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.Uno.svg

[CoreSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.Core
[WpfSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.Wpf
[FormsSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.Forms
[UnoSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.Uno

[DryIocWpfSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.DryIoc
[UnityWpfSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.Unity

[UnityFormsSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.Unity.Forms
[DryIocFormsSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.DryIoc.Forms

[DryIocUnoPlatformSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.DryIoc.Uno
[UnityUnoPlatformSponsorConnect]: https://sponsorconnect.dev/nuget/package/Prism.Unity.Uno

[CoreSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.Core%2Fvpre
[WpfSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.Wpf%2Fvpre
[FormsSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.Forms%2Fvpre
[UnoSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.Uno%2Fvpre

[DryIocWpfSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.DryIoc%2Fvpre
[UnityWpfSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.Unity%2Fvpre

[DryIocFormsSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.DryIoc.Forms%2Fvpre
[UnityFormsSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.Unity.Forms%2Fvpre

[DryIocUnoPlatformSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.DryIoc.Uno%2Fvpre
[UnityUnoPlatformSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FPrism.Unity.Uno%2Fvpre

[TwitterLogo]: https://dansiegelgithubsponsors.blob.core.windows.net/images/twitter.png
[TwitchLogo]: https://dansiegelgithubsponsors.blob.core.windows.net/images/twitch.png
[YouTubeLogo]: https://dansiegelgithubsponsors.blob.core.windows.net/images/youtube.png
[OctoSponsor]: https://dansiegelgithubsponsors.blob.core.windows.net/images/octosponsor.png
