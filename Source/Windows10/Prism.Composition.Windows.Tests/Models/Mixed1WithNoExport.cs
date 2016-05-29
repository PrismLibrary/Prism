namespace Prism.Composition.Windows.Tests.Models
{
    public class Mixed1WithNoExport : IMixed1WithNoExport, IMixedModels
    {
        private static int instances = 0;

        public Mixed1WithNoExport()
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
