namespace PrismMauiDemo.Views;

public class BackConfirmationPage : ContentPage
{
    public BackConfirmationPage()
    {
        Title = "Back confirmation";
        var allowed = new Switch { AutomationId = "AllowBack" };
        allowed.SetBinding(Switch.IsToggledProperty, "AllowBack");
        var status = new Label { AutomationId = "BackStatus" };
        status.SetBinding(Label.TextProperty, "Status");
        var back = new Button { Text = "GoBackAsync", AutomationId = "ManagedBack" };
        back.SetBinding(Button.CommandProperty, "BackCommand");
        var root = new Button { Text = "GoBackToRootAsync", AutomationId = "ManagedRoot" };
        root.SetBinding(Button.CommandProperty, "RootCommand");
        var push = new Button { Text = "Push another confirmation page", AutomationId = "PushConfirmation" };
        push.SetBinding(Button.CommandProperty, "PushCommand");
        Content = new VerticalStackLayout
        {
            Padding = 24, Spacing = 16,
            Children =
            {
                new Label { Text = "Use the native back button. With Allow back off, this page must remain visible. Async confirmation waits two seconds; repeated taps must produce only one request." },
                new Label { Text = "Allow back" }, allowed, status, back, root, push
            }
        };
    }
}

public class BackConfirmationMenuPage : ContentPage
{
    public BackConfirmationMenuPage()
    {
        Title = "Back confirmation lab";
        var sync = new Button { Text = "Synchronous confirmation", AutomationId = "SyncBack" };
        sync.SetBinding(Button.CommandProperty, "NavigateCommand");
        sync.CommandParameter = "SyncBackConfirmation";
        var asyncButton = new Button { Text = "Async confirmation (2 seconds)", AutomationId = "AsyncBack" };
        asyncButton.SetBinding(Button.CommandProperty, "NavigateCommand");
        asyncButton.CommandParameter = "AsyncBackConfirmation";
        Content = new VerticalStackLayout { Padding = 24, Spacing = 16, Children = { sync, asyncButton } };
    }
}
