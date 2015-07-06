

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Modularity;

namespace Prism.Wpf.Tests
{
    public class CompilerHelper 
    {
        private static string moduleTemplate =
            @"using System;
            using Prism.Modularity;

            namespace TestModules
            {
                #module#
	            public class #className#Class : IModule
	            {
                    public void Initialize()
                    {
                       Console.WriteLine(""#className#.Start"");
                    }
                }
            }";

        public static Assembly CompileFileAndLoadAssembly(string input, string output, params string[] references)
        {
            return CompileFile(input, output, references).CompiledAssembly;
        }

        public static CompilerResults CompileFile(string input, string output, params string[] references)
        {
            CreateOutput(output);

            List<string> referencedAssemblies = new List<string>(references.Length + 3);

            referencedAssemblies.AddRange(references);
            referencedAssemblies.Add("System.dll");
            referencedAssemblies.Add(typeof(IModule).Assembly.CodeBase.Replace(@"file:///", ""));
            referencedAssemblies.Add(typeof(ModuleAttribute).Assembly.CodeBase.Replace(@"file:///", ""));

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters(referencedAssemblies.ToArray(), output);

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(input))
            {
                if (stream == null)
                {
                    throw new ArgumentException("input");
                }

                StreamReader reader = new StreamReader(stream);
                string source = reader.ReadToEnd();
                CompilerResults results = codeProvider.CompileAssemblyFromSource(cp, source);
                ThrowIfCompilerError(results);
                return results;
            }
        }

        public static void CreateOutput(string output)
        {
            string dir = Path.GetDirectoryName(output);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            else
            {
                //Delete the file if exists
                if (File.Exists(output))
                {
                    try
                    {
                        File.Delete(output);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        //The file might be locked by Visual Studio, so rename it
                        if (File.Exists(output + ".locked"))
                            File.Delete(output + ".locked");
                        File.Move(output, output + ".locked");
                    }
                }
            }
        }

        public static CompilerResults CompileCode(string code, string output)
        {
            CreateOutput(output);
            List<string> referencedAssemblies = new List<string>();
            referencedAssemblies.Add("System.dll");
            referencedAssemblies.Add(typeof(IModule).Assembly.CodeBase.Replace(@"file:///", ""));
            referencedAssemblies.Add(typeof(ModuleAttribute).Assembly.CodeBase.Replace(@"file:///", ""));

            CompilerResults results = new CSharpCodeProvider().CompileAssemblyFromSource(
                new CompilerParameters(referencedAssemblies.ToArray(), output), code);

            ThrowIfCompilerError(results);

            return results;
        }

        public static string GenerateDynamicModule(string assemblyName, string moduleName, string outpath, params string[] dependencies)
        {
            CreateOutput(outpath);

            // Create temporary module.
            string moduleCode = moduleTemplate.Replace("#className#", assemblyName);
            if (!string.IsNullOrEmpty(moduleName))
            {
                moduleCode = moduleCode.Replace("#module#", String.Format("[Module(ModuleName = \"{0}\") #dependencies#]", moduleName));
            }
            else
            {
                moduleCode = moduleCode.Replace("#module#", "");
            }

            string depString = string.Empty;

            foreach (string module in dependencies)
            {
                depString += String.Format(", ModuleDependency(\"{0}\")", module);
            }

            moduleCode = moduleCode.Replace("#dependencies#", depString);

            CompileCode(moduleCode, outpath);

            return outpath;
        }

        public static string GenerateDynamicModule(string assemblyName, string moduleName, params string[] dependencies)
        {
            string assemblyFile = assemblyName + ".dll";
            string outpath = Path.Combine(assemblyName, assemblyFile);

            return GenerateDynamicModule(assemblyName, moduleName, outpath, dependencies);
        }

        public static void ThrowIfCompilerError(CompilerResults results)
        {
            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Compilation failed.");
                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(error.ToString());
                }
                Assert.IsFalse(results.Errors.HasErrors, sb.ToString());
            }
        }

        public static void CleanUpDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        //The file might be locked by Visual Studio, so rename it
                        if (File.Exists(file + ".locked"))
                            File.Delete(file + ".locked");
                        File.Move(file, file + ".locked");
                    }
                }
            }
        }
    }
}