using CommonServiceLocator;
using DryIoc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.DryIoc.Regions
{
    public class DryIocRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        private readonly IContainer _container;

        public DryIocRegionNavigationContentLoader(IServiceLocator serviceLocator, IContainer container) : base(serviceLocator)
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
