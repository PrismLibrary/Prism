using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Prism.Ioc.Internals;

namespace Prism.Ioc
{
    /// <summary>
    /// Provides a common exception class to be used by <see cref="IContainerProvider"/> when encountering a resolution exception
    /// </summary>
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "The ContainerResolutionException is a specialized Exception to debug issues resolving services.")]
    public class ContainerResolutionException : Exception
    {
        /// <summary>
        /// The message provided by <see cref="ContainerResolutionException"/> when no registration was found with the container
        /// </summary>
        public const string MissingRegistration = "No Registration was found in the container for the specified type";

        /// <summary>
        /// The message provided by <see cref="ContainerResolutionException"/> when the implementing type is abstract
        /// </summary>
        public const string CannotResolveAbstractType = "The Implementing Type is abstract.";

        /// <summary>
        /// The message provided by <see cref="ContainerResolutionException"/> when multiple constructors were found in the implementing type
        /// </summary>
        public const string MultipleConstructors = "The Implementing Type has multiple constructors which may not be resolvable";

        /// <summary>
        /// The message provided by <see cref="ContainerResolutionException"/> when no public constructors were found in the implementing type
        /// </summary>
        public const string NoPublicConstructors = "The Implementing Type has no public constructors which cause issues with the type being resolved.";

        /// <summary>
        /// The message provided by <see cref="ContainerResolutionException"/> when no public constructors were found in the implementing type
        /// </summary>
        public const string CyclicalDependency = "A cyclical dependency was detected. Type A requires an instance of Type A.";

        /// <summary>
        /// The message provided by <see cref="ContainerResolutionException"/> when we have no clue why the container failed.
        /// </summary>
        /// <remarks>
        /// Please file an issue with the Prism team if you encounter a ContainerResolutionException with this message
        /// </remarks>
        public const string UnknownError = "You seem to have hit an edge case. Please file an issue with the Prism team along with a duplication.";

        /// <summary>
        /// Creates a new instance of the <see cref="ContainerResolutionException"/>
        /// </summary>
        /// <param name="serviceType">The failed Service <see cref="Type"/> that was attempted to be resolved.</param>
        /// <param name="innerException">The actual <see cref="Exception"/> thrown by the Container.</param>
        public ContainerResolutionException(Type serviceType, Exception innerException)
            : this(serviceType, null, innerException)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ContainerResolutionException"/>
        /// </summary>
        /// <param name="serviceType">The failed Service <see cref="Type"/> that was attempted to be resolved.</param>
        /// <param name="serviceName">The Service Name/Key used to resolve the Service Type.</param>
        /// <param name="innerException">The actual <see cref="Exception"/> thrown by the Container.</param>
        public ContainerResolutionException(Type serviceType, string serviceName, Exception innerException)
            : base(GetErrorMessage(serviceType, serviceName), innerException)
        {
            ServiceType = serviceType;
            ServiceName = serviceName;
        }

        // Used by GetErrors()
        private ContainerResolutionException(Type serviceType, string message)
            : base(message)
        {
            ServiceType = serviceType;
        }

        /// <summary>
        /// Gets the ServiceType
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Gets the Service Key if one was used to resolve the ServiceType
        /// </summary>
        /// <remarks>
        /// This is most commonly the key used to resolve a View for Navigation.
        /// </remarks>
        public string ServiceName { get; }

        private bool IsKnownIssue
        {
            get
            {
                switch (Message)
                {
                    // Be sure to add any constants here as new use cases emerge.
                    case MissingRegistration:
                    case CannotResolveAbstractType:
                    case MultipleConstructors:
                    case NoPublicConstructors:
                    case CyclicalDependency:
                    case UnknownError:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Gets a Dictionary of errors and their corresponding types to help diagnose possible issues
        /// that resulted in the ContainerResolutionException.
        /// </summary>
        /// <returns>A <see cref="Dictionary{Type, Exception}"/> mapping types that encountered given exceptions in the resolution process.</returns>
        public ContainerResolutionErrorCollection GetErrors()
        {
            var errors = new ContainerResolutionErrorCollection();
            if (IsKnownIssue)
            {
                return errors;
            }

            var implementingType = TryFindImplementingType();
            if (implementingType is null)
            {
                errors.Add(ServiceType, new ContainerResolutionException(ServiceType, MissingRegistration));
                return errors;
            }
            else if (implementingType.IsAbstract)
            {
                errors.Add(ServiceType, new ContainerResolutionException(implementingType, CannotResolveAbstractType));
            }

            PopulateErrors(implementingType, ref errors);
            return errors;
        }

        private Type TryFindImplementingType()
        {
            var container = ContainerLocator.Current;
            var name = ServiceName;

            // A ViewModel generally isn't directly registered with the container
            // as a result we want to provide the ServiceType when it's a concrete type.
            var defaultValue = IsConcreteType(ServiceType) ? ServiceType : null;
            if (string.IsNullOrEmpty(name))
            {
                if (!container.IsRegistered(ServiceType))
                {
                    return defaultValue;
                }

                return container.GetRegistrationType(ServiceType);
            }
            else if (!container.IsRegistered(ServiceType, ServiceName))
            {
                return defaultValue;
            }

            return container.GetRegistrationType(name);
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "This method is meant to collect any exception thrown.")]
        private static void PopulateErrors(Type implementingType, ref ContainerResolutionErrorCollection errors)
        {
            var ctors = implementingType.GetConstructors();

            if (ctors.Length > 1)
            {
                errors.Add(implementingType, new ContainerResolutionException(implementingType, MultipleConstructors));
            }
            else if (ctors.Length == 0)
            {
                errors.Add(implementingType, new ContainerResolutionException(implementingType, NoPublicConstructors));
                return;
            }

            var ctor = ctors.OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
            var parameters = ctor.GetParameters();
            var parameterInstances = new List<object>();
            var container = ContainerLocator.Current;
            foreach (var parameter in parameters)
            {
                try
                {
                    var defaultImplementingType = IsConcreteType(parameter.ParameterType) ? parameter.ParameterType : null;
                    var parameterImplementingType = container.GetRegistrationType(parameter.ParameterType);
                    if (parameterImplementingType is null)
                        throw new ContainerResolutionException(parameter.ParameterType, MissingRegistration);

                    var instance = container.Resolve(parameter.ParameterType);
                    parameterInstances.Add(instance);
                }
                catch (Exception ex)
                {
                    // TODO: Add Exceptions Extensions lookup here to get root Exception
                    errors.Add(parameter.ParameterType, ex);
                    if (ex is ContainerResolutionException cre && !cre.IsKnownIssue)
                    {
                        foreach (var subError in cre.GetErrors())
                        {
                            errors.Add(subError.Key, subError.Value);
                        }
                    }
                }
            }

            // Return if we had an error with any children
            if (parameters.Length != parameterInstances.Count)
                return;

            try
            {
                // We generally expect some sort of InvocationException Exception here...
                ctor.Invoke(parameterInstances.ToArray());

                // If we managed to create an instance for every parameter and the
                // constructor didn't throw an exception when activating the instance
                // we really aren't sure what allowed us to get here...
                throw new ContainerResolutionException(implementingType, UnknownError);
            }
            catch (TargetInvocationException tie)
            {
                errors.Add(implementingType, tie);

                if (tie.InnerException != null)
                    errors.Add(implementingType, tie.InnerException);
            }
            catch (Exception ex)
            {
                errors.Add(implementingType, ex);
            }
        }

        private static bool IsConcreteType(Type type)
        {
            if (type.IsAbstract || type.IsEnum || type.IsPrimitive || type == typeof(object))
                return false;

            return true;
        }

        private static string GetErrorMessage(Type type, string name)
        {
            var message = $"An unexpected error occurred while resolving '{type.FullName}'";
            if (!string.IsNullOrEmpty(name))
                message += $", with the service name '{name}'";

            return message;
        }
    }
}
