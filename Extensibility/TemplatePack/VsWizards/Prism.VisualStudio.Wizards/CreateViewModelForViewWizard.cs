using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using EnvDTE;
using System.IO;

namespace Prism.VisualStudio.Wizards
{
    public class CreateViewModelForViewWizard : IWizard
    {
        EnvDTE.DTE _dte = null;
        string _viewName;
        string _viewModelName;
        string _templatesDirectory;

        public void BeforeOpeningFile(ProjectItem projectItem)
        {

        }

        public void ProjectFinishedGenerating(Project project)
        {

        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {

        }

        public void RunFinished()
        {
            string templatePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_templatesDirectory.Replace("Xamarin.Forms", "Code")), "PrismViewModel.zip\\PrismViewModel.vstemplate");

            Array activeProjects = (Array)_dte.ActiveSolutionProjects;
            Project activeProject = (Project)activeProjects.GetValue(0);

            ProjectItem viewModelsFolder = activeProject.ProjectItems.Item("ViewModels");
            if (viewModelsFolder != null)
            {
                viewModelsFolder.ProjectItems.AddFromTemplate(templatePath, $"{_viewModelName}.cs");
            }

            ProjectItem appXaml = activeProject.ProjectItems.Item("App.xaml");
            if (appXaml != null)
            {
                ProjectItem appXamlCS = appXaml.ProjectItems.Item(1);
                EditAppRegisterTypesForNavigation(appXamlCS.FileCodeModel.CodeElements);
                return;
            }
            
            ProjectItem app = activeProject.ProjectItems.Item("App.cs");
            if (app != null)
            {
                EditAppRegisterTypesForNavigation(app.FileCodeModel.CodeElements);
            }
        }

        void EditAppRegisterTypesForNavigation(CodeElements codeElements)
        {
            CodeFunction registerTypesFunction = GetRegisterTypesMethod(codeElements);
            if (registerTypesFunction != null)
            {
                TextPoint methodEndPoint = registerTypesFunction.GetEndPoint(vsCMPart.vsCMPartBody);
                EditPoint editPoint = methodEndPoint.CreateEditPoint();
                editPoint.Insert($"Container.RegisterTypeForNavigation<{_viewName}>();");
                editPoint.SmartFormat(methodEndPoint);
            }
        }

        CodeFunction GetRegisterTypesMethod(CodeElements codeElements)
        {
            foreach (CodeElement codeElement in codeElements)
            {
                if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
                {
                    CodeNamespace codeNamespace = (CodeNamespace)codeElement;
                    CodeClass codeClass = (CodeClass)codeNamespace.Children.Item("App");
                    return (CodeFunction)codeClass.Members.Item("RegisterTypes");
                }
            }

            return null;
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            _dte = automationObject as EnvDTE.DTE;
            _viewName = replacementsDictionary["$safeitemname$"];
            _viewModelName = $"{_viewName}ViewModel";
            _templatesDirectory = Path.GetDirectoryName(customParams[0] as string);
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
