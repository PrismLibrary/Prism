using System.Linq;
using DevOps.Extensions;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Components;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[PublicAPI]
public interface IHazPrismTests : IHazSolution, IHazConfiguration, INukeBuild
{
    Target Test => _ => _
        .OnlyWhenDynamic(() => Solution.GetProjectNames().Any(x => x.EndsWith("Tests")))
        .TryBefore<IPackageSigner>()
        .Unlisted()
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution)
                .EnableNoBuild());
        });
}
