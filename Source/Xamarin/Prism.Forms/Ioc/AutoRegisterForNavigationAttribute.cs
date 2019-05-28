using System;

namespace Prism.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoRegisterForNavigationAttribute : Attribute
    {
    }
}
