1.  **\#!155CharTopicSummary!\#:**

    Learn how to use the Prism library for WPF to create a solution, define and load a module and show a view in the shell windows.

    1.  In this lab and associated sample, you will learn the basic concepts of modular application development using the Prism Library, and apply them to create a solution that you can use as the starting point for building a composite Windows Presentation Foundation (WPF) application. After completing this lab, you will be able to do the following:

<!-- -->

1.  You will create a new solution based on the Prism Library.

    You will create and load a module.

    You will create a view and show it in the shell window.

    1.  

System Requirements
===================

1.  This guidance was designed to run on the Microsoft Windows 8, Windows 7, Windows Vista, Windows Server 2012, or Windows Server 2008 operating system. WPF applications built using this guidance require the .NET Framework 4.5.

    Before you can use the Prism Library, the following must be installed:

-   Microsoft Visual Studio 2013 Professional, Premium, or Ultimate editions

-   Microsoft .NET Framework 4.5 (installed with Visual Studio 2013)

-   Optional tool:

    -   [Microsoft Blend for Visual Studio 2013](http://www.microsoft.com/expression/products/Blend_Overview.aspx)

    1.  

Preparation
===========

1.  This topic requires you to have the following Prism Library and Unity Application Block (Unity) assemblies which can be downloaded from NuGet:

<!-- -->

1.  [Prism](http://aka.ms/prism-wpf-Prism50Nuget)

    [Prism.UnityExtensions](http://aka.ms/prism-wpf-Prism50UnityExtensionsNuget)

    1.  

    <!-- -->

    1.  **Note:** This hands-on lab uses the Unity container, but you can also use the Managed Extensibility Framework (MEF) with the Prism Library.

<!-- -->

1.  This hands-on lab assumes that you understand Prism basic concepts. For more information, see in the .

Procedures
==========

1.  This lab includes the following tasks:

<!-- -->

1.  Task 1: Creating a Solution Using the Prism Library

    Task 2: Adding a Module

    Task 3: Adding a View

    1.  

    <!-- -->

    1.  The next sections describe each of these tasks.

    <!-- -->

    1.  **Note:** The instructions for this hands-on lab are based on the HelloWorld solution. To open the solution in Visual Studio, run the file Desktop only - Open QS - Hello World QuickStart.lnk.

Task 1: Creating a Solution Using the Prism Library
===================================================

1.  In this task, you will create a solution using the Prism Library. You will be able to use this solution as a starting point for your composite WPF application. The solution includes recommended practices and techniques and is the basis for the procedures in Prism. To create a solution with the Prism Library, the following tasks must be performed:

<!-- -->

1.  **Create a solution with a shell project**. In this task, you create the initial Visual Studio solution and add a WPF Application project that is the basis of solutions built using Prism Library. This project is known as the shell project.

2.  **Set up the shell window**. In this task, you set up a window, the shell window, to host different user interface (UI) components in a decoupled way.

3.  **Set up the application's bootstrapper**. In this task, you set up code that initializes the application.

4.  

<!-- -->

1.  The following procedure describes how to create a solution with a shell project. A shell project is the basis of a typical application built using the Prism Library—it is a WPF Application project that contains the application's startup code, known as the bootstrapper, and a main window where views are typically displayed (the shell window).

To create a solution with a shell project

1.  In Visual Studio, create a new WPF application. To do this, point to **New** on the **File** menu, and then click **Project**. In the **Project types** list, select **Windows** inside the **Visual C\#** node. In the **Templates** box, click **WPF Application**. Finally, set the project's name to **HelloWorld.Desktop**, specify a valid location, and then click **OK**.

<!-- -->

1.  Visual Studio will create the HelloWorld project, as shown in the following illustration. This project will be the shell project of your application.

<!-- -->

1.  ![](media/image1.png)

<!-- -->

1.  HelloWorld project

<!-- -->

1.  In the HelloWorld Project, add references to the following NuGet packages:

<!-- -->

1.  **Prism.** In the NuGet Package Manager search for Prism. This package contains the implementation of the Prism Library composition components such as modularity, logging services, communication services, and definitions for several core interfaces. It also contains the implementation of Prism Library components that target WPF applications, including regions, events, and MVVM.

    **Prism.UnityExtensions.** In the NuGet Package Manager search for Prism.UnityExtensions**.** This package includes the Microsoft.Practices.UnityExtensions assembly which contains utility classes you can reuse in applications built with the Prism Library that consume the [Unity Application Block](http://www.msdn.com/unity). For example, it contains a bootstrapper base class, the **UnityBootstrapper** class, that creates and configures a Unity container with default Prism Library services when the application starts.

    1.  

    <!-- -->

    1.  The shell window is the top-level window of an application based on the Prism Library. This window is a place to host different UI components that exposes a way for itself to be dynamically populated by others, and it may also contain common UI elements, such as menus and toolbars. The shell window sets the overall appearance of the application.

        The following procedure explains how to set up the shell window.

To set up the shell window

1.  In Solution Explorer, rename the file MainWindow.xaml to Shell.xaml.

2.  Open the code-behind file Shell.xaml.cs and rename the **MainWindow** class to **Shell** using the Visual Studio refactoring tools. To do this, right-click **MainWindow** in the class signature, point to **Refactor**, and then click **Rename**, as shown in the following illustration. In the **Rename** dialog box, type **Shell** as the new name, and then click **OK**. If the **Preview Changes – Rename** dialog box appears, click **Apply**.

<!-- -->

1.  ![](media/image2.png)

<!-- -->

1.  MainWindow renaming using Visual Studio refactoring tools

<!-- -->

1.  In XAML view, open the Shell.xaml file, and then set the following attribute values to the **Window** root element:

<!-- -->

1.  x:Class = "HelloWorld.Desktop.Shell" (this matches the code behind class's name)

    Title = "Hello World"

    1.  Your code should look like the following.

    <!-- -->

    1.  XAML

    <!-- -->

    1.  &lt;Window x:Class="HelloWorld.Desktop.Shell"

        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"

        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"

        Title="Hello World" Height="300" Width="300"&gt;

        &lt;Grid&gt;

        &lt;/Grid&gt;

        &lt;/Window&gt;

    <!-- -->

    1.  

Regions
-------

1.  The following procedure describes how to add an **ItemsControl** control to the shell window and associate a region to it. In a subsequent task, you will dynamically add a view to this region.

To add a region to the shell window

1.  In the Shell.xaml file, add the following namespace definition to the root **Window** element. You need this namespace to use an attached property for regions that is defined in the Prism Library.

<!-- -->

1.  XAML

<!-- -->

1.  <span id="OLE_LINK1" class="anchor"><span id="OLE_LINK2" class="anchor"></span></span>xmlns:prism="http://www.codeplex.com/prism"

<!-- -->

1.  Replace the **Grid** control in the shell window with an **ItemsControl** control named **MainRegion**, as shown in the following code.

<!-- -->

1.  XAML

<!-- -->

1.  &lt;Window x:Class="HelloWorld.Desktop.Shell"

    xmlns:prism="http://www.codeplex.com/prism"

    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"

    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"

    Title="Hello World" Height="300" Width="300"&gt;

    **&lt;ItemsControl Name="MainRegion"/&gt;**

    &lt;/Window&gt;

<!-- -->

1.  The following illustration shows the shell window in the Design view.

<!-- -->

1.  ![](media/image3.png)

<!-- -->

1.  Shell window with an ItemsControl control

<!-- -->

1.  In the **ItemsControl** control definition, set the attached property **prism:RegionManager.RegionName** to "MainRegion", as shown in the following code. This attached property indicates that a region named MainRegion is associated to the control.

<!-- -->

1.  XAML

<!-- -->

1.  <span id="OLE_LINK3" class="anchor"><span id="OLE_LINK4" class="anchor"></span></span>&lt;ItemsControl Name="MainRegion" prism:RegionManager.RegionName="MainRegion"/&gt;

<!-- -->

1.  **Note:** When the shell window is instantiated, WPF resolves the value of the **prism:RegionManager.RegionName** attached property and invokes a callback in the **RegionManager** class. This callback creates a region and associates it with the **ItemsControl** control.

<!-- -->

1.  

Bootstrapper
------------

1.  The following procedure explains how to set up the application's bootstrapper.

To set up the application's bootstrapper

1.  Add a new class file named Bootstrapper.cs to the HelloWorld project.

2.  Add the following **using** statements at the top of the file. You will use them to refer to elements referenced in the **UnityBootstrapper** class.

<!-- -->

1.  C\#

<!-- -->

1.  <span id="OLE_LINK5" class="anchor"><span id="OLE_LINK6" class="anchor"></span></span>using System.Windows;

    using Microsoft.Practices.Prism.Modularity;

    using Microsoft.Practices.Prism.UnityExtensions;

    using Microsoft.Practices.Unity;

<!-- -->

1.  Update the **Bootstrapper** class's signature to inherit from the **UnityBootstrapper** class.

<!-- -->

1.  C\#

<!-- -->

1.  class Bootstrapper : UnityBootstrapper

    {

    }

<!-- -->

1.  Override the **CreateShell** method in the **Bootstrapper** class. In this method, create an instance of the shell window and return it, as shown in the following code.

<!-- -->

1.  C\#

<!-- -->

1.  protected override DependencyObject CreateShell()

    {

    return new Shell();

    }

<!-- -->

1.  **Note:** You return the shell object to have the **UnityBootstrapper** base class attach an instance of the region manager service to it. The region manager service is a service included in the Prism Library that manages regions in the application. By having a region manager instance attached to the shell window, you can declaratively register regions from XAML code that will exist in the scope of the shell window and child views.

<!-- -->

1.  Override the **InitializeShell** method in the **Bootstrapper** class. In this method, display the shell to the user.

<!-- -->

1.  C\#

<!-- -->

1.  <span id="OLE_LINK7" class="anchor"><span id="OLE_LINK8" class="anchor"></span></span>protected override void InitializeShell()

    {

    base.InitializeShell();

    Application.Current.MainWindow = (Window)this.Shell;

    Application.Current.MainWindow.Show();

    }

<!-- -->

1.  Override the **ConfigureModuleCatalog** method. In this template method, you populate the module catalog with modules. The module catalog interface is **Microsoft.Practices.Prism.Modularity.IModuleCatalog**, and it contains metadata for all the modules in the application. Because the application contains no modules at this point, the implementation of the **ConfigureModuleCatalog** method should simply call the base implementation and return. You can paste the following code in your **Bootstrapper** class to implement the method.

<!-- -->

1.  C\#

<!-- -->

1.  <span id="OLE_LINK9" class="anchor"><span id="OLE_LINK10" class="anchor"></span></span>protected override void ConfigureModuleCatalog()

    {

    base.ConfigureModuleCatalog();

    }

<!-- -->

1.  More details about module loading and module catalogs are described [Task 2: Adding a Module](#task-2-adding-a-module) later in this topic.

<!-- -->

1.  Open the file App.xaml.cs and initialize the Bootstrapper in the handler for the **Startup** event of the application, as shown in the following code. By doing this, the bootstrapper code will we executed when the application starts.

<!-- -->

1.  C\#

<!-- -->

1.  public partial class App : Application

    {

    <span id="OLE_LINK11" class="anchor"><span id="OLE_LINK12" class="anchor"></span></span> protected override void OnStartup(StartupEventArgs e)

    {

    base.OnStartup(e);

    Bootstrapper bootstrapper = new Bootstrapper();

    bootstrapper.Run();

    }

    }

<!-- -->

1.  Open the App.xaml file and remove the attribute **StartupUri**. Because you are manually instantiating the shell window in your bootstrapper, this attribute is not required. The code in the App.xaml file should look like the following.

<!-- -->

1.  C\#

<!-- -->

1.  &lt;Application x:Class="HelloWorld.Desktop.App"

    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"

    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"&gt;

    &lt;Application.Resources&gt;

    &lt;/Application.Resources&gt;

    &lt;/Application&gt;

<!-- -->

1.  Build and run the application. You should see an empty Hello World window, as shown in the following illustration.

<!-- -->

1.  ![](media/image4.png)

<!-- -->

1.  Hello World window

<!-- -->

1.  

Task 2: Adding a Module
=======================

1.  In this task, you will create a module and add it to your solution. A module in Prism is a logical unit in your application. Adding a module to your solution involves the following tasks:

<!-- -->

1.  **Creating a module**. In this task, you create a module project with a module class.

2.  **Configuring how the module is loaded**. In this task, you configure your application to load the module.

3.  

<!-- -->

1.  The following procedure describes how to create a module.

To create a module

1.  Add a new class library project to your solution. To do this, right-click the **HelloWorld.Desktop** solution node in Solution Explorer, point to **Add**, and then click **New Project**. In the **Project types** list, select **Windows** in the **Visual C\#** node. In the **Templates** box, click **Class Library**. Finally, set the project's name to HelloWorldModule, and then click **OK**. The following illustration shows your solution.

<!-- -->

1.  ![](media/image5.png)

<!-- -->

1.  Solution with a module named HelloWorldModule

<!-- -->

1.  Add references in your module to the following WPF assemblies. To do this, right-click the **HelloWorldModule** project in Solution Explorer, and then click **Add Reference**. In the **Add Reference** dialog box, select the Assemblies tab, then select the following assemblies, and then click **OK**:

<!-- -->

1.  **PresentationCore.dll**

    **PresentationFramework.dll**

    **WindowsBase.dll**

    **System.Xaml.dll**

    1.  Add references in your module to the following Prism Library assemblies. To do this, right-click the **HelloWorld.Desktop** solution in Solution Explorer, and then click **Manage NuGet Packages for Solution**. Click the **Installed Packages** button, select the following assemblies, and then click **Manage**:

    **Prism**

    1.  In the **Selected Projects** dialog, select HelloWorldModule, and then click **OK**. Finally, close the Manage NuGet Packages window by clicking **Close**.

    <!-- -->

    1.  Rename the Class1.cs file to HelloWorldModule.cs. To do this, right-click the file in Solution Explorer, click **Rename**, type the new name, and then press ENTER. In the dialog box that asks if you want to perform a rename of all references to your class, click **Yes**.

    2.  Open the file HelloWorldModule.cs and add the following **using** statement at the top of the file. You will use it to refer to modularity elements provided by the Prism Library.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  <span id="OLE_LINK13" class="anchor"><span id="OLE_LINK14" class="anchor"></span></span>using Microsoft.Practices.Prism.Modularity;

    <!-- -->

    1.  Change the class signature to implement the **IModule** interface, as shown in the following code.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  public class HelloWorldModule : IModule

        {

        }

    <!-- -->

    1.  In the **HelloWorldModule** class, add an empty definition of the **Initialize** method, as shown in the following code.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  public void Initialize()

        {

        }

    <!-- -->

    1.  Add a Views folder to the HelloWorldModule project. In this folder, you will store your view implementations. To do this, right-click the **HelloWorldModule** project in Solution Explorer, point to **Add**, and then click **New Folder**. Change the folder name to Views.

    <!-- -->

    1.  This step is recommended to organize your projects; this is useful when a module contains several artifacts. The following are other common folders that you can add to your module:

    **Services**. In this folder, you store service implementations and service interfaces.

    **ViewModels**. In this folder, you store view models.

    1.  The following illustration shows the solution with the **HelloWorldModule** module.

    <!-- -->

    1.  ![](media/image6.png)

    <!-- -->

    1.  Solution with the HelloWorldModule

    <!-- -->

    1.  Build the solution.

    2.  

    <!-- -->

    1.  At this point, you have a solution based on the Prism Library with a module. However, the module is not being loaded into the application. The following section describes module loading and how you can load modules with the Prism Library.

Module in the Application Life Cycle
------------------------------------

1.  Modules go through a three-step process during application startup:

<!-- -->

1.  Modules are discovered by the module catalog. The module catalog contains a collection of metadata about those modules. This metadata can be consumed by the module manager service.

2.  The module manager service coordinates the modules initialization. It manages the retrieval and the subsequent initialization of the modules. It loads modules—retrieving them if necessary—and validates them.

3.  Finally, the module manager instantiates the module and calls the module's **Initialize** method.

4.  

### Populating the Module Catalog

1.  The Prism Library provides several ways to populate the module catalog. In WPF, you can populate the module catalog from code, from a XAML file, from a configuration file, or from a directory. The following procedure explains how to populate the catalog from code to load the HelloWorldModule module into the HelloWorld.Desktop application.

To populate the module catalog with the HelloWorld module from code

1.  In your shell project, add a reference to the module project. To do this in Solution Explorer, right-click the **HelloWorld.Desktop** project, and then click **Add Reference**. In the **Reference Manager** dialog box, click the **Solution** tab, select the **HelloWorldModule** project, and then click **OK**.

2.  Open the Bootstrapper.cs file and explore the **ConfigureModuleCatalog** method. The method implementation is shown in the following code.

<!-- -->

1.  C\#

<!-- -->

1.  protected override void ConfigureModuleCatalog()

    {

    base.ConfigureModuleCatalog();

    }

<!-- -->

1.  The **ModuleCatalog** class is used to define the application's modules from code—it implements the methods included in the **IModuleCatalog** interface and adds an **AddModule** method for developers to manually register modules that should be loaded in the application. The signature of this method is shown in the following code.

<!-- -->

1.  C\#

<!-- -->

1.  public ModuleCatalog AddModule(Type moduleType, InitializationMode initializationMode, params string\[\] dependsOn)

<!-- -->

1.  The **AddModule** method returns the same module catalog instance and takes the following parameters:

<!-- -->

1.  **The module initializer class's type of module to load**. This type must implement the **IModule** interface.

    **The Initialization mode**. This parameter indicates how the module will be initialized. The possible values are **InitializationMode.WhenAvailable** and **InitializationMode.OnDemand**.

    **An array containing the names of the modules that the module depends on, if any**. These modules will be loaded before your module to ensure your module dependencies are available when it is loaded.

    1.  Update the **ConfigureModuleCatalog** method to register the HelloWorldModule module with the module catalog instance. To do this, you can replace the **ConfigureModuleCatalog** implementation with the following code.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  protected override void ConfigureModuleCatalog()

        {

        base.ConfigureModuleCatalog();

        <span id="OLE_LINK15" class="anchor"><span id="OLE_LINK16" class="anchor"></span></span> ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;

        moduleCatalog.AddModule(typeof(HelloWorldModule.HelloWorldModule));

        }

    <!-- -->

    1.  **Note:** In this example, the modules are directly referenced by the shell. That is why this example is able to use **typeof(Module)** to add modules to the catalog. But keep in mind that modules whose type is not already available can also be added to the catalog.

        The **WhenAvailable** initialization mode is the default value if no initialization mode is specified.

    <!-- -->

    1.  Build and run the solution. To verify that the HelloWorldModule module gets initialized, add a breakpoint to the **Initialize** method of the **HelloWorldModule** class. The breakpoint should be hit when the application starts.

    2.  

Task 3: Adding a View
=====================

1.  In this task, you will create and add a view to the **HelloWorldModule** module. Views are objects that contain visual content. Views are often user controls, but they do not have to be user controls. Adding a view to your module involves the following tasks:

<!-- -->

1.  **Creating the view**. In this task, you implement the view by creating the visual content and writing code to manage the UI elements in the view.

2.  **Showing the view in a region**. In this task, you obtain a reference to a region and add the view to it.

3.  

<!-- -->

1.  The following procedure describes how to create a view.

<!-- -->

1.  To create a view

    1.  Add a new WPF user control to your module. To do this, right-click the Views folder in Solution Explorer, point to **Add**, and then click **New Item**. In the **Add New Item** dialog box, select the User Control (WPF) template, set the name to **HelloWorldView.xaml**, and then click **Add**.

    2.  Add a "Hello World" text block to the view. To do this, you can replace your code in the file HelloWorldView.xaml with the following code.

    <!-- -->

    1.  XAML

    <!-- -->

    1.  <span id="OLE_LINK17" class="anchor"><span id="OLE_LINK18" class="anchor"></span></span>&lt;UserControl x:Class="HelloWorldModule.Views.HelloWorldView"

        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"

        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"&gt;

        &lt;Grid&gt;

        &lt;TextBlock Text="Hello World" Foreground="Green" HorizontalAlignment="Center" VerticalAlignment="Center" FontFamily="Calibri" FontSize="24" FontWeight="Bold"&gt;&lt;/TextBlock&gt;

        &lt;/Grid&gt;

        &lt;/UserControl&gt;

    <!-- -->

    1.  Save the file.

    2.  

Region Manager
--------------

1.  The region manager service is responsible for maintaining a collection of regions and creating new regions for controls. This service implements the **Microsoft.Practices.Prism.Regions.IRegionManager** interface. Typically, you interact directly with this service to locate regions in a decoupled way through their name and add views those regions. By default, the **UnityBootstrapper** base class registers an instance of this service in the application container. This means that you can obtain a reference to the region manager service in the HelloWorld application by using dependency injection.

    The following procedure explains how to obtain an instance of the region manager and add the HelloWorldView view to the shell's main region.

To show the view in the shell

1.  Open the HelloWorldModule.cs file.

2.  Add the following **using** statement to the top of the file. You will use it to refer to the region elements in the Prism Library.

<!-- -->

1.  C\#

<!-- -->

1.  using Microsoft.Practices.Prism.Regions;

<!-- -->

1.  Create a private read-only instance variable to hold a reference to the region manager. To do this, paste the following code inside the class body.

<!-- -->

1.  C\#

<!-- -->

1.  <span id="OLE_LINK19" class="anchor"><span id="OLE_LINK20" class="anchor"></span></span>private readonly IRegionManager regionManager;

<!-- -->

1.  Add the constructor of the **HelloWorldModule** class to obtain a region manager instance through constructor dependency injection and store it in the **regionManager** instance variable. To do this, the constructor has to take a parameter of type **Microsoft.Practices.Prism.Regions.IRegionManager**. You can paste the following code inside the class body to implement the constructor.

<!-- -->

1.  C\#

<!-- -->

1.  public HelloWorldModule(IRegionManager regionManager)

    {

    this.regionManager = regionManager;

    }

<!-- -->

1.  In the **Initialize** method, invoke the **RegisterViewWithRegion** method on the **RegionManager** instance. This method registers a region name with its associated view type in the region view registry; the registry is responsible for registering and retrieving of these mappings.

<!-- -->

1.  The **RegisterViewWithRegion** method has two overloads. When you want to register a view directly, you use the first overload that requires two parameters, the region name and the type of the view. This is shown in the following code.

<!-- -->

1.  C\#

<!-- -->

1.  public void Initialize()

    {

    <span id="OLE_LINK21" class="anchor"><span id="OLE_LINK22" class="anchor"></span></span> regionManager.RegisterViewWithRegion("MainRegion", typeof(Views.HelloWorldView));

    }

<!-- -->

1.  The UI composition approach used in the preceding code is known as view discovery. When using this approach, you specify the views and the region where the views will be loaded. When a region is created, it asks for its associated views and automatically loads them.

<!-- -->

1.  **Note:** The region's name must match the name defined in the **RegionName** attribute of the region.

<!-- -->

1.  Build and run the application. You should see the Hello World window with a "Hello World" message, as shown in the following illustration.

<!-- -->

1.  ![](media/image7.png)

<!-- -->

1.  Hello World message

<!-- -->

1.  

<!-- -->

1.  **Note:** To open the solution that results from performing the steps in this Hands-on Lab in Visual Studio, run the file Desktop only - Open QS - Hello World QuickStart.lnk.

More Information
================

1.  To learn about code samples included with Prism, see the following topics:

-   -   -   -   

1.  1.  


