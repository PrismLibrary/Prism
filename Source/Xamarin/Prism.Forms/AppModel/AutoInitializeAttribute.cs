using System;

namespace Prism.AppModel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class AutoInitializeAttribute : Attribute
    {
        public AutoInitializeAttribute(string name) : this(name, false) { }

        public AutoInitializeAttribute(bool isRequired) : this(null, isRequired) { }

        public AutoInitializeAttribute(string name, bool isRequired)
        {
            Name = name;
            IsRequired = isRequired;
        }

        public bool IsRequired { get; }

        public string Name { get; }
    }
}
