1.  **\#!155CharTopicSummary!\#:**

    Learn what's changed in Prism for WPF between 4.1 and 5.0 releases.

    1.  Prism 5.0 includes guidance in several new areas, resulting in new code in the Prism Library for WPF, new and updated QuickStarts, and updated documentation. Parts of the Prism Library changed between Prism 4.1 and Prism 5.0 to support the new guidance, fix existing issues, and respond to community requests.

        **Note:** For .NET 4.0, Silverlight, or Windows Phone 7.x applications use .

New Guidance
============

1.  Prism 5.0 contains several areas of new guidance as follows:

<!-- -->

1.  **Prism.Mvvm** is a portable class library that encapsulates Prism’s MVVM support. This library includes **ViewModelLocationProvider**. Views and view models can be wired up together using the new **ViewModelLocationProvider**’s convention-based approach as explained in . View model construction can be accomplished using a dependency injection container. The ViewModel Locator’s extensibility points are discussed in . **DelegateCommands** are now extendable and provide Async support. A new implementation of the **INotifyPropertyChanged** interface, the **BindabaleBase** class, was added.

    **The PopupWindowAction** class was added to the **Prism.Interactivity** assembly to open a custom window in response to an interaction request being raised.
    The **InvokeCommandAction** action provided by Prism now passes trigger parameters to the associated command.
    For more information see .

    The **EventAggregator** classes have been moved to the **Prism.PubSubEvents** portable class library.

    The **NavigationParameters** class can now be used to pass object parameters during navigation, using the overloads of the **RequestNavigate** method of a **Region** or **RegionManager** instance.

    1.  

Changes in the Prism Library
============================

1.  Prism Library 5.0 includes changes related to new functionality, code organization, and APIs.

Code Organization
-----------------

1.  The following organizational changes were made to the library:

<!-- -->

1.  The **Microsoft.Practices.Prism.PubSubEvents** portable class library contains **PubSubEvents**, **EventAggregator** and related classes.

    The **Microsoft.Practices.Prism.Mvvm** portable class library contains **ViewModelLocationProvider**, **BindableBase**, **ErrorsContainer, PropertySupport, CompositeCommand, DelegateCommand, DelegateCommandBase, and WeakEventHandlerManager**.

    The **Microsoft.Practices.Prism.Mvvm.Desktop** assembly contains the WPF **ViewModelLocator** attach property.

    The **EventAggregator** classes are marked obsolete in the Prism assembly.

    The Silverlight and phone versions of the Prism Library were taken out. If you need these libraries download the Prism 4.1 assemblies from NuGet.

    Prism and **Prism.Mvvm** are independent of each other but share **IActiveAware**. Therefore **IActiveAware** has been moved to **Prism.SharedInterfaces**.

    1.  

    <!-- -->

    1.  In version 5.0 of Prism, Pub Sub Eventing functionality was moved into a separate assembly (Prism.PubSubEvents). **DelegateCommand**, **CompositeCommand,** and **ViewModel** support were moved into another assembly (**Prism.Mvvm**). There are many advantages to separating **PubSubEvents** and **Mvvm** from the core Prism assembly.

    You can select only the functionality that you need. If you want Regions and Modularity, you use the core Prism assembly. If you want only **ViewModel** and commanding support, you use Prism.Mvvm. If you only want Pub Sub Eventing, you use **Prism.PubSubEvents**. Each assembly is smaller and easier to understand.

    You can now build your **ViewModel** code in a portable class library that leverages **Prism.Mvvm** and/or **Prism.PubSubEvents** since both are PCLs. By putting your **ViewModel** code in a PCL, your **ViewModel** code is constrained to using dependent libraries that are platform agnostic and thus can target multiple platforms.

    Updates to these smaller libraries can be made more easily and quickly.

    1.  

API Changes
-----------

1.  The Prism Library API changed in several key areas. The bootstrapper was heavily modified and reusable code was added to support the new areas of guidance provided in Prism.

### MVVM and Event Aggregator Changes. 

1.  Moving **ViewModel** and **EventAggregator** to a PCL causes several changes to the Prism Library. These changes include the following:

<!-- -->

1.  The **BindableBase** class in **Prism**.**Mvvm** should be used instead of **NotifcationObject**. The **NotificationObject** and **PropertySupport** classes are marked obsolete in the Prism assembly.

    When inheriting from the **BindableBase** class, use the **SetProperty** method to update the property's backing field and raise the corresponding property change event. A new **OnPropertyChanged** method that takes a lambda expression as a parameter has been added.

    Use the **PubSubEvents** class in the **Microsoft**.**Practices**.**PubSubEvents** portable class library instead of **CompositePresentationEvents**. The classes from the Events solution folder in the Prism assembly are marked obsolete.

    The **UriQuery** class was renamed to **NavigationParameters**, it keeps the same functionality as before, and adds support for passing object parameters.

    **DelegateCommand** includes support for async handlers and has been moved to the **Prism.Mvvm** portable class library. **DelegateCommand** and **CompositeCommand** both use the **WeakEventHandlerManager** to raise the **CanExecuteChanged** event. The **WeakEventHandlerManager** must be first constructed on the UI thread to properly acquire a reference to the UI thread’s **SynchronizationContext**.

    **EventAggregator** now must be constructed on the UI thread to properly acquire a reference to the UI thread’s **SynchronizationContext**.

    The **WeakEventHandlerManager is now** public.

    The **Execute** and **CanExecute** methods on **DelegateCommand** are now marked as virtual.

    1.  

### Additions to the Prism Library Core API

1.  **Microsoft.Practices.Prism.PubSubEvents** was added to help you send loosely coupled message using a portable class library.

    **Microsoft.Practices.Prism.Mvvm** was added to assist you in implementing MVVM using a portable class library and several platform specific libraries.

    **Microsoft.Practices.Prism.SharedInterfaces** has been added to share the **IActiveAware** interface between **Prism** and **Prism.Mvvm** assemblies, therefore the **IActiveAware** interface has been moved to this assembly. It is also intended for future use.

    1.  

CodePlex Issues Resolved
========================

1.  **8532: InteractionRequestTrigger** can cause memory leaks with some implementations.

    **9153:** 'Notification' really should be an interface.

    **9438:** Navigation to an existing view.

    **5495:** Event to Command.

    **8101: DelegateCommand** is not extendable.

    **5623:** Make **WeakEventHandlerManager** public.

    **9906**: A bug when using XAML module catalog in WPF.

    **7215:** Issue with ModuleCatalog.CreateFromXaml for WPF application with MEF Bootstrapper.

    **8703:** RegionManager::IsInDesignMode.

    **4349:** Default Region Behavior Order Problem.

    **3552:** Region manager in V2 fails to recognize non-WPF applications.

    1.  

Example Code Changes
====================

1.  Prism 5.0 contains eleven separate code samples that demonstrate portions of the provided guidance. Several samples from Prism 4.1 were removed or replaced, and new samples added.

<!-- -->

1.  **Basic MVVM QuickStart**. This QuickStart shows a very simple MVVM application that uses the **ViewModel** Locator and show a parent and child **ViewModels**. For more information, see the MVVM QuickStart**.**

    -   **MVVM QuickStart**. This QuickStart was removed for this version.

    -   **MVVM Reference Implementation**. This reference implementation was removed for this version.

    -   **View-Switching Navigation QuickStart**. This QuickStart now supports WPF. It demonstrates how to use the Prism region navigation API. For more information, see .

    -   **State-Based Navigation QuickStart**. This QuickStart now supports WPF. It shows an approach that uses the Visual State Manager to define the views (states) and the allowed transitions. For more information, see .

    -   **UI Composition QuickStart**. This QuickStart now supports WPF. It replaced the View Injection QuickStart and the View Discovery QuickStart from Prism 2.0. In the current versions, both concepts are shown in one example application. For more information, see .

    -   **Interactivity QuickStart**. This new QuickStart demonstrates how to exposes an interaction request to the view through the view model. The interactions can be a popup, confirmation, custom popup, and a more complex case where the popup needs a custom view model. It also shows Prism’s **InvokeCommandAction** action that passes the **EventArgs** from the trigger, as a command parameter. For more infromation, see .

    1.  

NuGet Packages Now Available
============================

1.  In your application, you can now use NuGet to add references to the Prism assemblies. These packages include:

<!-- -->

1.  Prism

    [Prism.Composition](http://aka.ms/prism-wpf-Prism50CompositionNuget)

    Prism.Interactivity

    [Prism.Mvvm](http://aka.ms/prism-wpf-Prism50MvvmNuget)

    [Prism.PubSubEvents](http://aka.ms/prism-wpf-Prism50PubSubEventsNuget)

    [Prism.UnityExtensions](http://aka.ms/prism-wpf-Prism50UnityExtensionsNuget)

    [Prism.MEFExtensions](http://aka.ms/prism-wpf-Prism50MefExtensionsNuget)

    1.  

    <!-- -->

    1.  The Prism NuGet package will download the Prism.Composition, Prism.Interactivity, Prism.Mvvm, and Prism.PubSubEvents packages. You only need to add references to the Prism.Interactivity, Prism.Mvvm, and Prism.PubSubEvents if you need finer granularity.

More Information
================

1.  For more information about how to upgrade a solution from version 4.1 to version 5.0 of the Prism Library, see .


