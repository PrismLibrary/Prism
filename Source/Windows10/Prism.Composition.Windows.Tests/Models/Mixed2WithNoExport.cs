namespace Prism.Composition.Windows.Tests.Models
{
    public class Mixed2WithNoExport : IMixed2WithNoExport, IMixedModels
    {
        private static int instances = 0;

        public Mixed2WithNoExport()
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
