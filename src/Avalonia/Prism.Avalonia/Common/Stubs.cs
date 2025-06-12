using System;

namespace Prism.Common
{
    internal static class Stubs
    {
        public static readonly Action Nop = () => { };
        public static readonly Action<Exception> Throw = ex => { throw ex; };
    }
}
