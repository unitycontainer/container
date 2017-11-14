// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Container.Registration
{
    /// <summary>
    /// Class that returns information about the types registered in a container.
    /// </summary>
    public class ContainerRegistration : IContainerRegistration
    {
        private readonly NamedTypeBuildKey buildKey;
        private static readonly TransientLifetimeManager Transient = new TransientLifetimeManager();

        internal ContainerRegistration(Type registeredType, string name, IPolicyList policies)
        {
            this.buildKey = new NamedTypeBuildKey(registeredType, name);
            MappedToType = GetMappedType(policies);
            LifetimeManager = GetLifetimeManager(policies) ?? Transient;
        }

        /// <summary>
        /// The type that was passed to the <see cref="IUnityContainer.RegisterType"/> method
        /// as the "from" type, or the only type if type mapping wasn't done.
        /// </summary>
        public Type RegisteredType { get { return this.buildKey.Type; } }

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="RegisteredType"/> property and this one will have the same value.
        /// </summary>
        public Type MappedToType { get; private set; }

        /// <summary>
        /// Name the type was registered under. Null for default registration.
        /// </summary>
        public string Name { get { return this.buildKey.Name; } }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public LifetimeManager LifetimeManager { get; private set; }

        private Type GetMappedType(IPolicyList policies)
        {
            var mappingPolicy = policies.Get<IBuildKeyMappingPolicy>(this.buildKey);
            if (mappingPolicy != null)
            {
                return mappingPolicy.Map(this.buildKey, null).Type;
            }
            return this.buildKey.Type;
        }

        private LifetimeManager GetLifetimeManager(IPolicyList policies)
        {
            return (LifetimeManager)policies.Get<ILifetimePolicy>(buildKey);
        }
    }
}
