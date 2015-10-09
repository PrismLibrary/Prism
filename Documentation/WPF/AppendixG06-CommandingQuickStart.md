1.  **\#!155CharTopicSummary!\#:**

    Learn how to use Prism commands to decouple UI actions from the command implementation and logic when implementing the MVVM pattern.

    1.  The Commanding QuickStart sample demonstrates how to build a Windows Presentation Foundation (WPF) application that uses delegate and composite commands provided by the Prism Library to handle UI actions in a decoupled way. This is useful when implementing the Model-View-ViewModel (MVVM) pattern. The Prism Library also provides an implementation of the ICommand interface.

Business Scenario
=================

1.  The Commanding QuickStart is based on a fictitious product ordering system. The main window represents a subset of a larger system. In this window, the user can place customer orders and submit them. The following illustration shows the QuickStart's main window.

<!-- -->

1.  ![](media/image1.png)

<!-- -->

1.  Commanding QuickStart

Building and Running the QuickStart
===================================

1.  This QuickStart requires Visual Studio 2012 or later and the .NET Framework 4.5.1 to run.

To build and run the QuickStart

1.  In Visual Studio, open the solution file Quickstarts\\Commanding\\Commanding\_Desktop.sln.

2.  On the **Build** menu, click **Rebuild Solution**.

3.  Press F5 to run the QuickStart.

4.  

Implementation Details
======================

1.  The QuickStart highlights the key implementation details of an application that uses commands. The following illustration shows the key artifacts in the application.

<!-- -->

1.  ![](media/image2.png)

<!-- -->

1.  Commanding QuickStart conceptual view

<!-- -->

1.  **Note:** The QuickStart contains a number of TODO comments to help navigate the important concepts in the code. Use the Task List window in Visual Studio to see a list of these important areas of code. Make sure you select **Comments** in the dropdown box. If you double-click an item in the list, the code file will open in the appropriate line.

Delegate Commands
-----------------

1.  By using the **DelegateCommand** command, you can supply delegates for the **Execute** and **CanExecute** methods. This means that when the **Execute** or **CanExecute** methods are invoked on the command, the delegates you supplied are invoked.

    In the Commanding QuickStart, the **Save** button on each order form is associated to a delegate command. The delegates for the **Execute** and **CanExecute** methods are the **Save** and **CanSave** methods of the **OrderViewModel** class, respectively (this class is the view model for an order; for the class definition, see the file Commanding.Modules.Order.Desktop\\ViewModels\\OrderViewModel.cs).

    The following code shows the constructor of the **OrderViewModel** class. In the method body, a delegate command named **SaveOrderCommand** is created—it passes delegates for the **Save** and **CanSave** methods as parameters.

<!-- -->

1.  C\#

<!-- -->

1.  public OrderViewModel( Services.Order order )

    {

    \_order = order;

    //TODO: 01 - Each Order defines a Save command.

    this.SaveOrderCommand = new DelegateCommand&lt;object&gt;( this.Save, this.CanSave );

    // Track all property changes so we can validate.

    this.PropertyChanged += this.OnPropertyChanged;

    this.Validate();

    }

<!-- -->

1.  The following code shows the implementation of the **Save** and **CanSave** methods.

<!-- -->

1.  C\#

<!-- -->

1.  private bool CanSave( object arg )

    {

    //TODO: 02 - The Order Save command is enabled only when all order data is valid.

    // Can only save when there are no errors and

    // when the order quantity is greater than zero.

    return this.errors.Count == 0 && this.Quantity &gt; 0;

    }

    private void Save( object obj )

    {

    // Save the order here.

    Console.WriteLine(

    String.Format( CultureInfo.InvariantCulture, "{0} saved.", this.OrderName ) );

    // Notify that the order was saved.

    this.OnSaved( new DataEventArgs&lt;OrderPresentationModel&gt;( this ) );

    }

<!-- -->

1.  The following code shows the **OnPropertyChanged** method implementation. This method is an event handler for the **PropertyChanged** event, which gets raised whenever the user changes a value in the order form. This method updates the order's total, validates the data, and raises the **CanExecuteChanged** event of the **SaveOrderCommand** command to notify the command's invokers about the state change.

<!-- -->

1.  C\#

<!-- -->

1.  private void OnPropertyChanged( object sender, PropertyChangedEventArgs e )

    {

    // Total is a calculated property based on price, quantity and shipping cost.

    // If any of these properties change, then notify the view.

    string propertyName = e.PropertyName;

    if ( propertyName == "Price" || propertyName == "Quantity" || propertyName == "Shipping" )

    {

    this.NotifyPropertyChanged( "Total" );

    }

    // Validate and update the enabled status of the SaveOrder

    // command whenever any property changes.

    this.Validate();

    this.SaveOrderCommand.RaiseCanExecuteChanged();

    }

<!-- -->

1.  The following code, located in the file Commanding.Modules.Order.Desktop\\Views\\OrdersEditorView.xaml, shows how the **Save** button is bound to the **SaveOrderCommand** command.

<!-- -->

1.  XAML

<!-- -->

1.  &lt;Button AutomationProperties.AutomationId="SaveButton" Grid.Row="6" Grid.Column="1" Content="Save" Command="{Binding SaveOrderCommand}"&gt;&lt;/Button&gt;

Composite Commands
------------------

1.  A **CompositeCommand** is a command that has multiple child commands. A **CompositeCommand** is used in the Commanding QuickStart for the **Save All** button on the main toolbar. When you click the **Save All** button, the **SaveAllOrdersCommand** composite command executes, and in consequence, all its child commands—**SaveOrderCommand** commands—execute for each pending order.

    The **SaveAllOrdersCommand** command is a globally available command, and it is defined in the **OrdersCommands** class (the class definition is located at Commanding.Modules.Order.Desktop\\OrdersCommands.cs). The following code shows the implementation of the **OrdersCommands** static class.

<!-- -->

1.  C\#

<!-- -->

1.  public static class OrdersCommands

    {

    public static CompositeCommand SaveAllOrdersCommand = new CompositeCommand();

    }

<!-- -->

1.  The following code, extracted from the file Commanding.Modules.Order.Desktop\\ViewModels\\OrdersEditorViewModel.cs, shows how child commands are registered with the **SaveAllOrdersCommand** command. In this case, a proxy class is used to access the command. For more information, see "Proxy Class for Global Commands" later in this topic.

<!-- -->

1.  C\#

<!-- -->

1.  private void PopulateOrders()

    {

    \_orders = new ObservableCollection&lt;OrderPresentationModel&gt;();

    foreach ( Services.Order order in this.ordersRepository.GetOrdersToEdit() )

    {

    // Wrap the Order object in a presentation model object.

    var orderPresentationModel = new OrderViewModel( order );

    \_orders.Add( orderPresentationModel );

    // Subscribe to the Save event on the individual orders.

    orderPresentationModel.Saved += this.OrderSaved;

    //TODO: 04 - Each Order Save command is registered with the application's SaveAll command.

    commandProxy.SaveAllOrdersCommand.RegisterCommand( orderPresentationModel.SaveOrderCommand );

    }

    }

<!-- -->

1.  When an order is saved, the **SaveOrderCommand** child command for that particular order must be unregistered. The following code shows how this is done in the implementation of the **OrderSaved** event handler, which executes when an order is saved.

<!-- -->

1.  C\#

<!-- -->

1.  private void OrderSaved(object sender, DataEventArgs&lt;OrderViewModel&gt; e)

    {

    if (e != null && e.Value != null)

    {

    OrderViewModel order = e.Value;

    if (this.Orders.Contains(order))

    {

    order.Saved -= this.OrderSaved;

    this.commandProxy.SaveAllOrdersCommand.UnregisterCommand(order.SaveOrderCommand);

    this.Orders.Remove(order);

    }

    }

    }

<!-- -->

1.  The following XAML markup code shows how the **SaveAllOrdersCommand** command is bound to the **SaveAllToolBarButton** button in the toolbar. This code is located at Commanding.Modules.Order.Desktop\\OrdersToolBar.xaml.

<!-- -->

1.  XAML

<!-- -->

1.  &lt;ToolBar&gt;

    &lt;Button AutomationProperties.AutomationId="SaveAllToolBarButton" Command="{x:Static inf:OrdersCommands.SaveAllOrdersCommand}"&gt;Save All Orders&lt;/Button&gt;

    &lt;Separator /&gt;

    &lt;/ToolBar&gt;

Proxy Class for Global Commands
-------------------------------

1.  To create a globally available command, you typically create a static instance of a **CompositeCommand** class and expose it publicly through a static class. This approach is straightforward, because you can access the command instance directly from your code. However, this approach makes your classes that use the command hard to test in isolation, because your classes are tightly coupled to the command. When testability is a concern in an application, a proxy class can be used to access global commands. A proxy class can be easily replaced with mock implementations when writing unit tests.

    The Commanding QuickStart implements a proxy class named **OrdersCommandProxy** to encapsulate the access to the **SaveAllOrdersCommand** (the class definition is located at Commanding.Modules.Order.Desktop\\OrdersCommands.cs). The class, shown in the following code, implements a public property to return the **SaveAllOrdersCommands** command instance defined in the **OrdersCommands** class.

<!-- -->

1.  C\#

<!-- -->

1.  public class OrdersCommandProxy

    {

    public virtual CompositeCommand SaveAllOrdersCommands

    {

    get { return OrdersCommands.SaveAllOrdersCommand; }

    }

    }

<!-- -->

1.  In the preceding code, note that the **SaveAllOrdersCommands** property can be overwritten in a mock class to return a mock command.

    For more information about creating globally available commands, see in .

Acceptance Tests
================

1.  The Commanding QuickStart includes a separate solution that includes acceptance tests. The acceptance tests describe how the application should perform when you follow a series of steps; you can use the acceptance tests to explore the functional behavior of the application in a variety of scenarios.

To run the Commanding QuickStart acceptance tests

1.  In Visual Studio, open the solution file QuickStarts\\Commanding\\Commanding.Tests.AcceptanceTest\\Commanding.Tests.AcceptanceTest.sln.

2.  Build the solution.

3.  Open **Test Explorer**.

4.  After building the solution, Visual Studio finds the tests. Click the **Run All** button to run the acceptance tests.

5.  

Outcome
-------

1.  You should see the QuickStart window and the tests automatically interact with the application. At the end of the test run, you should see that all tests have passed.

More Information
================

1.  For more information about commands, see the following topics:

<!-- -->

1.  1.  

<!-- -->

1.  To learn about other code samples included with Prism, see the following topics:

<!-- -->

1.  

<!-- -->

1.  1.  


