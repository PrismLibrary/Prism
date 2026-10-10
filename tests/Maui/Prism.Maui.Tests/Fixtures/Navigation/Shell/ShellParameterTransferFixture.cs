using Prism.Maui.Tests.Mocks.Shell;
using Prism.Navigation;
using NavigationMode = Prism.Navigation.NavigationMode;

namespace Prism.Maui.Tests.Fixtures.Navigation.Shell;

public sealed class ShellParameterTransferFixture
{
    [Fact]
    public void SingleUseTransferPreservesObjectsAndLiteralStringsWithoutChangingTheCaller()
    {
        var payload = new object();
        var original = new NavigationParameters
        {
            { "payload", payload },
            { "text", "literal%20text+suffix" },
            { "nullable", null }
        };

        var query = ShellParametersPrototype.ToShellQuery(original);
        var received = ShellParametersPrototype.FromShellQuery(query, NavigationMode.Back);
        query.Clear();

        Assert.Same(payload, received.GetValue<object>("payload"));
        Assert.Equal("literal%20text+suffix", received.GetValue<string>("text"));
        Assert.True(received.ContainsKey("nullable"));
        Assert.Equal(NavigationMode.Back, received.GetNavigationMode());
        Assert.Equal(3, original.Count);
        Assert.Same(payload, original.GetValue<object>("payload"));
    }

    [Fact]
    public void RepeatedPrismParametersAreRejectedInsteadOfSilentlyDroppingValues()
    {
        var original = new NavigationParameters
        {
            { "id", 1 },
            { "id", 2 }
        };

        Assert.Throws<ArgumentException>(() => ShellParametersPrototype.ToShellQuery(original));
        Assert.Equal(new[] { 1, 2 }, original.GetValues<int>("id"));
    }

    [Fact]
    public void FreshBackParametersDoNotReplayAForwardPayload()
    {
        var forward = ShellParametersPrototype.FromShellQuery(
            new ShellNavigationQueryParameters { { "payload", new object() } }, NavigationMode.New);
        var backward = ShellParametersPrototype.FromShellQuery(
            new ShellNavigationQueryParameters { { "result", "accepted" } }, NavigationMode.Back);

        Assert.True(forward.ContainsKey("payload"));
        Assert.False(backward.ContainsKey("payload"));
        Assert.Equal("accepted", backward.GetValue<string>("result"));
        Assert.Equal(NavigationMode.Back, backward.GetNavigationMode());
    }
}
