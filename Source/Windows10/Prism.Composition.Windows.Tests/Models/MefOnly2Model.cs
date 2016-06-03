using System.Composition;

namespace Prism.Composition.Windows.Tests.Models
{
    [Export(typeof(IMefOnly2Model))]
    [Export(typeof(IMefOnlyModels))]
    [Export("model2", typeof(IMefOnlyModels))]
    public class MefOnly2Model : IMefOnly2Model, IMefOnlyModels
    {
        private static int instances = 0;

        public MefOnly2Model()
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
