namespace Prism.Navigation.Regions;

public interface ITargetAwareRegion : IRegion
{
    object Target { get; }
}
