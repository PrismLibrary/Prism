using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;
using win = Windows;

namespace Prism.Utilities
{
    public static class ReflectionUtilities
    {
        public static Assembly GetCallingAssembly(Assembly ignore)
        {
            return null;

            // .NET Core 2.0-only
            // var s = new StackTrace();
            // var assemblies = s.GetFrames()
            // .Select(x => x.GetMethod().DeclaringType.GetTypeInfo().Assembly)
            // .Where(x => !x.FullName.StartsWith("Microsoft."))
            // .Where(x => !x.FullName.StartsWith("System."));

            // return assemblies
            // .Where(x => !Equals(x, ignore))
            // .First();
        }
    }
}
