using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin ("PrismTemplatePack", Namespace = "Prism.Extensibility", Version = "1.0")]

[assembly:AddinName ("Prism Template Pack")]
[assembly:AddinCategory ("IDE extensions")]
[assembly:AddinDescription ("Snippets, Item Templates, and Project Templates for use in Prism application development.")]
[assembly:AddinAuthor ("Brian Lagunas")]
[assembly:AddinUrl("https://github.com/PrismLibrary/Prism")]

//not compiling for some reason
//[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
//[assembly:AddinDependency ("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]

[assembly:AddinDependency ("::MonoDevelop.Core", "5.10.2")]
[assembly:AddinDependency ("::MonoDevelop.Ide", "5.10.2")]
