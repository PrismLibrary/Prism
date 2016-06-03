namespace Prism.Composition.Windows.Tests.Models
{
    public class UnityOnly2Model : IUnityOnly2Model, IUnityOnlyModels
    {
        private static int instances = 0;

        public UnityOnly2Model()
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
