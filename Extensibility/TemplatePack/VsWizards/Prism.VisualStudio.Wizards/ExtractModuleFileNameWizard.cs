using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Linq;
using System.Collections.Generic;
using EnvDTE;

namespace Prism.VisualStudio.Wizards
{
    public class ExtractModuleFileNameWizard : IWizard
    {
        public void BeforeOpeningFile(global::EnvDTE.ProjectItem projectItem)
        {

        }

        public void ProjectFinishedGenerating(global::EnvDTE.Project project)
        {
            foreach (ProjectItem item in project.ProjectItems)
            {
                if (item.Name == "Module.cs")
                {
                    RenameModule(project, item);
                    return;
                }
            }
        }

        void RenameModule(Project project, ProjectItem item)
        {
            var projectName = project.Name;
            var moduleName = String.Format("{0}{1}", projectName.Split('.').Last(), "Module");
            var className = String.Format("{0}.cs", moduleName);

            CodeClass codeClass = GetCodeClass(item.FileCodeModel.CodeElements);
            if (codeClass != null)
            {
                codeClass.Name = moduleName;
                item.Save();
                item.Name = className;
            }
        }

        CodeClass GetCodeClass(CodeElements codeElements)
        {
            foreach (CodeElement codeElement in codeElements)
            {
                if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
                {
                    CodeNamespace codeNamespace = (CodeNamespace)codeElement;

                    foreach (CodeElement child in codeNamespace.Children)
                    {
                        if (child.Kind == vsCMElement.vsCMElementClass)
                        {
                            return (CodeClass)child;
                        }
                    }
                }
            }

            return null;
        }

        public void ProjectItemFinishedGenerating(global::EnvDTE.ProjectItem projectItem)
        {

        }

        public void RunFinished()
        {

        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {

        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
