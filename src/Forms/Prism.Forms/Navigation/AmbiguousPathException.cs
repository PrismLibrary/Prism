using System;

namespace Prism.Navigation
{
    public class AmbiguousPathException : Exception
    {
        public const string AmbiguousSelectedTab = "The requested selected tab matches more than one child page";

        public string Name { get; }

        public AmbiguousPathException(string name)
        {
            Name = name;
        }
    }
}