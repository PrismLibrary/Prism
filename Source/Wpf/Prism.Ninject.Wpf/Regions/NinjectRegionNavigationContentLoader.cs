using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using Ninject;
using Prism.Regions;

namespace Prism.Ninject.Regions
{
    public class NinjectRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        private readonly IKernel _kernel;

        public NinjectRegionNavigationContentLoader(
            IServiceLocator serviceLoactor,
            IKernel kernel)
            : base(serviceLoactor)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Returns the set of candidates that may satisfiy this navigation request.
        /// </summary>
        /// <param name="region">The region containing items that may satisfy the navigation request.</param>
        /// <param name="candidateNavigationContract">The candidate navigation target.</param>
        /// <returns>An enumerable of candidate objects from the <see cref="IRegion"/></returns>
        protected override IEnumerable<object> GetCandidatesFromRegion(IRegion region,
            string candidateNavigationContract)
        {
            if (candidateNavigationContract == null || candidateNavigationContract.Equals(string.Empty))
            {
                throw new ArgumentNullException(nameof(candidateNavigationContract));
            }

            var contractCandidates = base.GetCandidatesFromRegion(region, candidateNavigationContract);

            if (!contractCandidates.Any())
            {
                // find by name
                var matchingRegistration = _kernel
                    .GetBindings(typeof(object))
                    .FirstOrDefault(r => candidateNavigationContract.Equals(
                        r.BindingConfiguration.Metadata.Name,
                        StringComparison.Ordinal));

                if (matchingRegistration == null)
                {
                    return contractCandidates;
                }

                var kernelTarget = matchingRegistration.ProviderCallback?.Target;
                var typeCandidateName = kernelTarget?.GetType().GetField("prototype").GetValue(kernelTarget).ToString();

                contractCandidates = base.GetCandidatesFromRegion(region, typeCandidateName);
            }

            return contractCandidates;
        }
    }
}