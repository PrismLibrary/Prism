using System;

namespace Prism.Windows.AppModel
{
    /// <summary>
    /// This attribute indicates that the marked property will have its state saved on suspension.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,
                    AllowMultiple = false)]
    public sealed class RestorableStateAttribute : Attribute
    {

    }
}
