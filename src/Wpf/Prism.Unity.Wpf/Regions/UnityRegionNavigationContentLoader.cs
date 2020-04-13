using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Ioc;
using Prism.Regions;
using Unity;

namespace Prism.Unity.Regions
{
    /// <summary>
    /// Specialization of the default RegionNavigationContentLoader that queries the corresponding <see cref="IUnityContainer"/>
    /// to obtain the name of the view's type registered for the contract name.
    /// </summary>
    public class UnityRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        private IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityRegionNavigationContentLoader"/> class.
        /// </summary>
        /// <param name="containerExtension"><see cref="IContainerExtension"/> used to create the instance of the view from its <see cref="Type"/>.</param>
        /// <param name="container"><see cref="IUnityContainer"/> where the views are registered.</param>
        public UnityRegionNavigationContentLoader(IContainerExtension containerExtension, IUnityContainer container)
            : base(containerExtension)
        {
            this.container = container;
        }

        /// <summary>
        /// Returns the set of candidates that may satisfiy this navigation request.
        /// </summary>
        /// <param name="region">The region containing items that may satisfy the navigation request.</param>
        /// <param name="candidateNavigationContract">The candidate navigation target.</param>
        /// <returns>An enumerable of candidate objects from the <see cref="IRegion"/></returns>
        protected override IEnumerable<object> GetCandidatesFromRegion(IRegion region, string candidateNavigationContract)
        {
            if (candidateNavigationContract == null || candidateNavigationContract.Equals(string.Empty))
                throw new ArgumentNullException(nameof(candidateNavigationContract));

            IEnumerable<object> contractCandidates = base.GetCandidatesFromRegion(region, candidateNavigationContract);

            if (!contractCandidates.Any())
            {
                //First try friendly name registration. If not found, try type registration
                var matchingRegistration = this.container.Registrations.Where(r => candidateNavigationContract.Equals(r.Name, StringComparison.Ordinal)).FirstOrDefault();
                if (matchingRegistration == null)
                {
                    matchingRegistration = this.container.Registrations.Where(r => candidateNavigationContract.Equals(r.RegisteredType.Name, StringComparison.Ordinal)).FirstOrDefault();
                }
                if (matchingRegistration == null) return new object[0];

                string typeCandidateName = matchingRegistration.MappedToType.FullName;

                contractCandidates = base.GetCandidatesFromRegion(region, typeCandidateName);
            }

            return contractCandidates;
        }
    }
}
