using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigModuleEditor
{
    /// <summary>
    /// This class contains a list of GUIDs specific to this sample, 
    /// especially the package GUID and the commands group GUID. 
    /// </summary>
    public static class GuidStrings
    {
        public const string ConfigModuleEditorFactory = "a8339d4c-a1f0-4140-a48d-aff13cd11bc3";
    }
    /// <summary>
    /// List of the GUID objects.
    /// </summary>
    internal static class GuidList
    {
        public static readonly Guid ConfigModuleEditorFactory = new Guid(GuidStrings.ConfigModuleEditorFactory);
    };
}
