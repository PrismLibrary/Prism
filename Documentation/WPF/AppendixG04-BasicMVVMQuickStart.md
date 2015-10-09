1.  **\#!155CharTopicSummary!\#:**

    Learn how to separate the view from the state and view logic into a separate view model class, and the application data model into a model class.

    1.  The Model-View-ViewModel (MVVM) QuickStart provides sample code that demonstrates how to separate the state and logic that support a view into a separate class named **ViewModel** using the Prism Library. The view model sits on top of the application data model to provide the state or data needed to support the view, insulating the view from needing to know about the full complexity of the application. The view model also encapsulates the interaction logic for the view that does not directly depend on the view elements themselves. This QuickStart provides a tutorial on implementing the MVVM pattern.

        A common approach to designing the views and view models in an MVVM application is the first sketch out or storyboard for what a view looks like on the screen. Then you analyze that screen to identify what properties the view model needs to expose to support the view, without worrying about how that data will get into the view model. After you define what the view model needs to expose to the view and implement that, you can then dive into how to get the data into the view model. Often, this involves the view model calling to a service to retrieve the data, and sometimes data can be pushed into a view model from some other code such as an application controller.

        This QuickStart leads you through the following steps:

<!-- -->

1.  Analyzing the view to decide what state is needed from a view model to support it

    Defining the view model class with the minimum implementation to support the view

    Defining the bindings in the view that point to view model properties

    Attaching the view to the view model

    1.  

Business Scenario
=================

1.  The main window of the Basic MVVM Application QuickStart represents a subset of a survey application. In this window, an empty survey with different types of questions is shown; and there is a button to submit the questionnaires. The following illustration shows the QuickStart main window.

<!-- -->

1.  <span id="_Ref196790494" class="anchor"></span>![](media/image1.png)

<!-- -->

1.  MVVM QuickStart user interface

Building and Running the QuickStart
===================================

1.  This QuickStart requires Microsoft Visual Studio 2012 or later and the .NET Framework 4.5.1.

To build and run the MVVM QuickStart

1.  In Visual Studio, open the solution file Quickstarts\\BasicMVVMQuickstart\_Desktop\\BasicMVVMQuickstart\_Desktop.sln.

2.  In the **Build** menu, click **Rebuild Solution**.

3.  Press F5 to run the QuickStart.

4.  

Implementation Details
======================

1.  The QuickStart highlights the key elements and considerations to implement the MVVM pattern in a basic application.

Analyzing What Properties Are Needed on the View Model
------------------------------------------------------

1.  Open the Views\\MainWindow view in the designer. The list of color selections will be dynamically populated. When analyzing a view to define a view model, you want to identify each individual item of data and behavior that you need. In this case, assuming the questions are fixed and will not be dynamically driven, you need the following properties exposed from your view model:

<!-- -->

1.  Name: string

    > Age: int

    > Quest: string

    > FavoriteColor: string

    > Submit: Command

    > Reset: Command

    1.  

    <!-- -->

    1.  Because the first four properties are related to questionnaires, a questionnaire class is created to store them. The questionnaire class will be the model of the application, and the view model will only expose a property of type **Questionnaire** to support them.

        Note that even things like buttons represent something that needs support from the view model. You can either expose a command, as shown in this QuickStart, or you can expose a method. With the former, you will need a property exposed from the view model with an object that implements the **ICommand** interface; with the latter, you need a behavior that can target a method.

        As we want to demonstrate parent-child view model composition, the application UI is composed by two views: **MainWindow**, which contains the **Reset** and **Submit** buttons and an instance of the second view, which is the **QuestionnaireView** that includes the questionnaire's questions.

        The **QuestionnaireView** is directly instantiated in the XAML code, as seen in the following code.

    <!-- -->

    1.  XAML

    <!-- -->

    1.  &lt;Window x:Class="BasicMVVMQuickstart\_Desktop.Views.MainWindow" ...&gt;

        &lt;Grid x:Name="LayoutRoot"

        Background="{StaticResource MainBackground}"&gt;

        &lt;Grid MinWidth="300" MaxWidth="800"&gt;

        ...

        &lt;views:QuestionnaireView Grid.Row="1"

        DataContext="{Binding QuestionnaireViewModel}"

        Height="246" VerticalAlignment="Top"&gt;

        &lt;/views:QuestionnaireView&gt;

        ...

        &lt;/Grid&gt;

        &lt;/Grid&gt;

        &lt;/Window&gt;

    <!-- -->

    1.  In order to populate this child view with its corresponding view model, its **DataContext** is set to a property of the **MainWindow's** view model that contains an instance of the child **QuestionnaireView's** view model.

Implementing the View Model to Support the View
-----------------------------------------------

1.  Open the QuestionnaireViewModel.cs file. The view model implements the Questionnaire and **AllColors** properties and derives from the **BindableBase** class.

<!-- -->

1.  C\#

<!-- -->

1.  public class QuestionnaireViewModel : BindableBase

    {

    private Questionnaire questionnaire;

    public QuestionnaireViewModel()

    {

    this.Questionnaire = new Questionnaire();

    this.AllCollors = new\[\] { “Red”, “Blue”, “Green” };

    }

    public Questionnaire Questionnaire

    {

    get { return this.questionnaire; }

    set { SetProperty(ref this.questionnaire, value); }

<!-- -->

1.  }

    public IEnumerable&lt;string&gt; AllCollors { get; private set; }

    }

    1.  The **INotifyPropertyChanged** interface is implemented on the **BindableBase** base class. The property changed notification is added to the whole **Questionnaire** property, using the **SetProperty** method of the **BindableBase** class as shown in the following code.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  {

        get { return this.questionnaire; }

        set { SetProperty(ref this.questionnaire, value); }

        }

    <!-- -->

    1.  **Note:** The view model class typically derives from the **BindableBase** class. In some cases, the model can derive from **BindableBase**, when the property that needs to update the view when its value is changed is stored in the model.

    <!-- -->

    1.  To support **INotifyPropertyChanged**, your class needs to derive from the **BindableBase** class, and the property setter needs to call the **SetProperty** method of the **BindableBase** class.

        The following code shows how the **BindableBase** class implements the **INotifyPropertyChanged** interface. Note that the **SetProperty** method updates the property’s value and fires the **PropertyChanged** event when a property changes its value. Alternatively, you can use the **OnPropertyChanged** method, passing a lambda expression that references the property, to fire the **PropertyChanged** event. This is useful for when one property update triggers another property update. And also provides backward compatibility with the previous version of Prism.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  public abstract class BindableBase : INotifyPropertyChanged

        {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty&lt;T&gt;(ref T storage, T value, \[CallerMemberName\] string propertyName = null)

        {

        if (object.Equals(storage, value)) return false;

        storage = value;

        this.OnPropertyChanged(propertyName);

        return true;

        }

        protected void OnPropertyChanged(string propertyName)

        {

        var eventHandler = this.PropertyChanged;

        if (eventHandler != null)

        {

        eventHandler(this, new PropertyChangedEventArgs(propertyName));

        }

        }

        protected void OnPropertyChanged&lt;T&gt;(Expression&lt;Func&lt;T&gt;&gt; propertyExpression)

        {

        var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);

        this.OnPropertyChanged(propertyName);

        }

        }

    <!-- -->

    1.  The method is used so that when the questionnaire property changes its value and updates the user interface. In any view where the data might change in the view model and you want those changes to be reflected on the screen, you need to implement this pattern for all the properties in the view model.

        The questionnaire property, the collection property, and the command property are initialized in the view model class constructor.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  public QuestionnaireViewModel()

        {

        this.questionnaire = new Questionnaire();

    <!-- -->

    1.  Collection properties should always be initialized to either an empty collection or, if it is appropriate, to populate the collection in the constructor, you can do so, typically by calling a service. If you do that, make sure you do so in a way that will not break the designer.

        Additionally, if you expose **ICommand** properties that the view can bind command properties to, you need to create an instance of a command object. In this case, because you will use the **DelegateCommand** type from Prism, you need to create an instance of that and point it to a handling method. **DelegateCommand** also has the ability to carry along a strongly typed parameter if the **CommandParameter** property of a source control is also set. This is not used in the QuickStart, so the argument type is just specified as object.

        The following code shows the **OnSubmit** handler method, located in the **MainWindowViewModel** class.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  To keep the solution simple, this handler method returns the values entered for the questions to the Output window in Visual Studio with the help of a helper method that is already implemented in the view model class. A real implementation of a command handler would typically do something like call out to a service to save the results to a repository or retrieve data if it was a Load type of operation. It might also cause navigation to another view to occur by calling to a navigation service.

Wiring Up the View Elements to the View Model
---------------------------------------------

1.  The bindings in the view elements point to the view model properties, as shown in the following table. Note that these bindings are located in both the MainWindow view and in the QuestionnaireView view.

| Element name      | Property     | Value                                                        |
|-------------------|--------------|--------------------------------------------------------------|
| NameTextBox       | Text         | {Binding Path= Questionnaire.Name, Mode=TwoWay}              |
| AgeTextBox        | Text         | {Binding Path=Questionnaire.Age, Mode=TwoWay}                |
| Question1Response | Text         | {Binding Path=Questionnaire.Quest, Mode=TwoWay}              |
| ColorRun          | Foreground   | {Binding Questionnaire.FavoriteColor, TargetNullValue=Black} |
| Colors            | ItemsSource  | {Binding Path=AllColors}                                     |
| Colors            | SelectedItem | {Binding Questionnaire.FavoriteColor, Mode=TwoWay}           |
| SubmitButton      | Command      | {Binding SubmitCommand}                                      |
| ResetButton       | Command      | {Binding ResetCommand}                                       |

Creating the View and View Model and Hooking Them Up
----------------------------------------------------

1.  There are several ways of hooking up the view model with the view. You can create the view model in the view's code behind and set it in its **DataContext** property or set it declaratively in the view's Xaml code. To instantiate the view model in XAML, the view model class must have a default constructor.

    Another approach, is creating a component that can locate the corresponding view model and put it in the **DataContext** automatically, this component is typically called **View Model Locator**.

    Prism provides an implementation of the View Model Locator pattern, which is an attached property.

    Open MainWindow.xaml and look for the code where the view model locator property is attached. The attached property is shown in the last line of the following code.

<!-- -->

1.  XAML

<!-- -->

1.  &lt;Window x:Class="BasicMVVMQuickstart\_Desktop.Views.MainWindow"

    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"

    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"

    xmlns:basicMvvmQuickstartDesktop="clr-namespace:BasicMVVMQuickstart\_Desktop"

    xmlns:viewModel="clr-namespace:Microsoft.Practices.Prism.Mvvm;assembly=Microsoft.Practices.Prism.Mvvm.Desktop"

    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"

    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"

    xmlns:views="clr-namespace:BasicMVVMQuickstart\_Desktop.Views"

    mc:Ignorable="d"

    Title="Basic MVVM Quickstart"

    Height="350"

    Width="525"

    d:DataContext="{d:DesignInstance basicMvvmQuickstartDesktop:QuestionnaireViewDesignViewModel, IsDesignTimeCreatable=True}"

    viewModel:ViewModelLocator.AutoWireViewModel="True"&gt;

<!-- -->

1.  Prism's view model locator is an attached property that when set to true it will try to locate the view model of the view, and then set the view's data context to an instance of the view model. To locate the corresponding view model, the view model locator uses two approaches. First it will look for the view model in a view name/view model registration mapping. If a registration is not found, it will fall back to a convention-based approach, that will locate the view models, by replacing “.View” from the view namespace with “.ViewModel” and appending ‘’ViewModel’’ to the view’s name. For more information about ways to hook up views to view models; see "."

Adding Design-Time Support
--------------------------

1.  When you use the view model locator, your view models are created at runtime. Therefore, when you are designing your view, the view model is not yet constructed and you will not see the view model data at design time.

    To solve this situation, you can use the **d:DataContext** designer property and set it to a view model created specifically for design time. This view model will be constructed only at design time, it is a simplification on the real view model, and may contain mock data.

    You can see how this property is used in the MainWindow page, in the following code.

    1.  XAML

    <!-- -->

    1.  d:DataContext="{d:DesignInstance basicMvvmQuickstartDesktop:QuestionnaireViewDesignViewModel, IsDesignTimeCreatable=True}"

    Note that you need to specify the class that will be used as the **DesignInstance**, and then set the **IsDesignTimeCreatable** property to **true**. The design view model class used as **DesignInstance** is a class that must have a default constructor.

    You can see how the design view model for the QuickStart is defined, in the following code:

    1.  C\#

    <!-- -->

    1.  public class QuestionnaireViewDesignViewModel

        {

        public QuestionnaireViewDesignViewModel()

        {

        this.QuestionnaireViewModel = new QuestionnaireViewModel();

        }

        public QuestionnaireViewModel QuestionnaireViewModel { get; set; }

        }

    This design view model just has to initialize the properties used in the view for binding and populate them with mock data. As it is used only for design, it is not necessary to derive from **BindableBase**, nor implement the **INotifyPropertyChanged** interface.

More Information
================

1.  For more information about implementing the MVVM pattern, see the following topics:

<!-- -->

1.  1.  

<!-- -->

1.  To learn about other code samples included with Prism, see the following topics:

<!-- -->

1.  1.  


