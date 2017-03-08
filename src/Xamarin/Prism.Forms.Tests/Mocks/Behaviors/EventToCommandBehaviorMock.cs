using Prism.Behaviors;

namespace Prism.Forms.Tests.Mocks.Behaviors
{
    public class EventToCommandBehaviorMock : EventToCommandBehavior
    {
        public void RaiseEvent(params object[] args)
        {
            _handler.DynamicInvoke(args);
        }
    }
}