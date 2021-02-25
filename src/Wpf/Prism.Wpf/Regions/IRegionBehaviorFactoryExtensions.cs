namespace Prism.Regions
{
    /// <summary>
    /// Extension methods for the IRegionBehaviorFactory.
    /// </summary>
    public static class IRegionBehaviorFactoryExtensions
    {
        /// <summary>
        /// Adds a particular type of RegionBehavior if it was not already registered. the <paramref name="behaviorKey"/> string is used to check if the behavior is already present
        /// </summary>
        /// <typeparam name="T">Type of the behavior to add.</typeparam>
        /// <param name="regionBehaviorFactory">The IRegionBehaviorFactory instance</param>
        /// <param name="behaviorKey">The behavior key that's used to find if a certain behavior is already added.</param>
        public static void AddIfMissing<T>(this IRegionBehaviorFactory regionBehaviorFactory, string behaviorKey) where T : IRegionBehavior
        {
            var behaviorType = typeof(T);
            regionBehaviorFactory.AddIfMissing(behaviorKey, behaviorType);
        }
    }
}
