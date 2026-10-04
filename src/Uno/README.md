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

Prism also registers Uno's default global URI, so the prefix can be omitted:

```xml
<Page x:Class="MyApp.MainPage" ViewModelLocator.AutowireViewModel="False">
    <ContentControl RegionManager.RegionName="MainRegion" />
</Page>
```

Use explicit prefixes to disambiguate library types with the same name. Built-in
WinUI types take precedence over globally registered library types.

Microsoft WinUI heads and Uno projects with implicit namespaces disabled use
explicit `using:Prism.Mvvm`, `using:Prism.Navigation.Regions`, etc.

See [Uno's namespace documentation](https://platform.uno/docs/articles/features/implicit-xaml-namespaces.html).
