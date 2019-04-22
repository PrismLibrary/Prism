using Prism.AppModel;
using System;

namespace Prism.Ioc
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true)]
    public class RegisterForNavigationAttribute : Attribute
    {
        public Type ViewType { get; set; }
        public Type ViewModelType { get; set; }
        public string Name { get; set; }
        public RuntimePlatform? RuntimePlatform { get; set; }
    }
}
