using System;
using System.Windows;
using System.Windows.Input;

namespace Prism.Interactivity
{
    /// <summary>
    /// Base behavior to handle connecting a <see cref="System.Windows.Controls.Control"/> to a Command.
    /// </summary>
    /// <typeparam name="T">The target object must derive from Control</typeparam>
    /// <remarks>
    /// CommandBehaviorBase can be used to provide new behaviors for commands.
    /// </remarks>
    public class CommandBehaviorBase<T> where T : UIElement
    {
        private ICommand _command;
        private object _commandParameter;
        private readonly WeakReference _targetObject;
        private readonly EventHandler _commandCanExecuteChangedHandler;

        /// <summary>
        /// Constructor specifying the target object.
        /// </summary>
        /// <param name="targetObject">The target object the behavior is attached to.</param>
        public CommandBehaviorBase(T targetObject)
        {
            _targetObject = new WeakReference(targetObject);

            _commandCanExecuteChangedHandler = CommandCanExecuteChanged;
        }

        bool _autoEnabled = true;
        public bool AutoEnable
        {
            get { return _autoEnabled; }
            set
            {
                _autoEnabled = value;
                UpdateEnabledState();
            }
        }

        /// <summary>
        /// Corresponding command to be execute and monitored for <see cref="ICommand.CanExecuteChanged"/>
        /// </summary>
        public ICommand Command
        {
            get { return _command; }
            set
            {
                if (_command != null)
                {
                    _command.CanExecuteChanged -= _commandCanExecuteChangedHandler;
                }

                _command = value;
                if (_command != null)
                {
                    _command.CanExecuteChanged += _commandCanExecuteChangedHandler;
                    UpdateEnabledState();
                }
            }
        }

        /// <summary>
        /// The parameter to supply the command during execution
        /// </summary>
        public object CommandParameter
        {
            get { return _commandParameter; }
            set
            {
                if (_commandParameter != value)
                {
                    _commandParameter = value;
                    UpdateEnabledState();
                }
            }
        }

        /// <summary>
        /// Object to which this behavior is attached.
        /// </summary>
        protected T TargetObject
        {
            get
            {
                return _targetObject.Target as T;
            }
        }


        /// <summary>
        /// Updates the target object's IsEnabled property based on the commands ability to execute.
        /// </summary>
        protected virtual void UpdateEnabledState()
        {
            if (TargetObject == null)
            {
                Command = null;
                CommandParameter = null;
            }
            else if (Command != null)
            {
                if (AutoEnable)
                    TargetObject.IsEnabled = Command.CanExecute(CommandParameter);
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            UpdateEnabledState();
        }

        /// <summary>
        /// Executes the command, if it's set, providing the <see cref="CommandParameter"/>
        /// </summary>
        protected virtual void ExecuteCommand(object parameter)
        {
            if (Command != null)
                Command.Execute(CommandParameter ?? parameter);
        }
    }
}