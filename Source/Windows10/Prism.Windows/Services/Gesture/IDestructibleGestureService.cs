using Windows.UI.Core;

namespace Prism.Services
{
    public interface IDestructibleGestureService
    {
        void Destroy(CoreWindow window);
    }
}
