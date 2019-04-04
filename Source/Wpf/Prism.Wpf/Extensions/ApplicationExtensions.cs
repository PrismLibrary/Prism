using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Prism.Extensions
{
    public static class ApplicationExtensions
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        public static Window GetActiveWindow(this Application application)
        {
            return application.Windows.OfType<Window>().SingleOrDefault(window => new WindowInteropHelper(window).Handle == GetActiveWindow());
        }
    }
}
