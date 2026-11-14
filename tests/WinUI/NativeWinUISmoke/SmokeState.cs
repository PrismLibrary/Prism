using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NativeWinUISmoke;

public sealed class SmokeState
{
    private readonly Dictionary<string, SmokeDialogObservation> _dialogs = new();
    private int _sequence;

    public SmokeState()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
        File.WriteAllText(LogPath, $"Native WinUI smoke started {DateTimeOffset.Now:O}{Environment.NewLine}");
        Write($"Log: {LogPath}");
    }

    public ObservableCollection<string> Messages { get; } = new();
    public string LogPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PrismNativeWinUISmoke", "smoke.log");
    public int ModuleRegisterTypesCount { get; set; }
    public int ModuleInitializedCount { get; set; }
    public int Failures { get; private set; }

    public int NextSequence() => ++_sequence;

    public SmokeDialogObservation GetDialog(string id)
    {
        if (!_dialogs.TryGetValue(id, out var observation))
            _dialogs.Add(id, observation = new SmokeDialogObservation());
        return observation;
    }

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
