﻿using System;
using DryIoc;
using Prism.Ioc;

namespace Prism.DryIoc
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        /// <summary>
        /// Create <see cref="Rules" /> to alter behavior of <see cref="IContainer" />
        /// </summary>
        /// <returns>An instance of <see cref="Rules" /></returns>
        protected virtual Rules CreateContainerRules() => Rules.Default.WithAutoConcreteTypeResolution()
                                                                       .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                                                                       .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);

        protected override IContainerExtension CreateContainerExtension()
        {
            return new DryIocContainerExtension(new Container(CreateContainerRules()));
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ContainerException));
        }
    }
}
