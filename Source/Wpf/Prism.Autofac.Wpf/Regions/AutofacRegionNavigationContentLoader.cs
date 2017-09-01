using Autofac;
using Autofac.Core;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Unity.Regions
{
    public class AutofacRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        private IContainer container;

        public AutofacRegionNavigationContentLoader(IServiceLocator serviceLocator, IContainer container)
            : base(serviceLocator)
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
                //First try friendly name registration.
                var matchingRegistration = container.ComponentRegistry.Registrations.Where(r => r.Services.OfType<KeyedService>().Any(s => s.ServiceKey.Equals(candidateNavigationContract))).FirstOrDefault();

                //If not found, try type registration
                if (matchingRegistration == null)
                    matchingRegistration = container.ComponentRegistry.Registrations.Where(r => candidateNavigationContract.Equals(r.Activator.LimitType.Name, StringComparison.Ordinal)).FirstOrDefault();

                if (matchingRegistration == null)
                    return new object[0];

                string typeCandidateName = matchingRegistration.Activator.LimitType.FullName;

                contractCandidates = base.GetCandidatesFromRegion(region, typeCandidateName);
            }

            return contractCandidates;
        }
    }
}
