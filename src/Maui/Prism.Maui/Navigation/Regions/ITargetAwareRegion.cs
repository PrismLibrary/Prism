using System.ComponentModel;
using Prism.Ioc;
using Prism.Navigation.Xaml;

namespace Prism.Navigation.Regions;

/// <summary>Associates a region with the visual element and container that host it.</summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface ITargetAwareRegion : IRegion
{
    /// <summary>Gets or sets the visual element hosting the region.</summary>
    VisualElement TargetElement { get; set; }

    /// <summary>Gets the container provider associated with the hosting element.</summary>
    IContainerProvider Container => TargetElement.GetContainerProvider();
}
