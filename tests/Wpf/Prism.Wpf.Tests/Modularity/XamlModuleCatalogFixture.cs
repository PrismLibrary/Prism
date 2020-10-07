using System;
using System.IO;
using System.Linq;
using Prism.Modularity;
using Xunit;

namespace Prism.Wpf.Tests.Modularity
{
    public class XamlModuleCatalogFixture
    {
        private const string _simpleCatalogPackUri = "/Prism.Wpf.Tests;component/Modularity/ModuleCatalogXaml/SimpleModuleCatalog.xaml";
        private const string _invalidDependencyCatalogPackUri = "/Prism.Wpf.Tests;component/Modularity/ModuleCatalogXaml/InvalidDependencyModuleCatalog.xaml";

        [Fact]
        public void XamlModuleCatalog_LoadsXamlFile()
        {
            var catalog = new XamlModuleCatalog(_simpleCatalogPackUri);
            catalog.Initialize();

            Assert.NotNull(catalog);
        }

        [Fact]
        public void XamlModuleCatalog_HasModules()
        {
            var catalog = new XamlModuleCatalog(_simpleCatalogPackUri);
            catalog.Initialize();

            Assert.NotNull(catalog);
            Assert.Equal(5, catalog.Modules.Count());
        }

        [Fact]
        public void XamlModuleCatalog_SupportsLegacyFileFormat()
        {
            var expectedModulePath = Path.GetFullPath("Module3");

            var catalog = new XamlModuleCatalog(_simpleCatalogPackUri);
            catalog.Initialize();

            Assert.NotNull(catalog);

            var module3 = catalog.Modules.SingleOrDefault(x => x.ModuleName == "Module3");

            Assert.NotNull(module3);

            Uri uri = new Uri(module3.Ref);

            Assert.True(uri.IsFile);
            Assert.Equal(expectedModulePath, uri.LocalPath);
        }

        [Fact]
        public void XamlModuleCatalog_HasGroups()
        {
            var catalog = new XamlModuleCatalog(_simpleCatalogPackUri);
            catalog.Initialize();

            Assert.NotNull(catalog);
            Assert.Equal(3, catalog.Groups.Count());
        }

        [Fact]
        public void XamlModuleCatalog_ModuleRefs_Equal_GroupRefs()
        {
            var catalog = new XamlModuleCatalog(_simpleCatalogPackUri);
            catalog.Initialize();

            Assert.NotNull(catalog);

            foreach (var group in catalog.Groups)
            {
                foreach (var module in group)
                {
                    if (!string.IsNullOrEmpty(group.Ref))
                        Assert.Equal(group.Ref, module.Ref);
                }
            }
        }

        [Fact]
        public void XamlModuleCatalog_GroupsHaveValidRefs()
        {
            var catalog = new XamlModuleCatalog(_simpleCatalogPackUri);
            catalog.Initialize();

            Assert.NotNull(catalog);

            foreach (var group in catalog.Groups)
            {
                if (!string.IsNullOrEmpty(group.Ref))
                {
                    var uri = new Uri(group.Ref);

                    Assert.True(uri.IsFile);
                }
            }
        }

        [Fact]
        public void XamlModuleCatalog_GroupWithNoRef_ModulesHaveValidRefs()
        {
            var catalog = new XamlModuleCatalog(_simpleCatalogPackUri);
            catalog.Initialize();

            Assert.NotNull(catalog);

            foreach (var group in catalog.Groups)
            {
                foreach (var module in group)
                {
                    Assert.NotNull(module.Ref);

                    var uri = new Uri(module.Ref);

                    Assert.True(uri.IsFile);
                }
            }
        }

        [Fact]
        public void ShouldThrowOnInvalidDependency()
        {
            var catalog = new XamlModuleCatalog(_invalidDependencyCatalogPackUri);

            var ex = Assert.Throws<ModularityException>(() =>
            {
                catalog.Initialize();
            });

            Assert.Contains("A module declared a dependency on another module which is not declared to be loaded", ex.Message);
        }
    }
}
