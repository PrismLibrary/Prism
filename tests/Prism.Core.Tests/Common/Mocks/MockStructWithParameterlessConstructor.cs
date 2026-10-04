namespace Prism.Tests.Common.Mocks;

public struct MockStructWithParameterlessConstructor
{
    public MockStructWithParameterlessConstructor()
    {
        Value = 42;
    }

    public int Value { get; }
}
