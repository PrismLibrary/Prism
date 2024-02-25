namespace Prism;

internal static class DesignerProperties
{
    public static bool GetIsInDesignMode(DependencyObject _) =>
        Windows.ApplicationModel.DesignMode.DesignModeEnabled;
}
