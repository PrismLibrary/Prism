using Prism.Mvvm;
using Prism.Tests.Mocks.Models;
using Prism.Tests.Mocks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Prism.Tests.Mvvm
{
    public class PropertyChangedListenerFixture
    {
        [Fact]
        public void DefaultUseCase()
        {
            var model = new MockPropertyChangedModel();
            // create wrapper
            var viewModel = new MockPropertyChangedViewModel(model);

            model.Value = 1;
            Assert.Equal("Model value is 1", viewModel.Value);

            model.Value2 = 1;
            Assert.Equal("Model value2 is 1", viewModel.Value2);

            // disconnect
            viewModel.Dispose();

            model.Value = 10;
            Assert.Equal("Model value is 1", viewModel.Value);
        }
    }
}
