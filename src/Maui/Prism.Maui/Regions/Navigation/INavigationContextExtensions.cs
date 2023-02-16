namespace Prism.Regions.Navigation;

public static class INavigationContextExtensions
{
    public static string NavigatedName(this INavigationContext context)
    {
        var uri = context.Uri;
        if (!uri.IsAbsoluteUri)
        {
            uri = new Uri(new Uri("nav://local.app"), context.Uri);
        }

        return uri.LocalPath.StartsWith("/") ? uri.LocalPath.Substring(1) : uri.LocalPath;
    }
}
