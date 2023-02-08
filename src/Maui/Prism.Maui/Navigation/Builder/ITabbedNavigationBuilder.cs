using System.ComponentModel;

namespace Prism.Navigation.Builder;

public interface ITabbedNavigationBuilder : INavigationBuilder
{
    ITabbedNavigationBuilder CreateTab(string segmentNameOrUri);
    ITabbedNavigationBuilder CreateTab<TViewModel>()
        where TViewModel : class, INotifyPropertyChanged;
    ITabbedNavigationBuilder SelectTab(string segmentNameOrUri);
    ITabbedNavigationBuilder SelectTab<TViewModel>()
        where TViewModel : class, INotifyPropertyChanged;
}
