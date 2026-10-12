namespace Prism.DefaultView.Tests;

public class LifecycleEqualView
{
    public override bool Equals(object obj)
    {
        return obj is LifecycleEqualView;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
