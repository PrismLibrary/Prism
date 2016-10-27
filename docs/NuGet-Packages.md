#NuGet Packages
### Core Packages

These are the base packages for each platform, together with the Prism's Core assembly as a cross-platform PCL.

| Platform | Assembly | Package | Version |
| -------- | -------- | ------- | ------- |
| PCL | Prism.dll | [Prism.Core][1] | [![21]][1] |
| WPF | Prism.Wpf.dll | [Prism.Wpf][2] | [![22]][2] |
| Xamarin.Forms | Prism.Forms.dll | [Prism.Forms][3] | [![23]][3] |
| Windows 10 UWP | Prism.Windows.dll | [Prism.Windows][4] | [![24]][4] |

### Container-specific packages

Each supported IoC container has its own package assisting in the setup and usage of that container together with Prism. The assembly is named using this convention: Prism.*Container.Platform*.dll, e.g. **Prism.Unity.Wpf.dll**. 

Following matrix shows the platform specific support currently available.

| Package                | Version    | WPF | Win10 UWP | Xamarin.Forms |
|------------------------|------------|:---:|:---:|:---:|
| [Prism.Autofac][7] <sup>(*)</sup>  | [![27]][7] |  X  |  X  |  &darr;  |
| [Prism.Autofac.Forms][12]   | [![32]][12] |  -  |  -  |  X  |
| [Prism.DryIoc.Forms][13]   | [![33]][13] |  -  |  -  |  X  |
| [Prism.Mef][6]  <sup>(**)</sup> | [![26]][6] |  X  | - | - |
| [Prism.Ninject][9] <sup>(*)</sup>   | [![29]][9] |  X  |     |  &darr;  |
| [Prism.Ninject.Forms][11]| [![31]][11]|  -  |  -  |  X  |
| [Prism.StructureMap][8]| [![28]][8] |  X  |     |     |
| [Prism.Unity][5] <sup>(*)</sup>  | [![25]][5] |  X  |  X  |  &darr;  |
| [Prism.Unity.Forms][10]| [![30]][10]|  -  |  -  |  X  |


<sup>(*)</sup> As Xamarin Forms also supports UWP now, adding Prism.Unity, Prism.Ninject, or Prism.Autofac puts in some incorrect dependencies. Therefore we created a new package for Xamarin Forms projects. 

<sup>(**)</sup> MEF is supported with WPF for compatibility with previous versions. It will not be added to Windows 10 UWP or Xamarin Forms.

Note that adding the container-specific package to your project, will also pull in the correct platform-specific package and the core PCL library. E.g. when you'd like to use Unity in a WPF project, add the Prism.Unity package and the rest will be pulled in as well.

![NuGet package tree](images/NuGetPackageTree.png)

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

[21]: https://img.shields.io/nuget/vpre/Prism.Core.svg
[22]: https://img.shields.io/nuget/vpre/Prism.Wpf.svg
[23]: https://img.shields.io/nuget/vpre/Prism.Forms.svg
[24]: https://img.shields.io/nuget/vpre/Prism.Windows.svg
[25]: https://img.shields.io/nuget/vpre/Prism.Unity.svg
[26]: https://img.shields.io/nuget/vpre/Prism.Mef.svg
[27]: https://img.shields.io/nuget/vpre/Prism.Autofac.svg
[28]: https://img.shields.io/nuget/vpre/Prism.StructureMap.svg
[29]: https://img.shields.io/nuget/vpre/Prism.Ninject.svg
[30]: https://img.shields.io/nuget/vpre/Prism.Unity.Forms.svg
[31]: https://img.shields.io/nuget/vpre/Prism.Ninject.Forms.svg
[32]: https://img.shields.io/nuget/vpre/Prism.Autofac.Forms.svg
[33]: https://img.shields.io/nuget/vpre/Prism.DryIoc.Forms.svg
