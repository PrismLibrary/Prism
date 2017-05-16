using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class OtherContentPageMock : ContentPageMock
    {
        public OtherContentPageMock() : this(null)
        {
        }

        public OtherContentPageMock(PageNavigationEventRecorder recorder) : base(recorder)
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}
