using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;

namespace DevOps.Extensions;

static class SolutionExtensions
{
    public static IEnumerable<string> GetProjectNames(this Solution solution)
    {
        if (solution.Path.HasExtension(".sln"))
            return solution.AllProjects.Select(x => x.Name);

        var document = JsonDocument.Parse(File.ReadAllText(solution.Path));
        var projects = new List<string>();
        foreach(var rootNode in document.RootElement.EnumerateObject())
        {
            if(rootNode.Name == "solution")
            {
                foreach(var node in rootNode.Value.EnumerateObject())
                {
                    if(node.Name == "projects")
                    {
                        node.Value.EnumerateArray()
                            .ForEach(x => projects.Add(Path.GetFileNameWithoutExtension(x.GetString())));
                    }
                }
            }
        }

        return projects;
    }

    public static bool HasExtension(this AbsolutePath path, string extension) =>
        Path.GetExtension(path.ToString()) == extension;
}
