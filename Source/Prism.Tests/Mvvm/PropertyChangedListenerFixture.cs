using Prism.Mvvm;
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
            var count = 0;
            var mockViewModel = new MockViewModel();
            var l = new PropertyChangedListener(mockViewModel)
            {
                { nameof(MockViewModel.MockProperty), (s, e) => count++ }
            };

            Assert.Equal(0, count);
            mockViewModel.MockProperty = 10;
            Assert.Equal(1, count);

            l.Dispose();

            mockViewModel.MockProperty = 11;
            Assert.Equal(1, count);
        }
    }
}
