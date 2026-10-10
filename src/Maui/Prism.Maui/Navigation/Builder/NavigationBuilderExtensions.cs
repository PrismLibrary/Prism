using System.Diagnostics.CodeAnalysis;
using Prism.Common;
using Prism.Navigation.Builder;

namespace Prism.Navigation;

/// <summary>Builds navigation routes using registered views, view models, navigation pages, and tabs.</summary>
public static class NavigationBuilderExtensions
{
    /// <summary>
    /// Creates a <see cref="INavigationBuilder"/> using the current instance of the <see cref="INavigationService"/>.
    /// </summary>
    /// <param name="navigationService">The <see cref="INavigationService"/>.</param>
    /// <returns><see cref="INavigationBuilder"/></returns>
    public static INavigationBuilder CreateBuilder(this INavigationService navigationService) =>
           new NavigationBuilder(navigationService);

    internal static string GetNavigationKey<TViewModel>(object builder)
    {
        var vmType = typeof(TViewModel);
        if (vmType.IsAssignableFrom(typeof(VisualElement)))
            throw new NavigationException(NavigationException.MvvmPatternBreak, typeof(TViewModel).Name);

        if (builder is not IRegistryAware registryAware)
            throw new Exception("The builder does not implement IRegistryAware");

        return registryAware.Registry.GetViewModelNavigationKey(vmType);
    }

    /// <summary>Adds a parent-navigation segment to the route.</summary>
    /// <param name="builder">The route builder to update.</param>
    /// <returns>The same builder with a <c>..</c> segment appended.</returns>
    public static INavigationBuilder RelativeBack(this INavigationBuilder builder) =>
        builder.AddSegment("..");

    /// <summary>
    /// This will force the generated Navigation URI to return an Absolute URI resetting the current <see cref="Window"/>'s <see cref="Page"/> property.
    /// </summary>
    /// <param name="builder">The <see cref="INavigationBuilder"/>.</param>
    /// <returns>The <see cref="INavigationBuilder"/>.</returns>
    public static INavigationBuilder UseAbsoluteNavigation(this INavigationBuilder builder) =>
        builder.UseAbsoluteNavigation(true);

    /// <summary>
    /// Adds the specified segment `ViewA` to the Navigation URI
    /// </summary>
    /// <param name="builder">The <see cref="INavigationBuilder"/>.</param>
    /// <param name="segmentName">The navigation segment name `ViewA`.</param>
    /// <param name="useModalNavigation">An optional parameter whether to force Modal Navigation or use the default behavior based on the Navigation Stack.</param>
    /// <returns>The <see cref="INavigationBuilder"/>.</returns>
    public static INavigationBuilder AddSegment(this INavigationBuilder builder, string segmentName, bool? useModalNavigation = null) =>
        builder.AddSegment(segmentName, o =>
        {
            if (useModalNavigation.HasValue)
                o.UseModalNavigation(useModalNavigation.Value);
        });

    /// <summary>
    /// Adds the registered Navigation Segment name for the specified ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel to navigate to.</typeparam>
    /// <param name="builder">The <see cref="INavigationBuilder"/>.</param>
    /// <returns>The <see cref="INavigationBuilder"/>.</returns>
    public static ICreateTabBuilder AddSegment<TViewModel>(this ICreateTabBuilder builder)  =>
        builder.AddSegment<TViewModel>(b => { });

    /// <summary>Adds the registered view for a view model to a tab's route and configures the segment.</summary>
    /// <typeparam name="TViewModel">The registered view model identifying the destination.</typeparam>
    /// <param name="builder">The tab route builder.</param>
    /// <param name="configureSegment">The callback configuring segment parameters and options.</param>
    /// <returns>The updated tab route builder.</returns>
    public static ICreateTabBuilder AddSegment<TViewModel>(this ICreateTabBuilder builder, Action<ISegmentBuilder> configureSegment) =>
        builder.AddSegment(GetNavigationKey<TViewModel>(builder), configureSegment);

    /// <summary>Adds the registered view for a view model to the navigation route.</summary>
    /// <typeparam name="TViewModel">The registered view model identifying the destination.</typeparam>
    /// <param name="builder">The route builder.</param>
    /// <returns>The updated route builder.</returns>
    public static INavigationBuilder AddSegment<TViewModel>(this INavigationBuilder builder) =>
        builder.AddSegment<TViewModel>(b => { });

    /// <summary>Adds the registered view for a view model to the route and configures the segment.</summary>
    /// <typeparam name="TViewModel">The registered view model identifying the destination.</typeparam>
    /// <param name="builder">The route builder.</param>
    /// <param name="configureSegment">The callback configuring segment parameters and options.</param>
    /// <returns>The updated route builder.</returns>
    public static INavigationBuilder AddSegment<TViewModel>(this INavigationBuilder builder, Action<ISegmentBuilder> configureSegment) =>
        builder.AddSegment(GetNavigationKey<TViewModel>(builder), configureSegment);

    /// <summary>Adds the registered view for a view model with an explicit modal navigation choice.</summary>
    /// <typeparam name="TViewModel">The registered view model identifying the destination.</typeparam>
    /// <param name="builder">The route builder.</param>
    /// <param name="useModalNavigation">Whether the segment uses modal navigation.</param>
    /// <returns>The updated route builder.</returns>
    public static INavigationBuilder AddSegment<TViewModel>(this INavigationBuilder builder, bool useModalNavigation) =>
        builder.AddSegment<TViewModel>(b => b.UseModalNavigation(useModalNavigation));

    // Will check for the Navigation key of a registered NavigationPage
    /// <summary>Adds the last registered navigation page type to the route.</summary>
    /// <param name="builder">The route builder.</param>
    /// <returns>The updated route builder.</returns>
    public static INavigationBuilder AddNavigationPage(this INavigationBuilder builder) =>
        builder.AddNavigationPage(b => { });

    /// <summary>Adds the last registered navigation page type and configures its route segment.</summary>
    /// <param name="builder">The route builder.</param>
    /// <param name="configureSegment">The callback configuring segment parameters and options.</param>
    /// <returns>The updated route builder.</returns>
    /// <exception cref="NavigationException">No navigation page type is registered.</exception>
    public static INavigationBuilder AddNavigationPage(this INavigationBuilder builder, Action<ISegmentBuilder> configureSegment)
    {
        if (builder is not IRegistryAware registryAware)
            throw new Exception("The builder does not implement IRegistryAware");

        var registrations = registryAware.Registry.ViewsOfType(typeof(NavigationPage));
        if (!registrations.Any())
            throw new NavigationException(NavigationException.NoPageIsRegistered, nameof(NavigationPage));

        var registration = registrations.Last();
        return builder.AddSegment(registration.Name, configureSegment);
    }

    /// <summary>Adds the last registered navigation page type to the tab's route.</summary>
    /// <param name="builder">The tab route builder.</param>
    /// <returns>The updated tab route builder.</returns>
    public static ICreateTabBuilder AddNavigationPage(this ICreateTabBuilder builder) =>
        builder.AddNavigationPage(b => { });

    /// <summary>Adds the last registered navigation page type to a tab and configures its segment.</summary>
    /// <param name="builder">The tab route builder.</param>
    /// <param name="configureSegment">The callback configuring segment parameters and options.</param>
    /// <returns>The updated tab route builder.</returns>
    /// <exception cref="NavigationException">No navigation page type is registered.</exception>
    public static ICreateTabBuilder AddNavigationPage(this ICreateTabBuilder builder, Action<ISegmentBuilder> configureSegment)
    {
        if (builder is not IRegistryAware registryAware)
            throw new Exception("The builder does not implement IRegistryAware");

        var registrations = registryAware.Registry.ViewsOfType(typeof(NavigationPage));
        if (!registrations.Any())
            throw new NavigationException(NavigationException.NoPageIsRegistered, nameof(NavigationPage));

        var registration = registrations.Last();
        return builder.AddSegment(registration.Name, configureSegment);
    }

    /// <summary>
    /// Adds a NavigationPage to the Navigation Uri
    /// </summary>
    /// <param name="builder">The <see cref="INavigationBuilder"/>.</param>
    /// <param name="useModalNavigation">When <see langword="true" /> this will force Modal Navigation.</param>
    /// <returns>The <see cref="INavigationBuilder"/>.</returns>
    /// <remarks>This should only be used when you have a single <see cref="NavigationPage"/> registered for Navigation. Typically this is automatically registered for you by Prism.</remarks>
    public static INavigationBuilder AddNavigationPage(this INavigationBuilder builder, bool useModalNavigation) =>
        builder.AddNavigationPage(o => o.UseModalNavigation(useModalNavigation));

    /// <summary>
    /// Navigates back to the specified view model asynchronously.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel to navigate to.</typeparam>
    /// <param name="builder">The <see cref="INavigationBuilder"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<INavigationResult> GoBackToAsync<TViewModel>(this INavigationBuilder builder) =>
        builder.GoBackToAsync(GetNavigationKey<TViewModel>(builder));

    //public static INavigationBuilder AddSegment(this INavigationBuilder builder, string segmentName, params string[] createTabs)
    //{
    //    return builder;
    //}

    //public static INavigationBuilder AddSegment(this INavigationBuilder builder, string segmentName, bool useModalNavigation, params string[] createTabs)
    //{
    //    return builder;
    //}

    //public static INavigationBuilder AddSegment(this INavigationBuilder builder, string segmentName, string selectTab, bool? useModalNavigation, params string[] createTabs)
    //{
    //    return builder;
    //}

    /// <summary>
    /// Navigates to the URI generated by the <see cref="INavigationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="INavigationBuilder"/>.</param>
    [ExcludeFromCodeCoverage(Justification = "Fire-and-forget UI entry point; use INavigationBuilder.NavigateAsync for testable navigation.")]
    public static async void Navigate(this INavigationBuilder builder)
    {
        await builder.NavigateAsync();
    }

    /// <summary>
    /// Navigates to the URI generated by the <see cref="INavigationBuilder"/> with a specified callback
    /// when the Navigation is unsuccessful with an Exception encountered while navigating. This is typically a <see cref="NavigationException"/>. You should check that the navigation was not cancelled.
    /// </summary>
    /// <param name="builder">The <see cref="INavigationBuilder"/>.</param>
    /// <param name="onError">Delegate to handle when we encounter a Navigation Exception</param>
    [ExcludeFromCodeCoverage(Justification = "Fire-and-forget UI entry point; use INavigationBuilder.NavigateAsync for testable navigation.")]
    public static async void Navigate(this INavigationBuilder builder, Action<Exception> onError)
    {
        await builder.NavigateAsync(onError);
    }

    /// <summary>
    /// Navigates to the URI generated by the <see cref="INavigationBuilder"/> with a specified callback when the Navigation is successful.
    /// </summary>
    /// <param name="builder">The <see cref="INavigationBuilder"/>.</param>
    /// <param name="onSuccess">Delegate to handle when the navigation is successful</param>
    [ExcludeFromCodeCoverage(Justification = "Fire-and-forget UI entry point; use INavigationBuilder.NavigateAsync for testable navigation.")]
    public static async void Navigate(this INavigationBuilder builder, Action onSuccess)
    {
        await builder.NavigateAsync(onSuccess, _ => { });
    }

    /// <summary>
    /// Navigates to the URI generated by the <see cref="INavigationBuilder"/> with callbacks for navigation success and errors.
    /// </summary>
    /// <param name="builder">The <see cref="INavigationBuilder"/>.</param>
    /// <param name="onSuccess">Delegate to handle when the navigation is successful</param>
    /// <param name="onError">Delegate to handle when we encounter a Navigation Exception</param>
    [ExcludeFromCodeCoverage(Justification = "Fire-and-forget UI entry point; use INavigationBuilder.NavigateAsync for testable navigation.")]
    public static async void Navigate(this INavigationBuilder builder, Action onSuccess, Action<Exception> onError)
    {
        await builder.NavigateAsync(onSuccess, onError);
    }

    /// <summary>
    /// Forces Modal Navigation
    /// </summary>
    /// <param name="builder">The <see cref="ISegmentBuilder"/>.</param>
    /// <returns>The <see cref="INavigationBuilder"/>.</returns>
    public static ISegmentBuilder UseModalNavigation(this ISegmentBuilder builder) =>
        builder.UseModalNavigation(true);

    /// <summary>
    /// Adds a Segment to the <see cref="ICreateTabBuilder"/> for deep linking within a created tab
    /// </summary>
    /// <param name="builder">The <see cref="ICreateTabBuilder"/>.</param>
    /// <param name="segmentNameOrUri">The Navigation Segment name `ViewA` or uri `ViewA?id=5`</param>
    /// <returns>The <see cref="INavigationBuilder"/>.</returns>
    public static ICreateTabBuilder AddSegment(this ICreateTabBuilder builder, string segmentNameOrUri) =>
            builder.AddSegment(segmentNameOrUri, null);

    /// <summary>
    /// Will dynamically create a Tab within the <see cref="TabbedPage"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ITabbedSegmentBuilder"/>.</param>
    /// <param name="segmentName">The name of the <see cref="TabbedPage"/>.</param>
    /// <param name="configureSegment">Delegate to configure the generated <see cref="TabbedPage"/>.</param>
    /// <returns>The <see cref="ITabbedSegmentBuilder"/>.</returns>
    public static ITabbedSegmentBuilder CreateTab(this ITabbedSegmentBuilder builder, string segmentName, Action<ISegmentBuilder> configureSegment) =>
        builder.CreateTab(o => o.AddSegment(segmentName, configureSegment));

    /// <summary>
    /// Dynamically creates a tab within a <see cref="TabbedPage"/> based on a Segment Name `ViewA` or Uri `ViewA?id=5`.
    /// </summary>
    /// <param name="builder">The <see cref="ITabbedSegmentBuilder"/>.</param>
    /// <param name="segmentNameOrUri">The View name or Uri</param>
    /// <returns>The <see cref="ITabbedSegmentBuilder"/>.</returns>
    public static ITabbedSegmentBuilder CreateTab(this ITabbedSegmentBuilder builder, string segmentNameOrUri) =>
        builder.CreateTab(o => o.AddSegment(segmentNameOrUri));

    /// <summary>Creates a tab using the registered view for a view model.</summary>
    /// <typeparam name="TViewModel">The registered view model identifying the tab's page.</typeparam>
    /// <param name="builder">The tabbed page segment builder.</param>
    /// <returns>The updated tabbed page segment builder.</returns>
    public static ITabbedSegmentBuilder CreateTab<TViewModel>(this ITabbedSegmentBuilder builder)
    {
        var navigationKey = GetNavigationKey<TViewModel>(builder);
        return builder.CreateTab(navigationKey);
    }

    /// <summary>
    /// Selects the active tab for the specified ViewModel
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel</typeparam>
    /// <param name="builder">The <see cref="ITabbedSegmentBuilder"/>.</param>
    /// <returns>The <see cref="ITabbedSegmentBuilder"/>.</returns>
    public static ITabbedSegmentBuilder SelectTab<TViewModel>(this ITabbedSegmentBuilder builder)
    {
        var navigationKey = GetNavigationKey<TViewModel>(builder);
        return builder.SelectedTab(navigationKey);
    }

    /// <summary>
    /// Will Select a specific Tab within the <see cref="TabbedPage"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ITabbedNavigationBuilder"/>.</param>
    /// <param name="navigationSegments">The Navigation Segment Name or Names.</param>
    /// <returns></returns>
    /// <remarks>Typically only a single Navigation Segment should be needed. In the event multiple tabs use a <see cref="NavigationPage"/> you should specify the name of the NavigationPage &amp; the name of the Current or Top most Page within the tab you want to navigate to</remarks>
    /// <example>
    /// builder.SelectTab("NavigationPage", "ViewA");
    /// </example>
    public static ITabbedNavigationBuilder SelectTab(this ITabbedNavigationBuilder builder, params string[] navigationSegments) =>
        builder.SelectTab(string.Join("|", navigationSegments));
}
