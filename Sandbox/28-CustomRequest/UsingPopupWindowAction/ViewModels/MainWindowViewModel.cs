using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.ShowDialog;

namespace UsingPopupWindowAction.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Unity Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ShowDialogRequest<INotification> NotificationRequest { get; set; }
        public DelegateCommand NotificationCommand { get; set; }

        public ShowDialogRequest<IConfirmation> ConfirmationRequest { get; set; }
        public DelegateCommand ConfirmationCommand { get; set; }

        public ShowDialogRequest<INotification> CustomPopupRequest { get; set; }
        public DelegateCommand CustomPopupCommand { get; set; }

        public ShowDialogRequest<ItemSelectionViewModel> CustomNotificationRequest { get; set; }
        public DelegateCommand CustomNotificationCommand { get; set; }

        public MainWindowViewModel()
        {
            NotificationRequest = new ShowDialogRequest<INotification>();
            NotificationCommand = new DelegateCommand(RaiseNotification);

            ConfirmationRequest = new ShowDialogRequest<IConfirmation>();
            ConfirmationCommand = new DelegateCommand(RaiseConfirmation);

            CustomPopupRequest = new ShowDialogRequest<INotification>();
            CustomPopupCommand = new DelegateCommand(RaiseCustomPopup);

            CustomNotificationRequest = new ShowDialogRequest<ItemSelectionViewModel>();
            CustomNotificationCommand = new DelegateCommand(RaiseCustomInteraction);
        }

        void RaiseNotification()
        {
            NotificationRequest.RaiseNotification("Notification", "Notification Message", () => Title = "Notified");
        }

        void RaiseConfirmation()
        {
            ConfirmationRequest.RaiseConfirmation("Confirmation", "Confirmation Message", c => Title = c ? "Confirmed" : "Not Confirmed");
        }

        void RaiseCustomPopup()
        {
            CustomPopupRequest.RaiseNotification("Custom Popup", "Custom Popup Message", () => Title = "Good to go");
        }

        private void RaiseCustomInteraction()
        {
            CustomNotificationRequest.Raise("Custom Notification", new ItemSelectionViewModel(), r =>
                        Title = r.Confirmed ? $"User selected: { r.SelectedItem}" : "User cancelled");
        }
    }
}
