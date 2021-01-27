using System.Collections.Generic;
using Prism.AppModel;
using Prism.Mvvm;
using Prism.Services;

namespace HelloPageDialog.ViewModels
{
    public partial class PageDialogDemoBaseViewModel : BindableBase
    {
        protected IPageDialogService _pageDialogs { get; }

        public PageDialogDemoBaseViewModel(IPageDialogService pageDialogs)
        {
            _pageDialogs = pageDialogs;
            FlowDirections = new[]
            {
                FlowDirection.MatchParent,
                FlowDirection.LeftToRight,
                FlowDirection.RightToLeft
            };
        }

        private bool _useExplicitFlow;
        public bool UseExplicitFlow
        {
            get => _useExplicitFlow;
            set => SetProperty(ref _useExplicitFlow, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private FlowDirection _flowDirection;
        public FlowDirection FlowDirection
        {
            get => _flowDirection;
            set => SetProperty(ref _flowDirection, value);
        }

        public IEnumerable<FlowDirection> FlowDirections { get; }
    }
}
