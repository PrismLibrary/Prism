#Prism Library for WPF 

The Prism Library helps architects and developers create composite applications for Windows Presentation Foundation (WPF) using the Model-View-ViewModel pattern. The Prism Library can support those wanting to build a number of application styles with WPF, but it is was primarily constructed for applications composed of discrete, functionally complete pieces that work together to create a single, integrated user interface (UI), often referred to as a composite application. The Prism Library accelerates the development of applications using proven design patterns. 

The Prism Library is primarily designed to help architects and developers create applications that need to accomplish the following:

 * Build clients composed of independent, yet cooperating, modules or pieces.
 * Separate the concerns of module builders from the concerns of the shell developer; by doing this, business units can concentrate on developing domain-specific modules instead of the WPF architecture.
 * Separate the concerns of presentation, presentation logic, and application model through support for presentation model patterns such as Model-View-ViewModel (MVVM).
 * Use an architectural infrastructure to produce a consistent and high quality integrated application.

When building your application with the Prism Library, you may use the Unity Extensions for the Prism Library and the Unity Application Block (Unity) or the Managed Extensibility Framework (MEF) Extensions for the Prism Library and MEF. These are built on the .NET Framework 4.5 for WPF, as shown in the following illustration.

![Positioning of Unity and MEF on top of .NET Framework](images/Ch13LibraryFig1.png)

The Prism Library addresses common requirements for building both composite and non-composite applications on the WPF platform. As a whole, the Prism Library accelerates development by providing the services and components to address these needs.

The Prism Library ships signed binaries through NuGet packages to allow you to take advantage of Prism immediately without the need to compile and as source in case you want to make modifications or just see how it works.

## Add Reference using NuGet and Accessing the Library Source Code

Add references to the Prism binaries in your code by searching NuGet for Prism. See the list of current [Nuget packages](https://github.com/PrismLibrary/Prism/blob/master/Documentation/DownloadandSetupPrism.md#nuget-packages).

## Organization of the Prism Library

* **Prism**. This assembly contains the core functionality of Prism that is shared across all supported platforms. This includes:
 * MVVM classes such as **BindableBase**, **PropertySupport**, **ViewModelLocationProvider**
 * Commanding which includes the **DelegateCommand** and **CompositeCommand.**.
 * Interfaces and components to help send loosely coupled messages between modules. The components include the **PubSubEvents and EventAggregator**.
 
* **Prism.Wpf**. This assembly contains interfaces and components to help build composite applications. These components include the **ModuleManager**, **ModuleCatalog**, and **Bootstrapper**. Additionally, this assembly contains the **RegionManager** component that helps compose the user interface from multiple parts.  It also contains behaviors and actions for interactions with the UI based on Blend for Visual Studio 2013 Behaviors (available in the Blend SDK), largely in support of the MVVM pattern. This includes **InteractionRequest**, **InteractionRequestTrigger**, **Confirmation**, and **Notification**. Additionally the PopupWindowAction responds to the **InteractionRequestTrigger**.

* **Prism.Unity**. This assembly provides components to use the Unity Application Block (Unity) with the Prism Library. These components include **UnityBootstrapper** and **UnityServiceLocatorAdapter**.

* **Prism.Mef**. This assembly provides components to use Managed Extensibility Framework (MEF) with the Prism Library. These components include **MefBootstrapper** and **MefServiceLocatorAdapter**.
 
* **Prism.Autofac**. This assembly provides components to use the Autofac dependency injection container with the Prism Library. These components include **AutofacBootstrapper** and **AutofacServiceLocatorAdapter**.
  
* **Prism.StructureMap**. This assembly provides components to use the StructureMap dependency injection container with the Prism Library. These components include **StructureMapBootstrapper** and **StructureMapServiceLocatorAdapter**.
   
* **Prism.Ninject**. This assembly provides components to use the Ninject dependency injection container with the Prism Library. These components include **NinjectBootstrapper** and **NinjectServiceLocatorAdapter**.

## The Prism Library Source

The source for Prism, Prism.Wpf, Prism.Unity, Prism.Mef, Prism.Autofac, Prism.StructureMap, and Prism.Ninject assemblies can be found in the PrismLibrary folder where the Prism repo has been forked. These assemblies target WPF applications.

## Modifying the Library

If you want to modify the Prism Library, you can replace the NuGet referenced assemblies with your own version of the binaries.

## Running the Tests

If you modify the Prism Library and want to verify that existing functionality is not broken, execute the unit tests for the projects. To run all the desktop unit tests in the solution file PrismLibrary.sln, on the **Test** menu, point to **Run**, and then click **All Tests in Solution**.

## More Information

Prism's community sites are:

 * Prism: <https://github.com/PrismLibrary/Prism>.
 * Issues: <https://github.com/PrismLibrary/Prism/issues>.
 * Support: <http://stackoverflow.com/questions/tagged/prism>.

For more information about Unity, see the following:

 * "Unity Application Block" on MSDN: <http://www.msdn.com/unity>.
 * Unity community site on CodePlex: <http://www.codeplex.com/unity>.

For more information about MEF, see the following:

 * "[Managed Extensibility Framework Overview](http://msdn.microsoft.com/en-us/library/dd460648.aspx)" on MSDN.
 * MEF community site on CodePlex: <http://mef.codeplex.com/>.

For more information about service locator, see the Common Service Locator on CodePlex: <http://commonservicelocator.codeplex.com/>.


