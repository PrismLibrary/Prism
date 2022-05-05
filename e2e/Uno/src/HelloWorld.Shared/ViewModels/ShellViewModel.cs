using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;

namespace HelloWorld.ViewModels
{
    class ShellViewModel : BindableBase
    {
        public string Title { get; set; } = "Hello Uno for Prism";
    }
}
