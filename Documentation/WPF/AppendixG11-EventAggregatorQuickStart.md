1.  **\#!155CharTopicSummary!\#:**

    Learn how to create loosely coupled pub/sub events to communicate with decoupled components. This QuickStart uses Event Aggregator.

    1.  The Event Aggregation QuickStart sample demonstrates how to build a composite application that uses the Prism Library’s Event Aggregator service. This service enables you to establish loosely coupled communications between components in your application. The Event Aggregator is a Portable Class Library (PCL) so it can be used on WPF, Windows Phone 8, and Windows Store apps.

Business Scenario
=================

1.  The main window of the Event Aggregation QuickStart represents a subset of a fictitious financial system. In this window, users can add funds to customers and see the activity log for each customer. The following illustration shows the QuickStart main window.

<!-- -->

1.  <span id="_Ref196790494" class="anchor"></span>![](media/image1.png)

<!-- -->

1.  Event Aggregation QuickStart user interface

Building and Running the QuickStart
===================================

1.  This QuickStart requires Visual Studio 2012 or later and the .NET Framework 4.5.1 to run.

To build and run the Event Aggregation QuickStart

1.  In Visual Studio, open the solution file Quickstarts\\EventAggregation\\EventAggregation\_Desktop.sln.

2.  On the **Build** menu, click **Rebuild Solution**.

3.  Press **F5** to run the **QuickStart.**

4.  

Implementation Details
======================

1.  The QuickStart highlights the key elements that interact when using the Event Aggregator service. This section describes the key artifacts of the QuickStart, which are shown in the following illustration.

<!-- -->

1.  ![](media/image2.png)

<!-- -->

1.  Event Aggregation QuickStart conceptual view

The FundAddedEvent Event
------------------------

1.  The **FundAddedEvent** event is raised when the user adds a fund for a customer. This event is used by the modules ModuleA and ModuleB to communicate in a loosely coupled way. The following code shows the event class signature; the class extends the **PubSubEvent&lt;TPayload&gt;** class, specifying **FundOrder** as the payload type. This code is located at EventAggregation.Infrastructure.Dektop\\FundAddedEvent.cs.

<!-- -->

1.  C\#

<!-- -->

1.  public class FundAddedEvent : PubSubEvent&lt;FundOrder&gt;

    {

    }

<!-- -->

1.  The following code is the class definition for the **FundOrder** class; this class represents a fund order and specifies the ticker symbol and the customer's identifier. This code is located at EventAggregation.Infrastructure.Desktop\\FundOrder.cs.

<!-- -->

1.  C\#

<!-- -->

1.  public class FundOrder

    {

    public string CustomerId { get; set; }

    public string TickerSymbol { get; set; }

    }

Event Publishing
----------------

1.  When the user adds a fund for a customer, the event **FundAddedEvent** is published by the **AddFundPresenter** class (located at ModuleA.Desktop\\AddFundPresenter.cs). The following code shows how the **FundAddedEvent** is published.

<!-- -->

1.  C\#

<!-- -->

1.  void AddFund(object sender, EventArgs e)

    {

    FundOrder fundOrder = new FundOrder();

    fundOrder.CustomerId = View.Customer;

    fundOrder.TickerSymbol = View.Fund;

    if (!string.IsNullOrEmpty(fundOrder.CustomerID) && !string.IsNullOrEmpty(fundOrder.TickerSymbol))

    eventAggregator.GetEvent&lt;FundAddedEvent&gt;().Publish(fundOrder);

    }

<!-- -->

1.  In the preceding code, first a **FundOrder** instance is created and set up. Then, the **FundAddedEvent** is retrieved from the Event Aggregator service and the **Publish** method is invoked on it; this supplies the recently created **FundOrder** instance as the **FundAddedEvent** event's parameter.

Event Subscription
------------------

1.  The ModuleB module contains a view named **ActivityView**. An instance of this view shows the activity log for a single customer. The ModuleB initializer class creates two instances of this view, one for Customer1 and one for Customer2, as shown in the following code (this code is located at ModuleB.Desktop\\ModuleB.cs).

<!-- -->

1.  C\#

<!-- -->

1.  public void Initialize()

    {

    ActivityView activityView1 = Container.Resolve&lt;ActivityView&gt;();

    ActivityView activityView2 = Container.Resolve&lt;ActivityView&gt;();

    activityView1.CustomerId = "Customer1";

    activityView2.CustomerId = "Customer2";

    IRegion rightRegion = RegionManager.Regions\["RightRegion"\];

    rightRegion.Add(activityView1);

    rightRegion.Add(activityView2);

    }

<!-- -->

1.  When an instance of the **ActivityView** view is created, its presenter subscribes an event handler to the **FundAddedEvent** event using a filter expression. This filter expression defines a condition that the event's argument must meet for the event handler to be invoked. In this case, the condition is satisfied if the fund order corresponds to the customer associated to the view. The event handler contains code to display the new fund added to the customer in the user interface.

    The following code shows the **CustomerId** property of the **ActivityPresenter** class. In the property setter, an event handler for the **FundAddedEvent** event is subscribed using the Event Aggregator service.

<!-- -->

1.  C\#

<!-- -->

1.  public string CustomerId

    {

    get { return \_customerId; }

    set

    {

    \_customerId = value;

    FundAddedEvent fundAddedEvent = eventAggregator.GetEvent&lt;FundAddedEvent&gt;();

    if (subscriptionToken != null)

    {

    fundAddedEvent.Unsubscribe(subscriptionToken);

    }

    subscriptionToken = fundAddedEvent.Subscribe(FundAddedEventHandler, ThreadOption.UIThread, false, FundOrderFilter);

    View.Title = string.Format(CultureInfo.CurrentCulture, Resources.ActivityTitle, CustomerId);

    }

    }

<!-- -->

1.  The following line, extracted from the preceding code, shows how the event handler is subscribed to the **FundAddedEvent** event.

<!-- -->

1.  C\#

<!-- -->

1.  subscriptionToken = fundAddedEvent.Subscribe(FundAddedEventHandler, ThreadOption.UIThread, false, FundOrderFilter);

<!-- -->

1.  In the preceding line, the following parameters are passed to configure the subscription:

<!-- -->

1.  The **FundAddedEventHandler** action**.** This event handler is executed when the **Add** button is clicked and the filter condition is satisfied.

    The **ThreadOption.UIThread** option**.** This option specifies that the event handler will run on the user interface thread.

    The **KeepSubscriberReferenceAlive** flag. This flag is **false** and indicates that the lifetime of the subscriber's reference is not managed by the event. This is set to **false** because the lifetime of the subscriber, the presenter class, is managed by its view, which contains a reference to it.

    The **filter** predicate. This filter is a condition that specifies that the event handler is invoked only when the fund is added to the view's corresponding customer.

    1.  

Unit and Acceptance Tests
=========================

1.  The Event Aggregator QuickStart includes unit tests within the solution. Unit tests verify if individual units of source code work as expected.

Unit Tests
----------

To run the Event Aggregator QuickStart unit tests

1.  On the **Test** menu of Visual Studio, point to **Run**, and then click **All Tests**.

    1.  

### Outcome

1.  You should see the Test Results pane in Visual Studio indicating that all the unit tests passed.

Acceptance Tests
----------------

1.  The Event Aggregator QuickStart includes a separate solution that includes acceptance tests. The acceptance tests describe how the application should perform when you follow a series of steps; you can use the acceptance tests to explore the functional behavior of the application in a variety of scenarios.

To run the Event Aggregator QuickStart acceptance tests

1.  In Visual Studio, open the solution file QuickStarts\\EventAggregation\\EventAggregation.Tests.AcceptanceTest\\EventAggregation.Tests.AcceptanceTest.sln.

2.  Open **Test Explorer**.

3.  After building the solution, Visual Studio finds the tests. Click the **Run All** button to run the acceptance tests.

4.  

### Outcome

1.  You should see the QuickStart window and the tests automatically interact with the application. At the end of the test run, you should see that all tests have passed.

More Information
================

1.  For more information about event aggregation, see .

<!-- -->

1.  To learn about other code samples included with Prism, see the following topics:

<!-- -->

1.  

<!-- -->

1.  
