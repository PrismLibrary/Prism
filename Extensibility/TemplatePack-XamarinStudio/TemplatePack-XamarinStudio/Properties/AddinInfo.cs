using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin ("PrismTemplatePack", Namespace = "Prism.Extensibility", Version = "1.5")]

[assembly:AddinName ("Prism Template Pack")]
[assembly:AddinCategory ("IDE extensions")]
[assembly:AddinDescription ("Code Templates, Item Templates, and Project Templates for use in Prism application development.")]
[assembly:AddinAuthor ("Brian Lagunas")]
[assembly:AddinUrl("https://github.com/PrismLibrary/Prism")]

[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
