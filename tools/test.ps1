param([string]$solutionPath, [string]$configuration)

Write-Host "Starting Test Run: $solutionPath - $configuration"

if($solutionPath -like '*PrismLibrary_Core*')
{
    Write-Host "Testing Prism Core"
    dotnet test .\Source\Prism.Tests\Prism.Tests.csproj -c $configuration
}
elseif($solutionPath -like '*PrismLibrary_Win10*')
{
    Write-Host "Testing Windows 10"
    Write-Host "UWP tests are not currently not supported by appveyor: https://github.com/appveyor/ci/issues/393"
    # xunit.console .\Source\Windows10\Prism.Windows.Tests\bin\Debug\Prism.Windows.Tests.dll /appveyor
    vstest.console /logger:Appveyor .\Source\Windows10\Prism.Windows.Tests\bin\$configuration\Prism.Windows.Tests.dll
}
elseif($solutionPath -like '*PrismLibrary_Wpf*')
{
    Write-Host "Testing WPF"
    # ls Source\Wpf\Prism.Wpf.Tests\bin
    vstest.console /logger:Appveyor .\Source\Wpf\\Prism.Wpf.Tests\bin\$configuration\Prism.Wpf.Tests.dll
    vstest.console /logger:Appveyor .\Source\Wpf\Prism.Autofac.Wpf.Tests\bin\$configuration\Prism.Autofac.Wpf.Tests.dll
    vstest.console /logger:Appveyor .\Source\Wpf\Prism.DryIoc.Wpf.Tests\bin\$configuration\Prism.DryIoc.Wpf.Tests.dll
    vstest.console /logger:Appveyor .\Source\Wpf\Prism.Mef.Wpf.Tests\bin\$configuration\Prism.Mef.Wpf.Tests.dll
    vstest.console /logger:Appveyor .\Source\Wpf\Prism.StructureMap.Wpf.Tests\bin\$configuration\Prism.StructureMap.Wpf.Tests.dll
    vstest.console /logger:Appveyor .\Source\Wpf\Prism.Unity.Wpf.Tests\bin\$configuration\Prism.Unity.Wpf.Tests.dll
}
elseif($solutionPath -like '*PrismLibrary_XF*' -and $configuration -eq "Test")
{
    Write-Host "Testing Prism Forms"
    dotnet test .\Source\Xamarin\Prism.Forms.Tests\Prism.Forms.Tests.csproj -c Test --no-build
    dotnet test .\Source\Xamarin\Prism.Autofac.Forms.Tests\Prism.Autofac.Forms.Tests.csproj -c Test --no-build
    dotnet test .\Source\Xamarin\Prism.DryIoc.Forms.Tests\Prism.DryIoc.Forms.Tests.csproj -c Test --no-build
    dotnet test .\Source\Xamarin\Prism.Unity.Forms.Tests\Prism.Unity.Forms.Tests.csproj -c Test --no-build
}