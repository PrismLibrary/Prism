using System;

namespace Prism.Navigation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NavigationParameterAttribute : Attribute
    {
        public NavigationParameterAttribute(string name) : this(name, false) { }

        public NavigationParameterAttribute(bool isRequired) : this(null, isRequired) { }

        public NavigationParameterAttribute(string name, bool isRequired)
        {
            Name = name;
            IsRequired = isRequired;
        }

        public bool IsRequired { get; }

        public string Name { get; }
    }
}
