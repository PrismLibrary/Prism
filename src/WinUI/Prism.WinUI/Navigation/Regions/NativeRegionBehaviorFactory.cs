using System.Collections;
using System.Globalization;
using Prism.Properties;

namespace Prism.Navigation.Regions;

// Registrations are application-wide; behavior instances are resolved by the owning region.
internal sealed class NativeRegionBehaviorFactory : IRegionBehaviorFactory
{
    private readonly IContainerProvider _container;
    private readonly Dictionary<string, Type> _registeredBehaviors = new();

    internal NativeRegionBehaviorFactory(IContainerProvider container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    public void AddIfMissing(string behaviorKey, Type behaviorType)
    {
        ArgumentNullException.ThrowIfNull(behaviorKey);
        ArgumentNullException.ThrowIfNull(behaviorType);
        if (!typeof(IRegionBehavior).IsAssignableFrom(behaviorType))
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                Resources.CanOnlyAddTypesThatInheritIFromRegionBehavior, behaviorType.Name), nameof(behaviorType));

        _registeredBehaviors.TryAdd(behaviorKey, behaviorType);
    }

    public void AddOrReplace(string behaviorKey, Type behaviorType)
    {
        ArgumentNullException.ThrowIfNull(behaviorKey);
        ArgumentNullException.ThrowIfNull(behaviorType);
        if (!typeof(IRegionBehavior).IsAssignableFrom(behaviorType))
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                Resources.CanOnlyAddTypesThatInheritIFromRegionBehavior, behaviorType.Name), nameof(behaviorType));

        _registeredBehaviors[behaviorKey] = behaviorType;
    }

    public bool ContainsKey(string behaviorKey) => _registeredBehaviors.ContainsKey(behaviorKey);

    public IRegionBehavior CreateFromKey(string key) => CreateFromKey(key, _container);

    internal IRegionBehavior CreateFromKey(string key, IContainerProvider provider)
    {
        if (!_registeredBehaviors.TryGetValue(key, out var behaviorType))
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                Resources.TypeWithKeyNotRegistered, key), nameof(key));

        return (IRegionBehavior)provider.Resolve(behaviorType, (typeof(IContainerProvider), provider));
    }

    public IEnumerator<string> GetEnumerator() => _registeredBehaviors.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
