// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
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
        #region Fields

        private readonly Type _type;
        private readonly string _name;
        private static readonly TransientLifetimeManager Transient = new TransientLifetimeManager();

        #endregion


        #region Constructors

        public ContainerRegistration(Type registeredType, string name, IPolicyList policies)
        {
            _type = registeredType;
            _name = name;

            MappedToType = GetMappedType(policies);
            LifetimeManager = GetLifetimeManager(policies);
        }

        #endregion


        #region IContainerRegistration

        /// <summary>
        /// The type that was passed to the <see cref="IUnityContainer.RegisterType"/> method
        /// as the "from" type, or the only type if type mapping wasn't done.
        /// </summary>
        public Type RegisteredType => _type;

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="RegisteredType"/> property and this one will have the same value.
        /// </summary>
        public Type MappedToType { get; protected set; }

        /// <summary>
        /// Name the type was registered under. Null for default registration.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public LifetimeManager LifetimeManager { get; protected set; } = Transient;

        #endregion


        #region IBuildKeyMappingPolicy

        NamedTypeBuildKey Map(NamedTypeBuildKey buildKey, IBuilderContext context)
        {
            return new NamedTypeBuildKey(MappedToType, _name);
        }

        #endregion


        #region Legacy

        private Type GetMappedType(IPolicyList policies)
        {
            var buildKey = new NamedTypeBuildKey(_type, _name);
            var mappingPolicy = policies.Get<IBuildKeyMappingPolicy>(buildKey);
            if (mappingPolicy != null)
            {
                return mappingPolicy.Map(buildKey, null).Type;
            }
            return buildKey.Type;
        }

        private LifetimeManager GetLifetimeManager(IPolicyList policies)
        {
            var key = new NamedTypeBuildKey(_type, Name);
            return (LifetimeManager)policies.Get<ILifetimePolicy>(key) ?? Transient;
        }

        #endregion
    }
}
