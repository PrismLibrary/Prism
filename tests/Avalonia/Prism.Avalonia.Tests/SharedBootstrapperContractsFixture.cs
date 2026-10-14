using System;
using System.Collections.Generic;
using Avalonia;
using Prism.Dialogs;
using Prism.Ioc;
using Xunit;

namespace Prism.Avalonia.Tests;

public class SharedBootstrapperContractsFixture
{
    [Fact]
    public void RunPreservesHookOrder()
    {
        var bootstrapper = new RecordingBootstrapper();
        bootstrapper.Run();
        Assert.Equal(new[] { "locator", "initialize", "initialized" }, bootstrapper.Calls);
    }

    [Fact]
    public void NullCatalogPreservesAvaloniaExceptionMessage()
    {
        var bootstrapper = new RecordingBootstrapper();
        var error = Assert.Throws<InvalidOperationException>(() => bootstrapper.RegisterRequiredTypesWithoutCatalog());
        Assert.Equal("IModuleCatalog was null", error.Message);
    }

    [Fact]
    public void InitializeShellPreservesAvaloniaObjectContract()
    {
        var bootstrapper = new RecordingBootstrapper();
        var shell = new AvaloniaObject();
        bootstrapper.AssignShell(shell);
        Assert.Same(shell, bootstrapper.CurrentShell);
    }

    [Fact]
    public void DialogParameterKeysPreserveAvaloniaParentWindow()
    {
        Assert.Equal("windowName", KnownDialogParameters.WindowName);
        Assert.Equal("nonModal", KnownDialogParameters.ShowNonModal);
        Assert.Equal("parentWindow", KnownDialogParameters.ParentWindow);
    }

    private sealed class RecordingBootstrapper : PrismBootstrapperBase
    {
        public List<string> Calls { get; } = new();
        public AvaloniaObject CurrentShell => Shell;
        public void AssignShell(AvaloniaObject shell) => InitializeShell(shell);
        public void RegisterRequiredTypesWithoutCatalog() => RegisterRequiredTypes(null!);
        protected override void ConfigureViewModelLocator() => Calls.Add("locator");
        protected override void Initialize() => Calls.Add("initialize");
        protected override void OnInitialized() => Calls.Add("initialized");
        protected override IContainerExtension CreateContainerExtension() => throw new NotSupportedException();
        protected override void RegisterTypes(IContainerRegistry containerRegistry) => throw new NotSupportedException();
        protected override AvaloniaObject CreateShell() => throw new NotSupportedException();
    }
}
