using Prism.Modularity;
using Xamarin.Forms.Xaml;

namespace Prism.Forms.Tests.Mocks.Modules
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasicCatalog : ModuleCatalog
    {
        public BasicCatalog()
        {
            InitializeComponent();
        }
    }
}
