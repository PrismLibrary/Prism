param([string]$solutionPath, [string]$configuration)

Write-Host "Starting Test Run: $solutionPath - $configuration"

if($solutionPath -like '*PrismLibrary_Core*' -and $configuration -eq 'Release')
{
    dotnet test $solutionPath -c $configuration
}
elseif($solutionPath -like '*PrismLibrary_Win10*' -and $configuration -eq 'Release')
{

}
elseif($solutionPath -like '*PrismLibrary_Wpf*' -and $configuration -eq 'Release')
{
    vstest.console /logger:Appveyor Source\Wpf\\Prism.Wpf.Tests\bin\Release\Prism.Wpf.Tests.dll
    vstest.console /logger:Appveyor Source\Wpf\Prism.Autofac.Wpf.Tests\bin\Release\Prism.Autofac.Wpf.Tests.dll
    vstest.console /logger:Appveyor Source\Wpf\Prism.DryIoc.Wpf.Tests\bin\Release\Prism.DryIoc.Wpf.Tests.dll
    vstest.console /logger:Appveyor Source\Wpf\Prism.Mef.Wpf.Tests\bin\Release\Prism.Mef.Wpf.Tests.dll
    vstest.console /logger:Appveyor Source\Wpf\Prism.StructureMap.Wpf.Tests\bin\Release\Prism.StructureMap.Wpf.Tests.dll
    vstest.console /logger:Appveyor Source\Wpf\Prism.Unity.Wpf.Tests\bin\Release\Prism.Unity.Wpf.Tests.dll
}
elseif($solutionPath -like '*PrismLibrary_XF*' -and $configuration -eq 'Test')
{
    dotnet test $solutionPath -c $configuration
}