using Prism.Commands;
using Prism.ShowDialog;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace UsingPopupWindowAction.ViewModels
{
    public class ItemSelectionViewModel : Confirmation
    {
        private string selectedItem;

        public ItemSelectionViewModel()
        {
            this.AcceptCommand.ObservesProperty(() => this.SelectedItem);
        }

        public string SelectedItem
        {
            get { return this.selectedItem; }
            set { this.SetProperty(ref this.selectedItem, value); }
        }

        public IList<string> Items { get; private set; } = new List<string> { "item1", "item2", "item3", "item4", "item5", "item6"};

        protected override bool CanAccept()
        {
            return this.SelectedItem != null;
        }
    }
}
