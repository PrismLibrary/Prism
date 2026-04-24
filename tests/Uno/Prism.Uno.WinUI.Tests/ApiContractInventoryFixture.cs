using System.Reflection;
using Xunit;

namespace Prism.Uno.WinUI.Tests;

public class ApiContractInventoryFixture
{
    private static readonly Assembly PrismAssembly = typeof(PrismApplicationBase).Assembly;

    public static IEnumerable<object[]> ExportedPrismTypeCases()
    {
        return PrismAssembly
            .GetExportedTypes()
            .Where(t => t.Namespace is not null && t.Namespace.StartsWith("Prism", StringComparison.Ordinal))
            .OrderBy(t => t.FullName, StringComparer.Ordinal)
            .Select(t => new object[] { t.FullName! });
    }

    public static IEnumerable<object[]> PublicMemberCases()
    {
        const BindingFlags memberFlags =
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly;

        return PrismAssembly
            .GetExportedTypes()
            .Where(t => t.Namespace is not null && t.Namespace.StartsWith("Prism", StringComparison.Ordinal))
            .OrderBy(t => t.FullName, StringComparer.Ordinal)
            .SelectMany(t => t
                .GetMembers(memberFlags)
                .Where(m => m.MemberType is MemberTypes.Method or MemberTypes.Property or MemberTypes.Field or MemberTypes.Event)
                .OrderBy(m => m.Name, StringComparer.Ordinal)
                .Select(m => new object[] { t.FullName!, m.Name, m.MemberType.ToString() }))
            .Take(450);
    }

    [Theory]
    [MemberData(nameof(ExportedPrismTypeCases))]
    public void ExportedPrismTypeIsLoadable(string fullTypeName)
    {
        Assert.NotNull(PrismAssembly.GetType(fullTypeName));
    }

    [Theory]
    [MemberData(nameof(PublicMemberCases))]
    public void PublicContractMemberIsDiscoverable(string fullTypeName, string memberName, string memberKind)
    {
        var type = PrismAssembly.GetType(fullTypeName);
        Assert.NotNull(type);

        const BindingFlags lookupFlags =
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static;

        var members = type!.GetMember(memberName, lookupFlags);
        Assert.Contains(members, m => string.Equals(m.MemberType.ToString(), memberKind, StringComparison.Ordinal));
    }
}
