namespace Prism.Composition.Windows.Tests.Models
{
    public class UnityOnly1Model : IUnityOnly1Model, IUnityOnlyModels
    {
        private static int instances = 0;

        public UnityOnly1Model()
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
