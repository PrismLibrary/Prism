1.  **\#!155CharTopicSummary!\#:**

    Learn how to navigate to existing controls in the visual tree via state changes using the Prism for WPF library.

    1.  The State-Based Navigation QuickStart sample demonstrates navigation using the WPF Visual State Manager (VSM) with the Model-View-ViewModel (MVVM) pattern and the Prism Library. This approach uses the Visual State Manager to define the different application states that the application has, define animations for both the states and the transitions between states; the animations associated to states are active while the state is active for the duration of the specified timeline.

        One important aspect of application design is getting the navigation right. To define the navigation of the application, you need to design the screens, interaction, and the visual appearance of the application.

Business Scenario
=================

1.  The main window of the State-Based Navigation QuickStart represents a subset of a chat application. This window shows the list of contacts of the user. The user can alternate among different views of their contacts: list, icons, or contact detail. The messages from the user's contacts are displayed as they arrive. In the detail view of a contact, you can send a message to that contact. The following illustration shows the QuickStart main window.

<!-- -->

1.  <span id="_Ref196790494" class="anchor"></span>![](media/image1.png)

<!-- -->

1.  State-Based Navigation QuickStart user interface

Building and Running the QuickStart
===================================

1.  The QuickStart ships as source code—this means you must compile it before you run it. This QuickStart requires Microsoft Visual Studio 2012 or later and the .NET Framework 4.5.1.

To build and run the State-based Navigation QuickStart

1.  In Visual Studio, open the solution file Quickstarts\\State-Based Navigation\_Desktop\\State-Based Navigation.sln.

2.  In the **Build** menu, click **Rebuild Solution**.

3.  Press F5 to run the QuickStart.

4.  5.  

Implementation Details
======================

1.  The QuickStart highlights the key elements and considerations to implement an approach for navigation that uses the VSM. For more information about the VSM, see [VisualStateManager Class](http://msdn.microsoft.com/en-us/library/system.windows.visualstatemanager.aspx) on MSDN. In this QuickStart, most of the UI is in a few classes (the **ChatView** and **SendMessagePopupView** classes), and the visual states determine what is shown and how to go from one state to another. Some states change visibility of elements within the view, some states change enablement, and some states activate components. This section describes the key artifacts of the QuickStart, which are shown in the following illustration.

<!-- -->

1.  ![](media/image2.png)

<!-- -->

1.  State-Based Navigation QuickStart conceptual view

<!-- -->

1.  Notice that the Extensible Application Markup Language (XAML) file contains several states (you can compare states to views) grouped in visual state groups. There can be only one active state in each group. Therefore, the state of the application is a combination of four visual states (one of each visual state group). The different transitions are driven by the view. In the preceding illustration, the conditions represented over each transition arrow are the ones that trigger the transition from one state to another. The definition of the animations associated to the transitions and the behaviors that trigger them is also defined in the view's XAML file.

<!-- -->

1.  **Note:** In the QuickStart, there are only two states per **VisualStateGroup**. This is not mandatory; however, if you have more states, the transition logic could be more complex.

<!-- -->

1.  The following illustration shows states of the application and what visual states are active to create them.

<!-- -->

1.  ![](media/image3.png)

<!-- -->

1.  Application states and their active visual states

Logical Views (States)
----------------------

1.  Typically, the logical views are a form of a UI element that lets users interact with the application. In this application, the logical views are really just states to which the single physical view transitions. A state can involve a single user control or any complex set of user controls. The State-Based Navigation QuickStart has the following states: The list view, the icons view, and the contact view. Additionally, the QuickStart has the send message child view.

    Most of these logical view definitions are contained in the Views/ChatViews.xaml file. The following code shows the different logical views within the XAML file.

<!-- -->

1.  XAML

<!-- -->

1.  &lt;ContentControl x:Name="MainPane"

    HorizontalContentAlignment="Stretch" VerticalContentAlignment="Stretch"

    Grid.Row="2"&gt;

    &lt;Grid&gt;

    ...

    &lt;!-- Buttons (shared between all views) --&gt;

    &lt;Grid x:Name="ButtonsPanel" Grid.Row="0"&gt;

    ...

    &lt;RadioButton x:Name="ShowAsListButton" ... /&gt;

    &lt;RadioButton x:Name="ShowAsIconsButton" ... /&gt;

    &lt;Button x:Name="ShowDetailsButton" ... /&gt;

    &lt;/Grid&gt;

    **&lt;!-- Contacts view--&gt;**

    **&lt;ListBox x:Name="Contacts"**

    **ItemsSource="{Binding ContactsView}"**

    **Style="{Binding Source={StaticResource ContactsList}}"**

    **HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Grid.Row="2" Grid.RowSpan="3" Visibility="Collapsed"/&gt;**

    **&lt;!-- Avatars view--&gt;**

    **&lt;ListBox x:Name="Avatars"**

    **ItemsSource="{Binding ContactsView}"**

    **Style="{Binding Source={StaticResource AvatarsList}}"**

    **HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Grid.Row="2" Grid.RowSpan="3" Visibility="Collapsed"**

    **AutomationProperties.AutomationId="AvatarsView"/&gt;**

    **&lt;!-- Details view --&gt;**

    **&lt;Grid x:Name="Details"**

    **Background="White" Visibility="Collapsed"**

    **HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Grid.RowSpan="5"&gt;**

    **...**

    **&lt;/Grid&gt;**

    &lt;/Grid&gt;

    &lt;/ContentControl&gt;

<!-- -->

1.  <span id="DataBinding" class="anchor"></span>Notice that the views have their **Visibility** property set to **Collapsed**. This means that the controls that compose each view will not be shown and no space will be reserved for them. In this way, navigation between the different views will consist of all views initially collapsed, and navigating into the associated visual state will trigger an animation that will change their visibility to **Visible** (and the animation for the previous state will be stopped, resulting in the visibility for its associated logical view to be reset to the original **Collapsed** value).

Transitions Between Visual States
---------------------------------

1.  Transitions determine how to go from one view to another. The [**DataStateBehavior**](http://msdn.microsoft.com/en-us/library/ff723952(Expression.40).aspx) behavior is used to switch between two visual states based on whether a conditional property binding evaluates to **True** or to **False**. With the **DataStateBehavior** behavior, you can compare two values. One value comes from a binding. You can declare the other value to compare to explicitly. If the two values are equal, the visual state specified for **True** is activated. If the two values are not equal, the visual state specified for **False** is activated. The following code shows the behaviors defined in the chat view.

<!-- -->

1.  XAML

<!-- -->

1.  &lt;i:Interaction.Behaviors&gt;

    &lt;ei:DataStateBehavior Binding="{Binding ShowDetails}"

    Value="True"

    TrueState="ShowDetails" FalseState="ShowContacts"/&gt;

    &lt;ei:DataStateBehavior Binding="{Binding IsChecked, ElementName=ShowAsListButton}"

    Value="True"

    TrueState="ShowAsList" FalseState="ShowAsIcons"/&gt;

    &lt;ei:DataStateBehavior Binding="{Binding ConnectionStatus}"

    Value="Available"

    TrueState="Available" FalseState="Unavailable"/&gt;

    &lt;ei:DataStateBehavior Binding="{Binding SendingMessage}"

    Value="True"

    TrueState="SendingMessage" FalseState="NotSendingMessage"/&gt;

    &lt;/i:Interaction.Behaviors&gt;

<!-- -->

1.  Notice that depending on the value of the bound property, different states are shown. Apart from the contact list, icons, and details view, there are states for enabling or disabling the application (when the service is not available) and for activating or deactivating the busy indicator (when the application is busy).

    Typically, an animation is triggered to make the transition smooth from one state to another. When navigating to a different state, the source view is hidden and the target one is shown. The animation associated to states is permanent. The following code example shows the flipping animation that occurs during a transition. The animation associated to transitions is transient.

<!-- -->

1.  XAML

<!-- -->

1.  &lt;VisualStateGroup x:Name="VisualizationStates"&gt;

    &lt;VisualStateGroup.Transitions&gt;

    &lt;VisualTransition From="ShowAsIcons" To="ShowAsList"&gt;

    &lt;Storyboard SpeedRatio="2"&gt;

    ...

    &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Angle" Storyboard.TargetName="rotate"&gt;

    &lt;EasingDoubleKeyFrame KeyTime="0:0:0" Value="360"/&gt;

    &lt;EasingDoubleKeyFrame KeyTime="0:0:0.5" Value="270"/&gt;

    &lt;EasingDoubleKeyFrame KeyTime="0:0:0.5" Value="90"/&gt;

    &lt;EasingDoubleKeyFrame KeyTime="0:0:1" Value="0"/&gt;

    &lt;/DoubleAnimationUsingKeyFrames&gt;

    &lt;ObjectAnimationUsingKeyFrames Storyboard.TargetProperty="(FrameworkElement.Visibility)" Storyboard.TargetName="Avatars"&gt;

    &lt;DiscreteObjectKeyFrame KeyTime="0:0:0.5" &gt;

    &lt;DiscreteObjectKeyFrame.Value&gt;

    &lt;Visibility&gt;Collapsed&lt;/Visibility&gt;

    &lt;/DiscreteObjectKeyFrame.Value&gt;

    &lt;/DiscreteObjectKeyFrame&gt;

    &lt;/ObjectAnimationUsingKeyFrames&gt;

    &lt;ObjectAnimationUsingKeyFrames Storyboard.TargetProperty="(FrameworkElement.Visibility)" Storyboard.TargetName="Contacts"&gt;

    &lt;DiscreteObjectKeyFrame KeyTime="0:0:0.5"&gt;

    &lt;DiscreteObjectKeyFrame.Value&gt;

    &lt;Visibility&gt;Visible&lt;/Visibility&gt;

    &lt;/DiscreteObjectKeyFrame.Value&gt;

    &lt;/DiscreteObjectKeyFrame&gt;

    &lt;/ObjectAnimationUsingKeyFrames&gt;

    &lt;/Storyboard&gt;

    &lt;/VisualTransition&gt;

    &lt;VisualTransition From="ShowAsList" To="ShowAsIcons" ... /&gt;

    &lt;/VisualStateGroup.Transitions&gt;

    ...

    &lt;/VisualStateGroup&gt;

<!-- -->

1.  The states are grouped in different visual state groups. Only one state in a state group can be displayed at a time. For that reason, the **ShowAsList** state (contact list view), and the **ShowAsIcon** state (icons view) are mutually exclusive. The **ShowingDetails** and **NotShowingDetails** states belong to a different group; therefore, the application can be on the **ShowAsIcons** and **ShowingDetails** state at the same time. In this case, the **ShowingDetails** state goes to the foreground and overlaps the icons view; when transitioning to the **NotShowingDetails** states, the details view is collapsed, and the previous view, icons view, is shown. In this way, you do not have to store the previous state for returning because it is active in the background.

Interaction Requests
--------------------

1.  Interaction requests provide an abstract approach for view models to request interaction with the user. For more information about interaction requests, see in .

    The QuickStart uses interaction requests for two different situations: receiving and sending messages:

<!-- -->

1.  **Receiving messages**. The code in the view models create the objects that support the interactions (by raising an event with a payload to communicate with the view) and expose them through properties so they can be consumed by views. In the following code example from the **ChatViewModel** class, notice that the **ShowReceivedMessageRequest** property is defined and then used on the **OnMessageReceived** event handler to raise the **Message** instance.

    1.  C\#

    <!-- -->

    1.  public IInteractionRequest ShowReceivedMessageRequest

        {

        get { return this.showReceivedMessageRequest; }

        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs a)

        {

        this.showReceivedMessageRequest.Raise(a.Message);

        }

    <!-- -->

    1.  On the view side, it should detect an interaction request and then present an appropriate display for the request. The custom **InteractionRequestTrigger** automatically subscribes to the **Raised** event of the bound **IInteractionRequest**. The following code example, located in the ChatView.xaml file, shows this.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  &lt;prism:InteractionRequestTrigger SourceObject="{Binding ShowReceivedMessageRequest}"&gt;

        &lt;cb:ShowNotificationAction TargetName="NotificationList" /&gt;

        &lt;/prism:InteractionRequestTrigger&gt;

    <!-- -->

    1.  In the State-Based Navigation QuickStart, the custom **ShowNotificationAction** class is used to temporarily add the received message to a collection and sets this collection as the **DataContext** of a pop-up window. In this manner, the messages will be displayed in a non-modal window for a determined amount of time before disappearing.

    **Sending messages**. To display the send message window, the **SendMessageRequest** interaction request is used. The **Raise** method of this interaction request is invoked in the **SendMessage** method shown in the following code example from the **ChatViewModel**.

    1.  C\#

    <!-- -->

    1.  public IInteractionRequest SendMessageRequest

        {

        get { return this.sendMessageRequest; }

        }

        public void SendMessage()

        {

        var contact = this.CurrentContact;

        SendMessageViewModel viewModel = new SendMessageViewModel();

        viewModel.Title = "Send message to " + contact.Name;

        this.sendMessageRequest.Raise(

        viewModel,

        sendMessage =&gt;

        {

        if (sendMessage.Confirmed)

        {

        this.SendingMessage = true;

        this.chatService.SendMessage(

        contact,

        sendMessage.Message,

        result =&gt;

        {

        this.SendingMessage = false;

        });

        }

        });

        }

    <!-- -->

    1.  On the view side, when the interaction request is detected, the **PopupWindowAction** displays the **SendMessagePopView** pop-up window, as shown in the following code example from the ChatView.xaml file.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  &lt;prism:InteractionRequestTrigger SourceObject="{Binding SendMessageRequest}"&gt;

        &lt;prism:PopupWindowAction IsModal="True"&gt;

        &lt;prism:PopupWindowAction.WindowContent&gt;

        &lt;vs:SendMessagePopupView /&gt;

        &lt;/prism: PopupWindowAction.WindowContent&gt;

        &lt;/prism:PopupWindowAction&gt;

        &lt;/prism:InteractionRequestTrigger&gt;

    <!-- -->

    1.  Note that the **IsModal** property of the **PopupWindowAction** action is set to true to specify that this interaction should be modal. To specify the view that will be displayed when the interaction occurs, use the **WindowContent** property.

    <!-- -->

    1.  

Chat Service
------------

1.  The chat service is used for retrieving the contacts and their data; it is also used for sending and receiving messages from other users. The following code example shows the service interface.

<!-- -->

1.  C\#

<!-- -->

1.  public interface IChatService

    {

    event EventHandler ConnectionStatusChanged;

    event EventHandler&lt;MessageReceivedEventArgs&gt; MessageReceived;

    bool Connected { get; set; }

    void GetContacts(Action&lt;IOperationResult&lt;IEnumerable&lt;Contact&gt;&gt;&gt; callback);

    void SendMessage(Contact contact, string message, Action&lt;IOperationResult&gt; callback);

    }

<!-- -->

1.  The service contains the following members:

<!-- -->

1.  The **Connected** property. This property stores the state of the service: **Connected**/**Disconnected**.

    The **ConnectionStatusChanged** event handler. This event handler reacts to changes in the connection status.

    The **MessageReceived** event handler. This event handler reacts when a new message is received.

    The **GetContacts** method. This method retrieves the contacts of the user.

    The **SendMessage** method. This method sends messages to the users' contacts.

    1.  

    <!-- -->

    1.  The service has a timer to simulate incoming messages from other users. On every tick of the timer, there is a 33 percent chance that a message will be received based on a random draw. Additionally, the timer is also used to simulate connection drops; however, the chance of this happening is quite low (1/150). You can see this in the following code example that shows the **OnTimerTick** event handler.

    <!-- -->

    1.  C\#

    <!-- -->

    1.  private void OnTimerTick(object sender, EventArgs args)

        {

        if (this.Connected)

        {

        var coinToss = this.random.Next(3);

        if (coinToss == 0)

        {

        this.ReceiveMessage(

        this.GetRandomMessage(this.random.Next(receivedMessages.Length)),

        this.GetRandomContact(this.random.Next(this.contacts.Count)));

        }

        else

        {

        coinToss = this.random.Next(150);

        if (coinToss == 0)

        {

        this.Connected = false;

        }

        }

        }

        }

Custom Behaviors
----------------

1.  Behaviors are a self-contained unit of functionality. There are two types of behaviors:

<!-- -->

1.  Behaviors that do not have the concept of invocation; instead, it acts more like an add-on to an object.

    Triggers and actions that are closer to the invocation model.

    1.  

    <!-- -->

    1.  Additional functionality can be easily attached to an object in the XAML or through the designer. They can react to handle an event or a trigger in the UI. The following behaviors are used and defined in the QuickStart (located in the Infrastructure/Behaviors folder):

    **ShowNotificationAction**. This custom behavior allows a view model to push notifications into a target element in the UI. In the QuickStart, it is used to display the chat messages that are received by the user in the lower-right corner of the UI.

    1.  

    <!-- -->

    1.  The following behaviors are part of the Prism Library Prism.Interactivity project:

    **PopupWindowAction**. This concrete implementation displays a specified window or the default one configured with a data template.

    1.  

Acceptance Tests
================

1.  The State-Based Navigation QuickStart includes a separate solution that includes acceptance tests. The acceptance tests describe how the application should perform when you follow a series of steps; you can use the acceptance tests to explore the functional behavior of the application in a variety of scenarios.

To run the State-Based Navigation QuickStart acceptance tests

1.  In Visual Studio, open the solution file Quickstarts\\State-Based Navigation\_Desktop\\State-Based Navigation.Tests.AcceptanceTest\\State-Based Navigation.Tests.AcceptanceTest.sln.

2.  Build the Solution.

3.  Open Test Explorer.

4.  After building the solution, Visual Studio finds the tests. Click the **Run All** button to run the acceptance tests.

5.  

Outcome
-------

1.  You should see the QuickStart window and the tests automatically interact with the application. At the end of the test run, you should see that all tests have passed.

More Information
================

1.  To learn about other navigation topics included with Prism, see the following topics:

<!-- -->

1.  1.  

<!-- -->

1.  To learn about other code samples included with Prism, see the following topics:

<!-- -->

1.  1.  

    <!-- -->

    1.  


