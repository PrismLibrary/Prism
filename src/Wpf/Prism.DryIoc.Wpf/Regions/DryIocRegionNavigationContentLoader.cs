using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc;
using Prism.Ioc;
using Prism.Regions;

namespace Prism.DryIoc.Regions
{
    public class DryIocRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        private readonly IContainer _container;

        public DryIocRegionNavigationContentLoader(IContainerExtension containerExtension, IContainer container) : base(containerExtension)
        {
            _container = container;
        }

        protected override IEnumerable<object> GetCandidatesFromRegion(IRegion region, string candidateNavigationContract)
        {
            if (candidateNavigationContract == null || candidateNavigationContract.Equals(string.Empty))
                throw new ArgumentNullException(nameof(candidateNavigationContract));

            IEnumerable<object> contractCandidates = base.GetCandidatesFromRegion(region, candidateNavigationContract);

            if (!contractCandidates.Any())
            {
                var matchingRegistration = _container.GetServiceRegistrations().Where(r => candidateNavigationContract.Equals(r.OptionalServiceKey?.ToString(), StringComparison.Ordinal)).FirstOrDefault();
                if (matchingRegistration.OptionalServiceKey == null)
                    matchingRegistration = _container.GetServiceRegistrations().Where(r => candidateNavigationContract.Equals(r.ImplementationType.Name, StringComparison.Ordinal)).FirstOrDefault();

                if (matchingRegistration.ServiceType == null)
                    return new object[0];

                string typeCandidateName = matchingRegistration.ImplementationType.FullName;
                contractCandidates = base.GetCandidatesFromRegion(region, typeCandidateName);
            }

            return contractCandidates;
        }
    }
}
