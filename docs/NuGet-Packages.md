# NuGet Packages

## Core Packages

These are the base packages for each platform, together with the Prism's Core assembly as a cross-platform PCL.

| Platform | Assembly | Package | Version |
| -------- | -------- | ------- | ------- |
| PCL | Prism.dll | [Prism.Core][1] | [![Prism.Core badge](https://img.shields.io/nuget/vpre/Prism.Core.svg)][1] |
| WPF | Prism.Wpf.dll | [Prism.Wpf][2] | [![Prism.WPF badge](https://img.shields.io/nuget/vpre/Prism.Wpf.svg)][2] |
| Xamarin.Forms | Prism.Forms.dll | [Prism.Forms][3] | [![Prism.Forms badge](https://img.shields.io/nuget/vpre/Prism.Forms.svg)][3] |
| Windows 10 UWP | Prism.Windows.dll | [Prism.Windows][4] | [![Prism.Windows badge](https://img.shields.io/nuget/vpre/Prism.Windows.svg)][4] |

### Container-specific packages

Each supported IoC container has its own package assisting in the setup and usage of that container together with Prism. The assembly is named using this convention: Prism.*Container.Platform*.dll, e.g. **Prism.Unity.Wpf.dll**. 

Following matrix shows the platform specific support currently available.

| Package                | Version    | WPF | Win10 UWP | Xamarin.Forms |
|------------------------|------------|:---:|:---:|:---:|
| [Prism.Autofac][7] <sup>(*)</sup>  | [![Prism.Autofac badge](https://img.shields.io/nuget/vpre/Prism.Autofac.svg)][7] |  X  |  X  |  &darr;  |
| [Prism.Autofac.Forms][12]   | [![Prism.Autofac.Forms badge](https://img.shields.io/nuget/vpre/Prism.Autofac.Forms.svg)][12] |  -  |  -  |  X  |
| [Prism.DryIoc][14]   | [![Prism.DryIoc badge](https://img.shields.io/nuget/vpre/Prism.DryIoc.svg)][14] |  X  |  -  |  -  |
| [Prism.DryIoc.Forms][13]   | [![Prism.DryIoc.Forms badge](https://img.shields.io/nuget/vpre/Prism.DryIoc.Forms.svg)][13] |  -  |  -  |  X  |
| [Prism.Mef][6]  <sup>(**)</sup> | [![Prism.Mef badge](https://img.shields.io/nuget/vpre/Prism.Mef.svg)][6] |  X  | - | - |
| [Prism.Ninject][9] <sup>(*)</sup>   | [![Prism.Ninject badge](https://img.shields.io/nuget/vpre/Prism.Ninject.svg)][9] |  X  |     |  &darr;  |
| [Prism.Ninject.Forms][11]| [![Prism.Ninject.Forms badge](https://img.shields.io/nuget/vpre/Prism.Ninject.Forms.svg)][11]|  -  |  -  |  X  |
| [Prism.StructureMap][8]| [![Prism.StructureMap badge](https://img.shields.io/nuget/vpre/Prism.StructureMap.svg)][8] |  X  |     |     |
| [Prism.Unity][5] <sup>(*)</sup>  | [![Prism.Unity badge](https://img.shields.io/nuget/vpre/Prism.Unity.svg)][5] |  X  |  X  |  &darr;  |
| [Prism.Unity.Forms][10]| [![Prism.Unity.Forms badge](https://img.shields.io/nuget/vpre/Prism.Unity.Forms.svg)][10]|  -  |  -  |  X  |


<sup>(*)</sup> As Xamarin Forms also supports UWP now, adding Prism.Unity, Prism.Ninject, or Prism.Autofac puts in some incorrect dependencies. Therefore we created a new package for Xamarin Forms projects. 

<sup>(**)</sup> MEF is supported with WPF for compatibility with previous versions. It will not be added to Windows 10 UWP or Xamarin Forms.

Note that adding the container-specific package to your project, will also pull in the correct platform-specific package and the core PCL library. E.g. when you'd like to use Unity in a WPF project, add the Prism.Unity package and the rest will be pulled in as well.

![NuGet package tree](images/NuGetPackageTree.png)

### Overview of assemblies

To recapitulate the packages described above, this is the list of all assemblies added to your solution by Prism 6 depending on the container and platform used.

#### Prism PCL

| Assembly | Package |
| -------- | ------- |
| Prism.dll | [Prism.Core][1] |

#### WPF

| Assembly | Package |
| -------- | ------- |
| Prism.Wpf.dll | [Prism.Wpf][2] |
| Prism.Unity.Wpf.dll | [Prism.Unity][5] |
| Prism.Mef.Wpf.dll | [Prism.Mef][6] |
| Prism.Autofac.Wpf.dll | [Prism.Autofac][7] |
| Prism.StructureMap.Wpf.dll | [Prism.StructureMap][8] |
| Prism.Ninject.Wpf.dll | [Prism.Ninject][9] |
| Prism.DryIoc.Wpf.dll | [Prism.DryIoc][14] |

#### Xamarin.Forms

| Assembly | Package |
| -------- | ------- |
| Prism.Forms.dll | [Prism.Forms][3] |
| Prism.Unity.Forms.dll | [Prism.Unity.Forms][10] |
| Prism.Ninject.Forms.dll | [Prism.Ninject.Forms][11] |
| Prism.Autofac.Forms.dll | [Prism.Autofac.Forms][11] |
| Prism.DryIoc.Forms.dll | [Prism.DryIoc.Forms][11] |

#### Universal Windows Platform

| Assembly | Package |
| -------- | ------- |
| Prism.Windows.dll | [Prism.Windows][4] |
| Prism.Unity.Windows.dll | [Prism.Unity][5] |
| Prism.Autofac.Windows.dll | [Prism.Autofac][7] |


[1]: https://www.nuget.org/packages/Prism.Core/
[2]: https://www.nuget.org/packages/Prism.Wpf/
[3]: https://www.nuget.org/packages/Prism.Forms/
[4]: https://www.nuget.org/packages/Prism.Windows/
[5]: https://www.nuget.org/packages/Prism.Unity/
[6]: https://www.nuget.org/packages/Prism.Mef/
[7]: https://www.nuget.org/packages/Prism.Autofac/
[8]: https://www.nuget.org/packages/Prism.StructureMap/
[9]: https://www.nuget.org/packages/Prism.Ninject/
[10]: https://www.nuget.org/packages/Prism.Unity.Forms/
[11]: https://www.nuget.org/packages/Prism.Ninject.Forms/
[12]: https://www.nuget.org/packages/Prism.Autofac.Forms/
[13]: https://www.nuget.org/packages/Prism.DryIoc.Forms/
[14]: https://www.nuget.org/packages/Prism.DryIoc/
