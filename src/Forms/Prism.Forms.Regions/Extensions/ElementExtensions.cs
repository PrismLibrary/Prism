using Xamarin.Forms;

namespace Prism.Extensions
{
    internal static class ElementExtensions
    {
        public static bool CheckForParentPage(this VisualElement visualElement)
        {
            return GetParentPage(visualElement) != null;
        }

        public static Element GetRoot(this Element element)
        {
            switch (element.Parent)
            {
                case null:
                    return element;
                default:
                    return GetRoot(element.Parent);
            }
        }

        public static Page GetParentPage(this Element visualElement)
        {
            switch (visualElement.Parent)
            {
                case Page page:
                    return page;
                case null:
                    return null;
                default:
                    return GetParentPage(visualElement.Parent);
            }
        }
    }
}
