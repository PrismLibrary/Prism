using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace HelloWorld.Skia.Tizen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new HelloWorld.App(), args);
            host.Run();
        }
    }
}
