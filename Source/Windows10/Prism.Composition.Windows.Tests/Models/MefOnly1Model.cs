using System.Composition;

namespace Prism.Composition.Windows.Tests.Models
{
    [Export(typeof(IMefOnly1Model))]
    [Export(typeof(IMefOnlyModels))]
    public class MefOnly1Model : IMefOnly1Model, IMefOnlyModels
    {
        private static int instances = 0;

        public MefOnly1Model()
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
