using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Prism.Common;
using Prism.Ioc;
using Prism.Properties;

namespace Prism.Regions
{
    /// <summary>
    /// Implementation of <see cref="IRegionNavigationContentLoader"/> that relies on a <see cref="IContainerProvider"/>
    /// to create new views when necessary.
    /// </summary>
    public class RegionNavigationContentLoader : IRegionNavigationContentLoader
    {
        private readonly IContainerProvider container;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationContentLoader"/> class with a service locator.
        /// </summary>
        /// <param name="container">The <see cref="IContainerExtension" />.</param>
        public RegionNavigationContentLoader(IContainerExtension container)
        {
            this.container = container;
        }

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
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            if (navigationContext == null)
                throw new ArgumentNullException(nameof(navigationContext));

            string candidateTargetContract = this.GetContractFromNavigationContext(navigationContext);

            var candidates = this.GetCandidatesFromRegion(region, candidateTargetContract);

            var acceptingCandidates =
                candidates.Where(
                    v =>
                    {
                        if (v is INavigationAware navigationAware && !navigationAware.IsNavigationTarget(navigationContext))
                        {
                            return false;
                        }

                        if (!(v is FrameworkElement frameworkElement))
                        {
                            return true;
                        }

                        navigationAware = frameworkElement.DataContext as INavigationAware;
                        return navigationAware == null || navigationAware.IsNavigationTarget(navigationContext);
                    });


            var view = acceptingCandidates.FirstOrDefault();

            if (view != null)
            {
                return view;
            }

            view = this.CreateNewRegionItem(candidateTargetContract);

            region.Add(view);

            return view;
        }

        /// <summary>
        /// Provides a new item for the region based on the supplied candidate target contract name.
        /// </summary>
        /// <param name="candidateTargetContract">The target contract to build.</param>
        /// <returns>An instance of an item to put into the <see cref="IRegion"/>.</returns>
        protected virtual object CreateNewRegionItem(string candidateTargetContract)
        {
            object newRegionItem;
            try
            {
                newRegionItem = this.container.Resolve<object>(candidateTargetContract);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.CannotCreateNavigationTarget, candidateTargetContract),
                    e);
            }
            return newRegionItem;
        }

        /// <summary>
        /// Returns the candidate TargetContract based on the <see cref="NavigationContext"/>.
        /// </summary>
        /// <param name="navigationContext">The navigation contract.</param>
        /// <returns>The candidate contract to seek within the <see cref="IRegion"/> and to use, if not found, when resolving from the container.</returns>
        protected virtual string GetContractFromNavigationContext(NavigationContext navigationContext)
        {
            if (navigationContext == null) throw new ArgumentNullException(nameof(navigationContext));

            var candidateTargetContract = UriParsingHelper.GetAbsolutePath(navigationContext.Uri);
            candidateTargetContract = candidateTargetContract.TrimStart('/');
            return candidateTargetContract;
        }

        /// <summary>
        /// Returns the set of candidates that may satisfiy this navigation request.
        /// </summary>
        /// <param name="region">The region containing items that may satisfy the navigation request.</param>
        /// <param name="candidateNavigationContract">The candidate navigation target as determined by <see cref="GetContractFromNavigationContext"/></param>
        /// <returns>An enumerable of candidate objects from the <see cref="IRegion"/></returns>
        protected virtual IEnumerable<object> GetCandidatesFromRegion(IRegion region, string candidateNavigationContract)
        {
            if (string.IsNullOrEmpty(candidateNavigationContract))
                throw new ArgumentNullException(nameof(candidateNavigationContract));

            if (region == null)
                throw new ArgumentNullException(nameof(region));

            var contractCandidates = region.Views.Where(v =>
                string.Equals(v.GetType().Name, candidateNavigationContract, StringComparison.Ordinal) ||
                string.Equals(v.GetType().FullName, candidateNavigationContract, StringComparison.Ordinal));

            if (!contractCandidates.Any())
            {
                //var matchingRegistration = _container.GetServiceRegistrations().Where(r => candidateNavigationContract.Equals(r.OptionalServiceKey?.ToString(), StringComparison.Ordinal)).FirstOrDefault();
                //if (matchingRegistration.OptionalServiceKey == null)
                //    matchingRegistration = _container.GetServiceRegistrations().Where(r => candidateNavigationContract.Equals(r.ImplementationType.Name, StringComparison.Ordinal)).FirstOrDefault();

                //if (matchingRegistration.ServiceType == null)
                //    return new object[0];

                //string typeCandidateName = matchingRegistration.ImplementationType.FullName;
                //contractCandidates = base.GetCandidatesFromRegion(region, typeCandidateName);
            }

            // Unity
            //if (!contractCandidates.Any())
            //{
            //    //First try friendly name registration. If not found, try type registration
            //    var matchingRegistration = this.container.Registrations.Where(r => candidateNavigationContract.Equals(r.Name, StringComparison.Ordinal)).FirstOrDefault();
            //    if (matchingRegistration == null)
            //    {
            //        matchingRegistration = this.container.Registrations.Where(r => candidateNavigationContract.Equals(r.RegisteredType.Name, StringComparison.Ordinal)).FirstOrDefault();
            //    }
            //    if (matchingRegistration == null) return new object[0];

            //    string typeCandidateName = matchingRegistration.MappedToType.FullName;

            //    contractCandidates = base.GetCandidatesFromRegion(region, typeCandidateName);
            //}

            return contractCandidates;
        }
    }
}
