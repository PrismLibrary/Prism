# Prism Assembly Versioning
Assembly versioning is an important, and often ignored, aspect of a project.  There are three assembly attributes defined in the AssemblyInfo.cs file of each project of Prism.  These attribute are the **AssemblyAttribute**, the **AssemblyFileAttribute**, and the **AssemblyInformationAttribute**.

```
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0")]
```
By convention, the four parts of each version are referred to as the **Major** Version, **Minor** Version, **Build**, and **Revision**.

This document decribes how Prism unitilizes each of these attributes to version its assemblies, and what numbering convention each attribute follows.

## AssemblyVersion
The **AssemblyVersion** is used by the CLR to bind to strongly named assemblies. It is stored in the AssemblyDef manifest metadata table of the built assembly, and in the AssemblyRef table of any assembly that references it. When you reference a strongly named assembly, which Prism is strongly named, you are tightly bound to that specific AssemblyVersion of that assembly.

For example; assume you referenced version 1.0.0.0 of an assembly and then built and released your application to your users.  Then, an updated version of 1.0.2.0 which fixes a critical bug became available, and was dropped into the executing folder (possibly GAC) of your application. That binding would fail and your application would crash.  You would have to reference the new 1.0.2.0 version and rebuild and redeploy your application.

To avoid this, Prism has adopted the following format for the **AssemblyVersion**:

**[Major].[Minor]**

We will only update the **AssemblyVersion** when there is a major or minor change in the functionality of the assembly.  We will not update the **AssemblyVersion** when releasing bug fixes and updates.

## AssemblyFileVersion
The **AssemblyFileVersion** is intended to uniquely identify the build of an assembly.  When patches and bug fixes are released, the **AssemblyFileVersion** is incremented to reflect those changes.

Prism has standardized on the following format for **AssemblyFileVersion**:

**[Major].[Minor].[Revision]**

This will allow Prism to release bug fixes and updates without having to increment the **AssemblyVersion**.  This enables you as a developer to patch your applications without having to re-reference Prism assemblies or recompile and re-release your application.

## AssemblyInformationVersion
The **AssemblyInformationalVersion** is intended to allow coherent versioning of the entire product, which may consist of many assemblies that are independently versioned, and potentially developed by disparate teams. The **AssemblyInformationAttribute** is used to communicate to the community, or customers, what version of Prism is the current release.

For example; Assume we release a product with an **AssemblyInformationAttribute** of 2.0.0.0.  This product may be made up of many assemblies with their **AssemblyVersion** ranging from 1.0.0.0 to 1.9.9.9.  Although the individual assemblies are not versioned at 2.0.0.0, the product itself is a 2.0 release.  

The **AssemblyInformationAttribute** lets us market Prism as an overall product made up of several related assemblies that have different assembly versions.

Prism has standardize on the following format for **AssemblyInformationVersioning**:

**[Major].[Minor].[Revision]**

_Note: This is the version that the Prism NuGet packages will match_
