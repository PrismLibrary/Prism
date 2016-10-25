#Extending the Prism Library 5.0 for WPF

Prism contains assets that represent recommended practices for Windows Presentation Foundation (WPF) client development. Developers can use an unmodified version of the guidance to create composite applications using the Model-View-ViewModel (MVVM) pattern. However, because each application is unique, you should analyze whether Prism is suitable for your particular needs. In some cases, you will want to customize the guidance to incorporate your enterprise's best practices and frequently repeated developer tasks.

The Prism Library can serve as the foundation for your WPF client applications. The Prism Library was designed so that significant pieces can be customized or replaced to fit your specific scenario. You can modify the source code for the existing library to incorporate new functionality. Developers can replace key components in the architecture with ones of their own design because of the reliance on a container to locate and construct key components in the architecture. In the library, you can even replace the container itself if you want. Other common areas to customize include creating or customizing the bootstrapper to select a module discovery strategy for module loading, calling your own logger, using your own container, and creating your own region adapters.

This topic describes several key extensibility points in the Prism Library. These tend to be more advanced topics and are not expected to be performed for most developers using the Prism Library. A solid understanding of the goals and design decisions in the Prism Library will help to ensure any extensions to Prism functionality don’t create side effects or degrade the architecture. It is recommended that the main topics of the Prism documentation are read before extending the Prism Library. Most of the techniques described in this document rely on replacing or modifying Prism Library default configuration during the bootstrapping sequence when the application starts, so reading the section in is a prerequisite.

The following are the key extensibility points in the Prism Library covered in this topic:


 * **Application Bootstrapper**. This demonstrates the key extensibility point of the Prism Library.
 * **Modularity**. This demonstrates extensibility points when building a modular application.
 * **Region Management**. This demonstrates extending how regions behave, how they are hosted, and how they interact with their views.
 * **Region Navigation**. This demonstrates how to change your logical navigation structure.
 * **View Model Locator**. This demonstrates how to modify the conventions when using the View Model Locator.
    
## Guidelines for Extensibility

Use these guidelines when you extend the Prism Library. You can extend the library by adding or replacing services, modifying the source code, or adding new application capabilities.

### Exposing Functionality

A library should provide a public API to expose its functionality. The interface of the API should be independent of the internal implementation. Developers should not be required to understand the library design or implementation to effectively use its default functionality. Whenever possible, the API should apply to common scenarios for a specific functionality.

### Extending Libraries

The Prism Library provides extensibility points that developers can use to tailor the library to suit their needs. For example, when using the Prism Library, you can replace the provided logging service with your own logging service.

You can extend the library without modifying its source code. To accomplish this, you should use extensibility points, such as public base classes or interfaces. Developers can extend the base classes or implement the interfaces and then add their extensions to the library. When defining the set of extensibility points, consider the effect on usability. A large number of extensibility points can make the library complicated to use and difficult to configure.

Some developers may be interested in customizing the code, which means they will modify the source code instead of using the extension points. To support this effort, the library design should provide the following:

 * It should follow object-oriented design principles whenever practical.
 * It should use appropriate patterns.
 * It should efficiently use resources.
 * It should adhere to security principles (for example, distrust of user input and principle of least privilege).

## Recommendations for Modifying the Prism Library

When modifying the source code, follow these best practices:

 * Make sure you understand how the library works by reading the topics that describe its design. Consider changing the library's namespace if you significantly alter the code or if you want to use your customized version of the library together with the original version.
 * Consider authoring your own assemblies that use the Prism Library’s built in extensibility points first before altering or replacing the Prism Library binaries.
 * Use strong naming. A strong name allows the assembly to be uniquely identified, versioned, and checked for integrity. You will need to generate your own key pair to sign your modified version of the application block. For more information, see [Strong-Named Assemblies](http://msdn2.microsoft.com/en-us/library/wd40t7ad(vs.71).aspx) on MSDN. Alternatively, you can choose to not sign your custom version. This is referred to as weak naming.

## Extensibility Points in the Prism Library

This section outlines the extension points, by functional area, and associated information for extending the library.

### Container and Bootstrapper

The Prism Library directly supports both the Unity Application Block (Unity) and Managed Extensibility Framework (MEF) as dependency injection containers; however, because the container is accessed through the **IServiceLocator** interface, the container can be replaced.

Each Prism application configures the Prism Library through a bootstrapper class. Each stage in the bootstrapping process is replaceable, as well as the sequence itself. The bootstrapper provides a key extensibility point to replace default implementations with custom implementations or register additional types and services.

Logging
-------

Some Prism Library components log information, warning messages, or error messages. To avoid a dependency on a particular logging approach, it logs these messages to the **ILoggerFacade** interface. A common extension is to provide a custom logger for specific applications.

Modules
-------

The Prism Library provides various ways to populate the module catalog and load modules; however, your scenario may have needs that the library does not provide.

Module loading includes the following three phases, which can be customized:

 * **Module discovery**. This is the process of populating a module catalog. Frequently, this is done directly or by sweeping a directory, but your application may need to do this some other way, such as from a database. In these cases, you can create a custom catalog that populates itself from an appropriate source.
 * **Module retrieval and loading**. This is the process of acquiring the module binaries locally and loading the module into the current application domain. The library provides the **FileModuleTypeLoader**, but you may want to implement your own retrieval strategy.
 * **Module initialization**. This is the process of initializing a module. In the library, this is done by the **ModuleInitializer**, but it can be replaced by providing a new object that implements **IModuleInitialzer**.

### Regions

The Prism Library provides default control adapters for enabling a control as a region. Extensions around regions may involve providing custom region adapters, custom regions, or replacing the region manager. If you have a custom WPF control or a third-party control that does not work with the provided region adapters, you may want to create custom region adapters that will. It is also possible to replace the default **RegionManager** by supplying a new **IRegionManager** in the container.

### Region Navigation

The region feature of the Prism Library also supports navigation, including back/forward journaling support. Views within a region can extend and participate in navigation through the **INavigationAware** interface. Developers familiar with Silverlight navigation features will find **Region** analogous to the **Frame** class. Region navigation supports several extensibility points that make it possible to change the logical navigation structure of the application in addition to replacement of navigation services.

The **RegionNavigationContentLoader** class provides the ability to load content into a region based on the **NavigationContext**. If the content being navigated to is already in the region, the **RegionNavigationContentLoader** will locate that content and make it active instead of creating new content to add to the region. The **RegionNavigationContentLoader.GetCandidatesFromRegion** method searches the region’s views matching them by type. However, it is possible to have a view whose type does not match the type used to resolve it. For example, you could register your view with a dependency injection container using a "friendly" name that does not match the name of your view type.

    [Export("FriendlyName")]
    public class MyViewType

The Prism Library ships with **UnityRegionNavigationContentLoader** and **MefRegionNavigationContentLoader** that override the base **GetCandidatesFromRegion** method providing special handling necessary to find view types based on possible friendly name registration. If you are not using either **UnityRegionNavigationContentLoader** or **MefRegionNavigationContentLoader**, then make sure to add handling to a subclass of **RegionNavigationContentLoader** specific to the dependency injection container you are using.

## Container and Bootstrapper

The Prism Library contains the **Bootstrapper** base class. The Unity and MEF components derive from this class as **UnityBootstrapper** and **MefBootstrapper**, respectively. The **Bootstrapper** base class defines an abstract **Run** method that leaves the exact sequencing of the process up to the derived classes. Almost every method is marked as virtual, allowing you to override individual methods to customize and extend the bootstrapping process.

For most type instantiation, a bootstrapper will use the dependency injection container. However, there are some parts of the bootstrapping process that cannot use the container:

 * **Creating the logger**. Generally, the logger is created first (before the container) because the bootstrapper needs to log information about creating the container. For more information about changing the logging implementation, see the section, "Logging."
 * **Creating and configuring catalogs**. Catalogs (for example, **ModuleCatalog** and **AggregateCatalog**) are created before the container because they are used during construction of the container.
 * **Creating the shell**. Because the shell may already exist before the bootstrapping sequence runs, the **CreateShell** method is left as abstract for the application developer to implement. The application developer can use the container to instantiate or locate the shell because the container has been created and initialized.

### Replacing Default Prism Library Types

There may be times when you need to change or extend the underlying implementation of a Prism Library type for an application. Because the Prism Library relies on dependency injection, you can replace the type during the bootstrapping sequence and both your application and the Prism Library will use the new type.

#### Replacing Default Types Using Unity

Any replacement types registered in the container before the **UnityBootstrapper**.**ConfigureContainer** method is called will replace the type. The **ConfigureContainer** default implementation uses the **RegisterTypeIfMissing** method to only add a Prism Library type if that associated interface is not already registered.

To replace Prism Library types in Unity, first derive your new type from the interface or class you want to replace. The following code example shows a replacement for the **IEventAggregator** interface.


    // when using Unity
    public class ReplacementEventAggregator : IEventAggregator
    {
        // ...
    }


Now that you have the replacement type, override the **ConfigureContainer** method in the bootstrapper and register interface and type before calling the base class. The following code example shows how to register the replacement for the **IEventAggregator**.

    // when using Unity
    protected override void ConfigureContainer()
    {
        this.RegisterTypeIfMissing(typeof(IEventAggregator), typeof(ReplacementEventAggregator), true);
        base.ConfigureContainer();
    }

#### Replacing Default Types Using MEF

Any replacement types registered in the container before the **MefBootstrapper**.**ConfigureContainer** method is called will replace the type. The **ConfigureContainer** default implementation only adds a Prism Library type if that associated interface is not already registered.

To replace Prism Library types in MEF, first derive your new type from the interface you want to replace and apply the appropriate MEF Export attributes to it. The following code example shows a replacement for the **IEventAggregator** interface.

    // when using MEF
    [Export(typeof(IEventAggregator))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ReplacementEventAggregator : IEventAggregator
    {
        // ...
    }

Now that you have the replacement type, override the **ConfigureAggregateCatalog** method in the bootstrapper and add a catalog that contains the type to the **AggregateCatalog**. The following code example shows how to use a **TypeCatalog** to add the replacement type. An **AssemblyCatalog** could also have been used.
 
    // when using MEF
    protected override void ConfigureAggregateCatalog()
    {
        this.AggregateCatalog.Catalogs.Add(new TypeCatalog(typeof(ReplacementEventAggregator)));
        base.ConfigureAggregateCatalog();
    }

#### Registering Non-MEF Attributed Types with the MEF Container

Registering types with MEF is simple if you own the code and can take a direct dependency on MEF, because all you need to do is add an **Export** attribute to the types. However, in some situations, you may need to register types with MEF when you cannot take a direct dependency on the MEF assemblies. This problem was encountered while the developers added MEF support to Prism because one of the design goals was to ensure that the core Prism libraries were not container-specific. This meant that the **Microsoft.Practices.Prism** assembly could not reference **System.ComponentModel.Composition** and use the **Export** attribute. Instead, the team created derived classes in the **Microsoft.Practices.Prism.MefExtensions** assembly that derived from the types the team wanted to expose and exported the appropriate type. The following code example from the **MefRegionManager** class shows an example of this approach by deriving from **RegionManager** and exporting the new type as an **IRegionManager**.

    [Export(typeof(IRegionManager))]
    public class MefRegionManager : RegionManager
    {
    }

### Creating a Minimal Bootstrapper

Some applications do not use many of the features in the Prism Library. In some cases, application developers may want the absolute minimum level of services—only dependency injection and service location. To do this, override the **ConfigureContainer** method in the bootstrapper and implement the following.

    // when using UnityBootstrapper
    protected override void ConfigureContainer()
    {
        // Base class implementation deliberately not called
        // base.ConfigureContainer();
        this.Container.AddNewExtension<UnityBootstrapperExtension>();
        Container.RegisterInstance<ILoggerFacade>(Logger);
        this.Container.RegisterInstance(this.ModuleCatalog);
        RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);
    }

    protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
    {
        return null;
    }

    protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
    {
        return null;
    }


**Note:** The overrides of the region adapters and mappings are required because Unity cannot determine the appropriate concrete type to return when an implementation of an interface is requested. These calls associate the concrete type to return for each interface. When concrete types are requested Unity is able to directly resolve them by instantiating that type.

    // when using MEFBootstrapper
    protected override void ConfigureContainer()
    {
        // Base class implementation deliberately not called
        // base.ConfigureContainer();

        this.Container.ComposeExportedValue<ILoggerFacade>(this.Logger);
        this.Container.ComposeExportedValue<IServiceLocator>(new MefServiceLocatorAdapter(this.Container));
        this.Container.ComposeExportedValue<AggregateCatalog>(this.AggregateCatalog);
    }

### Changing Dependency Injection Containers

If you want to use Prism with a container other than Unity or MEF in your application, there are several things you need to do. First, you need to write a Service Locator adapter for your container. You can use the **MefServiceLocatorAdapter** and the **UnityServiceLocatorAdapter** as examples of how this can be done. You will also need to write a container-specific bootstrapper class. Next, you need to create a new container-specific bootstrapper, derived from the **Bootstrapper** class, and implement the necessary methods, using the **MefBootstrapper** and **UnityBootstrapper** as examples.

## Logging

The Prism Library is designed to log messages throughout the library. To do this logging in a way that is not tied to a specific logging library, the Prism Library uses a logging façade, **ILoggerFacade**, to log its messages. This interface contains a single method named **Log** that logs messages. By default, the **UnityBootstrapper** and **MefBoostrapper** create a **TextLogger** as the designated logger.

There are three steps for creating and integrating a custom logger:

1. Create a class that implements the **ILoggerFacade** interface.
2.  Implement the **Log** method.
3.  In your application bootstrapper class, override the **CreateLogger** method to return a new instance of your logging class.

The **Log** method in the **ILoggerFacade** interface takes three parameters:

 * **Message**. This is the message to be logged.
 * **Category**. This is the category of the event to be logged. The valid options are **Debug**, **Exception**, **Info**, and **Warn**.
 * **Priority**. This is the priority of the event to be logged. The valid options are **None**, **High**, **Medium**, and **Low**.

The following code example shows a custom logger that wraps some other logging framework that takes only a string.

    // CustomLogger
    using Microsoft.Practices.Prism.Logging;
    ...

    public class CustomLogger : ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority)
        {
            string messageToLog =
                String.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "{1}: {2}. Priority: {3}. Timestamp:{0:u}.",
                    DateTime.Now,
                    category.ToString().ToUpperInvariant(),
                    message,
                    priority.ToString());
                    MyOtherLoggingFramework.Log(messageToLog);
            }
        }


    // ApplicationBootstrapper
    using Microsoft.Practices.Prism.Logging;
    ...

    public class ApplicationBootstrapper : UnityBootstrapper
    {
        ...
        protected override ILoggerFacade CreateLogger()
        {
            return new CustomLogger();
        }
    }

## Modules

The following sections describe how the modularity features can be extended during registration, assembly discovery, type discovery, and module initialization.

### Adding Features to the Module Catalog

The Prism Library provides **ModuleCatalog** as both a class you can populate directly through the **AddModule** methods, or you can derive from add methods to populate the **Items** property.

The **ModuleCatalog** class in the Prism Library provides a lot of additional capabilities beyond the **IModule** interface. There are many different overloads of the **AddModule** method, module group dependency checking, and sorting. There are several ways to extend the functionality of the **ModuleCatalog**:

 * **Derive from ModuleCatalog**. If you need to change the behavior of **ModuleCatalog**, derive a new class and override one of the virtual methods.
 * **Write extension methods on IModuleCatalog**. If you need additional functionality in your application where you use **IModuleCatalog**, write an extension method on the interface.
 * **Write extension methods on ModuleCatalog**. If you need additional functionality, but only in places where you use **ModuleCatalog**, write an extension method on the concrete type.

### Discovering Modules from a Custom Source

The Prism Library supports populating the module catalog from application configuration and from a XAML file. You can extend Prism in your application to support loading from other data sources, such as a web service, database, or other external files.

The following describes several ways to populate the catalog.

 * **Use the static CreateFromXaml method**. If your data is already in the **Modularity:ModuleCatalog** XAML schema, or if it can easily be converted, you can use this method to directly populate a **ModuleCatalog**.
 * **Replace the IConfigurationStore in the ConfigurationModuleCatalog**. If you are running a WPF desktop application, you can implement an **IConfigurationStore** to return the module section for the **ConfigurationModuleCatalog**.
 * **Derive from ModuleCatalog**. You can also follow the example of the **ConfigurationModuleCatalog** to derive from **ModuleCatalog**, acquire your data, and then call the **AddModule** method to populate the catalog.

The following code examples show how to load a custom configuration module file from disk.

    // Bootstrapper
    protected override Microsoft.Practices.Prism.Modularity.IModuleCatalog CreateModuleCatalog()
    {
        ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog();
        catalog.Store = new MyModuleCatalogStore();
        return catalog;
    }

    // MyModuleCatalogStore
    public class MyModuleCatalogStore : IConfigurationStore
    {

        public ModulesConfigurationSection RetrieveModuleConfigurationSection()
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap()
            {
               ExeConfigFilename = "MyModuleCatalog.config"
            };
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            return configuration.GetSection("modules") as ModulesConfigurationSection;
        }
    }

### Retrieving and Loading Modules from a Custom Assembly Source

If your application has a packaging or distribution mechanism other than assemblies, you can implement your own **IModuleTypeLoader** to download and access types.

The Prism 4.1 Library **MefXapModuleTypeLoader** class is an example of this. It uses the MEF **DeploymentCatalog** to download XAP files, locate the assemblies, and register them with the MEF catalog.

Each **IModuleTypeLoader** implements the **CanLoadModuleType** method to allow the **ModuleManager** to determine the appropriate type loader to use for obtaining a module. The following code example shows the **MefXapModuleTypeLoader** implementation**.**

    // MefXapModuleTypeLoader.cs
    public bool CanLoadModuleType(ModuleInfo moduleInfo)
    {
        if (moduleInfo == null)
        {
            throw new ArgumentNullException("moduleInfo");
        }

        if (!string.IsNullOrEmpty(moduleInfo.Ref))
        {
            Uri uriRef;
            return Uri.TryCreate(moduleInfo.Ref, UriKind.RelativeOrAbsolute, out uriRef);
        }

        return false;
    }

After you have your module type loader, you need to ensure it is in the **ModuleManager**'s collection of type loaders. The following code example is from the **Prism.MefExtensions.Silverlight** project.

    // MefModuleManager.Silverlight.cs
    public override IEnumerable<IModuleTypeLoader> ModuleTypeLoaders
    {
        get
        {
            if (this.mefTypeLoaders == null)
            {
                this.mefTypeLoaders = new List<IModuleTypeLoader>()
                    { this.MefXapModuleTypeLoader }
            }
            return this.mefTypeLoaders;
        }
        set
        {
            this.mefTypeLoaders = value;
        }
    }

Changing How Modules Are Initialized
------------------------------------

**Derive from ModuleManager**. If you need to change the fundamental behavior of the module loading and initialization sequence, derive from a new class and override virtual methods.

    **Replace IModuleIntializer**. If you need to change how module types are instantiated and initialized, replace **IModuleIntializer**.

    **Write a custom IModuleTypeLoader**. If you need to change how assemblies are loaded and module types discovered within assemblies, write a custom **IModuleTypeLoader**. For more information, see the section, [Retrieving and Loading Modules from a Custom Assembly Source](#retrieving-and-loading-modules-from-a-custom-assembly-source).

## Regions

The following sections describe how the region management features of the Prism Library can be extended when regions are attached to controls, how regions behave, and how a region discovers its views.

### Region Adapters

Region adapters control how items placed in a region interact with the host control. The following sections describe how to extend this behavior by creating a custom region adapter and controlling the registration of the adapters.

#### Creating a Custom Region Adapter

To expose a UI control as a region, a region adapter is used. Region adapters are responsible for creating a region and associating it to the control. By doing this, developers can manage the UI control's contents in a consistent way through the **IRegion** interface. Each region adapter adapts a particular type of UI control. The Prism Library provides three region adapters out-of-the-box:

 * **ContentControlRegionAdapter**. This adapter adapts controls of type **System.Windows.Controls.ContentControl** and derived classes.
 * **SelectorRegionAdapter**. This adapter adapts controls derived from the class **System.Windows.Controls.Primitives.Selector**, such as the **System.Windows.Controls.TabControl** control.
 * **ItemsControlRegionAdapter**. This adapter adapts controls of type **System.Windows.Controls.ItemsControl** and derived classes.

There are some scenarios in which none of the preceding region adapters suit the developer needs. In those cases, custom region adapters can be created to adapt controls not supported by the Prism Library out-of-the-box.

Region adapters implement the **Microsoft.Practices.Prism.Regions.IRegionAdapter** interface. This interface defines a single method named Initialize that takes the object to adapt and returns a new region associated with the adapted control. The interface definition is shown in the following code.

    public interface IRegionAdapter
    {
        IRegion Initialize(object regionTarget, string regionName);
    }
    
To create a region adapter, you derive your class from **RegionAdapterBase<T>** and implement the **CreateRegion** and **Adapt** methods. Optionally, override the **AttachBehaviors** method to attach special logic to customize the region behavior. If you want to interact with the control that hosts the region, you should also implement **IHostAwareRegionBehavior**.

The **CreateRegion** method is an abstract method defined in the **RegionAdapterBase** class. It returns a region instance (an object that implements the **IRegion** interface) to be associated with the adapted control. The Prism Library provides the following region implementations out-of-the-box:

 * **Region**. This region allows multiple active views. This is the region used for controls derived from the **Selector** class.
 * **SingleActiveRegion**. This region allows a maximum of one active view at a time. This is the region used for **ContentControl** controls.
 * **AllActiveRegion**. This region keeps all the views in it active. Deactivation of views is not allowed. This is the region used for **ItemsControl** controls.

The **Adapt** method is also an abstract method defined in the **RegionAdapterBase** class. It adapts the control to the region created earlier. The **Adapt** method takes two parameters: the region with which the adapted control has to be associated and the control to adapt. The following code example shows the **ContentControlRegionAdapter**.

    public class ContentControlRegionAdapter : RegionAdapterBase<ContentControl>
    {
        public ContentControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, ContentControl regionTarget)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            bool contentIsSet = regionTarget.Content != null;
            contentIsSet = contentIsSet || (BindingOperations.GetBinding(regionTarget, ContentControl.ContentProperty) != null);

            if (contentIsSet)
            {
                throw new InvalidOperationException(Resources.ContentControlHasContentException);
            }

            region.ActiveViews.CollectionChanged += delegate
            {
                regionTarget.Content = region.ActiveViews.FirstOrDefault();
            };

            region.Views.CollectionChanged +=
                (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && region.ActiveViews.Count() == 0)
                    {
                        region.Activate(e.NewItems\[0\]);
                    }
                };
        }
    
        protected override IRegion CreateRegion()
        {
             return new SingleActiveRegion();
        }
    }
   

**Note:** The region adapter will be registered as a singleton service and will be kept alive throughout the application's lifetime, so make sure you do not keep references to possibly shorter lived objects, such as UI controls or region instances.

Region adapter mappings are used by the region manager service to associate the correct region adapters for XAML-defined regions. The following section describes how to customize the registration of region adapter mappings.

#### Customizing the Region Adapter Mappings

One phase of the bootstrapping process is to register the default region adapter mappings. These mappings are used by the region manager to associate the correct adapters for XAML-defined regions. By default, an **ItemsControlRegionAdapter**, a **ContentControlRegionAdapter**, and a **SelectorRegionAdapter** are registered. For more information about these adapters, see .

The following code example shows the default implementation of the **ConfigureRegionAdapterMappings** method. To customize the registration of region adapters, override this method in your applications bootstrapper.

    // Bootstrapper.cs
    protected virtual RegionAdapterMappings ConfigureRegionAdapterMappings()
    {
        RegionAdapterMappings regionAdapterMappings = ServiceLocator.Current.GetInstance<RegionAdapterMappings>();
        if (regionAdapterMappings != null)
        {
            regionAdapterMappings.RegisterMapping(typeof(Selector), ServiceLocator.Current.GetInstance<SelectorRegionAdapter>());
            regionAdapterMappings.RegisterMapping(typeof(ItemsControl), ServiceLocator.Current.GetInstance<ItemsControlRegionAdapter>());
            regionAdapterMappings.RegisterMapping(typeof(ContentControl), ServiceLocator.Current.GetInstance<ContentControlRegionAdapter>());
        }
        return regionAdapterMappings;
    }

### Region Behaviors

Region behaviors are used by the Prism Library to provide most of the functionality for a region. During the bootstrapping process, the bootstrapper registers the region behaviors that are attached to each region by default. Additionally, adapters may add behaviors only when a region is associated with a specific control type.

#### Adding a Region Behavior for All Regions

After you create a behavior, or extend an existing one, you can register it so it will be added to all new regions. You can do this by overriding the **ConfigureDefaultRegionBehaviors** in the bootstrapper. The following code example shows how to add a custom behavior for all regions.

    protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
    {
        IRegionBehaviorFactory factory = base.ConfigureDefaultRegionBehaviors();
        factory.AddIfMissing("MyBehavior", typeof(MyCustomBehavior));
    }

#### Adding a Region Behavior for a Single Region

The following code example shows how to add a region behavior to a single region.

    IRegion region = regionManager.Region\["Region1"\];
    region.Behaviors.Add("MyBehavior", new MyRegion());

### Replacing an Existing Region Behavior

If you want to replace a default behavior with a different behavior, you can add it by overriding the **ConfigureDefaultRegionBehaviors** method in your application-specific bootstrapper and registering your behavior with the same key value as the default behavior. The Prism Library adds a default region behavior only if a behavior with that key has not already been added.

Occasionally, you may want to add or a replace a region behavior to regions on a particular view. If those regions are defined in XAML, like most regions are, the region may not be initially available for attaching your custom behavior. You will need to monitor the availability of the region and attach your behavior when the region becomes available. The following code example shows how to replace the **AutoPopulateBehavior** with your custom version when the region becomes available.

    public class MyView : UserControl
    {
        public MyView()
        {
            InitializeComponent();

            ObservableObject<IRegion> observableRegion = RegionManager.GetObservableRegion(this.MyRegionHostControl);

            observableRegion.PropertyChanged += (sender, args) =>
            {
                IRegion region = ((ObservableObject<IRegion>)sender).Value;
                region.Behaviors.Add(AutoPopulateBehavior.BehaviorKey,
                    new CustomAutoPopulateBehavior());
            };
        }
    }

#### Removing a Region Behavior

Although there is no way to remove an existing behavior after it is added, you can prevent a behavior from being added by overriding the **ConfigureDefaultRegionBehaviors** method in your application-specific bootstrapper.

#### Changing How Views Are Discovered

You may want to control how views are registered or created when using view discovery. The following are approaches to extending view discovery:

 * **Custom RegionViewRegistry**. If you want to have extra control over registration of types (for example, scoping the registry) or control over the creation of your types, you should derive from this class.
 * **Custom AutoPopulateBehavior**. If you want to change where the region discovers its registered views (if you do not want to use the **RegionViewRegistry**) or if you want to change which views are actually added to the region (for example, if you want to provide the ability to filter), you can create a custom **AutoPopulateBehavior** for a single region or change the default for all regions.
  
## Region Navigation

The following sections describe how to extend the region navigation features of the Prism Library.

### Changing Your Logical Navigation Structure

The region navigation features in the Prism Library use the type name of each view as the navigation Uniform Resource Identifier (URI). Your application may want to expose a URI navigation scheme independent of the view type names.

WPF applications can replace the **IRegionNavigationContentLoader** implementation to achieve the same result. Multi-targeted applications may also want to use this approach to maintain a single place where the URI structure for the application is defined.

To change the logical navigation structure, derive a new class from **RegionNavigationContentLoader** and override the **GetContractFromNavigationContext** method. In the method, translate the incoming contract name to the view type name to load. It is recommended to call the base class because it conveniently parses the URI into a contract string to inspect. The following code example shows a custom region content loader that maps "Home" to the Home view and "About" to the About view.

**Note:** This example uses MEF, so export attributes are applied at the top of the class to make it available in the MEF container.

    [Export(typeof(IRegionNavigationContentLoader))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CustomRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        [ImportingConstructor]
        public CustomRegionNavigationContentLoader(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        protected override string GetContractFromNavigationContext(NavigationContext navigationContext)
        {
    
            string contract = base.GetContractFromNavigationContext(navigationContext);
        
            if (contract.Equals("Home", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(HomeView).Name;
            }
    
            if (contract.Equals("About", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(AboutView).Name;
            }
        
            return contract;
        }
    }

After you have your custom navigation content loader, replace it as the implementation of the **IRegionNavigationContentLoader** in the container. For more information about replacing types in the container, see the section, [Container and Bootstrapper](#container-and-bootstrapper), earlier in this topic.

### Advanced Navigation Replacements

The following sections describe replacing major portions of the region navigation infrastructure provided by the Prism Library. Most developers will not have scenarios that require this level of customization.

#### RegionNavigationContentLoader

The **RegionNavigationContentLoader** type implements the **IRegionNavigationContentLoader** interface. You may either derive from **RegionNavigationContentLoader** and override methods, or replace the implementation of the interface entirely.

In addition to the **LoadContent** method, the **RegionNavigationContentLoader** type has two other possible methods to override, but they should be required only in uncommon navigation scenarios:

 * **GetCandidatesFromRegion**. This method uses a filter to determine the views in a region that are candidates for handling the navigation request. Applications that need to do special filtering or ordering of candidate views will need to override this method.
 * **CreateNewRegionItem**. This method is called to create a view if a candidate is not found that can handle the navigation request. The default implementation uses the **IServiceLocator** to create an instance of the view. Applications that need special logic outside of the container to return instances or singletons of views will need to override this method.

#### IRegionNavigationJournal/IRegionNavigationJournalEntry

The region navigation service contains a journal that records the navigation history and provides back and forward navigation. The default implementation is a standard stack implementation. Applications that want to implement more advanced journal and history features (such as the Internet Explorer **Back** button drop-down menu) may need to replace the **RegionNavigationJournal** to change behavior and may need to replace the **RegionNavigationJournalEntry** to provide additional data, such as a Title field and an Icon field, with each entry.

#### IRegionNavigationService

## View Model Locator

The View Model Locator is used in the MVVM Basic QuickStart to wire the view and the view model using its standard convention. This section describes how to change the conventions for naming and locating views, naming, locating and associating view models with views.

For guidance on determining whether to use the View Model Locator or to wire your view and view model together using MEF, see . As background, the Stock Trader reference implementation uses MEF to wire the view and the view model.

### Changing the View Model Locator Conventions

The **ViewModelLocationProvider** provides a static method called **SetDefaultViewTypeToViewModelTypeResolver** that can be used to provide your own convention for associating views to view models.

    ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
    {
        ...
        return viewModelType;
    });

By default, if located in the **View** namespace, the view will update the namespace to **ViewModel** and append the "ViewModel" suffix to the view name. Prism will look for this view model in the same assembly.

### Configuring the ViewModelLocationProvider to Use a Container

The following example shows how to configure the **ViewModelLocationProvider** to construct a view model using a container.

When bootstrapping your application use the **SetDefaultViewModelFactory** method to use your container to resolve view model types. The following is an example using Microsoft's Unity dependency injection container.

    IUnityContainer \_container = new UnityContainer()
    ...
    ViewModelLocationProvider.SetDefaultViewModelFactory((t)=> \_container.Resolve(t));

The default strategy for creating the view models is using the **Activator.CreateInstance** method, which is a valid approach if you have a default constructor in the view model and there are no dependencies to be injected.


