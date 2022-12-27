using System.Collections.Generic;
using AvantiPoint.Nuke.Maui.CI;

namespace DevOps.Jobs;

class PullRequestJob : CIJobBase
{
    public PullRequestJob(string name, string solution)
    {
        Name = $"Build {name}";
        InvokedTargets = new[] { "Compile", $"--solution {solution}" };
    }

    public override HostedAgent Image => HostedAgent.Windows;

    public override bool PublishArtifacts => false;

    public override string Name { get; }

    public override IEnumerable<string> DotNetSdks => new[] { "6.0.x", "7.0.x" };

    public override IEnumerable<string> InvokedTargets { get; }
}
