using System.ComponentModel;
using Prism.Ioc;
using Prism.Navigation.Xaml;

namespace Prism.Navigation.Regions;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface ITargetAwareRegion : IRegion
{
    VisualElement TargetElement { get; set; }

    IContainerProvider Container => TargetElement.GetContainerProvider();
}
