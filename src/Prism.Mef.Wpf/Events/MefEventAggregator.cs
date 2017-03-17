

//using Prism.PubSubEvents;

using System.ComponentModel.Composition;
using Prism.Events;

namespace Prism.Mef.Events
{
    /// <summary>
    /// Exports the EventAggregator using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(IEventAggregator))]
    public class MefEventAggregator : EventAggregator
    {
    }
}