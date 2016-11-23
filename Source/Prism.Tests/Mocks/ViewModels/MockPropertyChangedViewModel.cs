using Prism.Mvvm;
using Prism.Tests.Mocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Tests.Mocks.ViewModels
{
    class MockPropertyChangedViewModel : BindableBase, IDisposable
    {
        private PropertyChangedListener Listener { get; }

        public MockPropertyChangedViewModel(MockPropertyChangedModel model)
        {
            Listener = new PropertyChangedListener(model)
            {
                { nameof(MockPropertyChangedModel.Value), (s, e) => Value = $"Model value is {((MockPropertyChangedModel)s).Value}" },
                { nameof(MockPropertyChangedModel.Value2), (s, e) => Value2 = $"Model value2 is {((MockPropertyChangedModel)s).Value2}" }
            };
        }

        private string value;

        public string Value
        {
            get { return value; }
            set { SetProperty(ref this.value, value); }
        }

        private string value2;

        public string Value2
        {
            get { return value2; }
            set { SetProperty(ref this.value2, value); }
        }


        public void Dispose()
        {
            Listener.Dispose();
        }
    }
}
