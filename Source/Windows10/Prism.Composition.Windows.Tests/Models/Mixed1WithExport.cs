using System.Composition;

namespace Prism.Composition.Windows.Tests.Models
{
    [Export(typeof(IMixed1WithExport))]
    [Export(typeof(IMixedModels))]
    public class Mixed1WithExport : IMixed1WithExport, IMixedModels
    {
        private static int instances = 0;

        public Mixed1WithExport()
        {
            instances++;
        }

        public int ActiveInstances
        {
            get
            {
                return instances;
            }

            set
            {
                instances = value;
            }
        }
    }
}
