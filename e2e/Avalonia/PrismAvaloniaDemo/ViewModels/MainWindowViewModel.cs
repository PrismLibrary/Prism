namespace SampleApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        Title = "Welcome to Prism.Avalonia!";
    }

#pragma warning disable CA1822 // Mark members as static
    public string Greeting => "Hello from, Prism.Avalonia!";
#pragma warning restore CA1822 // Mark members as static
}
