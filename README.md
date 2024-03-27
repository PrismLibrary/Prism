# Prism

Prism is a framework for building loosely coupled, maintainable, and testable XAML applications in WPF, Xamarin Forms, Uno Platform and WinUI. Separate releases are available for each platform and those will be developed on independent timelines. Prism provides an implementation of a collection of design patterns that are helpful in writing well-structured and maintainable XAML applications, including MVVM, dependency injection, commands, EventAggregator, and others. Prism's core functionality is a shared code base supported in .NET Standard 2.0, .NET Framework 4.6 / 4.7, and .NET6.0/.NET8.0. Those things that need to be platform specific are implemented in the respective libraries for the target platform. Prism also provides great integration of these patterns with the target platform.

## Licensing

The Prism Team would first and foremost like to thank all of those developers who have stepped up over the past 4 years with GitHub Sponsors. We are committed to ensuring the longevity and success of the Prism Library. As a result Prism 9.0 is now [Dual License](LICENSE). We continue to offer a FREE Community License for the vast majority of our community, while the Commercial License will now be required by larger organizations to help fund and support the development of Prism. We additionally have the Commercial Plus License which grants access to a number of additional support libraries that build on top of Prism as well as a private Discord channel where you can ask questions and interact with the Prism Team.

## Build Status

|          | Status |
| -------- | ------ |
| Full Build | [![Prism CI](https://github.com/PrismLibrary/Prism/actions/workflows/ci.yml/badge.svg)](https://github.com/PrismLibrary/Prism/actions/workflows/ci.yml) |
| Prism.Core | [![build_core](https://github.com/PrismLibrary/Prism/actions/workflows/build_core.yml/badge.svg)](https://github.com/PrismLibrary/Prism/actions/workflows/build_core.yml) |
| Prism.Wpf | [![build_wpf](https://github.com/PrismLibrary/Prism/actions/workflows/build_wpf.yml/badge.svg)](https://github.com/PrismLibrary/Prism/actions/workflows/build_wpf.yml) |
| Prism.Forms | [![build_forms](https://github.com/PrismLibrary/Prism/actions/workflows/build_forms.yml/badge.svg)](https://github.com/PrismLibrary/Prism/actions/workflows/build_forms.yml) |
| Prism.Uno | [![build_uno](https://github.com/PrismLibrary/Prism/actions/workflows/build_uno.yml/badge.svg)](https://github.com/PrismLibrary/Prism/actions/workflows/build_uno.yml) |
| Prism.Maui | [![build_maui](https://github.com/PrismLibrary/Prism/actions/workflows/build_maui.yml/badge.svg)](https://github.com/PrismLibrary/Prism/actions/workflows/build_maui.yml) |

## Support

- Documentation is maintained in [the Prism-Documentation repo](https://github.com/PrismLibrary/Prism-Documentation) under /docs and can be found in a readable format on [the website](https://docs.prismlibrary.com/).
- StackOverflow: **NOTE** The Prism Team no longer supports or engages with questions posted on StackOverflow. Questions posted there may or may not receive correct answers.
- For general questions and support, post your questions in [GitHub Discussions](https://github.com/PrismLibrary/Prism/discussions).
- You can enter bugs and feature requests in our [Issues](https://github.com/PrismLibrary/Prism/issues/new/choose).
- Enterprise Support: If you are interested in Enterprise Support please email the Prism Team at <support@prismlibrary.com>

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

Official Prism releases are available on NuGet. Prism packages are also available on the private Commercial Plus Feed which will be updated with each merged PR. If you want to take advantage of a new feature as soon as it's merged into the code base, or if there is a critical bug you need fixed we invite you to purchase the Commercial Plus License to take advantage of these packages.

### Core Packages

These are the base packages for each platform, together with the Prism's Core assembly as a cross-platform PCL.

| Platform | Package | NuGet | Commercial Plus Feed |
| -------- | ------- | ------- | ----- |
| Cross Platform | [Prism.Core][CoreNuGet] | [![CoreNuGetShield]][CoreNuGet] | [![CorePrismNuGetShield]][CorePrismNuGet] |
| Cross Platform | [Prism.Events][EventsNuGet] | [![EventsNuGetShield]][EventsNuGet] | [![EventsPrismNuGetShield]][CorePrismNuGet] |
| Cross Platform | [Prism.Container.Abstractions][ContainerAbstractionsNuGet] | [![ContainerAbstractionsNuGetShield]][ContainerAbstractionsNuGet] | [![ContainerAbstractionsPrismNuGetShield]][CorePrismNuGet] |
| Cross Platform | [Prism.Container.DryIoc][ContainerDryIocNuGet] | [![ContainerDryIocNuGetShield]][ContainerDryIocNuGet] | [![ContainerDryIocPrismNuGetShield]][CorePrismNuGet] |
| Cross Platform | [Prism.Container.Unity][ContainerUnityNuGet] | [![ContainerUnityNuGetShield]][ContainerUnityNuGet] | [![ContainerUnityPrismNuGetShield]][CorePrismNuGet] |
| WPF | [Prism.Wpf][WpfNuGet] | [![WpfNuGetShield]][WpfNuGet] | [![WpfPrismNuGetShield]][WpfPrismNuGet] |
| Xamarin.Forms | [Prism.Forms][FormsNuGet] | [![FormsNuGetShield]][FormsNuGet] | [![FormsPrismNuGetShield]][FormsPrismNuGet] |
| Uno Platform and WinUI | [Prism.Uno][UnoNuGet] | [![UnoNuGetShield]][UnoNuGet] | [![UnoPrismNuGetShield]][UnoPrismNuGet] |

### Container-specific packages

Each supported IoC container has its own package assisting in the setup and usage of that container together with Prism. The assembly is named using this convention: Prism.*Container.Platform*.dll, e.g. **Prism.Unity.Wpf.dll**. Starting with version 7.0, Prism is moving to separate packages for each platform. Be sure to install the package for the Container and the Platform of your choice.

#### WPF

| Package | NuGet | Commercial Plus Feed |
|---------|-------|-------|
| [Prism.DryIoc][DryIocWpfNuGet] | [![DryIocWpfNuGetShield]][DryIocWpfNuGet] | [![DryIocWpfPrismNuGetShield]][DryIocWpfPrismNuGet] |
| [Prism.Unity][UnityWpfNuGet] | [![UnityWpfNuGetShield]][UnityWpfNuGet] | [![UnityWpfPrismNuGetShield]][UnityWpfPrismNuGet] |

#### Xamarin Forms

| Package | NuGet | Commercial Plus Feed |
|---------|-------|-------|
| [Prism.DryIoc.Forms][DryIocFormsNuGet] | [![DryIocFormsNuGetShield]][DryIocFormsNuGet] | [![DryIocFormsPrismNuGetShield]][DryIocFormsPrismNuGet] |
| [Prism.Unity.Forms][UnityFormsNuGet] | [![UnityFormsNuGetShield]][UnityFormsNuGet] | [![UnityFormsPrismNuGetShield]][UnityFormsPrismNuGet] |
| [Prism.Forms.Regions][PrismFormsRegionsNuget] | [![PrismFormsRegionsNuGetShield]][PrismFormsRegionsNuGet] | [![PrismFormsRegionsPrismNuGetShield]][PrismFormsRegionsPrismNuGet] |

#### Uno Platform

| Package | NuGet | Commercial Plus Feed |
|---------|-------|-------|
| [Prism.DryIoc.Uno][DryIocUnoPlatformNuGet] | [![DryIocUnoPlatformNuGetShield]][DryIocUnoPlatformNuGet] | [![DryIocUnoPlatformPrismNuGetShield]][DryIocUnoPlatformPrismNuGet] |

![NuGet package tree](images/NuGetPackageTree.png)

A detailed overview of each assembly per package is available [here](http://prismlibrary.github.io/docs/getting-started/NuGet-Packages.html).

## Samples

For stable samples be sure to check out the samples repo for the platform you are most interested in.

- [Prism for WPF Samples](https://github.com/PrismLibrary/Prism-Samples-Wpf)
- [Prism for Xamarim.Forms](https://github.com/PrismLibrary/Prism-Samples-Forms)
- [Prism for Uno Platform](#) (Coming soon)
- [Prism for .NET MAUI](#) (Coming soon)

## Contributing

We strongly encourage you to get involved and help us evolve the code base.

- You can see what our expectations are for pull requests [here](https://github.com/PrismLibrary/Prism/blob/master/.github/CONTRIBUTING.md).

[CoreNuGet]: https://www.nuget.org/packages/Prism.Core/
[EventsNuGet]: https://www.nuget.org/packages/Prism.Events/
[ContainerAbstractionsNuGet]: https://www.nuget.org/packages/Prism.Container.Abstractions/
[ContainerDryIocNuGet]: https://www.nuget.org/packages/Prism.Container.DryIoc/
[ContainerUnityNuGet]: https://www.nuget.org/packages/Prism.Container.Unity/
[WpfNuGet]: https://www.nuget.org/packages/Prism.Wpf/
[FormsNuGet]: https://www.nuget.org/packages/Prism.Forms/
[UnoNuGet]: https://www.nuget.org/packages/Prism.Uno.WinUI/

[PrismFormsRegionsNuGet]: https://www.nuget.org/packages/Prism.Forms.Regions/
[PrismFormsRegionsPrismNuGet]: #
[PrismFormsRegionsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Forms.Regions.svg
[PrismFormsRegionsPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Forms.Regions/vpre

[DryIocWpfNuGet]: https://www.nuget.org/packages/Prism.DryIoc/
[UnityWpfNuGet]: https://www.nuget.org/packages/Prism.Unity/

[UnityFormsNuGet]: https://www.nuget.org/packages/Prism.Unity.Forms/
[DryIocFormsNuGet]: https://www.nuget.org/packages/Prism.DryIoc.Forms/

[DryIocUnoPlatformNuGet]: https://www.nuget.org/packages/Prism.DryIoc.Uno.WinUI/

[CoreNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Core.svg
[EventsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Events.svg
[ContainerAbstractionsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Container.Abstractions.svg
[ContainerDryIocNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Container.DryIoc.svg
[ContainerUnityNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Container.Unity.svg
[WpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Wpf.svg
[FormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Forms.svg
[UnoNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Uno.WinUI.svg

[DryIocWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.svg
[UnityWpfNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.svg

[DryIocFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.Forms.svg
[UnityFormsNuGetShield]: https://img.shields.io/nuget/vpre/Prism.Unity.Forms.svg

[DryIocUnoPlatformNuGetShield]: https://img.shields.io/nuget/vpre/Prism.DryIoc.Uno.WinUI.svg

[CorePrismNuGet]: #
[WpfPrismNuGet]: #
[FormsPrismNuGet]: #
[UnoPrismNuGet]: #

[DryIocWpfPrismNuGet]: #
[UnityWpfPrismNuGet]: #

[UnityFormsPrismNuGet]: #
[DryIocFormsPrismNuGet]: #

[DryIocUnoPlatformPrismNuGet]: #

[CorePrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Core/vpre
[EventsPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Events/vpre
[ContainerAbstractionsPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Container.Abstractions/vpre
[ContainerDryIocPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Container.DryIoc/vpre
[ContainerUnityPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Container.Unity/vpre
[WpfPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Wpf/vpre
[FormsPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Forms/vpre
[UnoPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Uno.WinUI/vpre

[DryIocWpfPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.DryIoc/vpre
[UnityWpfPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Unity/vpre

[DryIocFormsPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.DryIoc.Forms/vpre
[UnityFormsPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.Unity.Forms/vpre

[DryIocUnoPlatformPrismNuGetShield]: https://nuget.prismlibrary.com/shield/Prism.DryIoc.Uno.WinUI/vpre

[TwitterLogo]: https://dansiegelgithubsponsors.blob.core.windows.net/images/twitter.png
[TwitchLogo]: https://dansiegelgithubsponsors.blob.core.windows.net/images/twitch.png
[YouTubeLogo]: https://dansiegelgithubsponsors.blob.core.windows.net/images/youtube.png
[OctoSponsor]: https://dansiegelgithubsponsors.blob.core.windows.net/images/octosponsor.png
