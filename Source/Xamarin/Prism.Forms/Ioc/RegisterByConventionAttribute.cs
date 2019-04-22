using System;

namespace Prism.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RegisterByConventionAttribute : Attribute
    {
        public bool Automatic { get; set; }
    }
}
