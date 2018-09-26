using System;
using Windows.ApplicationModel.Activation;

namespace Prism
{
    public interface IResumeArgs
    {
        ApplicationExecutionState PreviousExecutionState { get; set; }
        ActivationKind Kind { get; set; }
        DateTime SuspensionDate { get; set; }
    }
}
