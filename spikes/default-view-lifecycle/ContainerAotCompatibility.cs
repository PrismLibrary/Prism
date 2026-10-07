// DIAGNOSTIC HARNESS ONLY. The public 9.x container contract has no ContainerAot.
// These tests exercise the ordinary reflection fallback, not generated/AOT lookup.
// Never include this file in a production build or treat this as AOT validation.
namespace Prism.Ioc;

internal static class ContainerAot
{
    internal static bool TryGetImplementationType(System.Type type, out System.Type result)
    {
        result = null;
        return false;
    }

    internal static bool TryGetImplementationType(string name, System.Reflection.Assembly assembly, out System.Type result)
    {
        result = null;
        return false;
    }
}
