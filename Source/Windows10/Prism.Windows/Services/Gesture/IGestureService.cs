using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;

namespace Prism.Services
{
    public interface IGestureService
    {
        event TypedEventHandler<object, KeyDownEventArgs> KeyDown;

        GestureBarrier CreateBarrier(Gesture evt);

        event EventHandler BackRequested;
        event EventHandler ForwardRequested;
        event EventHandler MenuRequested;
        event EventHandler RefreshRequested;
        event EventHandler SearchRequested;

        bool RaiseBackRequested();
        bool RaiseForwardRequested();
        bool RaiseMenuRequested();
        bool RaiseRefreshRequested();
        bool RaiseSearchRequested();
    }
}