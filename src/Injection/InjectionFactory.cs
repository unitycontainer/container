// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;
using Unity.Builder.Policy;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Injection
{
    /// <summary>
    /// A class that lets you specify a factory method the container
    /// will use to create the object.
    /// </summary>
    /// <remarks>This is a significantly easier way to do the same
    /// thing the old static factory extension was used for.</remarks>
    public class InjectionFactory : InjectionMember, IInjectionFactory
    {
        private readonly Func<IUnityContainer, Type, string, object> factoryFunc;

        /// <summary>
        /// Create a new instance of <see cref="InjectionFactory"/> with
        /// the given factory function.
        /// </summary>
        /// <param name="factoryFunc">Factory function.</param>
        public InjectionFactory(Func<IUnityContainer, object> factoryFunc)
            : this((c, t, s) => factoryFunc(c))
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="InjectionFactory"/> with
        /// the given factory function.
        /// </summary>
        /// <param name="factoryFunc">Factory function.</param>
        public InjectionFactory(Func<IUnityContainer, Type, string, object> factoryFunc)
        {
            this.factoryFunc = factoryFunc ?? throw new ArgumentNullException(nameof(factoryFunc));
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="serviceType">Type of interface being registered. If no interface,
        /// this will be null. This parameter is ignored in this implementation.</param>
        /// <param name="implementationType">Type of concrete type being registered.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            var policy = new FactoryDelegateBuildPlanPolicy(factoryFunc);
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set<IBuildPlanPolicy>(policy,
                new NamedTypeBuildKey(implementationType ?? throw new ArgumentNullException(nameof(implementationType)), name));
        }
    }
}
