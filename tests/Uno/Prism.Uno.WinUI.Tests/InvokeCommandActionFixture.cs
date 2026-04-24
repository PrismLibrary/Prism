using Prism.Interactivity;
using Xunit;

namespace Prism.Uno.WinUI.Tests;

public class InvokeCommandActionFixture
{
    [Fact]
    public void ImplementsIActionContract()
    {
        Assert.Contains(typeof(Microsoft.Xaml.Interactivity.IAction), typeof(InvokeCommandAction).GetInterfaces());
    }

    [Fact]
    public void ExposesExpectedDependencyProperties()
    {
        Assert.NotNull(typeof(InvokeCommandAction).GetField("AutoEnableProperty"));
        Assert.NotNull(typeof(InvokeCommandAction).GetField("CommandProperty"));
        Assert.NotNull(typeof(InvokeCommandAction).GetField("CommandParameterProperty"));
        Assert.NotNull(typeof(InvokeCommandAction).GetField("TriggerParameterPathProperty"));
    }

    [Fact]
    public void ExposesExecuteMethod()
    {
        var execute = typeof(InvokeCommandAction).GetMethod(nameof(InvokeCommandAction.Execute));
        Assert.NotNull(execute);
        Assert.Equal(typeof(object), execute!.ReturnType);
    }

    [Fact]
    public void ExposesAutoEnableAndCommandProperties()
    {
        Assert.NotNull(typeof(InvokeCommandAction).GetProperty(nameof(InvokeCommandAction.AutoEnable)));
        Assert.NotNull(typeof(InvokeCommandAction).GetProperty(nameof(InvokeCommandAction.Command)));
        Assert.NotNull(typeof(InvokeCommandAction).GetProperty(nameof(InvokeCommandAction.CommandParameter)));
        Assert.NotNull(typeof(InvokeCommandAction).GetProperty(nameof(InvokeCommandAction.TriggerParameterPath)));
    }

    [Fact]
    public void ContainsExecutableCommandBehaviorNestedType()
    {
        var nestedType = typeof(InvokeCommandAction).GetNestedType("ExecutableCommandBehavior", System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(nestedType);
    }
}
