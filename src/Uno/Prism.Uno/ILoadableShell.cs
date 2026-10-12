namespace Prism;

#nullable enable
/// <summary>Allows an application shell to complete its loading presentation after Prism initialization.</summary>
public interface ILoadableShell
{
    /// <summary>Signals that application initialization has completed and the shell can finish loading.</summary>
    void FinishLoading();
}
