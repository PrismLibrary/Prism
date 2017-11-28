using CommonServiceLocator;
using Grace.DependencyInjection;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Grace.Wpf.Regions
{
    public class GraceRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        private DependencyInjectionContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraceRegionNavigationContentLoader"/> class.
        /// </summary>
        /// <param name="serviceLocator"><see cref="IServiceLocator"/> used to create the instance of the view from its <see cref="Type"/>.</param>
        /// <param name="container"><see cref="DependencyInjectionContainer"/> where the views are registered.</param>
        public GraceRegionNavigationContentLoader(IServiceLocator serviceLocator, DependencyInjectionContainer container)
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
                var strategy = container
                    .StrategyCollectionContainer
                    .GetActivationStrategyCollectionByName(candidateNavigationContract)?
                    .GetPrimary();

                if (strategy == null)
                {
                    return new object[0];
                }

                contractCandidates = base.GetCandidatesFromRegion(region, strategy.ActivationType.FullName);
            }

            return contractCandidates;
        }
    }
}