using System;
using AvantiPoint.Nuke.Maui;
using AvantiPoint.Nuke.Maui.Tools.NuGetKeyVaultSignTool;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using static AvantiPoint.Nuke.Maui.Tools.NuGetKeyVaultSignTool.NuGetKeyVaultSignToolTasks;

[PublicAPI]
public interface IPackageSigner : IHazAzureKeyVaultCertificate, INukeBuild
{
    [CI]
    GitHubActions GitHub => TryGetValue(() => GitHub);

    Target Sign => _ => _
        .OnlyWhenStatic(() =>
            !string.IsNullOrEmpty(AzureKeyVault) &&
            !string.IsNullOrEmpty(AzureKeyVaultCertificate) &&
            !string.IsNullOrEmpty(AzureKeyVaultClientId) &&
            !string.IsNullOrEmpty(AzureKeyVaultClientSecret) &&
            !string.IsNullOrEmpty(AzureKeyVaultTenantId))
        .Unlisted()
        .TryAfter<IHazPrismTests>()
        .Executes(() =>
        {
            NuGetKeyVaultSignTool(_ => _
                .SetAzureKeyVaultUrl(new Uri(AzureKeyVault, UriKind.Absolute))
                .SetCertificateName(AzureKeyVaultCertificate)
                .SetClientId(AzureKeyVaultClientId)
                .SetClientSecret(AzureKeyVaultClientSecret)
                .SetTenantId(AzureKeyVaultTenantId));
        });
}
