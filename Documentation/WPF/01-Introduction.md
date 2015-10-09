1.  **\#!155CharTopicSummary!\#:**

    1.  Learn how to create a composite application from loosely coupled WPF components that can evolve independently using the Prism library.

    <!-- -->

    1.  Composite applications typically feature multiple screens, rich user interaction and data visualization, and that embody significant presentation and business logic. These applications typically interact with multiple back-end systems and services and, using a layered architecture, may be physically deployed across multiple tiers. It is expected that the application will evolve significantly over its lifetime in response to new requirements and business opportunities. In short, these applications are "built to last" and "built for change." Applications that do not demand these characteristics may not benefit from using Prism.

        Prism includes reference implementations, QuickStarts, reusable library code (the Prism Library), and extensive documentation. This version of Prism targets the Microsoft .NET Framework 4.5 and includes new guidance around the Model-View-ViewModel (MVVM) pattern, navigation, and the Managed Extensibility Framework (MEF). Because Prism is built on the .NET Framework 4.5 (which includes WPF) , familiarity with these technologies is useful for evaluating and adopting Prism.

        It should be noted that while Prism is not difficult to learn, developers must be ready and willing to embrace patterns and practices that may be new to them. Management understanding and commitment is crucial, and the project deadline must accommodate an investment of time up front for learning these patterns and practices.<span id="_Installing_the_Composite" class="anchor"><span id="_Create_Your_Hello" class="anchor"><span id="_Building_and_running" class="anchor"><span id="_Building_the_Composite" class="anchor"><span id="_Building_and_running_1" class="anchor"><span id="NewInThisRelease" class="anchor"><span id="WhenToUseThisGuidance" class="anchor"></span></span></span></span></span></span></span>

Why Use Prism?
==============

1.  Designing and building rich WPF client applications that are flexible and easy to maintain can be challenging. This section describes some of the common challenges you might encounter when building WPF client applications, and describes how Prism helps you to address those challenges.

Client Application Development Challenges
-----------------------------------------

1.  Typically, developers of client applications face quite a few challenges. Application requirements can change over time. New business opportunities and challenges may present themselves, new technologies may become available, or even ongoing customer feedback during the development cycle may significantly affect the requirements of the application. Therefore, it is important to build the application so that it is flexible and can be easily modified or extended over time. Designing for this type of flexibility can be hard to accomplish. It requires an architecture that allows individual parts of the application to be independently developed and tested and that can be modified or updated later, in isolation, without affecting the rest of the application.

    Most enterprise applications are sufficiently complex that they require more than one developer, maybe even a large team of developers that includes user interface (UI) designers and localizers in addition to developers. It can be a significant challenge to decide how to design the application so that multiple developers or subteams can work effectively on different pieces of the application independently, yet ensuring that the pieces come together seamlessly when integrated into the application.

    Designing and building applications in a *monolithic* style can lead to an application that is very difficult and inefficient to maintain. In this case, "monolithic" refers to an application in which the components are very tightly coupled and there is no clear separation between them. Typically, applications designed and built this way suffer from problems that make the developer's life hard. It is difficult to add new features to the system or replace existing features, it is difficult to resolve bugs without breaking other portions of the system, and it is difficult to test and deploy. Also, it impacts the ability of developers and designers to work efficiently together.

The Composite Approach
----------------------

1.  An effective remedy for these challenges is to partition the application into a number of discrete, loosely coupled, semi-independent components that can then be easily integrated together into an application "shell" to form a coherent solution. Applications designed and built this way are often known as composite applications.

    Composite applications provide many benefits, including the following:

<!-- -->

1.  They allow modules to be individually developed, tested, and deployed by different individuals or subteams; they also allow them to be modified or extended with new functionality more easily, thereby allowing the application to be more easily extended and maintained. Note that even single-person projects experience benefits in creating more testable and maintainable applications using the composite approach.

    They provide a common shell composed of UI components contributed from various modules that interact in a loosely coupled way. This reduces the contention that arises from multiple developers adding new functionality to the UI, and it promotes a common appearance.

    They promote reuse and a clean separation of concerns between the application's horizontal capabilities, such as logging and authentication, and the vertical capabilities, such as business functionality that is specific to your application. This also allows you to more easily manage the dependencies and interactions between application components.

    They help maintain a separation of roles by allowing different individuals or subteams to focus on a specific task or piece of functionality according to their focus or expertise. In particular, it provides a cleaner separation between the UI and the business logic of the application—this means the UI designer can focus on creating a richer user experience.

    1.  

    <!-- -->

    1.  Composite applications are highly suited to a range of client application scenarios. For example, a composite application is ideal for creating a rich end-user experience over disparate back-end systems. The following illustration shows an example of this type of a composite application.

    <!-- -->

    1.  Composite application with multiple back-end systems

    <!-- -->

    1.  ![](media/image1.png)

    <!-- -->

    1.  Additionally, a composite application can be useful when there are independently evolving components in the UI that heavily integrate with each other and that are often maintained by separate teams. The following illustration shows a screen shot of this type of application. Each of the areas highlighted represent independent components that are composed into the UI.

    <!-- -->

    1.  Stock Trader Reference Implementation composite application

    <!-- -->

    1.  ![](media/image2.png)

    <!-- -->

    1.  In this case, the composite application allows the UI to be dynamic composed. This delivers a flexible user experience. For example, it can allow new functionality to be dynamically added to the application at run time, which enables rich end-user customization and extensibility.

Challenges Not Addressed by Prism
---------------------------------

1.  Although Prism helps you to address many of the challenges you might face when building WPF applications, there are many other challenges that you might face, depending on your application scenario and requirements. For example, Prism does not directly address the following topics:

<!-- -->

1.  Occasional connectivity and data synchronization

    Service and messaging infrastructure design

    Authentication and authorization

    Application performance

    Application versioning

    Error handling and fault tolerance

    1.  

<span id="PrismAssets" class="anchor"><span id="GettingStartedWithPrism" class="anchor"><span id="Prerequisites" class="anchor"></span></span></span>Prerequisites
==================================================================================================================================================================

1.  Prism assumes you have hands-on experience with WPF . There are a few important concepts that Prism uses heavily, and you should become familiar with them. They include the following:

-   **XAML (Extensible Application Markup Language)**. The language to declaratively define and initialize the user interface in WPF applications.

-   **Data binding**. This is how UI elements are connected to components and data in WPF.

-   **Resources**. These are how styles, data templates, and control templates are created and managed in WPF.

-   **Commands**. These are how user gestures and input are connected to controls.

-   **User controls**. These are components that provide custom behavior or custom appearance.

-   **Dependency properties**. These are extensions to the common language runtime (CLR) property system to enable property setting and monitoring in support of data binding, routed commands, and events.

-   **Behaviors.** Behaviors are objects that encapsulate interactive functionality that can be easily applied to controls in the user interface.

1.  

An Overview of Prism
====================

Architectural Goals
-------------------

1.  The guidance is designed to help architects and developers achieve the following objectives:

-   Create an application from modules that can be built, assembled and, optionally, deployed by independent teams using WPF.

-   Minimize cross-team dependencies and allow teams to specialize in different areas, such as user interface (UI) design, business logic implementation, and infrastructure code development.

-   Use an architecture that promotes reusability across independent teams.

-   Increase the quality of applications by abstracting common services that are available to all the teams.

-   Incrementally integrate new capabilities.

1.  

Prism Design Goals
------------------

1.  Prism was designed to help you design and build rich, flexible, and easy-to-maintain WPF applications. The Prism Library implements design patterns that embody important architectural design principles, such as separation of concerns and loose coupling. Using the design patterns and capabilities provided by the Prism Library, you can design and build applications using loosely coupled components that can evolve independently but that can be easily and seamlessly integrated into the overall application.

    1.  Prism is designed around the core architectural design principles of separation of concerns and loose coupling. This allows Prism to provide many benefits, including the following:

    -   **Reuse**. Prism promotes reuse by allowing components and services to be easily developed, tested and integrated into one or more applications. Reuse can be achieved at the component level through the reuse of unit-tested components that can be easily discovered and integrated at run time through dependency injection, and at the application level through the use of modules that encapsulate application-level capabilities that can be reused across applications.

    -   **Extensibility**. Prism helps to create applications that are easy to extend by managing component dependencies, allowing components to be more easily integrated or replaced with alternative implementations at run time, and by providing the ability to decompose an application into modules that can be independently updated and deployed. Many of the components in the Prism Library itself can also be extended or replaced.

    -   **Flexibility**. Prism helps to create flexible applications by allowing them to be more easily updated as new capabilities are developed and integrated. Prism also allows WPF applications to be developed using common services and components, allowing the application to be deployed and consumed in the most appropriate way. It also allows applications to provide different experiences based on role or configuration.

    -   **Team Development**. Prism promotes team development by allowing separate teams to develop and even deploy different parts of the application independently. Prism helps to minimize cross-team dependencies and allows teams to focus on different functional areas (such as UI design, business logic implementation, and infrastructure code development), or on different business-level functional areas (such as profile, sales, inventory, or logistics).

    -   **Quality**. Prism can help to increase the quality of applications by allowing common services and components to be fully tested and made available to the development teams. In addition, by providing fully tested implementations of common design patterns, and the guidance needed to fully leverage them, Prism allows development teams to focus on their application requirements instead of implementing and testing infrastructure code.

    1.  

    <!-- -->

    1.  It is important to note that Prism was designed so that you can use any of Prism's capabilities and design patterns individually, or all together, depending on your requirements and your application scenario. Prism was designed so that it could be incrementally adopted, allowing you to use the capabilities and design patterns that make sense for your particular application without requiring major structural changes.

        Finally, because software testing should be considered a first-class development activity and tightly integrated into the development process, Prism provides extensive support for various types of software testing, thereby allowing you to design and build applications that are easy to test. Prism itself was developed with testing in mind. It was developed to meet multiple strict quality gates to ensure that it meets Microsoft security standards and that it will function correctly on multiple operating systems, with multiple versions of Visual Studio, and with multiple programming languages. Unit tests were run after each check-in. In addition, the Prism library was tested against several additional quality gates, as listed in the following table.

| Test                           | Description                                                                                                                                           |
|--------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------|
| Acceptance Testing             | Validates the application functionality using user scenarios to drive the test requirements. Tests can be executed manually or automated.             |
| Application Building Exercises | Team members build applications consuming the deliverable software.                                                                                   |
| Black Box Testing              | Manual acceptance tests perform from the user point of view.                                                                                          |
| Cross Platform Testing         | All automated tests are run on multiple platforms.                                                                                                    |
| Globalization Testing          | All automated tests are run on multiple languages.                                                                                                    |
| Performance Testing            | Measures how fast a particular aspect of a system performs under-load.                                                                                |
| Security Review                | Internal Microsoft security audit standards that cover thread models, identifying attack factors and running the code though security analysis tools. |
| Stress Testing                 | Measures stability of the system under extreme loads; specifically looking to drive out issues like memory leaks and threading issues.                |
| White Box Testing              | In-depth source code analysis validating the coding standards, structure and how it maps to the overall architecture.                                 |

1.  The Prism Library source code includes unit and UI automation tests, as shown in the following table. You can use these as an educational resource, or you can run the tests against the Prism Library itself. This allows you to customize, re-compile, test and deploy a modified version of the Prism Library using similar quality gates as the Prism team.

| Test                | Description                                                                            |
|---------------------|----------------------------------------------------------------------------------------|
| UI Automation Tests | Limited range of acceptance testing; driving the application from the user perspective |
| Unit Tests          | Validates the implementation of a class                                                |

Prism Key Concepts
------------------

1.  Prism provides capabilities and design patterns that may be unfamiliar to you, especially if you're new to design patterns and composite application development. This section provides a brief overview of the main concepts behind Prism and defines some of the terminology that you will see used throughout the documentation and code.

<!-- -->

1.  **Modules**. Modules are packages of functionality that can be independently developed, tested, and (optionally) deployed. In many situations, modules are developed and maintained by separate teams. A typical Prism application is built from multiple modules. Modules can be used to represent specific business-related functionality (for example, profile management) and encapsulate all the views, services, and data models required to implement that functionality. Modules can also be used to encapsulate common application infrastructure or services (for example, logging and exception management services) that can be reused across multiple applications.

    > **Module catalog**. In a composite application, modules must be discovered and loaded at run time by the host application. In Prism, a module catalog is used to specify which modules to are to be loaded, when they are loaded, and in what order. The module catalog is used by the **ModuleManager** and **ModuleLoader** components, which are responsible for downloading the modules if they are remote, loading the module's assemblies into the application domain, and for initializing the module. Prism allows the module catalog to be specified in different ways, including programmatically using code, declaratively using XAML, or using a configuration file. You can also implement a custom module catalog if you need to.

    > **Shell**. The shell is the host application into which modules are loaded. The shell defines the overall layout and structure of the application, but it is typically unaware of the exact modules that it will host. It usually implements common application services and infrastructure, but most of the application's functionality and content is implemented within the modules. The shell also provides the top-level window or visual element that will then host the different UI components provided by the loaded modules.

    > **Views**. Views are UI controls that encapsulate the UI for a particular feature or functional area of the application. Views are used in conjunction with the MVVM pattern, which is used to provide a clean separation of concerns between the UI and the application's presentation logic and data. Views are used to encapsulate the UI and define user interaction behavior, thereby allowing the view to be updated or replaced independently of the underlying application functionality. Views use data binding to interact with view model classes.

    > **View models**. View models are classes that encapsulate the application's presentation logic and state. They are part of the MVVM pattern. View models encapsulate much of the application's functionality. View models define properties, commands, and events, to which controls in the view can data-bind.

    > **Models**. Model classes encapsulate the application data and business logic. They are used as part of the MVVM pattern. Models encapsulate data and any associated validation and business rules to ensure data consistency and integrity.

    > **Commands**. Commands are used to encapsulate application functionality in a way that allows them to be defined and tested independently of the application's UI. They can be defined as command objects or as command methods in the view model. Prism provides the **DelegateCommand** class and the **CompositeCommand** class. The latter is used to represent a collection of commands which are all invoked together.

    > **Regions**. Regions are logical placeholders defined within the application's UI (in the shell or within views) into which views are displayed. Regions allow the layout of the application's UI to be updated without requiring changes to the application logic. Many common controls can be used as a region, allowing views to be automatically displayed within controls, such as a **ContentControl**, **ItemsControl**, **ListBox**, or **TabControl**. Views can be displayed within a region programmatically or automatically. Prism also provides support for implementing navigation with regions. Regions can be located by other components through the **RegionManager** component, which uses **RegionAdapter** and **RegionBehavior** components to coordinate the display of views within specific regions.

    > **Navigation**. Navigation is defined as the process by which the application coordinates changes to its UI as a result of the user's interaction with the application or internal application state changes. Prism supports two styles of navigation: state-based navigation, where the state of an existing view is updated to implement simple navigation scenarios, and view-switching navigation, where new views are created and old views replaced within the application's UI. View-switching navigation uses a Uniform Resource Identifier (URI)–based navigation mechanism in conjunction with Prism regions to allow flexible navigation schemes to be implemented.

    > **EventAggregator**. Components in a composite application often need to communicate with other components and services in the application in a loosely coupled way. To support this, Prism provides the **EventAggregator** component, which implements a pub-sub event mechanism, thereby allowing components to publish events and other components to subscribe to those events without either of them requiring a reference to the other. The **EventAggregator** is often used to allow components defined in different modules to communicate with each other.

    > **Dependency injection container**. The Dependency Injection (DI) pattern is used throughout Prism to allow the dependencies between components to be managed. Dependency injection allows component dependencies to be fulfilled at run time, and it supports extensibility and testability. Prism is designed to work with Unity or MEF, or with any other dependency injection containers via the **ServiceLocator**.

    > **Services**. Services are components that encapsulate non-UI related functionality, such as logging, exception management, and data access. Services can be defined by the application or within a module. Services are often registered with the dependency injection container so that they can be located or constructed as required and used by other components that depend on them.

    > **Controllers**. Controllers are classes that are used to coordinate the construction and initialization of views that are to be displayed in a region within the application's UI. Controllers encapsulate the presentation logic that determines which views are to be displayed. The controller will use Prism's view-switching navigation mechanism, which provides an extensible URI-based navigation mechanism to coordinate the construction and placement of views within regions. The Application Controller pattern defines an abstraction that maps to this responsibility.

    > **Bootstrapper**. The **Bootstrapper** component is used by the application to initialize the various Prism components and services. It is used to initialize the dependency injection container to register any application-level components and services with it. It is also used to configure and initialize the module catalog and the shell's view and view model or presenter.

    1.  

    <!-- -->

    1.  Prism is designed so that you can use any of the preceding capabilities and design patterns individually, or all together, depending on your requirements and your application scenario. You can use the MVVM pattern, modularity, regions, commands, or events in any combination without having to adopt all of them. Of course, if you want to take full advantage of the benefits that separation of concerns and loose coupling offers, you will typically use many of Prism's capabilities and design patterns in conjunction with each other. The following illustration shows a typical Prism application architecture and shows how all the various capabilities of Prism can work together within a multi-module composite application.

    <!-- -->

    1.  ![](media/image3.png)

    <!-- -->

    1.  Typical composite application architecture with the Prism Library

    <!-- -->

    1.  Most Prism applications consist of a shell application that defines regions for displaying top-level views and shared services that can be accessed by the loaded modules. The shell defines a suitable catalog to specify which modules are to be loaded at startup time , as appropriate. A dependency injection container is also defined, which allows component dependencies to be fulfilled at run time. Shared services and components are registered with the container by the **Bootstrapper** when the application starts.

        Individual modules encapsulate a portion of the overall application's functionality and, using a separated presentation pattern such as MVVM, define views, view models, models, and service components. When the modules are loaded, views defined within the modules are displayed within the regions defined by the shell. After initialization completes, the user then navigates within the application using state-based or view-switching navigation to coordinate the visual update or display of new views within the application's regions.

Using Prism
-----------

1.  Now that you've seen the major capabilities and design patterns that Prism supports, it's time to see how easily you can start to use Prism when developing a new application. This section provides an overview of the first few steps required to create a basic Prism application. You can extend this basic application to leverage the additional capabilities and design patterns provided by Prism, as required by your scenario.

<!-- -->

1.  **Note:** Although the Prism Library can be easily used to build new composite WPF applications, you can also use Prism with existing applications that want to take advantage of one or more Prism capabilities or design patterns.

<!-- -->

1.  A Prism application typically consists of a shell project and multiple module projects. The following illustration shows common activities needed when developing a composite application using the Prism Library.

<!-- -->

1.  ![](media/image4.png)

<!-- -->

1.  Activities for creating a composite application

<!-- -->

1.  A typical Prism application leverages most or all of the Prism capabilities and design patterns described earlier to be able to fully realize the benefits of the loose coupling and separation of concerns architectural design principles. However, for this example, the steps required to create a basic Prism application that consists of a single module that defines a single view are described.

<!-- -->

1.  **Prism Library References**

    Most of your projects will need to reference the Prism Library assemblies. Prism provides signed binaries through NuGet packages so that you can use the Visual Studio **Manage NuGet Packages** dialog box to add references to them. You can also include the Prism Library projects in your solution and then use project references to them. The latter has the advantage of being able to use features like Go To Definition to step down into the Prism types, as well as being able to build and sign the Prism Library assemblies with your own strong name or certificate as part of your build process.

### Define the Shell

1.  The application shell provides the basic layout for the application. This layout is defined using regions that modules can use to place views. Views, like shells, can use regions to define discoverable areas that content can be added to, as shown in the following illustration. Shells typically set the appearance for the entire application and contain the styles that are used throughout the application.

<!-- -->

1.  ![](media/image5.png)

<!-- -->

1.  Shells, views, and regions

### Create the Bootstrapper

1.  The bootstrapper is the glue that connects the application with the Prism Library services and the Unity or MEF containers. Each application creates an application-specific bootstrapper, which typically inherits from either **UnityBootstrapper** or **MefBootstrapper**, as shown in the following illustration. You will need to decide the approach you want to use to populate the module catalog. Minimally, each application will provide a module catalog and a shell.

<!-- -->

1.  By default, the bootstrapper logs events using the .NET Framework **Trace** class. Most applications will want to supply their own logging services, such as Enterprise Library logging. Applications can supply their logging service in their bootstrapper.

    1.  By default, the **UnityBootstrapper** and **MefBootstrapper** enable the Prism Library services. These can be disabled or replaced in your application-specific bootstrapper.

    <!-- -->

    1.  ![](media/image6.png)

    <!-- -->

    1.  Diagram demonstrating connecting to the Prism Library

### Create the Module

1.  The module contains the views and services specific to a piece of the application's functionality. Frequently, these are contained in separate assemblies and developed by separate teams. A module is denoted by a class that implements the **IModule** interface. These modules, during initialization, register their views and services and may add one or more views to the shell. Depending on your module discovery approach, you may need to apply attributes to your module classes or define dependencies between your modules.

### Add a Module View to the Shell

1.  Modules take advantage of the shell's regions for placing content. During initialization, modules use the **RegionManager** to locate regions in the shell and add one or more views to those regions or register one or more view types to be created within those regions. The **RegionManager** is responsible for keeping track of regions throughout the application and is a core service initialized from the bootstrapper.

    The remaining topics in this guide provide details about Prism key concepts.

<span id="_1.5_Compiling_the" class="anchor"><span id="ExploringPrism" class="anchor"></span></span>Exploring Prism
-------------------------------------------------------------------------------------------------------------------

1.  <span id="WhatsNewinThisRelease" class="anchor"><span id="WhatsIntheBox" class="anchor"></span></span>Prism consists of the following:

<!-- -->

1.  [**Prism Library source code**](http://aka.ms/prism-wpf-code). The source code for the Prism Library assemblies, including the core Prism functionality, plus Unity and MEF extensions, which provide additional components for using Prism with the [Unity Application Block](http://msdn.microsoft.com/en-us/library/dd203101.aspx) (Unity) and the [Managed Extensibility Framework](http://msdn.microsoft.com/en-us/library/dd460648.aspx). The source code also includes Prism.PubSubEvents and Prism.Mvvm assemblies.

    [**Prism binary assemblies**](http://aka.ms/prism-wpf-nuget). Signed binary versions of the Prism Library assemblies. These assemblies can be downloaded from NuGet by searching for Prism, Prism.Composition, Prism.PubSubEvents, and Prism.Mvvm, Prism.Interactivity, Prism.UnityExtensions, and Prism.MefExtensions. These NuGet packages will load dependencies such as the [Unity Application Block](http://msdn.microsoft.com/en-us/library/dd203101.aspx) and the [Service Locator](http://commonservicelocator.codeplex.com/).

    1.  The Prism NuGet package will download the Prism.Composition, Prism.PubSubEvents, Prism.Mvvm, Prism.Interactivity, Prism.PubSubEvents, and Prism.Mvvm NuGet packages.

    [**Code samples**](http://aka.ms/prism-wpf-code). Prism includes a reference implementation sample and QuickStart samples. The Stock Trader Reference Implementation is a comprehensive sample application that illustrates how Prism can be used to implement real-world application scenarios. The reference implementation is intentionally incomplete, but they illustrate how many of the patterns in Prism can work together within a single application. The QuickStart samples include several small, focused sample applications that illustrate the MVVM pattern, navigation, UI composition, modularity, commanding, event aggregation, and interactivity.

    [**Documentation**](http://aka.ms/prism-wpf-doc). The Prism 5.0 documentation provides an overview of the goals and concepts behind Prism and detailed guidance on using each of the capabilities and design patterns provided by Prism. The next section provides an overview of the topics covered.

    1.  

### Exploring the Documentation

1.  The Prism documentation spans a wide range of topics, including an overview of common development challenges and the composite application approach, an overview of the Prism Library and the design patterns that it implements, as well as step-by-step instructions for using the Prism Library during development. The documentation is intended to appeal to a broad technical audience to help the reader to understand and use Prism within their own applications. The documentation includes the following:

<!-- -->

1.  . This topic discusses what needs to happen to get a modular Prism application up and running.

    . Applications based on the Prism Library rely on a dependency injection container. Although Prism has the ability to work with nearly any dependency injection container, the Prism Library provides two default options for dependency injection containers: Unity or MEF. This topic discusses the different capabilities and what you need to think about when working with a dependency injection container.

    . This topic discusses the core concepts, key decisions, and core scenarios when you create a modular client application with Prism.

    . Using the MVVM pattern, you separate the UI of your application and the underlying presentation and business logic into three separate classes: the view, model, and view model. This topic discusses the core concepts behind the MVVM pattern and describes how to implement it in your application using Prism.

    . This topic provides guidance on implementing more advanced scenarios using the MVVM pattern, including how to implement composite commands (commands that represent a group of commands), and how to handle asynchronous web service and user interactions. This topic also provides guidance on using a dependency injection container, such as Unity or MEF, to handle the construction and wire-up of the MVVM classes.

    . Regions are placeholders that allow a developer to specify where views will be displayed in the application's UI. In Prism, there are two approaches to displaying views in a region: view discovery and view injection. This topic describes how to work with regions and the UI. It also includes information for UI designers to understand composite applications.

    . Navigation is the process by which the application coordinates changes to its UI as a result of the user's interaction with the application or internal application state changes. This topic provides guidance on implementing state-based navigation, where the state of the UI in a view is updated to reflect navigation, and view-switching navigation, where a new view is created and displayed in a region.

    . This topic discusses the various options for communicating between components in different modules, using commanding, the EventAggregator, region context, and shared services.

    . This topic addresses deployment considerations for Prism WPF applications.

    . This appendix provides a concise summary of the terms, concepts, design patterns and capabilities provided by Prism.

    . This appendix describes the software design patterns applied in the Prism Library and the Stock Trader RI. This topic primarily targets architects and developers wanting to familiarize themselves with the patterns used to address the challenges in building composite applications.

    . This appendix provides an overview of the Prism Library for WPF.

    . This appendix discusses what you need to know if you are upgrading from previous versions of Prism.

    . This appendix discusses how you can extend Prism modularity, behaviors, and navigation.

    . Prism includes the source code for several samples that demonstrate key concepts. For more information, see the next section, "Code Samples Using the Prism Library for WPF."

    . This hands-on labs demonstrates building a simple composite application, step-by-step, in WPF. It primarily targets developers who want to understand the basic concepts of the Prism Library.

    . This hands-on lab walks you through the process of for publishing and updating a Prism WPF application with ClickOnce.

    1.  

### <span id="QuickStarts" class="anchor"><span id="ExploringtheQuickStarts" class="anchor"></span></span>Exploring the Code Samples

1.  The code samples illustrate specific Prism-related concepts. The samples are an ideal starting point if you want to gain an understanding of a key concept and you are comfortable learning new techniques by examining source code. Prism includes the following:

<!-- -->

1.  . The Stock Trader RI is a composite application that demonstrates an implementation of the baseline architecture using the Prism Library.

    . These QuickStarts demonstrate how to build WPF applications composed of modules. The modules can be statically loaded, when the shell contains a reference to the module's assembly, or dynamically loaded, when modules are dynamically discovered and loaded at run time. The QuickStarts also demonstrate using the Unity container and MEF.

    . This QuickStart demonstrates how to create a view and view model that work together when the view model needs to interact with the user or a user gesture needs to raise an event that invokes a command. In each of these scenarios the view model should not need to know about the view. The first scenario is handled by using **InteractionRequests** and **InteractionRequestTriggers**. The second scenario is handled by **InvokeCommandAction**.

    . This QuickStart demonstrates how to build an application that implements the MVVM presentation pattern, showing some of the more common challenges that developers can face, such as validation, UI interactions, and data templates.

    . This QuickStart demonstrates how to build a WPF UI that uses commands provided by the Prism.Mvvm Library to handle UI actions in a decoupled way.

    . This QuickStart demonstrates how to build WPF UIs composed of different views that are dynamically loaded into regions and that interact with each other in a decoupled way. It illustrates how to use both the view discovery and view injection approaches for UI composition.

    . This QuickStart demonstrates an approach to define the navigation of a simple application. The approach used in this QuickStart uses the WPF Visual State Manager (VSM) to define the different states that the application has and defines animations for both the states and the transitions between states.

    . This QuickStart demonstrates how to use the Prism Region Navigation API. The QuickStart shows multiple navigation scenarios, including navigating to a view in a region, navigating to a view in a region contained in another view (nested navigation), navigation journal support, just-in-time view creation, passing contextual information when navigating to a view, views and view models participating in navigation, and using navigation as part of an application built through modularity and UI composition.

    . This QuickStart demonstrates how to build a WPF application that uses the Event Aggregator service. This service enables you to establish loosely coupled communications between components in your application.

<!-- -->

1.  

<span id="ExploringtheReferenceImplementations" class="anchor"><span id="UpgradingfromEarlierReleases" class="anchor"><span id="AnOverviewofPrism" class="anchor"><span id="PrismDesignGoals" class="anchor"><span id="UsingPrism" class="anchor"><span id="DefinetheShell" class="anchor"><span id="CreatetheBootstrapper" class="anchor"><span id="CreatetheModule" class="anchor"><span id="AddaModuleViewtotheShell" class="anchor"></span></span></span></span></span></span></span></span></span>More Information
=======================================================================================================================================================================================================================================================================================================================================================================================================================================================================================================================

1.  Prism assumes you have hands-on experience with WPF. If you need general information about WPF , see the following resources:

-   [Windows Presentation Foundation](http://msdn2.microsoft.com/en-us/library/ms754130.aspx) on MSDN.

-   MacDonald, Matthew. *Pro WPF in C\# 2010: Windows Presentation Foundation in .NET 4*, Apress, 2010.

-   Nathan, Adam. *WPF 4 Unleashed*. Sams Publishing, 2010.

1.  

Community
---------

1.  Prism's community sites are:

<!-- -->

1.  Prism: <http://www.codeplex.com/Prism>

    PubSubEvents (Event Aggregator): <http://www.codeplex.com/pnpPubSub>

    MVVM (Model-View-ViewModel): <http://www.codeplex.com/pnpMvvm>

    1.  On this these community sites, you can post questions, provide feedback, or connect with other users for sharing ideas. Community members can also help Microsoft plan and test future offerings and download additional content, such as extensions and training material.


