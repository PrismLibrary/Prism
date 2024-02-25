using System.Runtime.InteropServices;

namespace Prism;

internal static partial class DependencyObjectExtensions
{
#if UNO_WASM
    // Related to :
    // https://github.com/dotnet/runtime/issues/76959
    // https://github.com/unoplatform/uno/blob/56d3f6ece16b2dba51776e95a3c847c9c4b68e8b/src/Uno.UI.Dispatching/Core/CoreDispatcher.wasm.cs#L17
    private static bool IsWebAssemblyThreadingSupported { get; }
        = Environment.GetEnvironmentVariable("UNO_BOOTSTRAP_MONO_RUNTIME_FEATURES")
            ?.Split(',').Any(v => v.Equals("threads", StringComparison.OrdinalIgnoreCase)) ?? false;

    private static bool IsWebAssembly { get; }
        = RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));
#endif

    /// <summary>
    /// Compatibility method to determine if the current thread can access a <see cref="DependencyObject"/>
    /// </summary>
    /// <param name="instance">The instance to check</param>
    /// <returns><c>true</c> if the current thread has access to the instance, otherwise <c>false</c></returns>
    public static bool CheckAccess(this DependencyObject instance)
    {
#if UNO_WASM
        if (IsWebAssembly && !IsWebAssemblyThreadingSupported)
        {
            // When threading is disabled, we need to return true 
            return true;
        }
#endif
        return instance.DispatcherQueue.HasThreadAccess;
    }
}
