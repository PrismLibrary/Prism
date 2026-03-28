using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Prism.Properties;

#nullable enable
namespace Prism.Commands
{
    /// <summary>
    /// The CompositeCommand composes one or more ICommands.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="CompositeCommand"/> allows you to register multiple commands and execute them all with a single call.
    /// This is useful for scenarios where an action should trigger multiple operations across different parts of an application.
    /// </para>
    /// <para>
    /// The composite command can optionally monitor the activity of its registered commands if they implement <see cref="IActiveAware"/>.
    /// When monitoring is enabled, the composite command will only execute commands that are active.
    /// </para>
    /// <para>
    /// The <see cref="CanExecute(object)"/> method returns <see langword="true"/> only if all registered commands can execute.
    /// </para>
    /// </remarks>
    public class CompositeCommand : ICommand
    {
        private readonly List<ICommand> _registeredCommands = new();
        private readonly bool _monitorCommandActivity;
        private readonly EventHandler _onRegisteredCommandCanExecuteChangedHandler;
        private readonly SynchronizationContext? _synchronizationContext;

        /// <summary>
        /// Initializes a new instance of <see cref="CompositeCommand"/>.
        /// </summary>
        /// <remarks>
        /// By default, the composite command will not monitor the activity of registered commands.
        /// </remarks>
        public CompositeCommand()
        {
            _onRegisteredCommandCanExecuteChangedHandler = new EventHandler(OnRegisteredCommandCanExecuteChanged);
            _synchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CompositeCommand"/>.
        /// </summary>
        /// <param name="monitorCommandActivity">Indicates when the command activity is going to be monitored.
        /// When <see langword="true"/>, the composite command will only execute registered commands that are active (if they implement <see cref="IActiveAware"/>).</param>
        /// <remarks>
        /// When activity monitoring is enabled, only commands that are active will be executed.
        /// </remarks>
        public CompositeCommand(bool monitorCommandActivity)
            : this()
        {
            _monitorCommandActivity = monitorCommandActivity;
        }

        /// <summary>
        /// Adds a command to the collection and signs up for the <see cref="ICommand.CanExecuteChanged"/> event of it.
        /// </summary>
        ///  <remarks>
        /// <para>
        /// If this command is set to monitor command activity, and <paramref name="command"/> 
        /// implements the <see cref="IActiveAware"/> interface, this method will subscribe to its
        /// <see cref="IActiveAware.IsActiveChanged"/> event.
        /// </para>
        /// <para>
        /// The same command cannot be registered twice, and a composite command cannot be registered within itself.
        /// </para>
        /// </remarks>
        /// <param name="command">The command to register.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when attempting to register a composite command within itself.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the same command is registered twice.</exception>
        public virtual void RegisterCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (command == this)
            {
                throw new ArgumentException(Resources.CannotRegisterCompositeCommandInItself);
            }

            lock (_registeredCommands)
            {
                if (_registeredCommands.Contains(command))
                {
                    throw new InvalidOperationException(Resources.CannotRegisterSameCommandTwice);
                }
                _registeredCommands.Add(command);
            }

            command.CanExecuteChanged += _onRegisteredCommandCanExecuteChangedHandler;
            OnCanExecuteChanged();

            if (_monitorCommandActivity)
            {
                if (command is IActiveAware activeAwareCommand)
                {
                    activeAwareCommand.IsActiveChanged += Command_IsActiveChanged;
                }
            }
        }

        /// <summary>
        /// Removes a command from the collection and removes itself from the <see cref="ICommand.CanExecuteChanged"/> event of it.
        /// </summary>
        /// <param name="command">The command to unregister.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is <see langword="null"/>.</exception>
        public virtual void UnregisterCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            bool removed;
            lock (_registeredCommands)
            {
                removed = _registeredCommands.Remove(command);
            }

            if (removed)
            {
                command.CanExecuteChanged -= _onRegisteredCommandCanExecuteChangedHandler;
                OnCanExecuteChanged();

                if (_monitorCommandActivity)
                {
                    if (command is IActiveAware activeAwareCommand)
                    {
                        activeAwareCommand.IsActiveChanged -= Command_IsActiveChanged;
                    }
                }
            }
        }

        private void OnRegisteredCommandCanExecuteChanged(object? sender, EventArgs e)
        {
            OnCanExecuteChanged();
        }


        /// <summary>
        /// Forwards <see cref="ICommand.CanExecute"/> to the registered commands and returns
        /// <see langword="true" /> if all of the commands return <see langword="true" />.
        /// </summary>
        /// <param name="parameter">Data used by the command.
        /// If the command does not require data to be passed, this object can be set to <see langword="null" />.
        /// </param>
        /// <returns><see langword="true" /> if all of the commands return <see langword="true" />; otherwise, <see langword="false" />.</returns>
        public virtual bool CanExecute(object? parameter)
        {
            bool hasEnabledCommandsThatShouldBeExecuted = false;

            ICommand[] commandList;
            lock (_registeredCommands)
            {
                commandList = _registeredCommands.ToArray();
            }
            foreach (ICommand command in commandList)
            {
                if (ShouldExecute(command))
                {
                    if (!command.CanExecute(parameter))
                    {
                        return false;
                    }

                    hasEnabledCommandsThatShouldBeExecuted = true;
                }
            }

            return hasEnabledCommandsThatShouldBeExecuted;
        }

        /// <summary>
        /// Occurs when any of the registered commands raise <see cref="ICommand.CanExecuteChanged"/>.
        /// </summary>
        public virtual event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Forwards <see cref="ICommand.Execute"/> to the registered commands.
        /// </summary>
        /// <param name="parameter">Data used by the command.
        /// If the command does not require data to be passed, this object can be set to <see langword="null" />.
        /// </param>
        public virtual void Execute(object? parameter)
        {
            Queue<ICommand> commands;
            lock (_registeredCommands)
            {
                commands = new Queue<ICommand>(_registeredCommands.Where(ShouldExecute).ToList());
            }

            while (commands.Count > 0)
            {
                ICommand command = commands.Dequeue();
                command.Execute(parameter);
            }
        }

        /// <summary>
        /// Evaluates if a command should execute.
        /// </summary>
        /// <param name="command">The command to evaluate.</param>
        /// <returns>A <see cref="bool"/> value indicating whether the command should be used 
        /// when evaluating <see cref="CanExecute"/> and <see cref="Execute"/>.</returns>
        /// <remarks>
        /// If this command is set to monitor command activity, and <paramref name="command"/>
        /// implements the <see cref="IActiveAware"/> interface, 
        /// this method will return <see langword="false" /> if the command's <see cref="IActiveAware.IsActive"/> 
        /// property is <see langword="false" />; otherwise it always returns <see langword="true" />.</remarks>
        protected virtual bool ShouldExecute(ICommand command)
        {
            if (_monitorCommandActivity && command is IActiveAware activeAwareCommand)
            {
                return activeAwareCommand.IsActive;
            }

            return true;
        }

        /// <summary>
        /// Gets the list of all the registered commands.
        /// </summary>
        /// <value>A list of registered commands.</value>
        /// <remarks>This returns a copy of the commands subscribed to the CompositeCommand.</remarks>
        public IList<ICommand> RegisteredCommands
        {
            get
            {
                IList<ICommand> commandList;
                lock (_registeredCommands)
                {
                    commandList = _registeredCommands.ToList();
                }

                return commandList;
            }
        }

        /// <summary>
        /// Raises <see cref="ICommand.CanExecuteChanged"/> on the UI thread so every 
        /// command invoker can requery <see cref="ICommand.CanExecute"/> to check if the
        /// <see cref="CompositeCommand"/> can execute.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                if (_synchronizationContext != null && _synchronizationContext != SynchronizationContext.Current)
                    _synchronizationContext.Post((o) => handler.Invoke(this, EventArgs.Empty), null);
                else
                    handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handler for IsActiveChanged events of registered commands.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">EventArgs to pass to the event.</param>
        private void Command_IsActiveChanged(object? sender, EventArgs e)
        {
            OnCanExecuteChanged();
        }
    }
}
