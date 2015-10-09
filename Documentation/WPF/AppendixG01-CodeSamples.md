1.  **\#!155CharTopicSummary!\#:**

    Code samples that illustrate Prism-related WPF concepts. Modularity, MVVM, Commands, UI Composition, Navigation, Event Aggregator, Composite Application.

    1.  The code samples for the Prism Library for WPF are focused applications that illustrate specific Prism-related concepts. The QuickStarts and Reference Implementation are an ideal starting point if you want to gain an understanding of a key concept such as Modularity, MVVM, Commands, UI Composition, Navigation, Event Aggregations, User Interactivity, and Composite Application. The Stock Trader Reference Implementation demonstrates proven practices for implementing composite applications. The samples include both source code and documentation.

        In order to build and run the samples select the appropriate shortcut file and press F5 to build and run.

Installing Prism
================

1.  This section describes how to install Prism. It involves the following three steps:

<!-- -->

1.  Install system requirements.

2.  Extract the Prism source code and documentation.

3.  Compile and run QuickStarts, Reference Implementation or Prism Library source code.

<!-- -->

1.  

Step 1: Install System Requirements 
------------------------------------

1.  Prism was designed to run on the Microsoft Windows 8 desktop, Microsoft Windows 7, Windows Vista, or Windows Server 2008 operating system. WPF applications built using this guidance require the .NET Framework 4.5.

    Before you can use the Prism Library, the following must be installed:

-   Microsoft .NET Framework 4.5 (installed with Visual Studio 2012) or Microsoft .NET Framework 4.51.

-   Microsoft Visual Studio 2012 Professional, Premium, or Ultimate editions or Microsoft Visual Studio 2013 Professional, Premium, or Ultimate editions.

1.  **Note:** Visual Studio 2013 Express Edition can be used to develop Prism applications using the Prism Library.

<!-- -->

1.  1.  Optionally, you should consider also installing the following:

    -   [Microsoft Blend for Visual Studio 2013](http://www.microsoft.com/expression/products/Blend_Overview.aspx). A professional design tool for creating compelling user experiences and applications for WPF.

Step 2: Extract the Prism Source Code, and Documentation
--------------------------------------------------------

1.  To install the Prism assets, right-click the downloaded file, and then click **Run as administrator**. This will extract the source code and documentation into the folder of your choice. <span id="_1.5_Compiling_the" class="anchor"><span id="_Toc276376482" class="anchor"></span></span>You may also need to right-click the file and unblock before you can extract the contents.

Step 3: Compile and run QuickStarts, Reference Implementation or Prism Library source code.
-------------------------------------------------------------------------------------------

1.  In order to build and run the code sample, select the appropriate shortcut file and press F5 to build and run.

| Name | Code sample download from Code Gallery                                                             | Category          | Summary                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
|------|----------------------------------------------------------------------------------------------------|-------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|      | [Download Stock Trader RI code](http://aka.ms/prism-wpf-RICode)                                    | Prism             | The Stock Trader RI application is a reference implementation that illustrates the baseline architecture. Within the application, you will see solutions for common, and recurrent, challenges that developers face when creating composite WPF applications.                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
                                                                                                                                 The Stock Trader RI illustrates a fictitious, but realistic financial investments scenario. Contoso Financial Investments (CFI) is a fictional financial organization that is modeled after real financial organizations. CFI is building a new composite application to be used by their stock traders.                                                                                                                                                                                                                   |
|      | [Download Hello World QuickStart code](http://aka.ms/prism-wpf-QSHelloWorldCode)                   | Get Started       | The Hello World QuickStarts are the ending solution for the . In this lab, you will learn the basic concepts of Prism and apply them to create a Prism Library solution that you can use as the starting point for building a composite WPF.                                                                                                                                                                                                                                                                              |
|      | 1.  [Download Modularity QuickStart code for Unity](http://aka.ms/prism-wpf-QSModularityUnityCode) 
                                                                                                            
            [Download Modularity QuickStart code for MEF](http://aka.ms/prism-wpf-QSModularityMEFCode)      
                                                                                                            
            1.                                                                                              | Modularity        | The Modularity QuickStarts demonstrate how to code, discover, and initialize modules using Prism. These QuickStarts represent an application composed of several modules that are discovered and loaded in the different ways supported by the Prism Library using MEF and Unity as the composition containers.                                                                                                                                                                                                           |
|      | [Download Interactivity QuickStart code](http://aka.ms/prism-wpf-QSInteractivityCode)              | Interactivity     | This QuickStart demonstrates how to create a view and view model that work together when the view model needs to interact with the user or user gesture needs to raise an event that invokes a command. In each of these scenarios the view model should not need to know about the view. The first scenario is handled by using **InteractionRequests** and **InteractionRequestTriggers**. The second scenario is handled by **InvokeCommandAction**.                                                                   |
|      | [Download MVVM QuickStart code](http://aka.ms/prism-wpf-QSMVVMCode)                                | MVVM              | The MVVM QuickStart demonstrates how to build a very simple application that implements the MVVM pattern.                                                                                                                                                                                                                                                                                                                                                                                                                 |
|      | [Download Command QuickStart code](http://aka.ms/prism-wpf-QSCommandingCode)                       | Commanding        | The Commanding QuickStart demonstrates how to build a WPF UI that uses commands provided by the Prism Library to handle UI actions in a decoupled way.                                                                                                                                                                                                                                                                                                                                                                    |
|      | [Download UI Composition QuickStart code](http://aka.ms/prism-wpf-QSUICompositionCode)             | UI Composition    | This QuickStart demonstrates how to build WPF UIs composed of different views that are dynamically loaded into regions and that interact with each other in a decoupled way. It illustrates how to use both the view discovery and view injection approaches for UI composition.                                                                                                                                                                                                                                          |
|      | [State-Based Navigation QuickStart](http://aka.ms/prism-wpf-QSStateBasedNavCode)                   | Navigation        | This QuickStart demonstrates an approach to define the navigation of a simple application. The approach used in this QuickStart uses the WPF Visual State Manager (VSM) to define the different states that the application has and defines animations for both the states and the transitions between states.                                                                                                                                                                                                            |
|      | [Download View-Switching Navigation QuickStart code](http://aka.ms/prism-wpf-QSViewSwitchNavCode)  | Navigation        | This QuickStart demonstrates how to use the Prism Region Navigation API. The QuickStart shows multiple navigation scenarios, including navigating to a view in a region, navigating to a view in a region contained in another view (nested navigation), navigation journal support, just-in-time view creation, passing contextual information when navigating to a view, views and view models participating in navigation, and using navigation as part of an application built through modularity and UI composition. |
|      | [Download Event Aggregation QuickStart code](http://aka.ms/prism-wpf-QSEACode)                     | Event Aggregation | This QuickStart demonstrates how to build a WPF application that uses the Event Aggregator service. This service enables you to establish loosely coupled communications between components in your application.                                                                                                                                                                                                                                                                                                          |


