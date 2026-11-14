namespace Prism;

internal static class DesignerProperties
{
    public static bool GetIsInDesignMode(DependencyObject _) =>
        global::Windows.ApplicationModel.DesignMode.DesignModeEnabled;
}
