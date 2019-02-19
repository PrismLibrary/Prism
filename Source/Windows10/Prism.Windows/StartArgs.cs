using Windows.ApplicationModel.Activation;

namespace Prism
{
    public class StartArgs : IStartArgs
    {
        public StartArgs(IActivatedEventArgs args, StartKinds startKind)
        {
            Arguments = args;
            StartKind = startKind;
        }

        public StartArgs(BackgroundActivatedEventArgs args, StartKinds startKind)
        {
            Arguments = args;
            StartKind = startKind;
        }

        public override string ToString()
        {
            return $"Args:{Arguments?.GetType()} Kind:{StartKind} Cause:{StartCause}";
        }

        public object Arguments { get; internal set; }

        public StartKinds StartKind { get; internal set; }

        public StartCauses StartCause
        {
            get
            {
                switch (Arguments)
                {
                    case IToastNotificationActivatedEventArgs t: return StartCauses.Toast;
                    case ILaunchActivatedEventArgs p when (p?.TileId == "App" && string.IsNullOrEmpty(p?.Arguments)): return StartCauses.PrimaryTile;
                    case ILaunchActivatedEventArgs j when (j?.TileId == "App" && !string.IsNullOrEmpty(j?.Arguments)): return StartCauses.JumpListItem;
                    case ILaunchActivatedEventArgs s when (!string.IsNullOrEmpty(s?.TileId) && s?.TileId != "App"): return StartCauses.SecondaryTile;
                    case IBackgroundActivatedEventArgs b: return StartCauses.BackgroundTrigger;
                    case IFileActivatedEventArgs f: return StartCauses.File;
                    case IPrelaunchActivatedEventArgs p: return StartCauses.Prelaunch;
                    case IProtocolActivatedEventArgs p: return StartCauses.Protocol;
                    case ILockScreenActivatedEventArgs l: return StartCauses.LockScreen;
                    case IShareTargetActivatedEventArgs s: return StartCauses.ShareTarget;
                    case IVoiceCommandActivatedEventArgs v: return StartCauses.VoiceCommand;
                    case ISearchActivatedEventArgs s: return StartCauses.Search;
                    case IDeviceActivatedEventArgs d: return StartCauses.Device;
                    case IDevicePairingActivatedEventArgs d: return StartCauses.DevicePairing;
                    case IContactPanelActivatedEventArgs c: return StartCauses.ContactPanel;
                    // https://blogs.windows.com/buildingapps/2017/07/28/restart-app-programmatically/#1sGJmiirzC2MtROE.97
#if !UAP10_0_15063
                    case ICommandLineActivatedEventArgs c: return StartCauses.CommandLine;
#endif
                    case IActivatedEventArgs r when (r != null && r.Kind == ActivationKind.Launch && r.PreviousExecutionState == ApplicationExecutionState.Terminated): return StartCauses.Restart;
                    case null: return StartCauses.Undetermined;
                    default: return StartCauses.Undetermined;
                }
            }
        }
    }
}
