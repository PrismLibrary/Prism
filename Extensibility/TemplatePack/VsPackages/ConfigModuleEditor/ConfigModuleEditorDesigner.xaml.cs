using ConfigModuleEditor.Models;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ConfigModuleEditor
{
    /// <summary>
    /// Interaction logic for ConfigModuleEditorDesigner.xaml
    /// </summary>
    public partial class ConfigModuleEditorDesigner : UserControl
    {
        public static readonly DependencyProperty ModulesProperty = DependencyProperty.Register("Modules", typeof(ObservableCollection<ModuleInfo>), typeof(ConfigModuleEditorDesigner), new PropertyMetadata(null));
        public ObservableCollection<ModuleInfo> Modules
        {
            get { return (ObservableCollection<ModuleInfo>)GetValue(ModulesProperty); }
            set { SetValue(ModulesProperty, value); }
        }

        public static readonly DependencyProperty ProjectModulesProperty = DependencyProperty.Register("ProjectModules", typeof(ObservableCollection<ProjectModuleInfo>), typeof(ConfigModuleEditorDesigner), new PropertyMetadata(null));
        public ObservableCollection<ProjectModuleInfo> ProjectModules
        {
            get { return (ObservableCollection<ProjectModuleInfo>)GetValue(ProjectModulesProperty); }
            set { SetValue(ProjectModulesProperty, value); }
        }

        public ConfigModuleEditorDesigner()
        {
            InitializeComponent();

            ProjectModules = new ObservableCollection<ProjectModuleInfo>();
            Modules = new ObservableCollection<ModuleInfo>();

            DataContext = this;
        }

        private void _dataGrid_EditModeStarting(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartingEventArgs e)
        {
            if (e.Cell.Field.Name != "AssemblyFile")
                return;

            var combo = (XamComboEditor)e.Editor;
            combo.SelectedItemChanged += XamComboEditor_SelectedItemChanged;
        }

        private void _dataGrid_EditModeEnding(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndingEventArgs e)
        {
            if (e.Cell.Field.Name != "AssemblyFile")
                return;

            var combo = (XamComboEditor)e.Editor;
            combo.SelectedItemChanged -= XamComboEditor_SelectedItemChanged;
        }

        private void XamComboEditor_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                return;

            var projectModuleInfo = (ProjectModuleInfo)e.NewValue;
            var datarecord = (DataRecord)_dataGrid.ActiveRecord;
            var module = (ModuleInfo)datarecord.DataItem;

            module.ModuleName = projectModuleInfo.ModuleName;
            module.ModuleType = projectModuleInfo.ModuleType;
        }

        private void _dataGrid_ExecutedCommand(object sender, Infragistics.Windows.Controls.Events.ExecutedCommandEventArgs e)
        {
            if (e.Command == DataPresenterCommands.EndEditModeAndCommitRecord || e.Command == DataPresenterCommands.DeleteSelectedDataRecords)
                OnIsDirtyChanged();
        }

        public event EventHandler IsDirtyChanged;
        void OnIsDirtyChanged()
        {
            var handler = IsDirtyChanged;
            if (handler != null)
                handler(this, new EventArgs());
        }
    }
}
