using Prism.Mvvm;

namespace Prism.Common;

public interface IRegistryAware
{
    IViewRegistry Registry { get; }
}