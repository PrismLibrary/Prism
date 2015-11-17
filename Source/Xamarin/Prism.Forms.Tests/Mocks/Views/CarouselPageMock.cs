using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class CarouselPageMock : CarouselPage
    {
        public CarouselPageMock()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);

            Children.Add(new ContentPage() { Title = "Page 1" });
            Children.Add(new ContentPageMock() { Title = "Page 2" });
            Children.Add(new ContentPage() { Title = "Page 3" });
        }
    }
}
