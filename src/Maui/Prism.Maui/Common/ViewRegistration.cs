namespace Prism.Common;

public record ViewRegistration
{
    public ViewType Type { get; init; }
    public Type View { get; init; }
    public Type ViewModel { get; init; }
    public string Name { get; init; }
}
