using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using Prism.VisualStudio.Wizards.Design;
using System;
using System.Collections.Generic;
using System.IO;

namespace Prism.VisualStudio.Wizards
{
    public class XamarinFormsProjectWizard : IWizard
    {
        private EnvDTE.DTE _dte = null;
        private string _solutionDir = null;
        private string _templateDir = null;
        private string _projectName = null;
        string _container = null;
        XamarinFormsNewProjectDialogResult _dialogResult;

        public void BeforeOpeningFile(ProjectItem projectItem) { }

        public void ProjectFinishedGenerating(Project project)
        {
            if (_dialogResult.CreateAndroid)
                CreateProject("Droid");

            if (_dialogResult.CreateiOS)
                CreateProject("iOS");

            if (_dialogResult.CreateUwp)
                CreateProject("UWP");

            if (_dialogResult.CreateWinPhone)
                CreateProject("WinPhone");

            if (_dialogResult.CreateWinStore)
                CreateProject("Windows");
        }

        void CreateProject(string platform)
        {
            string name = $"{_projectName}.{platform}";
            string projectPath = System.IO.Path.Combine(_solutionDir, System.IO.Path.Combine(_projectName, name));
            string templateName = $"{_container}App.{platform}";
            string templatePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_templateDir), $"{templateName}.zip\\{templateName}.vstemplate");
            _dte.Solution.AddFromTemplate(templatePath, projectPath, name);
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem) { }

        public void RunFinished() { }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            try
            {
                _dte = automationObject as EnvDTE.DTE;
                _projectName = replacementsDictionary["$safeprojectname$"];
                _container = replacementsDictionary["$container$"];
                _solutionDir = System.IO.Path.GetDirectoryName(replacementsDictionary["$destinationdirectory$"]);
                _templateDir = System.IO.Path.GetDirectoryName(customParams[0] as string);

                XamarinFormsNewProjectDialog dialog = new XamarinFormsNewProjectDialog();
                dialog.ShowDialog();
                _dialogResult = dialog.Result;

                if (_dialogResult.Cancelled)
                    throw new WizardBackoutException();
            }
            catch (Exception ex)
            {
                if (Directory.Exists(_solutionDir))
                    Directory.Delete(_solutionDir, true);

                throw;
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
