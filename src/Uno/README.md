# Uno XAML namespaces

Requires Uno 6.6 or later with `UnoEnableImplicitXamlNamespaces` enabled (the SDK default).

Use `http://prismlibrary.com` to reference Prism types and attached properties:

```xml
<Page x:Class="MyApp.MainPage"
      xmlns:prism="http://prismlibrary.com"
      prism:ViewModelLocator.AutowireViewModel="False">
    <ContentControl prism:RegionManager.RegionName="MainRegion" />
</Page>
```

The Prism packages generate global mappings in your Uno project by default,
so the prefix can be omitted:

```xml
<Page x:Class="MyApp.MainPage" ViewModelLocator.AutowireViewModel="False">
    <ContentControl RegionManager.RegionName="MainRegion" />
</Page>
```

To disable Prism's global mappings, set this property in your project:

```xml
<PropertyGroup>
    <PrismUnoGlobalXmlns>false</PrismUnoGlobalXmlns>
</PropertyGroup>
```

The canonical `http://prismlibrary.com` mappings remain available. The property
can also be supplied on the command line with `-p:PrismUnoGlobalXmlns=false`.

Use explicit prefixes to disambiguate library types with the same name. Built-in
WinUI types take precedence over globally registered library types.

Microsoft WinUI heads and Uno projects with implicit namespaces disabled use
explicit `using:Prism.Mvvm`, `using:Prism.Navigation.Regions`, etc.

See [Uno's namespace documentation](https://platform.uno/docs/articles/features/implicit-xaml-namespaces.html).
