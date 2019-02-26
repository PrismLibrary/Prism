using Prism.Ioc;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Prism.Services.Dialogs
{
    public class RegionAwareDialogService : IRegionAwareDialogService
    {

        private readonly IContainerExtension _containerExtension;
        private readonly IRegionManager _regionManager;

        public RegionAwareDialogService(IContainerExtension containerExtension,IRegionManager regionManager)
        {
            _containerExtension = containerExtension;
            _regionManager = regionManager;
        }

        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
             ShowDialogInternal(name, parameters, callback,false);
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            ShowDialogInternal(name, parameters, callback,true);
        }

        
        IDialogWindow CreateDialogWindow()
        {
            return _containerExtension.Resolve<IRegionAwareDialogWindow>();
        }

         void ShowDialogInternal(string name, IDialogParameters parameters, Action<IDialogResult> callback, bool isModal)
        {
            IDialogWindow dialogWindow = CreateDialogWindow();
            ConfigureDialogWindowEvents(dialogWindow, callback);
            ConfigureDialogWindowContent(name, dialogWindow, parameters);
            
            dialogWindow.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);

            if (isModal)
                dialogWindow.ShowDialog();
            else
                dialogWindow.Show();
        }

        private void ConfigureDialogWindowContent(string name, IDialogWindow window, IDialogParameters parameters)
        {
            var frameworkElement=window as DependencyObject;
            if (frameworkElement != null)
            {
                var scopedRegion=_regionManager.CreateRegionManager();
                RegionManager.SetRegionManager(frameworkElement, scopedRegion);
                RegionManager.UpdateRegions();

                scopedRegion.RequestNavigate("DialogShellRegion",name,parameters as NavigationParameters);
               
                if (window.Content is FrameworkElement fe)
                {
                    var viewModel = fe.DataContext;
                    window.DataContext = viewModel;
                }
            }
        }

        private void ConfigureDialogWindowEvents(IDialogWindow window, Action<IDialogResult> callback)
        {
            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = (o) =>
            {
                window.Result = o;
                window.Close();
            };

            RoutedEventHandler loadedHandler = null;
            loadedHandler = (o, e) =>
            {
                window.Loaded -= loadedHandler;
                window.GetDialogViewModel().RequestClose += requestCloseHandler;
            };
            window.Loaded += loadedHandler;

            CancelEventHandler closingHandler = null;
            closingHandler = (o, e) =>
            {
                if (!window.GetDialogViewModel().CanCloseDialog())
                    e.Cancel = true;
            };
            window.Closing += closingHandler;

            EventHandler closedHandler = null;
            closedHandler = (o, e) =>
                {
                    window.Closed -= closedHandler;
                    window.Closing -= closingHandler;
                    window.GetDialogViewModel().RequestClose -= requestCloseHandler;

                    window.GetDialogViewModel().OnDialogClosed();

                    if (window.Result == null)
                        window.Result = new DialogResult();

                    callback?.Invoke(window.Result);

                    window.DataContext = null;
                    window.Content = null;
                };
            window.Closed += closedHandler;
        }

       
    }
}
