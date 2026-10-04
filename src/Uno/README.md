# Uno XAML namespaces

Prism's Uno assemblies register `http://prismlibrary.com` (the MAUI spelling) and
`http://prismlibrary.com/` (the WPF spelling). Both resolve the current public CLR
namespaces, including `Prism.Navigation.Regions`, `Prism.Mvvm`,
`Prism.Interactivity`, `Prism.Dialogs` and `Prism.Ioc`. The DryIoc assembly registers
`Prism.DryIoc`. Legacy CodePlex schemas are not registered.

```xml
<Page xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:prism="http://prismlibrary.com"
      x:Class="MyApp.MainPage"
      prism:ViewModelLocator.AutowireViewModel="False">
    <ContentControl prism:RegionManager.RegionName="MainRegion" />
</Page>
```

On Uno compiler targets, Prism also registers the official global presentation
URI. With `UnoEnableImplicitXamlNamespaces` enabled (the Uno.Sdk default), the same
XAML can use Prism types and attached properties without per-file declarations:

```xml
<Page x:Class="MyApp.MainPage" ViewModelLocator.AutowireViewModel="False">
    <ContentControl RegionManager.RegionName="MainRegion" />
</Page>
```

Explicit namespaces remain available when implicit namespaces are disabled.
Built-in WinUI types take precedence over globally registered types of the same
name. Use explicit `xmlns` prefixes to disambiguate types from other libraries.
A consumer that customizes `UnoGlobalXamlNamespaceUri` must register its desired
CLR namespaces under that URI; Prism's global mapping uses Uno's default URI.

## Versions and platform boundary

[Uno's 6.6 release announcement](https://platform.uno/blog/uno-platform-6-6/)
introduces implicit XAML namespaces. The first stable pair is Uno.Sdk **6.6.29** /
Uno.WinUI **6.6.166**; the repository already uses Uno.Sdk 6.6.42 / Uno.WinUI
6.6.184. No .NET SDK major version change is required for this feature.

The public attribute is **System.Windows.Markup.XmlnsDefinitionAttribute** in
**Uno.Xaml.dll**. It predates 6.6 (verified in packages 5.0.19, 6.4.43 and 6.5.237).
It is distinct from Uno's internal **Microsoft.UI.Xaml.XmlnsDefinitionAttribute**.
The 6.6 compiler's
[GlobalNamespaceResolver](https://github.com/unoplatform/uno/blob/6.6.166/src/SourceGenerators/Uno.UI.SourceGenerators/XamlGenerator/GlobalNamespaceResolver.cs)
reads the public attribute for both canonical URI and global mappings.
See the [official registration documentation](https://platform.uno/docs/articles/features/implicit-xaml-namespaces.html).

Microsoft WinUI / Windows App SDK heads use Microsoft's XAML compiler. Prism
excludes these Uno assembly attributes for `windows10` TFMs; those heads continue
to use explicit `using:Prism.Mvvm`, `using:Prism.Navigation.Regions`, etc.
This feature does not make Microsoft's compiler support Uno's schema mappings or
implicit namespace syntax. Existing WPF/MAUI mappings are unchanged.

## Verification

`tests/Uno/Prism.Uno.XamlConsumer` is a real Uno-compiled library referenced by the
Uno unit tests. Its XAML exercises both canonical URI spellings, global/default
namespaces, cross-assembly attached properties, built-in type precedence and
explicit collision resolution. `XamlNamespaceFixture` checks assembly mappings
against exported CLR namespaces, DryIoc mappings and the absence of legacy schemas.

Build the consumer with an explicit inner TFM when narrowing the multitargeted
Prism projects:

```powershell
dotnet build tests/Uno/Prism.Uno.XamlConsumer/Prism.Uno.XamlConsumer.csproj -f net10.0 -p:UnoTargetFrameworks=net10.0
```

The `NegativeNamespaceCase` property selects intentionally invalid XAML. These
builds must fail to resolve `InvokeCommandAction`:

```powershell
dotnet build tests/Uno/Prism.Uno.XamlConsumer/Prism.Uno.XamlConsumer.csproj -f net10.0 -p:UnoTargetFrameworks=net10.0 -p:NegativeNamespaceCase=Codeplex
dotnet build tests/Uno/Prism.Uno.XamlConsumer/Prism.Uno.XamlConsumer.csproj -f net10.0 -p:UnoTargetFrameworks=net10.0 -p:NegativeNamespaceCase=CompositeWpf
```

To verify opt-out compatibility, build only the explicit namespace example:

```powershell
dotnet build tests/Uno/Prism.Uno.XamlConsumer/Prism.Uno.XamlConsumer.csproj -f net10.0 -p:UnoTargetFrameworks=net10.0 -p:XamlConsumerCase=ExplicitNamespaces -p:UnoEnableImplicitXamlNamespaces=false
```
