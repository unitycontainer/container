// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
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
    public class ContainerRegistration : IContainerRegistration,
                                         IIndexerOf<Type, IBuilderPolicy>,
                                         IBuildKey
    {
        #region Fields

        private LinkedNode _head;
        private readonly Type _type;
        private readonly string _name;

        #endregion



        #region Constructors


        internal ContainerRegistration(Type registeredType, string name)
        {
            _type = registeredType;
            _name = name;
        }

        internal ContainerRegistration(Type registeredType, string name, IPolicyList policies)
        {
            _type = registeredType;
            _name = name;

            MappedToType = GetMappedType(policies);
            LifetimeManagerType = GetLifetimeManagerType(policies);
            LifetimeManager = GetLifetimeManager(policies);
        }

        #endregion


        #region IBuildKey

        /// <summary>
        /// Return the <see cref="Type"/> stored in this build key.
        /// </summary>
        /// <value>The type to build.</value>
        Type IBuildKey.Type => _type;

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
        public Type MappedToType { get; }

        /// <summary>
        /// Name the type was registered under. Null for default registration.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// The registered lifetime manager instance.
        /// </summary>
        public Type LifetimeManagerType { get; }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public LifetimeManager LifetimeManager { get; }


        #endregion


        #region IIndexerOf

        public virtual IBuilderPolicy this[Type policyInterface]
        {
            get
            {
                switch (policyInterface)
                {
                    case ILifetimePolicy _:
                        return LifetimeManager;

                    case IBuildPlanPolicy _:
                        return null;// TODO: GetBuildPolicy();

                    default:
                        var hashCode = policyInterface.GetHashCode();
                        for (var node = _head; null != node; node = node.Next)
                        {
                            if (node.HashCode != hashCode || !node.Value
                                                                  .GetType()
                                                                  .GetTypeInfo()
                                                                  .IsAssignableFrom(policyInterface.GetTypeInfo()))
                            {
                                continue;
                            }

                            return node.Value;
                        }

                        return null;
                }
            }

            set
            {
                LinkedNode node;
                var hash = policyInterface?.GetHashCode() ?? 0;

                for (node = _head; node != null; node = node.Next)
                {
                    if (node.HashCode == hash && 
                        node.Value.GetType().GetTypeInfo()
                            .IsAssignableFrom(policyInterface.GetTypeInfo()))
                    {
                        break;
                    }
                }

                if (node != null)
                {
                    // Found it
                    node.Value = value;
                    return;
                }

                // Not found, so add a new one
                _head = new LinkedNode
                {
                    HashCode = hash,
                    Value = value,
                    Next = _head
                };
            }
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

        private Type GetLifetimeManagerType(IPolicyList policies)
        {
            var key = new NamedTypeBuildKey(MappedToType, Name);
            var lifetime = policies.Get<ILifetimePolicy>(key);

            if (lifetime != null)
            {
                return lifetime.GetType();
            }

            if (MappedToType.GetTypeInfo().IsGenericType)
            {
                var genericKey = new NamedTypeBuildKey(MappedToType.GetGenericTypeDefinition(), Name);
                var lifetimeFactory = policies.Get<ILifetimeFactoryPolicy>(genericKey);
                if (lifetimeFactory != null)
                {
                    return lifetimeFactory.LifetimeType;
                }
            }

            return typeof(TransientLifetimeManager);
        }

        private LifetimeManager GetLifetimeManager(IPolicyList policies)
        {
            var key = new NamedTypeBuildKey(MappedToType, Name);
            return (LifetimeManager)policies.Get<ILifetimePolicy>(key);
        }

        #endregion


        #region Nested Types

        public class LinkedNode
        {
            public int HashCode;
            public IBuilderPolicy Value;
            public LinkedNode Next;
        }


        #endregion
    }
}
