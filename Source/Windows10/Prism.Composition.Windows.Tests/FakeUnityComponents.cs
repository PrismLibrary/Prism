using System;
using Microsoft.Practices.Unity;
using System.Composition;

namespace Prism.Composition.Windows.Tests
{
    public interface IUnityComponent
    {
        IMefComponent MefComponent { get; }
        IMefComponent ImportedMefComponent { get; }
        void Foo();
    }

    public class UnityComponent11 : IUnityComponent
    {
        public Func<IMefComponent> MefComponentFactory { get; private set; }

        private readonly IMefComponent m_MefComponent;

        public UnityComponent11(Func<IMefComponent> mefComponentFactory)
        {
            MefComponentFactory = mefComponentFactory;
            m_MefComponent = mefComponentFactory();
        }

        public void Foo()
        {
        }

        public IMefComponent MefComponent
        {
            get { return m_MefComponent; }
        }

        [Import]
        public IMefComponent ImportedMefComponent { get; set; }
    }

    public class UnityComponent1 : IUnityComponent
    {
        public static int InstanceCount;

        private readonly IMefComponent m_MefComponent;

        public UnityComponent1(IMefComponent mefComponent)
        {
            m_MefComponent = mefComponent;

            InstanceCount++;
        }

        public void Foo()
        {
        }

        public IMefComponent MefComponent
        {
            get { return m_MefComponent; }
        }

        [Import]
        public IMefComponent ImportedMefComponent { get; set; }
    }

    public class UnityComponent2 : IUnityComponent
    {
        private readonly IMefComponent m_MefComponent;

        public UnityComponent2([Dependency("component2")] IMefComponent mefComponent)
        {
            m_MefComponent = mefComponent;
        }

        public void Foo()
        {
        }

        public IMefComponent MefComponent
        {
            get { return m_MefComponent; }
        }

        [Import("component2")]
        public IMefComponent ImportedMefComponent { get; set; }
    }

    public class UnityComponent3 : IUnityComponent
    {
        private readonly IMefComponent m_MefComponent;

        public UnityComponent3([Dependency("component2")] IMefComponent mefComponent)
        {
            m_MefComponent = mefComponent;
        }

        public void Foo()
        {
        }

        public IMefComponent MefComponent
        {
            get { return m_MefComponent; }
        }

        [Import("component2")]
        public IMefComponent ImportedMefComponent { get; set; }
    }

    public interface IUnityOnlyComponent
    {
        void Foo();
    }

    public class UnityOnlyComponent1 : IUnityOnlyComponent
    {
        public static int InstanceCount;

        public UnityOnlyComponent1()
        {
            InstanceCount++;
        }

        public void Foo()
        {
        }
    }

    public class UnityOnlyComponent2 : IUnityOnlyComponent
    {
        public void Foo()
        {
        }
    }

    public class UnityMixedComponent
    {
        public UnityMixedComponent(IMefComponent mefComponent, IUnityOnlyComponent unityComponent)
        {
            MefComponent = mefComponent;
            UnityComponent = unityComponent;
        }

        public IUnityOnlyComponent UnityComponent { get; set; }

        public IMefComponent MefComponent { get; set; }
    }
}