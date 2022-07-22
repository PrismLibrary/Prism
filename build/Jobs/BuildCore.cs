using System.Collections.Generic;
using AvantiPoint.Nuke.Maui.CI;

namespace DevOps.Jobs;

class BuildCore : CIBuild
{
    public override PullRequestTrigger OnPull => new PullRequestTrigger
    {
        Branches = new[] { "master" },
        IncludePaths = new[]
        {
            ".github/workflows/build_core.yml",
            "Directory.Build.props",
            "Directory.Build.targets",
            "xunit.runner.json",
            "src/Prism.Core/**",
            "tests/Prism.Core.Tests/**"
        }
    };

    public override IEnumerable<ICIStage> Stages => new[]
    {
        new CIStage
        {
            Jobs = new []
            {
                new PullRequestJob("Prism.Core", "PrismLibrary_Core.slnf")
            }
        }
    };
}
