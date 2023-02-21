using Uno.UI.Runtime.Skia;

namespace HelloWorld.Skia.Framebuffer;

public class Program
{
	public static void Main(string[] args)
	{
		try
		{
			Console.CursorVisible = false;

			var host = new FrameBufferHost(() => new AppHead(), args);
			host.Run();
		}
		finally
		{
			Console.CursorVisible = true;
		}
	}
}
