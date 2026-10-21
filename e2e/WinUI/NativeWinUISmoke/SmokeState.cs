using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NativeWinUISmoke;

public sealed class SmokeState
{
    public SmokeState()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
        File.WriteAllText(LogPath, $"Native WinUI smoke started {DateTimeOffset.Now:O}{Environment.NewLine}");
        Write($"Log: {LogPath}");
    }

    public ObservableCollection<string> Messages { get; } = new();
    public string LogPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PrismNativeWinUISmoke", "smoke.log");
    public bool ModuleInitialized { get; set; }
    public bool OnInitializedCompleted { get; set; }
    public int FinishLoadingCalls { get; set; }
    public int DialogOpenedCount { get; set; }
    public int DialogClosedCount { get; set; }
    public int Failures { get; private set; }

    public void Check(bool passed, string description)
    {
        if (!passed)
            Failures++;
        Write($"{(passed ? "PASS" : "FAIL")}: {description}");
    }

    public void Write(string message)
    {
        Debug.WriteLine(message);
        Messages.Add(message);
        File.AppendAllText(LogPath, message + Environment.NewLine);
    }
}
