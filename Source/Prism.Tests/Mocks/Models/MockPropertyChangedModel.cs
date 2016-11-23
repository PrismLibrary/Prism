using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Tests.Mocks.Models
{
    class MockPropertyChangedModel : BindableBase
    {
        private int value;

        public int Value
        {
            get { return value; }
            set { SetProperty(ref this.value, value); }
        }

        private int value2;

        public int Value2
        {
            get { return value2; }
            set { SetProperty(ref this.value2, value); }
        }

    }
}
