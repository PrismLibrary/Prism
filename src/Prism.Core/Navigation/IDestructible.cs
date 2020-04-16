namespace Prism.Navigation
{
    /// <summary>
    /// Tells Prism to destroy View upon NavigationFrom
    /// </summary>
    public interface IDestructible
    {
        void Destroy();
    }
}
