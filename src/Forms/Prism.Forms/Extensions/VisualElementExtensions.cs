using Xamarin.Forms;

namespace Prism.Extensions
{
    internal static class VisualElementExtensions
    {
        public static bool TryGetParentPage(this VisualElement element, out Page page)
        {
            page = GetParentPage(element);
            return page != null;
        }

        private static Page GetParentPage(Element visualElement)
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
