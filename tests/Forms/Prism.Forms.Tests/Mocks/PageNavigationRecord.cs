using Prism.Navigation;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationRecord
    {
        public object Sender { get; }
        public PageNavigationEvent Event { get; }
        public INavigationParameters NavigationParameters { get; }

        public PageNavigationRecord(object sender, 
            PageNavigationEvent @event, 
            INavigationParameters navigationParameters = null)
        {
            Sender = sender;
            Event = @event;
            NavigationParameters = navigationParameters;
        }

        public override string ToString()
        {
            return $"{Sender} - {Event}";
        }
    }
}
