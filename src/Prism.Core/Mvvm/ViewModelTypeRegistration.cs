using System.Diagnostics.CodeAnalysis;

namespace Prism.Mvvm;

internal sealed class ViewModelTypeRegistration
{
    public ViewModelTypeRegistration([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type) => Type = type;

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type Type { get; }
}
