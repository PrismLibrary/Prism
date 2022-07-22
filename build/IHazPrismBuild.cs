using System.Linq;
using DevOps.Extensions;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Components;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

public interface IHazPrismBuild : IHazSolution, IHazConfiguration
{
    bool RequiresMSBuild => Solution.GetProjectNames().Any(p => new[] { "Forms", "Uno", "WinUI" }.Any(n => p.EndsWith(n)));

    AbsolutePath Artifacts => WorkingDirectory / "Artifacts";

    Target Restore => _ => _
        .Unlisted()
        .Executes(() =>
        {
            NuGetRestore();
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .TryTriggers<IPackageSigner>(x => x.Sign)
        .TryTriggers<IHazPrismTests>(x => x.Test)
        .Produces(Artifacts)
        .Executes(() =>
        {
            Log.Information("Building Solution: {Solution}", Solution);
            var buildEngine = RequiresMSBuild ? "MSBuild" : "DotNet Build";
            Log.Information("Building with: {buildEngine}", buildEngine);

            if (RequiresMSBuild)
                MSBuild(_ => _
                    .SetProjectFile(Solution)
                    .SetConfiguration(Configuration));
            else
                DotNetBuild(_ => _
                    .SetProjectFile(Solution)
                    .SetConfiguration(Configuration));
        });
}
