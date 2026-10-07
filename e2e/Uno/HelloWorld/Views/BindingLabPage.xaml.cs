using HelloWorld.ViewModels;
using ContextModel = HelloWorld.Models.BindingContext;

namespace HelloWorld.Views;

public sealed partial class BindingLabPage : Page
{
    private bool _checking;

    public BindingLabPage() => InitializeComponent();

    private async void RunBindingChecks(object sender, RoutedEventArgs e)
    {
        if (_checking) return;
        _checking = true;
        CheckStatus.Text = "Running checks...";
        try
        {
            var vm = (BindingLabViewModel)DataContext;
            vm.Context = new ContextModel();
            vm.Items.Clear();
            vm.Items.Add(new ContextModel("Initial item"));
            await Expect(() => ContextTitle.Text == "Initial context" && NavigationIndicator.Visibility == Visibility.Visible, "initial nested binding");
            await Expect(() => HasRowText("Initial item"), "initial collection item");

            await Expect(() => (NestedAction.Content as string) == "Initial action" && NestedActionIcon.Text == "+" &&
                NestedAction.Command is not null && HasActionIndicator.IsChecked == true && HasTitleIndicator.IsChecked == true,
                "nested action text/icon/command and computed flags");

            var oldContext = vm.Context;
            oldContext.UpdateTitle("Updated context");
            oldContext.ToggleNavigationVisibility();
            await Expect(() => ContextTitle.Text == "Updated context" && NavigationIndicator.Visibility == Visibility.Collapsed, "inherited property notifications");

            vm.Context = new ContextModel("Replacement context");
            oldContext.UpdateTitle("Detached context");
            oldContext.UpdateAction("Detached context action", "!");
            await Expect(() => ContextTitle.Text == "Replacement context" && NavigationIndicator.Visibility == Visibility.Visible && (NestedAction.Content as string) == "Initial action" && NestedActionIcon.Text == "+", "context replacement / old context detached");

            vm.Context = null;
            await Expect(() => string.IsNullOrEmpty(ContextTitle.Text) && NavigationIndicator.Visibility == Visibility.Collapsed && string.IsNullOrEmpty((NestedAction.Content as string)) && NestedAction.Command is null && HasActionIndicator.IsChecked == false && HasTitleIndicator.IsChecked == false, "null context");
            vm.Context = new ContextModel("Restored context");
            await Expect(() => ContextTitle.Text == "Restored context" && NavigationIndicator.Visibility == Visibility.Visible, "context restored");

            vm.Context.UpdateAction("Updated action", "*");
            await Expect(() => (NestedAction.Content as string) == "Updated action" && NestedActionIcon.Text == "*", "nested action notifications");
            vm.Context.ReplaceAction("Replacement action", ">");
            await Expect(() => (NestedAction.Content as string) == "Replacement action" && NestedActionIcon.Text == ">" && HasActionIndicator.IsChecked == true,
                "nested action replacement / old action detached");
            vm.Context.ClearAction();
            await Expect(() => string.IsNullOrEmpty((NestedAction.Content as string)) && string.IsNullOrEmpty(NestedActionIcon.Text) &&
                NestedAction.Command is null && HasActionIndicator.IsChecked == false, "null nested action");
            vm.Context.ReplaceAction("Restored action", "+");
            await Expect(() => (NestedAction.Content as string) == "Restored action" && NestedActionIcon.Text == "+" &&
                NestedAction.Command is not null && HasActionIndicator.IsChecked == true, "restored nested action");
            // Invoke the command obtained through the control's actual nested binding.
            NestedAction.Command!.Execute(null);
            await Expect(() => ContextTitle.Text == "Action command invoked" && HasTitleIndicator.IsChecked == true,
                "nested bound command updates rendered title");

            vm.Items[0].UpdateTitle("Updated item");
            await Expect(() => HasRowText("Updated item"), "collection item notification");
            vm.Items.Add(new ContextModel("Added item"));
            await Expect(() => Rows.Items.Count == 2 && HasRowText("Added item"), "collection add notification");
            vm.Items.RemoveAt(0);
            await Expect(() => Rows.Items.Count == 1 && !HasRowText("Updated item") && HasRowText("Added item"), "collection remove notification");
            CheckStatus.Text = "PASS: 15 rendered binding checks. Now navigate to named detail, then Back.";
        }
        catch (Exception error)
        {
            CheckStatus.Text = $"FAIL: {error.Message}";
        }
        finally
        {
            _checking = false;
        }
    }

    private static async Task Expect(Func<bool> condition, string scenario)
    {
        // Give the UI dispatcher time to apply bindings and realize collection templates.
        for (var attempt = 0; attempt < 100; attempt++)
        {
            await Task.Delay(20);
            if (condition()) return;
        }
        throw new InvalidOperationException(scenario);
    }

    private bool HasRowText(string text) => Descendants(Rows).OfType<TextBlock>().Any(block => block.Text == text);

    private static IEnumerable<DependencyObject> Descendants(DependencyObject parent)
    {
        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
        {
            var child = VisualTreeHelper.GetChild(parent, index);
            yield return child;
            foreach (var descendant in Descendants(child)) yield return descendant;
        }
    }
}
