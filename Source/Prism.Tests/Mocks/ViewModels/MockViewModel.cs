// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Prism.Mvvm;

namespace Prism.Tests.Mocks.ViewModels
{
    public class MockViewModel : BindableBase
    {
        private int mockProperty;

        public int MockProperty
        {
            get
            {
                return this.mockProperty;
            }

            set 
            {
                this.SetProperty(ref mockProperty, value);
            }
        }

        internal void InvokeOnPropertyChanged()
        {
            this.OnPropertyChanged(() => this.MockProperty);
        }
    }
}
