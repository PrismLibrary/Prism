using System.Xml.Linq;

namespace Prism.Windows.AppModel
{
    /// <summary>
    /// This helper class loads the AppManifest in memory, letting you obtain properties that are not exposed through the Windows Store API.
    /// Currently, this class has methods for discovering if the Search contract is enabled and to get the Applications Id.
    /// Nevertheless, you can extend it to get other values that you may need.
    /// </summary>
    public static class AppManifestHelper
    {
        private static readonly XDocument manifest = XDocument.Load("AppxManifest.xml", LoadOptions.None);
        private static readonly XNamespace xNamespace = XNamespace.Get("http://schemas.microsoft.com/appx/manifest/foundation/windows10");

        /// <summary>
        /// Checks if the Search declaration was activated in the Package.appxmanifest.
        /// </summary>
        /// <returns>True if Search is declared</returns>
        public static bool IsSearchDeclared()
        {
            // Get the Extension nodes located under Package/Applications/Extensions
            var extensions = manifest.Descendants(xNamespace + "Extension");
            foreach (var extension in extensions)
            {
                if (extension.Attribute("Category") != null && extension.Attribute("Category").Value == "windows.search")
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Retrieves the Application Id from the AppManifest.
        /// </summary>
        /// <returns>The Application Id</returns>
        public static string GetApplicationId()
        {
            // Get the Application node located under Package/Applications
            var applications = manifest.Descendants(xNamespace + "Application");
            foreach (var application in applications)
            {
                if (application.Attribute("Id") != null)
                {
                    return application.Attribute("Id").Value;
                }
            }
            return string.Empty;
        }
    }
}
