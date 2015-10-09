1.  **\#!155CharTopicSummary!\#:**

    Learn how to solve many of the challenges of building composite applications. Includes composite views, modules, loosely coupled communication, commands.

    1.  Prism includes a sample called a reference implementation, which is a composite application that is based on a real-world scenario. This intentionally incomplete application illustrates the composite application baseline architecture. Within the application, you will see solutions for common, and recurrent, challenges that developers face when creating composite applications. We solve many of the challenges using design patterns such as Model-View-ViewModel (MVVM), Composite View, Event Aggregator, Plug-In, and Dependency Injection that embody important architectural design principles such as separation of concerns and loose coupling. Prism helps you to create a modular application design and build applications using loosely coupled components that can evolve independently but that can be easily and seamlessly integrated into the overall application.

        The reference implementation is not a real-world application; however, it is based on real-world challenges customers are facing. When you look at this application, do not look at it as a reference point for building a stock trader application—instead, look at is as a reference for building a composite application.

    <!-- -->

    1.  **\#!CALLOUT!\#:**

        **Note**: When looking at this application, it may seem inappropriate to implement it in the way it was implemented. For example, you might question why there are so many modules, and it may seem overly complex. The focus of Prism is to address challenges around building composite applications. For this reason, certain scenarios are used in the reference implementation to emphasize those challenges.

    <!-- -->

    1.  The following illustration shows the desktop version of the Stock Trader Reference Implementation (Stock Trader RI).

    <!-- -->

    1.  ![](media/image1.png)

    <!-- -->

    1.  Stock Trader RI

    <!-- -->

    1.  You can use the reference implementation in different ways. You can step through a running example that demonstrates application-specific code built on reusable guidance. You can also copy sections of the source code that implement any particular guidance into your own applications.

        The reference implementation was developed using a "test driven" approach and includes automated (unit) tests for most of its components. You can modify the reference implementation and use the unit tests to verify its functionality. The reference implementation for the Prism 5.0 release demonstrates several key features of the updated Prism library:

<!-- -->

1.  [Managed Extensibility Framework](http://msdn.microsoft.com/en-us/library/dd460648.aspx) (MEF) as the dependency injection container

    Modularity and user interface (UI) composition through custom attributes

    Model-View-ViewModel pattern (MVVM)

    Region-based navigation

    1.  

Building and Running the Reference Implementation
=================================================

1.  The Stock Trader RI requires Visual Studio 2012 or later and the .NET Framework 4.5.1. The reference implementation is compatible with [Blend for Visual Studio 2013](http://www.microsoft.com/expression/products/Blend_Overview.aspx).

To run the Stock Trader RI In Windows Explorer, double-click the following shortcut file to open the solution in Visual Studio:

1.  Open RI - StockTrader Reference Implementation.lnk

<!-- -->

1.  Press F5.

2.  

Interacting with the Reference Implementation
---------------------------------------------

1.  The features of the Stock Trader reference implementation are covered in greater detail later in the Scenarios section. The following steps provide a quick introduction to the basic features.

To see the pie chart and line chart for each stock

1.  Click the **Position** tab.

<!-- -->

1.  In the **Position** table, click the row that corresponds to the stock whose pie chart and line chart you want to see.

2.  

To see a news item corresponding to a stock

1.  Click the **Position** tab.

2.  In the **Position** table, click a stock in that corresponds to the stock you want to learn more about.

3.  Click a news article. If you click the control in the upper-right corner, a **News Reader** dialog box opens.

4.  

To add a stock to the watch list

1.  In the **Add to Watch List** box, type the stock symbol for the stock you want to add to the watch list. Valid values **i**nclude STOCK0 through STOCK9 as the stock symbol.

2.  Press ENTER.

3.  

To remove a stock from the watch list

1.  Click the **Watch List** button.

2.  In the watch list, click the **X** symbol next to the stock that you want to remove.

3.  

To buy or sell shares from a stock

1.  In the **Position** area, click the **+** or **–** symbol next to the stock that you want to buy or sell.

2.  In the **Buy & Sell** area, enter the following data:

3.  In the **Shares** box, type the number of shares you want to buy or sell.

4.  In the **Price Limit** box, type the appropriate price.

5.  In the **Order Type** drop-down box, click **Limit**, **Market**, or **Stop**.

6.  In the **Term** box, click **End of day** or **Thirty days**. Term is the length of time an order will be active before it is carried out or it expires.

7.  To submit the order, click the **Submit** button. To cancel the order, click the **Cancel** button.

8.  

To submit or cancel all your buy and sell orders

1.  If you have multiple orders that are ready to be bought or sold, the **Submit All** and **Cancel All** buttons are enabled on the **Buy & Sell** area and on the main task bar. The **Submit All** button will be enabled only if all individual orders are able to be submitted.

    1.  

    <!-- -->

    1.  The following illustration shows the Stock Trader RI **Buy & Sell** tab.

    <!-- -->

    1.  ![](media/image2.png)

    <!-- -->

    1.  Buy & Sell area in the Stock Trader RI

Acceptance Tests
----------------

1.  The Stock Trader RI includes a separate solution that includes acceptance tests. The acceptance tests describe how the reference implementation should perform when you follow a series of steps. You can use the acceptance tests to explore the functional behavior of the application in a variety of scenarios.

Outcome
-------

1.  You should see the Stock Trader RI shell window and the tests automatically interact with the application. At the end of the test pass, you should see that all tests have passed.

The Scenario
============

1.  The Stock Trader RI illustrates a fictitious, but realistic financial investments scenario. Contoso Financial Investments (CFI) is a fictional financial organization that is modeled after real financial organizations. CFI is building a new composite application to be used by their stock traders. This topic contains a summary of the scenario and demonstrates the business drivers that led to a series of technical decisions that ultimately result in the use of Prism.

Contoso Financial Investments Scenario
--------------------------------------

1.  Contoso Financial Investments (CFI) is a global investment firm with one hundred traders. Core to doing business in CFI, there is a 15-year-old legacy trader application developed in Visual C++ with the Microsoft Foundational Class Library that, over time, has become increasingly difficult to maintain.

### Operating Environment

1.  For the last several years, CFI's lack of maintainability has brought new development on the application to a standstill—this has left the application in maintenance mode. To meet new customer requirements, CFI adopted the Microsoft .NET Framework development platform and branched out, creating additional applications that were each maintained by separate teams in a silo. The idea was that having separately developed applications would actually result in the development effort being more efficient. Each team developing in their own silo meant that CFI could remove any contention that might arise, and it would pave the way for easily creating new teams. This would allow CFI to scale out their development teams into several locations, including setting up several offshore teams.

    The harsh reality is that the approach proved to be extremely inefficient on several levels. Because each application was developed in a silo, the trader is now required to maintain multiple copies of the same data throughout a growing suite of applications, including StockPortfolio, MarketView, and StockHist. The data is not identical, but there are elements of the data that are duplicated. To do their jobs, traders constantly jump back and forth between these various applications. To assist with this, CFI employed a "launcher" that quickly launched all the applications from a central place. The launcher also passed the user's logon credentials to the application to skip the logon screen for each application. The launcher is more of a bandage than anything else. It did not greatly improve the overall workflow of the traders in that the applications cannot integrate with one another, nor do they support a consistent UI.

### Operational Challenges

1.  Because of the lack of integration, getting a consolidated view of all the related data is not an easy task. There is a customer-facing reporting site that can pull from each of the back-end systems to create this "one" view, but it is littered with problems, the least of which is that if the data has not been properly duplicated, the reports do not work. In addition, entering the duplicate data is extremely time consuming and significantly impacts the number of orders that the trader is processing. Manually entering the data caused many errors in the system. Attempts to automatically synchronize the different systems have been too costly, because the schemas are very different and change frequently. With all these problems, CFI, like many other businesses, has managed to continue to operate as a profitable business. As customer demand has increased, CFI has invested the necessary funds to expand its services. It has also consistently grown its trading force whose jobs have become more and more difficult because of the inefficient operating conditions. Recently, however, this inefficiency has increased to the point that the business is starting to lose money:

<!-- -->

1.  The interaction time per transaction has greatly increased because of the time it takes to navigate the suite of applications.

    The cost of employee training and in-house support has greatly increased because of the high complexity and lack of consistency of the applications.

    Maintenance costs of the various applications are extremely prohibitive. For example, in a recent instance, a logic bug that was detected required changes in seven different systems. This critical bug took three weeks to fix because other parts of the system heavily depended on the code where the bug resided. This greatly increased the cost of fixing it, testing it, and deploying it—it brought the total price to $150,000. This included the effort to fix three additional bugs that were created as part of the original fix.

    CFI has been unable to keep up with emerging technologies that can offer it a competitive edge and reduced development costs.

    1.  

### Emerging Requirements

1.  Currently, CFI is faced with a new challenge around Service-Oriented Architecture (SOA). Fabrikam Web Traders, one of CFI's chief competitors, has offered its customers a rich client desktop experience for managing their portfolios remotely and on-site. The client is able to access Fabrikam's back-end systems through web services. Several large CFI customers are now requesting the same capabilities.

    Although there is no immediate threat, in the long term, the business impact can be crippling. If CFI continues with the current strategy and does not both improve its efficiency and adapt to changing market conditions, it will lose business to its competition.

### Meeting the Business and IT Objectives

1.  The Chief Executive Officer (CEO) is an opportunist who sees this challenge as an opportunity for CFI to rise to the occasion. Working with the Chief Information Officer (CIO) and Chief Technology Officer (CTO), they devise a three-point strategy for moving CFI forward. The strategy is as follows:

<!-- -->

1.  Reduce the cost of development. To do this, the new system should do the following:

    It should provide structure for teams to collaborate through a well-defined architecture.

    It should support distributed teams, including using some offshore developers.

    It should provide a shorter development life cycle—this improves the time to market.

    It should present data in ways that were previously prohibitive and time consuming to implement.

    It should support Test-Driven Development (TDD).

    It should support automated acceptance tests.

    It should support integration with third-party systems.

    Improve trader efficiency. To accomplish this, the system should do the following:

    It should support better multitasking.

    It should provide a UI that is better adapted to the trader workflow.

    It should consolidate existing applications.

    It should provide shorter interaction time per transaction (data visualizations).

    It should provide better information flow (contextual UI queues).

    It should provide better use of screen area (also known as screen real estate).

    It should provide integration among the different components of the system and with external components (services).

    It should present reduced training time.

    It should support users whether they are located remotely or are on-site.

    It should support corporate branding and UI styling.

    It should minimize the cost of adding new functionality to the system.

    It should support adding custom extensions provided by either the customer or third-party companies.

    Create a new customer-facing product offering. This offering should do the following:

    It should include a rich client desktop experience for portfolio management.

    It should provide UI customization and corporate branding to beat out the competition.

    It should provide extensibility for third-party vendors.

    1.  

    <!-- -->

    1.  The CTO has delivered these requirements to the senior architect, who is investigating various options for delivering them.

### Development Challenges

1.  For the architect, this project represents one of the most significant changes in the technology environment of CFI. Work will be spread across several software development teams, with additional development being outsourced. In the past, cooperation between the development teams has been limited, and development tended to occur on an ad-hoc basis. This was because he identified the following problems that are a result of current development methodology:

<!-- -->

1.  **Inconsistency**. Similar applications are developed in different ways. This results in higher maintenance and training costs.

    **Varying quality**. Developers with varying levels of experience lack guidance on implementing proven practices. This situation results in inconsistent quality among the applications they produce.

    **Poor productivity**. In many cases, developers across the company repeatedly solve the same problems in different applications, with little or no reuse of code. Because there was no central design, it was very difficult to get the applications to communicate with one another.

    1.  

### The Solution: Prism

1.  The senior architect needs a strategy to realize the architectural vision set forth and to resolve the development challenges identified in the previous section. After significant research, he decides that the best solution can be found in Prism offered by the Microsoft patterns & practices group.

    Prism is a set of assets for building complex WPF applications. Prism enables designing a composite application in the following ways:

<!-- -->

1.  It provides infrastructure and support for developing and maintaining WPF composite applications through non-invasive and lightweight APIs.

    It dynamically composes UI components.

    It supports application modules that are developed, tested, and deployed by separate teams.

    It allows incremental adoption.

    It provides an integrated and consistent user experience.

    It can be integrated with existing WPF applications.

    It supports a multi-targeted scenario.

    1.  

    <!-- -->

    1.  Prism from Microsoft patterns & practices meets the requirements of CFI and should allow them to achieve their goals by making development significantly more efficient and predictable. Support for integrating with existing WPF applications is of particular interest to the architect because CFI recently developed several WPF applications to address recent customer needs. He is confident that the guidance will assist him in delivering an effective solution that is robust, reliable, based on proven practices, and that can best use WPF . After presenting his findings to the CTO, the CTO agrees that Prism will help to deliver an effective solution efficiently and cost-effectively. He gives approval for the project to proceed.

Stock Trader RI Features
========================

1.  The CFI stock trader application is used for managing a trader's portfolio of investments. Using the stock trader application, traders can see their portfolios, view trend data, buy and sell shares, manage items in their watch lists, and view related news.

    The Stock Trader RI supports the following actions:

<!-- -->

1.  See the pie chart and line chart for each stock.

    See a news item that corresponds to a stock.

    Add a stock to the watch list.

    View the watch list.

    Remove a stock from the watch list.

    Buy or sell shares from a stock.

    Submit or cancel your entire buy and sell orders.

    1.  

Logical Architecture
====================

1.  The following illustration shows a high-level logical architecture view of the Stock Trader RI.

<!-- -->

1.  ![](media/image3.png)

<!-- -->

1.  Architectural view of the Stock Trader RI

<!-- -->

1.  The Stock Trader RIuses Prism Library for WPF.

    The following describes the main elements of the Stock Trader RI architecture:

<!-- -->

1.  **Application**. The application is lightweight and contains the shell that hosts each of the different UI components within the reference implementation. It also contains the **StockTraderRIBootstrapper**, which sets up the container and initializes module loading.

    **Modules**. The solution is divided into the following four modules, which are each maintained by separate teams in different locations:

    > **Watch module**. The Watch module contains the **Watch List** and **Add To Watch List** functionality.

    > **News module**. The News module contains the **NewsFeedService**, which handles retrieving stock news items.

    > **Market module**. The Market module handles retrieval of market trend data for the trader's positions and notifies the UI when those positions change. It also handles populating the Trend line for the selected position.

    > **Position module**. The Position module handles populating the list of positions in the trader's portfolio. It also contains the Buy/Sell order functionality.

    **Infrastructure**. The infrastructure contains functionality for both the Stock Trader RI and the Prism core:

    > **Prism Library**. This contains the core composition services and service interfaces for handling regions, commanding, and module loading. It also contains the container façade for the Unity Application Block (Unity) and MEF. The **StockTraderRIBootstrapper** inherits from the **MefBoostrapper**.

    > **Stock Trader RI Infrastructure Library**. This contains service interfaces specific to the Stock Trader RI, shared models, and shared commands.

    1.  

Implementation View
===================

1.  The Stock Trader RI is based on the Prism Library. The following illustration shows the Stock Trader RI (Desktop version) Solution Explorer.

<!-- -->

1.  ![](media/image4.png)

<!-- -->

1.  Stock Trader RI solution view

How the Stock Trader RI Works
=============================

1.  ![](media/image5.png)

<!-- -->

1.  Stock Trader RI startup process

<!-- -->

1.  The Stock Trader RI startup process is the following:

<!-- -->

1.  The application uses the **StockTraderRIBootstrapper**, which inherits from the Prism Library's **MefBootstrapper** for its initialization.

2.  The application initializes the Prism Library's **MefServiceLocatorAdapter** for use in the modules.

3.  The **StockTraderRIBootstrapper** creates and shows the shell view.

4.  The Prism Library's **ModuleCatalog** finds all the modules the application needs to load.

5.  The Prism Library's **ModuleManager** loads and initializes each module.

6.  Modules use the Prism Library's **RegionManager** service to add a view to a region.

7.  The Prism Library's **Region** displays the view.

8.  

Modules
-------

Services and Containers
-----------------------

Bootstrapping the Application
-----------------------------

1.  Modules get initialized during a bootstrapping process by a class named **MefBootstrapper**. The **MefBootstrapper** is responsible for starting the core composition services used in an application created with the Prism Library. The following code from the **MefBootstrapper** class shows how the Module Manager is located from the container.

<!-- -->

1.  C\#

<!-- -->

1.  // MefBootstrapper.cs

    protected override void InitializeModules()

    {

    IModuleManager manager = this.Container.GetExportedValue&lt;IModuleManager&gt;();

    manager.Run();

    }

<!-- -->

1.  The Module Manager manages the process of validating the module catalog, retrieving modules if they are remote, loading the modules into the application domain, and calling the **IModule.Initialize** method.

Configuring the Aggregate Catalog
---------------------------------

1.  C\#

<!-- -->

1.  // StockTraderRIBootstrapper.cs

    protected override void ConfigureAggregateCatalog()

    {

    this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(StockTraderRIBootstrapper).Assembly));

    this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(StockTraderRICommands).Assembly));

    this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(MarketModule).Assembly));

    this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(PositionModule).Assembly));

    this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(WatchModule).Assembly));

    this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(NewsModule).Assembly));

    }

Module Loading
--------------

1.  **Note:** Each module class (for example, **NewsModule**) in the reference implementation is empty. The use of MEF allows for discovery of types using declarative attributes, so there is not any work to be done during module initialization. If a module needed to do additional work when it is loaded, the module class should then implement **IModule** and perform this initialization in the **Initialize** method. The **ModuleManager** would then discover, load, and initialize that module.

<!-- -->

1.  C\#

<!-- -->

1.  // ArticleViewModel.cs

    \[Export(typeof(ArticleViewModel))\]

    \[PartCreationPolicy(CreationPolicy.Shared)\]

    public class ArticleViewModel : BindableBase

    {

    \[ImportingConstructor\]

    public ArticleViewModel(INewsFeedService newsFeedService, IRegionManager regionManager, IEventAggregator eventAggregator)

    {

    ...

    }

    }

<!-- -->

1.  In addition, other types, such as services, are available so they can be accessed either by the same module or other modules in a loosely coupled fashion.

Views
-----

1.  A view is any content that a module contributes to the UI. In the Stock Trader RI, views are discovered at run time and added to regions. Regions are classes associated with a control container, such as **ContentControl** or **TabControl**.

<!-- -->

1.  **Note:** In the Stock Trader RI, views are usually user controls. However, data templates in WPF are an alternative approach to rendering a view.

### View Registration in the Container

1.  Views can be registered through declarative attributes, directly in code, or through configuration. The Stock Trader RI uses MEF and the MVVM pattern to demonstrate the use of declarative attributes. Views associate themselves with a region through a custom export attribute, as shown in the following code example.

<!-- -->

1.  C\#

<!-- -->

1.  // ArticleViewModel.cs

    \[ViewExport(RegionName = RegionNames.ResearchRegion)\]

    \[PartCreationPolicy(CreationPolicy.Shared)\]

    public partial class ArticleView : UserControl

<!-- -->

1.  The **AutoPopulateExportedViewsBehavior** in the Stock Trader RI infrastructure discovers the views in the container and automatically populates them into the associated region, as shown in the following code example.

<!-- -->

1.  C\#

<!-- -->

1.  // AutoPopulateExportedViewsBehavior.cs

    \[ImportMany(AllowRecomposition = true)\]

    public Lazy&lt;object, IViewRegionRegistration&gt;\[\] RegisteredViews { get; set; }

    public void OnImportsSatisfied()

    {

    AddRegisteredViews();

    }

    private void AddRegisteredViews()

    {

    if (this.Region != null)

    {

    foreach (var viewEntry in this.RegisteredViews)

    {

    if (viewEntry.Metadata.RegionName == this.Region.Name)

    {

    var view = viewEntry.Value;

    if (!this.Region.Views.Contains(view))

    {

    this.Region.Add(view);

    }

    }

    }

    }

    }

### Model-View-ViewModel

1.  The Stock Trader RI uses the MVVM pattern to separate UI, presentation logic, and the data model. Using MVVM allows the view model to be unit tested because it has no direct knowledge of the view.

    The Prism Library provides the **BindableBase** class that the view models in the Stock Trader RI use to notify the user interface of property changes. **BindableBase** makes implementing **INotifyPropertyChanged** much easier.

    In the Stock Trader RI, the view and view model are connected through view discovery. The view is discovered by the **AutoPopulateExportedViewsBehavior** and instantiated through the container. Because the view declares an import of the view model, the container then instantiates the view model and injects it into the view, as shown in the following code example.

<!-- -->

1.  C\#

<!-- -->

1.  // ArticleView.xaml.cs

    \[Import\]

    ArticleViewModel ViewModel

    {

    set

    {

    this.DataContext = value;

    }

    }

<!-- -->

1.  For more information about view discovery, see .

Commands
--------

1.  Views can communicate with presenters and services in a loosely coupled fashion by using commands. The **Add To Watch List** control, as shown in the following illustration, uses the **AddWatchCommand**, which is a **DelegateCommand**, to notify the **WatchListService** whenever a new watch item is added.

<!-- -->

1.  <span id="_Ref201650380" class="anchor"></span>![](media/image6.png)

<!-- -->

1.  Add To Watch List control

<!-- -->

1.  C\#

<!-- -->

1.  // WatchListService.cs

    public WatchListService(IMarketFeedService marketFeedService)

    {

    ...

    AddWatchCommand = new DelegateCommand&lt;string&gt;(AddWatch);

    ...

    }

    private void AddWatch(string tickerSymbol)

    {

    ...

    }

<!-- -->

1.  C\#

<!-- -->

1.  // AddWatchViewModel.cs

    public class AddWatchViewModel : BindableBase

    {

    private string stockSymbol;

    private IWatchListService watchListService;

    \[ImportingConstructor\]

    public AddWatchViewModel(IWatchListService watchListService)

    {

    if (watchListService == null)

    {

    throw new ArgumentNullException("service");

    }

    this.watchListService = watchListService;

    }

    public string StockSymbol

    {

    get { return stockSymbol; }

    set

    {

    SetProperty(ref stockSymbol, value);

    }

    }

    public ICommand AddWatchCommand { get { return this.watchListService.AddWatchCommand; } }

<!-- -->

1.  XAML

<!-- -->

1.  &lt;!--AddWatchView.xaml --&gt;

    &lt;StackPanel Orientation="Horizontal"&gt;

    &lt;TextBox Name="AddWatchTextBox" MinWidth="100" Style="{StaticResource CustomTextBoxStyle}"

    Infrastructure:ReturnKey.Command="{Binding Path=AddWatchCommand}"

    Infrastructure:ReturnKey.DefaultTextAfterCommandExecution="Add to Watch List"

    Text="Add to Watch List"

    AutomationProperties.AutomationId="TextBoxBlock" Margin="5,0,0,0"/&gt;

    &lt;/StackPanel&gt;

<!-- -->

1.  This is using an attached behavior on the **Add To Watch List** text box, so when the user enters a stock symbol and then presses ENTER**,** the **AddWatchCommand** will be invoked, thereby passing the stock symbol to the **WatchListService**.

Event Aggregator
----------------

1.  C\#

<!-- -->

1.  // PositionSummaryViewModel.cs

    eventAggregator.GetEvent&lt;TickerSymbolSelectedEvent&gt;().Publish(CurrentPositionSummaryItem.TickerSymbol);

<!-- -->

1.  C\#

<!-- -->

1.  // ArticleViewModel.cs

    eventAggregator.GetEvent&lt;TickerSymbolSelectedEvent&gt;().Subscribe(OnTickerSymbolSelected, ThreadOption.UIThread);

<!-- -->

1.  **Note:** The notification of the event is on the UI thread to safely update the UI and avoid a WPF exception.

Technical Challenges
====================

1.  The Stock Trader Reference Implementation (Stock Trader RI) demonstrates how you can address common technical challenges that you face when you build composite applications in WPF. The following table describes the technical challenges that the Stock Trader RI addresses.

| Technical challenge                                                                                                                                                                                                                                                                                                | Feature in the                                                                                                                                                                      
                                                                                                                                                                                                                                                                                                                      Stock Trader RI                                                                                                                                                                      | Example of where feature is demonstrated                                                      |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------|
| **Views and composite UI**                                                                                                                                                                                                                                                                                         |
| **Regions:** The use of regions for placing the views without having to know how the layout is implemented.                                                                                                                                                                                                        | Regions defined in the shell and position module's orders view.                                                                                                                     | StockTraderRI\\Shell.xaml                                                                     
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Position\\Orders\\OrdersView.xaml                                        |
| **Composite view**: Shows how a composite view communicates with its child view.                                                                                                                                                                                                                                   | Order screen                                                                                                                                                                        | StockTraderRI.Modules.Position\\Orders\\OrderCompositeViewModel.cs                            
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Position\\Orders\\OrderDetailsViewModel.cs                               
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Position\\Orders\\OrderCommandsView.xaml.cs                              |
| **Compose UI across modules**: Shows how a module can have views in different parts of the shell that interact with each other.                                                                                                                                                                                    | The Watch module has a view and also is a part of the toolbar.                                                                                                                      | StockTraderRI.Modules.Watch\\AddWatch\\AddWatchView.xaml                                      
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Watch\\WatchList\\WatchListView.xaml                                     |
|                                                                                                                                                                                                                                                                                                                    | The News module has an article list view and a popup article reader view that shows the same articles.                                                                              | StockTraderRI.Modules.News\\Article\\ArticleView.xaml                                         
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.News\\Article\\NewsReader.xaml                                           |
| **Decoupled communication**                                                                                                                                                                                                                                                                                        |
| **Commands**: Shows the Command pattern. The command to buy or sell a stock is a delegate command. Each row in the list uses the same command instance but with a different parameter corresponding to the stock. This decouples the invoker from the receiver and shows passing additional data with the command. | Buy and Sell command invokers in **PositionSummaryView** and handlers in **OrdersController**                                                                                       | StockTraderRI.Modules.Position\\Controllers\\OrdersController.cs                              
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Position\\PositionSummary\\PositionSummaryView.xaml                      |
| **Composite commands**: Use composite commands to broadcast all of the commands. The **Submit All** or **Cancel All** commands execute all the individual instances of the **Submit** or **Cancel** commands.                                                                                                      | **Submit All** and **Cancel All** buttons                                                                                                                                           | StockTraderRI.Infrastructure\\StockTraderRICommands.cs                                        
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Position\\Orders\\OrderDetailsViewModel.cs                               
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Position\\Controllers\\OrdersController.cs                               |
| **Event Aggregator pattern:** Publish and Subscribe to events across decoupled modules. Publisher and Subscriber have no contract other than the event type.                                                                                                                                                       | Show relevant news content: When the user selects a position in the position list, the communication to the news module uses the **EventAggregator** service.                       | StockTraderRI.Modules.Position\\PositionSummary\\PositionSummaryPresentationModel.cs          
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.News\\Controllers\\NewsController.cs                                     |
|                                                                                                                                                                                                                                                                                                                    | Market feed updates: The consumers of the market feed service subscribe to an event to be notified when new feeds are available; the consumers then update the model behind the UI. | StockTraderRI.Modules.Market\\Services\\MarketFeedService.cs                                  
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Position\\PositionSummary\\ObservablePosition.cs                         
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Watch\\WatchList\\WatchListViewModel.cs                                  |
| **Services**: Services are also used to communicate between modules. Services are more contractual and flexible than commands.                                                                                                                                                                                     | Several service implementations in module assemblies                                                                                                                                | **Services**:                                                                                 
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Market\\Services\\MarketFeedService.cs                                   
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Market\\Services\\MarketHistoryService.cs                                
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.News\\Services\\NewsFeedService.cs                                       
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Watch\\Services\\WatchListService.cs                                     
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Position\\Services\\AccountPositionService.cs                            
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI.Modules.Position\\Services\\XmlOrdersService.cs                                  |
| **Other technical challenges**                                                                                                                                                                                                                                                                                     |
| **WPF:** Use WPF for the user interface                                                                                                                                                                                                                                                                            | Shell and module views                                                                                                                                                              | The starting point for Stock Trader RI - Desktop version is in the StockTraderRI\\App.xaml.cs |
| **Bootstrapper**: The use of a bootstrapper to initialize the application with global services.                                                                                                                                                                                                                    | Created bootstrapper with MEF and configuring global services, such as logging and defining the module catalog.                                                                     | **Bootstrapper**:                                                                             
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            StockTraderRI\\StockTraderRIBootstrapper.cs                                                    |

Unit and Acceptance Tests
-------------------------

1.  The Stock Trader RI includes unit tests within the solution. Unit tests verify whether individual units of source code work as expected.

To run the Stock Trader RI unit tests

1.  On the **Test** menu, point to **Run**, and then click **All Tests in Solution**.

    1.  

    <!-- -->

    1.  The Stock Trader RI includes a separate solution that includes acceptance tests. The acceptance tests describe how the application should perform when you follow a series of steps; you can use the acceptance tests to explore the functional behavior of the application in a variety of scenarios.

To run the Stock Trader RI acceptance tests

1.  In Visual Studio, open the solution file StockTrader RI\\StockTraderRI.Tests.AcceptanceTest\\StockTraderRI.Tests.AcceptanceTest.sln.

2.  Build the solution.

3.  Open Test Explorer.

4.  After building the solution, the test will be found. Click the **Run All** button to run the acceptance tests.

5.  

### Outcome

1.  You should see the reference implementation window and the tests automatically interact with the application. At the end of the test run, you should see that all tests have passed.

More Information
================

1.  To learn about other code samples included with Prism, see the following topics:

<!-- -->

1.  1.  

    <!-- -->

    1.  


