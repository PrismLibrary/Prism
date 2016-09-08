namespace Prism.Navigation
{
    public interface IInternalNavigationParent
    {
        NavigationParameters SharedParameters { get; set; }
    }
}
