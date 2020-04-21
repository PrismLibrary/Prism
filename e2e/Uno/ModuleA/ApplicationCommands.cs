using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;

namespace ModuleA
{
    public interface IApplicationCommands
    {
        CompositeCommand SaveCommand { get; }
        CompositeCommand ResetCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        CompositeCommand _saveCommand = new CompositeCommand(true); //invoke only on the active command - IActiveAware
        public CompositeCommand SaveCommand
        {
            get { return _saveCommand; }
        }

        CompositeCommand _resetCommand = new CompositeCommand();
        public CompositeCommand ResetCommand
        {
            get { return _resetCommand; }
        }
    }
}
