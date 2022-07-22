using AvantiPoint.Nuke.Maui.CI.GitHubActions;
using DevOps.Jobs;
using Nuke.Common;

[GitHubWorkflow(typeof(BuildCore))]
[GitHubWorkflow(typeof(BuildForms))]
[GitHubWorkflow(typeof(BuildUno))]
[GitHubWorkflow(typeof(BuildWpf))]
class Build : NukeBuild, IHazPrismBuild, IHazPrismTests, IPackageSigner
{
    public static int Main () => Execute<Build>();
}
