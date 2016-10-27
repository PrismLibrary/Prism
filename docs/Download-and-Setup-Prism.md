#Download and Setup Prism

Learn what’s included in Prism including the documentation, code samples, and libraries.  Additionally find out where to get the library and sample source code and the library NuGet packages.

For a list of the new features, bug fixes, and API changes, see the [release notes](https://github.com/PrismLibrary/Prism/wiki).

# Download and Setup the Prism Source Code

This section describes how to install Prism. It involves the following three steps:
-  Install system requirements.
-  Download and extract the Prism source code and documentation.
-  Compile and run the samples.

### Step 1: Install System Requirements 

Prism was designed to run on the Microsoft Windows 8 desktop, Microsoft Windows 7, Windows Vista, or Windows Server 2008 operating system. WPF applications built using this guidance require the .NET Framework 4.5.

Before you can use the Prism Library, the following must be installed:

-  Microsoft .NET Framework 4.5 or greater.
-  Microsoft Visual Studio 2012 or greater.
-  Xamarin for Visual Studio 3.11.1537 or greater.

### Step 2: Download and Extract the Prism Source Code and Documentation

The easiest way to download Prism source code, and documentation is to fork the [Prism repository](https://github.com/prismlibrary/prism).

You can download the source code, documentation, and samples for the Prism library from the following links:

-  [Prism Source Code and Documentation](https://github.com/PrismLibrary/Prism/releases)
-  Samples
    -  [WPF](https://github.com/PrismLibrary/Prism-Samples-Wpf)
    -  [Universal Windows Platform](https://github.com/PrismLibrary/Prism-Samples-Windows)
    -  [Xamarin.Forms](https://github.com/PrismLibrary/Prism-Samples-Forms)

_Optionally you can add the Prism assemblies directly to your projects by using the [NuGet packages](#nuget-packages)._

### Step 3: Compile and Run Samples

All samples use the Prism NuGet references so you can compile and run each solution directly.

# Adding Prism Library Source Projects to Solutions

As part of shipping the Prism Library as NuGet packages, the Prism Library projects were removed from the solutions of all sample projects. If you are a developer accustomed to stepping through the Prism Library code as you build your application, there are a couple of options:

-  **Add the Prism Library Projects back in**. To do this, right-click the solution, point to **Add**, and then click **Existing project**. Select the Prism Library projects. Then, to prevent inadvertently building these, click **Configuration Manager** on the **Build**
menu, and then clear the **Build** check box for all Prism Library projects in both the debug and release configurations.
-  **Set a breakpoint and step in**. Set a break point in your application's bootstrapper, and then step in to a method within the base class (F11 is the typical C\# keyboard shortcut for this). You may be asked to locate the Prism Library source code, but often, the full program database (PDB) file is available and the file will simply open. You may set breakpoints in any Prism Library project by opening the file and setting the breakpoint.
