using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;

namespace Prism.Autofac.Forms.Immutable
{
    //Newer versions of Autofac (e.g. 4.5.x) require implementations of IComponentRegistry to
    //  have a readonly Properties property of type IDictionary<string, object>
    internal interface IPropertiesDictionary
    {
        IDictionary<string, object> Properties { get; }
    }

    /// <summary>
    /// Implementation of IComponentRegistry that is provided for querying Type/Page registrations after
    /// the Prism Autofac container has been built; but does not allow subsequent registrations.
    /// </summary>
    public class RuntimeComponentRegistry : IComponentRegistry, IPropertiesDictionary
    {
        private IContainer _container;

        public RuntimeComponentRegistry(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public bool TryGetRegistration(Service service, out IComponentRegistration registration)
        {
            return _container.ComponentRegistry.TryGetRegistration(service, out registration);
        }

        public bool IsRegistered(Service service)
        {
            return _container.ComponentRegistry.IsRegistered(service);
        }

        public void Register(IComponentRegistration registration)
        {
            throw new InvalidOperationException("It is not possible to use ContainerBuilder.Update() with an immutable Autofac container; " 
                                                + " or to perform registration operations after the container has been built.");
        }

        public void Register(IComponentRegistration registration, bool preserveDefaults)
        {
            throw new InvalidOperationException("It is not possible to use ContainerBuilder.Update() with an immutable Autofac container; "
                                                + " or to perform registration operations after the container has been built.");
        }

        public void AddRegistrationSource(IRegistrationSource source)
        {
            throw new InvalidOperationException("It is not possible to use ContainerBuilder.Update() with an immutable Autofac container; "
                                                + " or to perform registration operations after the container has been built.");
        }

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service)
        {
            return _container.ComponentRegistry.RegistrationsFor(service);
        }

        public IEnumerable<IComponentRegistration> Registrations => _container.ComponentRegistry.Registrations;

        public IEnumerable<IRegistrationSource> Sources => _container.ComponentRegistry.Sources;

        //TODO: The problem here is that IComponentRegistry in Autofac 3.x does NOT have a Properties property
        //  but Autofac 4.x does.  We don't want to require Autofac 4.x at this time, because that requires .NETStandard
        //  which requires extra work in Xamarin.Forms and Prism projects. So attempting to retrieve the value
        //  from the _container.ComponentRegistry if available.
        //  When we get to the point where we can require Autofac 4.x, it will just be this simple line:
        //public IDictionary<string, object> Properties => _container.ComponentRegistry.Properties;
        public IDictionary<string, object> Properties
        {
            get
            {
                IDictionary<string, object> result = null;

                if (_container?.ComponentRegistry != null)
                {
                    try
                    {
                        PropertyInfo props = _container.ComponentRegistry.GetType().GetRuntimeProperty("Properties");
                        if (props != null)
                        {
                            result = props.GetValue(_container.ComponentRegistry, null) as IDictionary<string, object>;
                        }
                    }
                    catch (Exception)
                    {
                        //Could not retrieve Properties property from _container.ComponentRegistry
                        result = new Dictionary<string, object>();
                    }
                }

                return result;
            }
        }

        public bool HasLocalComponents => _container.ComponentRegistry.HasLocalComponents;

        //These events do not appear to be used by Prism.Autofac.Forms at all.
        public event EventHandler<ComponentRegisteredEventArgs> Registered;
        public event EventHandler<RegistrationSourceAddedEventArgs> RegistrationSourceAdded;

        public void Dispose()
        {
            _container = null;
        }        
    }
}
