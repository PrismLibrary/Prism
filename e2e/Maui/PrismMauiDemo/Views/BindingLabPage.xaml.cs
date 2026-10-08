using PrismMauiDemo.ViewModels;
using ContextModel = PrismMauiDemo.Models.BindingContext;

namespace PrismMauiDemo.Views;

public sealed partial class BindingLabPage : ContentPage
{
    private bool _checking;

    public BindingLabPage() => InitializeComponent();

#if PRISM_NATIVE_AOT_VALIDATION
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AutoRunAsync();
    }
#endif

    private async void RunBindingChecks(object sender, EventArgs e) => await RunBindingChecksAsync();

    private async Task RunBindingChecksAsync()
    {
        if (_checking) return;
        _checking = true;
        CheckStatus.Text = "Running checks...";
        try
        {
            var vm = (BindingLabViewModel)BindingContext;
            vm.Context = new ContextModel();
            vm.Items.Clear();
            vm.Items.Add(new ContextModel("Initial item"));
            await Expect(() => ContextTitle.Text == "Initial context" && Title == "Initial context" && NavigationIndicator.IsVisible && NavigationPage.GetHasNavigationBar(this), "initial nested binding");
            await Expect(() => Rows.Children.Count == 1 && Rows.Children[0] is Label { Text: "Initial item" }, "initial collection item");

            await Expect(() => NestedAction.Text == "Initial action" && NestedActionIcon.Text == "+" &&
                NestedAction.Command is not null && HasActionIndicator.IsChecked == true && HasTitleIndicator.IsChecked == true,
                "nested action text/icon/command and computed flags");

            var oldContext = vm.Context;
            oldContext.UpdateTitle("Updated context");
            oldContext.ToggleNavigationVisibility();
            await Expect(() => ContextTitle.Text == "Updated context" && Title == "Updated context" && !NavigationIndicator.IsVisible && !NavigationPage.GetHasNavigationBar(this), "inherited property notifications");

            vm.Context = new ContextModel("Replacement context");
            oldContext.UpdateTitle("Detached context");
            oldContext.UpdateAction("Detached context action", "!");
            await Expect(() => ContextTitle.Text == "Replacement context" && Title == "Replacement context" && NavigationIndicator.IsVisible && NavigationPage.GetHasNavigationBar(this) && NestedAction.Text == "Initial action" && NestedActionIcon.Text == "+", "context replacement / old context detached");

            vm.Context = null;
            await Expect(() => string.IsNullOrEmpty(ContextTitle.Text) && !NavigationIndicator.IsVisible && NavigationPage.GetHasNavigationBar(this) && string.IsNullOrEmpty(NestedAction.Text) && NestedAction.Command is null && HasActionIndicator.IsChecked == false && HasTitleIndicator.IsChecked == false, "null context");
            vm.Context = new ContextModel("Restored context");
            await Expect(() => ContextTitle.Text == "Restored context" && Title == "Restored context" && NavigationIndicator.IsVisible && NavigationPage.GetHasNavigationBar(this), "context restored");

            vm.Context.UpdateAction("Updated action", "*");
            await Expect(() => NestedAction.Text == "Updated action" && NestedActionIcon.Text == "*", "nested action notifications");
            vm.Context.ReplaceAction("Replacement action", ">");
            await Expect(() => NestedAction.Text == "Replacement action" && NestedActionIcon.Text == ">" && HasActionIndicator.IsChecked == true,
                "nested action replacement / old action detached");
            vm.Context.ClearAction();
            await Expect(() => string.IsNullOrEmpty(NestedAction.Text) && string.IsNullOrEmpty(NestedActionIcon.Text) &&
                NestedAction.Command is null && HasActionIndicator.IsChecked == false, "null nested action");
            vm.Context.ReplaceAction("Restored action", "+");
            await Expect(() => NestedAction.Text == "Restored action" && NestedActionIcon.Text == "+" &&
                NestedAction.Command is not null && HasActionIndicator.IsChecked == true, "restored nested action");
            // Invoke the command obtained through the control's actual nested binding.
            NestedAction.Command!.Execute(null);
            await Expect(() => ContextTitle.Text == "Action command invoked" && HasTitleIndicator.IsChecked == true,
                "nested bound command updates rendered title");

            vm.Items[0].UpdateTitle("Updated item");
            await Expect(() => Rows.Children.Count == 1 && Rows.Children[0] is Label { Text: "Updated item" }, "collection item notification");
            vm.Items.Add(new ContextModel("Added item"));
            await Expect(() => Rows.Children.Count == 2 && Rows.Children[1] is Label { Text: "Added item" }, "collection add notification");
            vm.Items.RemoveAt(0);
            await Expect(() => Rows.Children.Count == 1 && Rows.Children[0] is Label { Text: "Added item" }, "collection remove notification");
            CheckStatus.Text = "PASS: 15 rendered binding checks. Now navigate to named detail, then Back.";
            Console.WriteLine("[binding-check] PASS: 15 rendered checks");
        }
        catch (Exception error)
        {
            CheckStatus.Text = $"FAIL: {error.Message}";
            Console.WriteLine($"[binding-check] FAIL: {error.Message}");
        }
        finally
        {
            _checking = false;
        }
    }


#if PRISM_NATIVE_AOT_VALIDATION
    private static BindingLabPage? _current;
    private static BindingLabPage? _root;

    private async Task AutoRunAsync()
    {
        if (Environment.GetEnvironmentVariable("PRISM_NATIVEAOT_AUTORUN") != "1") return;
        _current = this;
        if (_root is not null) return;
        _root = this;
        try
        {
            Console.WriteLine($"[binding-run] dynamic-code={System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported}");
            await RunBindingChecksAsync();
            RequirePassed(this);
            var rootModel = (BindingLabViewModel)BindingContext;
            var rootContext = rootModel.Context;
            for (var visit = 1; visit <= 2; visit++)
            {
                Console.WriteLine($"[binding-run] detail-{visit} navigate-enter");
                await ((Prism.Commands.IAsyncCommand)NavigateButton.Command!).ExecuteAsync(null);
                await Expect(() => _current is not null && !ReferenceEquals(_current, this), "named detail appeared");
                var detail = _current!;
                if (ReferenceEquals(detail.BindingContext, rootModel)) throw new InvalidOperationException("detail reused root model");
                await Expect(() => detail.ContextTitle.Text == "Initial context", "fresh detail context");
                await detail.RunBindingChecksAsync();
                RequirePassed(detail);
                await ((Prism.Commands.IAsyncCommand)detail.BackButton.Command!).ExecuteAsync(null);
                await Expect(() => ReferenceEquals(_current, this), "Back restored root view");
                await Expect(() => ReferenceEquals(BindingContext, rootModel) && ReferenceEquals(rootModel.Context, rootContext) && ContextTitle.Text == "Action command invoked", "Back preserved root model and rendered state");
                RequirePassed(this);
                Console.WriteLine($"[binding-run] detail-{visit} Back PASS");
            }
            Console.WriteLine("[binding-run] COMPLETE root + two named detail / Back visits");
        }
        catch (Exception error)
        {
            Console.WriteLine($"[binding-run] FAIL: {error.Message}");
            CheckStatus.Text = $"FAIL: {error.Message}";
        }
    }

    private static void RequirePassed(BindingLabPage page)
    {
        if (!page.CheckStatus.Text.StartsWith("PASS:", StringComparison.Ordinal))
            throw new InvalidOperationException(page.CheckStatus.Text);
    }
#endif

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
}
