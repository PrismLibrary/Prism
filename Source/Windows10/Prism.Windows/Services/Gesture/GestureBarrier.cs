using System;

namespace Prism.Services
{
    public class GestureBarrier
    {
        public Gesture Gesture { internal set; get; }
        public Action Complete { internal set; get; }
        public event EventHandler Event;
        internal void RaiseEvent(EventArgs args)
            => Event?.Invoke(this, args);
    }
}