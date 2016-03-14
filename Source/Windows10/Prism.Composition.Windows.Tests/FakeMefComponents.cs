using System.Composition;

namespace Prism.Composition.Windows.Tests
{
    public interface IMefComponent
    {
        void Foo();
    }

    [Export(typeof(IMefComponent))]
    [Shared]
    public class MefComponent1 : IMefComponent
    {
        public static int InstanceCount;

        public MefComponent1()
        {
            InstanceCount++;
        }

        public void Foo()
        {
        }
    }

    [Export("component2", typeof(IMefComponent))]
    [Shared]
    public class MefComponent2 : IMefComponent
    {
        public void Foo()
        {
        }
    }

    public interface IMefComponentWithUnityDependencies
    {
        IUnityOnlyComponent UnityOnlyComponent { get; set; }
        IMefComponent MefOnlyComponent { get; set; }
        void Foo();
    }

    [Export(typeof(IMefComponentWithUnityDependencies))]
    [Shared]
    public class MefComponentWithUnityDependencies1 : IMefComponentWithUnityDependencies
    {
        public IMefComponent MefOnlyComponent { get; set; }
        public IUnityOnlyComponent UnityOnlyComponent { get; set; }

        [ImportingConstructor]
        public MefComponentWithUnityDependencies1(
            IUnityOnlyComponent unityOnlyComponent,
            IMefComponent mefComponent)
        {
            MefOnlyComponent = mefComponent;
            UnityOnlyComponent = unityOnlyComponent;
        }

        public void Foo()
        {
        }
    }

    [Export("component2", typeof(IMefComponentWithUnityDependencies))]
    [Shared]
    public class MefComponentWithUnityDependencies2 : IMefComponentWithUnityDependencies
    {
        public IMefComponent MefOnlyComponent { get; set; }
        public IUnityOnlyComponent UnityOnlyComponent { get; set; }
        public IUnityComponent MixedUnityMefComponent { get; set; }

        [ImportingConstructor]
        public MefComponentWithUnityDependencies2(
            IUnityOnlyComponent unityOnlyComponent,
            IMefComponent mefComponent,
            IUnityComponent mixedUnityMefComponent)
        {
            MefOnlyComponent = mefComponent;
            UnityOnlyComponent = unityOnlyComponent;
            MixedUnityMefComponent = mixedUnityMefComponent;
        }

        public void Foo()
        {
        }
    }

    public interface IMultipleMefComponent
    {
        void Foo();
    }

    public class MultipleMefComponent1 : IMultipleMefComponent
    {
        public void Foo()
        {
        }
    }

    public class MultipleMefComponent2 : IMultipleMefComponent
    {
        public void Foo()
        {
        }
    }

    [Export]
    public class MefMixedComponent
    {
        [ImportingConstructor]
        public MefMixedComponent(IMefComponent mefComponent, IUnityOnlyComponent unityComponent)
        {
            MefComponent = mefComponent;
            UnityComponent = unityComponent;
        }

        public IUnityOnlyComponent UnityComponent { get; set; }

        public IMefComponent MefComponent { get; set; }
    }
}