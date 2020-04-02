using System;
using Prism.AppModel;

namespace Prism.Forms.Tests.Mvvm.Mocks.ViewModels
{
    public class AutoInitializedViewModelMock : IAutoInitialize
    {
        [AutoInitialize(true)]
        public string Title { get; set; }

        public bool Success { get; set; }

        public DateTime SomeDate { get; set; }

        public MockHttpStatus Status { get; set; }
    }
}
