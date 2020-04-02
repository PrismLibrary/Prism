namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationRecord
    {
        public object Sender { get; }
        public PageNavigationEvent Event { get; }

        public PageNavigationRecord(object sender, PageNavigationEvent @event)
        {
            Sender = sender;
            Event = @event;
        }

        public override string ToString()
        {
            return $"{Sender} - {Event}";
        }
    }
}
