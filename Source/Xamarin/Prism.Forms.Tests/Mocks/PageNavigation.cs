using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigation : INavigation
    {
        readonly List<Page> _modalStack;

        private readonly List<Page> _navigationStack = new List<Page>();

        public PageNavigation()
        {
            _modalStack = new List<Page>();
        }

        public PageNavigation(PageNavigation pageNavigation, Page rootPage)
        {
            _modalStack = pageNavigation._modalStack;
            _navigationStack.Add(rootPage);
        }

        public void InsertPageBefore(Page page, Page before)
        {
            OnInsertPageBefore(page, before);
        }

        public IReadOnlyList<Page> ModalStack => _modalStack;

        public IReadOnlyList<Page> NavigationStack => _navigationStack;

        public Task<Page> PopAsync() => OnPopAsync(true);

        public Task<Page> PopAsync(bool animated) => OnPopAsync(animated);

        public Task<Page> PopModalAsync() => OnPopModal(true);

        public Task<Page> PopModalAsync(bool animated) => OnPopModal(animated);

        public Task PopToRootAsync() => OnPopToRootAsync(true);

        public Task PopToRootAsync(bool animated) => OnPopToRootAsync(animated);

        public Task PushAsync(Page root) => PushAsync(root, true);

        public Task PushAsync(Page root, bool animated) => OnPushAsync(root, animated);

        public Task PushModalAsync(Page modal) => PushModalAsync(modal, true);

        public Task PushModalAsync(Page modal, bool animated) => OnPushModalAsync(modal, animated);

        public void RemovePage(Page page) => OnRemovePage(page);

        protected virtual void OnInsertPageBefore(Page page, Page before)
        {
            var index = _navigationStack.IndexOf(before);
            if (index == -1)
                throw new ArgumentException("before must be in the pushed stack of the current context");
            _navigationStack.Insert(index, page);
        }

        protected virtual Task<Page> OnPopAsync(bool animated) => Task.FromResult(Pop());

        protected virtual Task<Page> OnPopModal(bool animated) => Task.FromResult(PopModal());

        protected virtual Task OnPopToRootAsync(bool animated)
        {
            var root = _navigationStack.Last();
            _navigationStack.Clear();
            _navigationStack.Add(root);
            return Task.FromResult(root);
        }

        protected virtual Task OnPushAsync(Page page, bool animated)
        {
            _navigationStack.Add(page);
            return Task.FromResult(page);
        }

        protected virtual Task OnPushModalAsync(Page modal, bool animated)
        {
            _modalStack.Add(modal);
            var newPageNavigation = new PageNavigation(this, modal);
            modal.SetInner(newPageNavigation);
            return Task.FromResult<object>(null);
        }

        protected virtual void OnRemovePage(Page page) => _navigationStack.Remove(page);

        private Page Pop()
        {
            var result = _navigationStack[_navigationStack.Count - 1];
            _navigationStack.RemoveAt(_navigationStack.Count - 1);
            return result;
        }

        private Page PopModal()
        {
            var result = _modalStack[_modalStack.Count - 1];
            _modalStack.RemoveAt(_modalStack.Count - 1);
            return result;
        }
    }
}
