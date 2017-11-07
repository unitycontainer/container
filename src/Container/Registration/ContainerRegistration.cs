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
                                         IBuildKeyMappingPolicy,
                                         IPolicyList,
                                         IBuildKey
    {
        #region Fields

        private LinkedNode _head;
        private readonly Type _type;
        private readonly string _name;
        private readonly IPolicyList _parenList;

        #endregion



        #region Constructors

        public ContainerRegistration(Type type, string name, object instance, LifetimeManager manager)
        {
            MappedToType = (instance ?? throw new ArgumentNullException(nameof(instance))).GetType();

            _type = type ?? MappedToType;
            _name = name;

            // TODO: Enable when available
            LifetimeManager = manager ?? new ContainerControlledLifetimeManager();
            //LifetimeManager.InUse = true;
            //LifetimeManager.SetValue(instance);

            _head = new LinkedNode
            {
                Interface = typeof(ILifetimePolicy),
                Policy = manager,
                BuildKey = this,
                Next = RegisteredType == MappedToType ? null : new LinkedNode
                {
                    Policy = this
                }
            };
        }

        public ContainerRegistration(IPolicyList parenList, Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            _type = typeFrom ?? typeTo;
            _name = name;
            _parenList = parenList;

            MappedToType = typeTo;
            LifetimeManager = lifetimeManager;

            if (null != injectionMembers && 0 < injectionMembers.Length)
            {
                foreach (var member in injectionMembers)
                {
                    member.AddPolicies(typeFrom, typeTo, name, this);
                }
            }

            if (MappedToType != RegisteredType)
            {
                _head = new LinkedNode
                {
                    Interface = typeof(IBuildKeyMappingPolicy),
                    BuildKey = this,
                    Policy = this,
                    Next = _head
                };
            }

            if (null != lifetimeManager)
            {
                _head = new LinkedNode
                {
                    Interface = typeof(ILifetimePolicy),
                    BuildKey = this,
                    Policy = lifetimeManager,
                    Next = _head
                };
            }
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


        #region IBuildKeyMappingPolicy

        NamedTypeBuildKey IBuildKeyMappingPolicy.Map(NamedTypeBuildKey buildKey, IBuilderContext context)
        {
            return new NamedTypeBuildKey(MappedToType, _name);
        }

        #endregion


        #region IPolicyList


        IBuilderPolicy IPolicyList.Get(Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
        {
            return _parenList.Get(policyInterface, buildKey, out containingPolicyList);
        }

        void IPolicyList.Set(Type policyInterface, IBuilderPolicy policy, object buildKey)
        {
            _head = new LinkedNode
            {
                Interface = policyInterface,
                BuildKey = buildKey,
                Policy = policy,
                Next = _head
            };
        }

        void IPolicyList.Clear(Type policyInterface, object buildKey)
        {
        }

        void IPolicyList.ClearAll()
        {
        }

        #endregion


        #region Legacy


        internal ContainerRegistration(Type registeredType, string name, IPolicyList policies)
        {
            _type = registeredType;
            _name = name;

            MappedToType = GetMappedType(policies);
            LifetimeManagerType = GetLifetimeManagerType(policies);
            LifetimeManager = GetLifetimeManager(policies);
        }



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
            public Type Interface;
            public object BuildKey;
            public IBuilderPolicy Policy;
            public LinkedNode Next;
        }


        #endregion
    }
}
