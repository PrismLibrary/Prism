1.  **\#!155CharTopicSummary!\#:**

    Learn how to get a Prism for Window Presentation Foundation (WPF) application up and running – bootstrapping the application.

What Is a Bootstrapper?
=======================

1.  A bootstrapper is a class that is responsible for the initialization of an application built using the Prism Library. By using a bootstrapper, you have more control of how the Prism Library components are wired up to your application.

<!-- -->

1.  ![](media/image1.png)

<!-- -->

1.  Basic stages of the bootstrapping process

<!-- -->

1.  The Prism Library provides some additional base classes, derived from **Bootstrapper**, that have default implementations that are appropriate for most applications. The only stages left for your application bootstrapper to implement are creating and initializing the shell.

Dependency Injection
--------------------

Creating the Shell
------------------

1.  In a traditional Windows Presentation Foundation (WPF) application, a startup Uniform Resource Identifier (URI) is specified in the App.xaml file that launches the main window.

    In an application created with the Prism Library, it is the bootstrapper's responsibility to create the shell or the main window. This is because the shell relies on services, such as the Region Manager, that need to be registered before the shell can be displayed.

Key Decisions
=============

1.  After you decide to use the Prism Library in your application, there are a number of additional decisions that need to be made:

<!-- -->

1.  You will need to decide whether you are using MEF, Unity, or another container for your dependency injection container. This will determine which provided bootstrapper class you should use and whether you need to create a bootstrapper for another container.

    You should think about the application-specific services you want in your application. These will need to be registered with the container.

    Determine whether the built-in logging service is adequate for your needs or if you need to create another logging service.

    Determine how modules will be discovered by the application: via explicit code declarations, code attributes on the modules discovered via directory scanning, configuration, or XAML.

    1.  

    <!-- -->

    1.  The rest of this topic provides more details.

Core Scenarios
==============

1.  Creating a startup sequence is an important part of building your Prism application. This section describes how to create a bootstrapper and customize it to create the shell, configure the dependency injection container, register application level services, and how to load and initialize the modules.

Creating a Bootstrapper for Your Application
--------------------------------------------

1.  If you choose to use either Unity or MEF as your dependency injection container, creating a simple bootstrapper for your application is easy. You will need to create a new class that derives from either **MefBootstrapper** or **UnityBootstrapper**. Then, implement the **CreateShell** method. Optionally, you may override the **InitializeShell** method for shell specific initialization.

### Implementing the CreateShell Method

1.  The **CreateShell** method allows a developer to specify the top-level window for a Prism application. The shell is usually the **MainWindow** or **MainPage**. Implement this method by returning an instance of your application's shell class. In a Prism application, you can create the shell object, or resolve it from the container, depending on your application's requirements.

    An example of using the **ServiceLocator** to resolve the shell object is shown in the following code example.

<!-- -->

1.  C\#

<!-- -->

1.  protected override DependencyObject CreateShell()

    {

    return ServiceLocator.Current.GetInstance&lt;Shell&gt;();

    }

<!-- -->

1.  **Note:** You will often see the **ServiceLocator** being used to resolve instances of types instead of the specific dependency injection container. The **ServiceLocator** is implemented by calling the container, so it makes a good choice for container agnostic code. You can also directly reference and use the container instead of the **ServiceLocator**.

### Implementing the InitializeShell Method

1.  C\#

<!-- -->

1.  protected override void InitializeShell()

    {

    Application.Current.MainWindow = Shell;

    Application.Current.MainWindow.Show();

    }

Creating and Configuring the Module Catalog
-------------------------------------------

1.  If you are building a module application, you will need to create and configure a module catalog. Prism uses a concrete **IModuleCatalog** instance to keep track of what modules are available to the application, which modules may need to be downloaded, and where the modules reside.

    The **Bootstrapper** provides a protected **ModuleCatalog** property to reference the catalog as well as a base implementation of the virtual **CreateModuleCatalog** method. The base implementation returns a new **ModuleCatalog**; however, this method can be overridden to provide a different **IModuleCatalog** instance instead, as shown in the following code from the **QuickStartBootstrapper** in the Modularity with MEF for WPF QuickStart.

<!-- -->

1.  C\#

<!-- -->

1.  protected override IModuleCatalog CreateModuleCatalog()

    {

    // When using MEF, the existing Prism ModuleCatalog is still

    // the place to configure modules via configuration files.

    return new ConfigurationModuleCatalog()

    }

Creating and Configuring the Container
--------------------------------------

1.  Containers play a key role in an application created with the Prism Library. Both the Prism Library and the applications built on top of it depend on a container for injecting required dependencies and services. During the container configuration phase, several core services are registered. In addition to these core services, you may have application-specific services that provide additional functionality as it relates to composition.

### Core Services

1.  The following table lists the core non-application specific services in the Prism Library.

| Service interface      | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
|------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **IModuleManager**     | Defines the interface for the service that will retrieve and initialize the application's modules.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| **IModuleCatalog**     | Contains the metadata about the modules in the application. The Prism Library provides several different catalogs.                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| **IModuleInitializer** | Initializes the modules.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| **IRegionManager**     | Registers and retrieves regions, which are visual containers for layout.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| **IEventAggregator**   | A collection of events that is loosely coupled between the publisher and the subscriber.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| **ILoggerFacade**      | A wrapper for a logging mechanism, so you can choose your own logging mechanism. The Stock Trader Reference Implementation (Stock Trader RI) uses the Enterprise Library Logging Application Block, via the **EnterpriseLibraryLoggerAdapter** class, as an example of how you can use your own logger. The logging service is registered with the container by the bootstrapper's **Run** method, using the value returned by the **CreateLogger** method. Registering another logger with the container will not work; instead override the **CreateLogger** method on the bootstrapper. |
| **IServiceLocator**    | Allows the Prism Library to access the container. If you want to customize or extend the library, this may be useful.                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |

### Application-Specific Services

1.  The following table lists the application-specific services used in the Stock Trader RI. This can be used as an example to understand the types of services your application may provide.

| Services in the Stock Trader RI | Description                                                                                                                                                 |
|---------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **IMarketFeedService**          | Provides real-time (mocked) market data. The **PositionSummaryViewModel** updates the position screen based on notifications it receives from this service. |
| **IMarketHistoryService**       | Provides historical market data used for displaying the trend line for the selected fund.                                                                   |
| **IAccountPositionService**     | Provides the list of funds in the portfolio.                                                                                                                |
| **IOrdersService**              | Persists submitted buy/sell orders.                                                                                                                         |
| **INewsFeedService**            | Provides a list of news items for the selected fund.                                                                                                        |
| **IWatchListService**           | Handles when new watch items are added to the watch list.                                                                                                   |

### Creating and Configuring the Container in the UnityBootstrapper

1.  **Note:** An example of this is when a module registers module-level services in its **Initialize** method.

<!-- -->

1.  C\#

<!-- -->

1.  // UnityBootstrapper.cs

    protected virtual void ConfigureContainer()

    {

    ...

    if (useDefaultConfiguration)

    {

    RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);

    RegisterTypeIfMissing(typeof(IModuleInitializer), typeof(ModuleInitializer), true);

    RegisterTypeIfMissing(typeof(IModuleManager), typeof(ModuleManager), true);

    RegisterTypeIfMissing(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings), true);

    RegisterTypeIfMissing(typeof(IRegionManager), typeof(RegionManager), true);

    RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true);

    RegisterTypeIfMissing(typeof(IRegionViewRegistry), typeof(RegionViewRegistry), true);

    RegisterTypeIfMissing(typeof(IRegionBehaviorFactory), typeof(RegionBehaviorFactory), true);

    RegisterTypeIfMissing(typeof(IRegionNavigationJournalEntry), typeof(RegionNavigationJournalEntry), false);

    RegisterTypeIfMissing(typeof(IRegionNavigationJournal), typeof(RegionNavigationJournal), false);

    RegisterTypeIfMissing(typeof(IRegionNavigationService), typeof(RegionNavigationService), false);

    RegisterTypeIfMissing(typeof(IRegionNavigationContentLoader), typeof(UnityRegionNavigationContentLoader), true);

    }

    }

<!-- -->

1.  The bootstrapper's **RegisterTypeIfMissing** method determines whether a service has already been registered—it will not register it twice. This allows you to override the default registration through configuration. You can also turn off registering any services by default; to do this, use the overloaded **Bootstrapper.Run** method passing in **false**. You can also override the **ConfigureContainer** method and disable services that you do not want to use, such as the event aggregator.

<!-- -->

1.  **Note:** If you turn off the default registration, you will need to manually register required services.

<!-- -->

1.  To extend the default behavior of **ConfigureContainer**, simply add an override to your application's bootstrapper and optionally call the base implementation, as shown in the following code from the **QuickStartBootstrapper** from the Modularity for WPF (with Unity) QuickStart. This implementation calls the base class's implementation, registers the **ModuleTracker** type as the concrete implementation of **IModuleTracker**, and registers the **callbackLogger** as a singleton instance of **CallbackLogger** with Unity.

<!-- -->

1.  C\#

<!-- -->

1.  protected override void ConfigureContainer()

    {

    base.ConfigureContainer();

    this.RegisterTypeIfMissing(typeof(IModuleTracker), typeof(ModuleTracker), true);

    this.Container.RegisterInstance&lt;CallbackLogger&gt;(this.callbackLogger);

    }

### Creating and Configuring the Container in the MefBootstrapper

1.  C\#

<!-- -->

1.  protected virtual void ConfigureContainer()

    {

    this.RegisterBootstrapperProvidedTypes();

    }

    protected virtual void RegisterBootstrapperProvidedTypes()

    {

    this.Container.ComposeExportedValue&lt;ILoggerFacade&gt;(this.Logger);

    this.Container.ComposeExportedValue&lt;IModuleCatalog&gt;(this.ModuleCatalog);

    this.Container.ComposeExportedValue&lt;IServiceLocator&gt;(new MefServiceLocatorAdapter(this.Container));

    this.Container.ComposeExportedValue&lt;AggregateCatalog&gt;(this.AggregateCatalog);

    }

<!-- -->

1.  **Note:** In the **MefBootstrapper**, the core services of Prism are added to the container as singletons so they can be located through the container throughout the application.

<!-- -->

1.  In addition to providing the **CreateContainer** and **ConfigureContainer** methods, the **MefBootstrapper** also provides two methods to create and configure the **AggregateCatalog** used by MEF. The **CreateAggregateCatalog** method simply creates and returns an **AggregateCatalog** object. Like the other methods in the **MefBootstrapper**, **CreateAggregateCatalog** is virtual and can be overridden if necessary.

    The **ConfigureAggregateCatalog** method allows you to add type registrations to the **AggregateCatalog** imperatively. For example, the **QuickStartBootstrapper** from the Modularity with MEF QuickStart explicitly adds ModuleA and ModuleC to the **AggregateCatalog**, as shown here.

<!-- -->

1.  C\#

<!-- -->

1.  protected override void ConfigureAggregateCatalog()

    {

    base.ConfigureAggregateCatalog();

    // Add this assembly to export ModuleTracker

    this.AggregateCatalog.Catalogs.Add(

    new AssemblyCatalog(typeof(QuickStartBootstrapper).Assembly));

    // Module A is referenced in in the project and directly in code.

    this.AggregateCatalog.Catalogs.Add(

    new AssemblyCatalog(typeof(ModuleA.ModuleA).Assembly));

    this.AggregateCatalog.Catalogs.Add(

    new AssemblyCatalog(typeof(ModuleC.ModuleC).Assembly));

    // Module B and Module D are copied to a directory as part of a post-build step.

    // These modules are not referenced in the project and are discovered by inspecting a directory.

    // Both projects have a post-build step to copy themselves into that directory.

    DirectoryCatalog catalog = new DirectoryCatalog("DirectoryModules");

    this.AggregateCatalog.Catalogs.Add(catalog);

    }

More Information
================

1.  For more information about MEF, **AggregateCatalog**, and **AssemblyCatalog**, see [Managed Extensibility Framework Overview](http://msdn.microsoft.com/en-us/library/dd460648.aspx) on MSDN.


