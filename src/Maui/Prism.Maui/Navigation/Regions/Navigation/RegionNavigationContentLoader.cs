using System.Globalization;
using Prism.Common;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Properties;

namespace Prism.Navigation.Regions.Navigation;

/// <summary>
/// Implementation of <see cref="IRegionNavigationContentLoader"/> that relies on a <see cref="IContainerProvider"/>
/// to create new views when necessary.
/// </summary>
public class RegionNavigationContentLoader : IRegionNavigationContentLoader
{
    /// <summary>
    /// Gets the view to which the navigation request represented by <paramref name="navigationContext"/> applies.
    /// </summary>
    /// <param name="region">The region.</param>
    /// <param name="navigationContext">The context representing the navigation request.</param>
    /// <returns>
    /// The view to be the target of the navigation request.
    /// </returns>
    /// <remarks>
    /// If none of the views in the region can be the target of the navigation request, a new view
    /// is created and added to the region.
    /// </remarks>
    /// <exception cref="ArgumentException">when a new view cannot be created for the navigation request.</exception>
    public object LoadContent(IRegion region, NavigationContext navigationContext)
    {
        ArgumentNullException.ThrowIfNull(region);
        ArgumentNullException.ThrowIfNull(navigationContext);

        string candidateTargetContract = GetContractFromNavigationContext(navigationContext);

        var candidates = GetCandidatesFromRegion(region, candidateTargetContract);

        var acceptingCandidates =
            candidates.Where(
                v => MvvmHelpers.IsNavigationTarget(v, navigationContext));

        var view = acceptingCandidates.FirstOrDefault();

        if (view != null)
        {
            return view;
        }

        view = CreateNewRegionItem(candidateTargetContract, region) as VisualElement;
        region.Add(view);

        return view;
    }

    /// <summary>
    /// Provides a new item for the region based on the supplied candidate target contract name.
    /// </summary>
    /// <param name="candidateTargetContract">The target contract to build.</param>
    /// <param name="region">The <see cref="IRegion"/> to create the new Region Item in.</param>
    /// <returns>An instance of an item to put into the <see cref="IRegion"/>.</returns>
    protected virtual object CreateNewRegionItem(string candidateTargetContract, IRegion region)
    {
        try
        {
            var registry = region.Container().Resolve<IRegionNavigationRegistry>();
            return registry.CreateView(region.Container(), candidateTargetContract);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (ContainerResolutionException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture, Resources.CannotCreateNavigationTarget, candidateTargetContract),
                e);
        }
    }

    /// <summary>
    /// Returns the candidate TargetContract based on the <see cref="NavigationContext"/>.
    /// </summary>
    /// <param name="navigationContext">The navigation contract.</param>
    /// <returns>The candidate contract to seek within the <see cref="IRegion"/> and to use, if not found, when resolving from the container.</returns>
    protected virtual string GetContractFromNavigationContext(NavigationContext navigationContext)
    {
        ArgumentNullException.ThrowIfNull(navigationContext);

        var candidateTargetContract = UriParsingHelper.EnsureAbsolute(navigationContext.Uri).AbsolutePath;
        candidateTargetContract = candidateTargetContract.TrimStart('/');
        return candidateTargetContract;
    }

    /// <summary>
    /// Returns the set of candidates that may satisfy this navigation request.
    /// </summary>
    /// <param name="region">The region containing items that may satisfy the navigation request.</param>
    /// <param name="candidateNavigationContract">The candidate navigation target as determined by <see cref="GetContractFromNavigationContext"/></param>
    /// <returns>An enumerable of candidate objects from the <see cref="IRegion"/></returns>
    protected virtual IEnumerable<VisualElement> GetCandidatesFromRegion(IRegion region, string candidateNavigationContract)
    {
        ArgumentNullException.ThrowIfNull(region);

        if (string.IsNullOrEmpty(candidateNavigationContract))
        {
            throw new ArgumentNullException(nameof(candidateNavigationContract));
        }

        var contractCandidates = RegionNavigationContentLoader.GetCandidatesFromRegionViews(region, candidateNavigationContract);

        if (!contractCandidates.Any())
        {
            var registry = region.Container().Resolve<IRegionNavigationRegistry>();
            var registration = registry.Registrations.FirstOrDefault(x => x.Type == ViewType.Region && (x.Name == candidateNavigationContract || x.View.Name == candidateNavigationContract || x.View.FullName == candidateNavigationContract));
            if (registration is null)
            {
                RegionNavigationContentLoader.GetCandidatesFromRegionViews(region, registration.View.FullName);
            }

            return [];
        }

        return contractCandidates;
    }

    private static IEnumerable<VisualElement> GetCandidatesFromRegionViews(IRegion region, string candidateNavigationContract)
    {
        return region.Views.OfType<VisualElement>().Where(v => ViewIsMatch(v.GetType(), candidateNavigationContract));
    }

    private static bool ViewIsMatch(Type viewType, string navigationSegment)
    {
        return viewType.FullName == navigationSegment || viewType.Name == navigationSegment;
    }
}
