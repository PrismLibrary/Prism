1.  <span id="_Toc314838515"
    > class="anchor"></span>**\#!155CharTopicSummary!\#:**

    > Learn what’s included in Prism 5.0 and how to compile and run the
    > associated WPF code samples.

<!-- -->

1.  Learn what’s included in Prism 5.0 including the documentation, WPF
    code samples, and libraries. Additionally find out where to get the
    library and sample source code and the library NuGet packages.

    For a list of the new features, assets, and API changes, see .

Documentation
=============

1.  Prism includes the following documentation:

<!-- -->

1.  [Developer's Guide to Microsoft Prism 5.0 on
    MSDN](http://aka.ms/prism-wpf-doc).

    [Prism Reference Documentation on
    MSDN](http://aka.ms/prism-wpf-prism50refdoc).

    [Developer's Guide to Microsoft Prism 5.0 in .pdf
    format](http://aka.ms/prism-wpf-pdf).

    [Prism Reference Documentation in chm
    format](http://aka.ms/prism-wpf-Prism50RefDocChm).

    1.  

NuGet Packages
==============

1.  [Prism](http://aka.ms/prism-wpf-Prism50Nuget): Downloads NuGet
    dependency packages—Prism.Composition, Prism.Interactivity,
    Prism.Mvvm, and Prism.PubSubEvents NuGet Packages.

    [Prism.Composition](http://aka.ms/prism-wpf-Prism50CompositionNuget):
    Modularity, UI Composition, Bootstrapping, Interactivity,
    IActiveAware, Navigation, and deprecated NotificationObject
    and PropertySupport.

    [Prism.Interactivity](http://aka.ms/prism-wpf-Prism50InteractivityNuget): Interactivity.

    [Prism.Mvvm](http://aka.ms/prism-wpf-Prism50MvvmNuget): The Portable
    Class Library for MVVM and the associated platform specific code to
    support MVVM. Includes Commanding, BindableBase, ErrorsContainer,
    IView, and ViewModelLocationProvider.

    [Prism.PubSubEvents](http://aka.ms/prism-wpf-Prism50PubSubEventsNuget):
    The Portable Class Library for PubSubEvents.

    [Prism.UnityExtensions](http://aka.ms/prism-wpf-Prism50UnityExtensionsNuget):
    Use these extensions to Prism to build Prism applications based
    on Unity.

    [Prism.MefExtensions](http://aka.ms/prism-wpf-Prism50MefExtensionsNuget):
    Use these extensions to Prism to build Prism applications based on
    Managed Extensibility Framework (MEF).

    1.  

    <!-- -->

    1.  The following table shows common Prism namespaces and in which
        assemblies and NuGet packages they can be found.

  ----------------------------------------------------------------------------------------------------------------------------------------
  Namespace                                                                Assembly                                  NuGet Package
  ------------------------------------------------------------------------ ----------------------------------------- ---------------------
  Microsoft.Practices.Prism.Logging                                        Microsoft.Practices.Prism.Composition     Prism.Composition
                                                                                                                     
  Microsoft.Practices.Prism.Modularity Microsoft.Practices.Prism.Regions                                             

  Microsoft.Practices.Prism.Interactivity                                  Microsoft.Practices.Prism.Interactivity   Prism.Interactivity

  Microsoft.Practices.Prism.Commands                                       Microsoft.Practices.Prism.Mvvm            Prism.Mvvm
                                                                                                                     
  Microsoft.Practices.Prism.Mvvm                                                                                     
                                                                                                                     
  Microsoft.Practices.Prism.ViewModel                                                                                

  Microsoft.Practices.Prism.PubSubEvents                                   Microsoft.Practices.Prism.PubSubEvents    Prism.PubSubEvents
  ----------------------------------------------------------------------------------------------------------------------------------------

<span id="_INSTALLING_AND_COMPILING" class="anchor"><span id="InstallingPrism" class="anchor"></span></span>Download and Setup the Prism Source Code
====================================================================================================================================================

1.  This section describes how to install Prism. It involves the
    following three steps:

<!-- -->

1.  Install system requirements.

2.  Download and extract the Prism source code and documentation.

3.  Compile and run the QuickStarts, Reference Implementation, or Prism
    > Library source code.

4.  

<span id="InstallSysReq" class="anchor"><span id="_Toc276376481" class="anchor"></span></span>Step 1: Install System Requirements 
----------------------------------------------------------------------------------------------------------------------------------

1.  Prism was designed to run on the Microsoft Windows 8 desktop,
    Microsoft Windows 7, Windows Vista, or Windows Server 2008
    operating system. WPF applications built using this guidance require
    the .NET Framework 4.5.

    Before you can use the Prism Library, the following must be
    installed:

<!-- -->

1.  Microsoft .NET Framework 4.5 (installed with Visual Studio 2012) or
    Microsoft .NET Framework 4.51.

    Microsoft Visual Studio 2012 or 2013 Professional, Premium, or
    Ultimate editions.

<!-- -->

1.  **Note:** Visual Studio 2013 Express Edition can be used to develop
    Prism applications using the Prism Library.

    1.  

    <!-- -->

    1.  Optionally, you should consider also installing the following:

<!-- -->

1.  [Microsoft Blend for Visual Studio
    2013](http://www.microsoft.com/expression/products/Blend_Overview.aspx).
    A professional design tool for creating compelling user experiences
    and applications for WPF.

    1.  

Step 2: Download and Extract the Prism Source Code and Documentation
--------------------------------------------------------------------

1.  You can download the source code for the Prism library, the
    reference implementation and the QuickStarts from the following
    link:

<!-- -->

1.  [**Prism 5.0**](http://aka.ms/prism-wpf-code)

    1.  

    <!-- -->

    1.  To install the Prism assets, right-click the exe file or zip
        file, and then click **Run as administrator**. This will extract
        the source code into the folder of your choice. <span
        id="_1.5_Compiling_the" class="anchor"><span id="_Toc276376482"
        class="anchor"></span></span>

<!-- -->

1.  **Note:** The Stock Trader Reference Implementation and the
    QuickStarts can also be downloaded separately. The table below
    provides links to the source code for each.

  ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Sample                                                                                  Category            Description
  --------------------------------------------------------------------------------------- ------------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  [Stock Trader Reference Implementation](http://aka.ms/prism-wpf-RICode)                 Prism               The Stock Trader RI application is a reference implementation that illustrates the baseline architecture. Within the application, you will see solutions for common, and recurrent, challenges that developers face when creating composite WPF applications.
                                                                                                              
                                                                                                              The Stock Trader RI illustrates a fictitious, but realistic financial investments scenario. Contoso Financial Investments (CFI) is a fictional financial organization that is modeled after real financial organizations. CFI is building a new composite application to be used by their stock traders.

  [Hello World Hands-on Lab](http://aka.ms/prism-wpf-QSHelloWorldCode)                    Get Started         The Hello World Hands-on Lab demonstrates the end solution for the hands-on lab "" In this lab, you will learn the basic concepts of Prism and apply them to create a Prism Library solution that you can use as the starting point for building a composite WPF.

  1.  [Modularity QuickStarts for Unity](http://aka.ms/prism-wpf-QSModularityUnityCode)   Modularity          The Modularity QuickStarts demonstrate how to code, discover, and initialize modules using Prism. These QuickStarts represent an application composed of several modules that are discovered and loaded in the different ways supported by the Prism Library using MEF and Unity as the composition containers.
                                                                                                              
      [Modularity QuickStarts for MEF](http://aka.ms/prism-wpf-QSModularityMEFCode)                           
                                                                                                              
      1.                                                                                                      
                                                                                                              
                                                                                                              

  [MVVM QuickStart](http://aka.ms/prism-wpf-QSMVVMCode)                                   MVVM                The MVVM QuickStart demonstrates how to build an application that implements the MVVM presentation pattern, showing some of the more common challenges that developers can face, such as wiring a view and view model using the ViewModelLocator, validation, UI interactions, and data templates.

  [Commanding QuickStart](http://aka.ms/prism-wpf-QSCommandingCode)                       Commanding          The Commanding QuickStart demonstrates how to build a WPF UI that uses commands provided by the Prism Library to handle UI actions in a decoupled way.

  [UI Composition QuickStart](http://aka.ms/prism-wpf-QSUICompositionCode)                UI Composition      This QuickStart demonstrates how to build WPF UIs composed of different views that are dynamically loaded into regions and that interact with each other in a decoupled way. It illustrates how to use both the view discovery and view injection approaches for UI composition.

  [State-Based Navigation QuickStart](http://aka.ms/prism-wpf-QSStateBasedNavCode)        Navigation          This QuickStart demonstrates an approach to define the navigation of a simple application. The approach used in this QuickStart uses the WPF Visual State Manager (VSM) to define the different states that the application has and defines animations for both the states and the transitions between states.

  [View-Switching Navigation QuickStart](http://aka.ms/prism-wpf-QSViewSwitchNavCode)     Navigation          This QuickStart demonstrates how to use the Prism Region Navigation API. The QuickStart shows multiple navigation scenarios, including navigating to a view in a region, navigating to a view in a region contained in another view (nested navigation), navigation journal support, just-in-time view creation, passing contextual information when navigating to a view, views and view models participating in navigation, and using navigation as part of an application built through modularity and UI composition.

  [Event Aggregation QuickStart](http://aka.ms/prism-wpf-QSEACode)                        Event Aggregation   This QuickStart demonstrates how to build a WPF application that uses the Event Aggregator service. This service enables you to establish loosely coupled communications between components in your application.

  [Interactivity QuickStart](http://aka.ms/prism-wpf-QSInteractivityCode)                 Interactivity       This QuickStart demonstrates how to create a view and view model that work together when the view model needs to interact with the user or user gesture needs to raise an event that invokes a command. In each of these scenarios the view model should not need to know about the view. The first scenario is handled by using **InteractionRequests** and **InteractionRequestTriggers**. The second scenario is handled by **InvokeCommandAction**.
  ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Step 3: Compile and Run QuickStarts, Reference Implementation, or Prism Library Source Code
-------------------------------------------------------------------------------------------

1.  In order to build and run the reference implementation and the
    QuickStarts, select the appropriate shortcut file and press F5 to
    build and run.

    The reference implementation and QuickStarts use NuGet references to
    the Prism library assemblies so you can compile and run each
    solution directly.

Adding Prism Library Source Projects to Solutions
=================================================

1.  As part of shipping the Prism Library as NuGet packages, the Prism
    Library projects were removed from the solutions of all QuickStarts
    and reference implementation projects. If you are a developer
    accustomed to stepping through the Prism Library code as you build
    your application, there are a couple of options:

<!-- -->

1.  **Add the Prism Library Projects back in**. To do this, right-click
    the solution, point to **Add**, and then click **Existing project**.
    Select the Prism Library projects. Then, to prevent inadvertently
    building these, click **Configuration Manager** on the **Build**
    menu, and then clear the **Build** check box for all Prism Library
    projects in both the debug and release configurations.

    **Set a breakpoint and step in**. Set a break point in your
    application's bootstrapper, and then step in to a method within the
    base class (F11 is the typical C\# keyboard shortcut for this). You
    may be asked to locate the Prism Library source code, but often, the
    full program database (PDB) file is available and the file will
    simply open. You may set breakpoints in any Prism Library project by
    opening the file and setting the breakpoint.

    1.  

Related Downloads
=================

-   [ManifestManagerUtility for
    ClickOnce](http://compositewpf.codeplex.com/releases/view/14771)

-   [MVVM
    Training](http://visualstudiogallery.msdn.microsoft.com/3ab5f02f-0c54-453c-b437-8e8d57eb9942)

1.  1.  


