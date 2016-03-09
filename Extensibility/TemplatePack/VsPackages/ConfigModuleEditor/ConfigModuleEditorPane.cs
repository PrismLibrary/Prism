using ConfigModuleEditor.Models;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;

namespace ConfigModuleEditor
{
    [ComVisible(true)]
    public sealed class ConfigModuleEditorPane : WindowPane, IVsPersistDocData
    {
        private ConfigModuleEditorDesigner _designer;
        XmlDocument _document = new XmlDocument();
        private string _fileName = string.Empty;
        bool _isDirty = false;

        protected override void Initialize()
        {
            base.Initialize();

            _designer = new ConfigModuleEditorDesigner();
            _designer.IsDirtyChanged += Designer_IsDirtyChanged;
            Content = _designer;

            LoadProjectModules();
        }

        private void Designer_IsDirtyChanged(object sender, EventArgs e)
        {
            _isDirty = true;
        }

        private void LoadModules(string filePath)
        {
            _document.Load(filePath);

            //XmlNode configurationNode = _document.SelectSingleNode("/configuration");
            //if (configurationNode == null)
            //    throw new Exception("InValid Config File format");

            var modulesNodes = _document.SelectNodes("/configuration/modules/module");
            if (modulesNodes.Count == 0)
                return;

            List<ModuleInfo> modules = new List<ModuleInfo>();

            foreach (XmlNode m in modulesNodes)
            {
                ModuleInfo info = new ModuleInfo();
                info.AssemblyFile = m.Attributes["assemblyFile"].Value;
                info.ModuleName = m.Attributes["moduleName"].Value;
                info.ModuleType = m.Attributes["moduleType"].Value;
                info.StartupLoaded = m.Attributes["startupLoaded"] != null ? Boolean.Parse(m.Attributes["startupLoaded"].Value) : false;

                var dependencies = m.SelectNodes("./dependencies/dependency");
                if (dependencies.Count > 0)
                {
                    foreach (XmlNode d in dependencies)
                    {
                        ModuleDependencyInfo dependencyInfo = new ModuleDependencyInfo();
                        dependencyInfo.ModuleName = d.Attributes["moduleName"].Value;
                        info.Dependencies.Add(dependencyInfo);
                    }
                }

                modules.Add(info);
            }

            _designer.Modules = new System.Collections.ObjectModel.ObservableCollection<ModuleInfo>(modules);
        }

        void LoadProjectModules()
        {
            try
            {
                var projects = GetProjects();

                List<ProjectModuleInfo> projectModules = new List<ProjectModuleInfo>();

                foreach (var project in projects)
                {
                    foreach (ProjectItem item in project.ProjectItems)
                    {
                        FileCodeModel2 codeModel = (FileCodeModel2)item.FileCodeModel;
                        if (codeModel != null)
                        {
                            CodeClass2 codeClass = GetCodeClass(codeModel.CodeElements);
                            if (codeClass != null)
                            {
                                if (IsIModule(codeClass.ImplementedInterfaces))
                                {
                                    var moduleInfo = new ProjectModuleInfo();
                                    moduleInfo.ModuleName = codeClass.Name;
                                    moduleInfo.AssemblyFile = String.Format("{0}.dll", project.Properties.Item("AssemblyName").Value);
                                    moduleInfo.ModuleType = GetModuleType(project, codeClass);
                                    projectModules.Add(moduleInfo);
                                    break;
                                }
                            }
                        }
                    }
                }

                _designer.ProjectModules = new System.Collections.ObjectModel.ObservableCollection<ProjectModuleInfo>(projectModules);
            }
            catch(Exception ex)
            {
                string message = "The was a problem reading the projects in this solution. Build the solution and try again.";
                string title = "Error Loading Projects";

                // Show a message box to prove we were here
                VsShellUtilities.ShowMessageBox(
                    this,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        CodeClass2 GetCodeClass(CodeElements codeElements)
        {
            foreach (CodeElement codeElement in codeElements)
            {
                if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
                {
                    CodeNamespace codeNamespace = (CodeNamespace)codeElement;

                    foreach (CodeElement2 child in codeNamespace.Children)
                    {
                        if (child.Kind == vsCMElement.vsCMElementClass)
                        {
                            return (CodeClass2)child;
                        }
                    }
                }
            }

            return null;
        }

        bool IsIModule(CodeElements implementedInterfaces)
        {
            foreach (CodeInterface codeInterface in implementedInterfaces)
            {
                if (codeInterface.FullName == "Prism.Modularity.IModule")
                {
                    return true;
                }
            }

            return false;
        }

        string GetModuleType(Project project, CodeClass codeClass)
        {
            IVsSolution solution = (IVsSolution)GetService(typeof(IVsSolution));
            DynamicTypeService typeResolver = (DynamicTypeService)GetService(typeof(DynamicTypeService));

            IVsHierarchy hierarchy = null;
            solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);

            var typeResolutionService = typeResolver.GetTypeResolutionService(hierarchy);

            return typeResolutionService.GetType(codeClass.FullName).AssemblyQualifiedName;
        }

        #region IPersistFileFormat Members

        public int GetGuidEditorType(out Guid pClassID)
        {
            pClassID = GuidList.ConfigModuleEditorFactory;
            return VSConstants.S_OK;
        }

        public int IsDocDataDirty(out int pfDirty)
        {
            if (_isDirty)
                pfDirty = 1;
            else
                pfDirty = 0;

            return VSConstants.S_OK;
        }

        public int SetUntitledDocPath(string pszDocDataPath)
        {
            return VSConstants.S_OK;
        }

        public int LoadDocData(string pszMkDocument)
        {
            _fileName = pszMkDocument;

            LoadModules(_fileName);

            _isDirty = false;

            return VSConstants.S_OK;
        }

        public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
        {
            pbstrMkDocumentNew = null;
            pfSaveCanceled = 0;

            switch (dwSave)
            {
                case VSSAVEFLAGS.VSSAVE_Save:
                case VSSAVEFLAGS.VSSAVE_SilentSave:
                    {
                        XmlNode configurationNode = _document.SelectSingleNode("/configuration");

                        var moduleConfigSection = configurationNode.SelectSingleNode(@"./configSections/section[@name=""modules""]");
                        if (moduleConfigSection == null)
                        {
                            XmlNode configSectionNode = configurationNode.SelectSingleNode("./configSections");
                            if (configSectionNode == null)
                            {
                                configSectionNode = _document.CreateElement("configSections");
                                configurationNode.InsertAfter(configSectionNode, null); //make it the first element
                            }

                            XmlElement section = _document.CreateElement("section");

                            XmlAttribute attribute = _document.CreateAttribute("name");
                            attribute.Value = "modules";
                            section.Attributes.Append(attribute);

                            attribute = _document.CreateAttribute("type");
                            attribute.Value = "Prism.Modularity.ModulesConfigurationSection, Prism.Wpf";
                            section.Attributes.Append(attribute);

                            configSectionNode.AppendChild(section);
                        }

                        var modulesNode = _document.SelectSingleNode("/configuration/modules");
                        if (modulesNode == null)
                        {
                            modulesNode = _document.CreateElement("modules");
                            configurationNode.AppendChild(modulesNode);
                        }

                        modulesNode.RemoveAll();

                        foreach (var m in _designer.Modules)
                        {
                            XmlElement element = _document.CreateElement("module");

                            XmlAttribute attribute = _document.CreateAttribute("assemblyFile");
                            attribute.Value = m.AssemblyFile;
                            element.Attributes.Append(attribute);

                            attribute = _document.CreateAttribute("moduleType");
                            attribute.Value = m.ModuleType;
                            element.Attributes.Append(attribute);

                            attribute = _document.CreateAttribute("moduleName");
                            attribute.Value = m.ModuleName;
                            element.Attributes.Append(attribute);

                            attribute = _document.CreateAttribute("startupLoaded");
                            attribute.Value = m.StartupLoaded.ToString();
                            element.Attributes.Append(attribute);

                            if (m.Dependencies.Count > 0)
                            {
                                XmlElement dependencies = _document.CreateElement("dependencies");
                                foreach (var d in m.Dependencies)
                                {
                                    XmlElement dElement = _document.CreateElement("dependency");

                                    XmlAttribute dAttribute = _document.CreateAttribute("moduleName");
                                    dAttribute.Value = d.ModuleName;
                                    dElement.Attributes.Append(dAttribute);

                                    dependencies.AppendChild(dElement);
                                }
                                element.AppendChild(dependencies);
                            }

                            modulesNode.AppendChild(element);
                        }

                        _document.Save(_fileName);

                        _isDirty = false;
                        break;
                    }
            }

            return VSConstants.S_OK;
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        public int OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
        {
            return VSConstants.S_OK;
        }

        public int RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }

        public int IsDocDataReloadable(out int pfReloadable)
        {
            // Allow file to be reloaded
            pfReloadable = 1;
            return VSConstants.S_OK;
        }

        public int ReloadDocData(uint grfFlags)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IDE Helpers

        public static DTE2 GetActiveIDE()
        {
            // Get an instance of currently running Visual Studio IDE.
            DTE2 dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            return dte2;
        }

        public static IList<Project> GetProjects()
        {
            Projects projects = GetActiveIDE().Solution.Projects;
            List<Project> list = new List<Project>();
            var item = projects.GetEnumerator();
            while (item.MoveNext())
            {
                var project = item.Current as Project;
                if (project == null)
                {
                    continue;
                }

                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(GetSolutionFolderProjects(project));
                }
                else
                {
                    list.Add(project);
                }
            }

            return list;
        }

        private static IEnumerable<Project> GetSolutionFolderProjects(Project solutionFolder)
        {
            List<Project> list = new List<Project>();
            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            {
                var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(GetSolutionFolderProjects(subProject));
                }
                else
                {
                    list.Add(subProject);
                }
            }
            return list;
        }

        #endregion
    }
}
