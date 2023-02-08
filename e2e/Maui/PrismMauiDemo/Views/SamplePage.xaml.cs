namespace PrismMauiDemo.Views;

public partial class SamplePage : ContentPage
{
    int count = 0;

    public SamplePage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;
        CounterLabel.Text = $"Current count: {count}";

        SemanticScreenReader.Announce(CounterLabel.Text);
    }
}

