using System;
using System.Reflection;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Container.Registration
{
    public class TypeRegistration : IContainerRegistration,
                                    IIndexerOf<Type, IBuilderPolicy>,
                                    IBuildKeyMappingPolicy,
                                    IPolicyList,
                                    IBuildKey
    {
        #region Fields

        private readonly Type _type;
        private readonly string _name;
        private ContainerRegistration.LinkedNode _head;
        private static readonly TransientLifetimeManager TransientLifetimeManager = new TransientLifetimeManager();

        #endregion


        #region Constructors

        public TypeRegistration(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            _type = typeFrom ?? typeTo ?? throw new ArgumentNullException(nameof(typeTo));
            _name = name;

            MappedToType = typeTo;
            LifetimeManager = lifetimeManager ?? TransientLifetimeManager;

            if (null != injectionMembers && 0 < injectionMembers.Length)
            {
                foreach (var member in injectionMembers)
                {
                    member.AddPolicies(typeFrom, typeTo, name, this);
                }
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
            var data = this[policyInterface];

            containingPolicyList = null;
            return null;
        }

        void IPolicyList.Set(Type policyInterface, IBuilderPolicy policy, object buildKey)
        {
            _head = new ContainerRegistration.LinkedNode
            {
                HashCode = policyInterface.GetHashCode(),
                Value = policy,
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


        public IBuilderPolicy this[Type policyInterface] {
            get
            {
                switch (policyInterface)
                {
                    case IBuildKeyMappingPolicy _:
                        return this;

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

                            return node.Value as IBuilderPolicy;
                        }

                        return  null;
                }

            }
            set => _head = new ContainerRegistration.LinkedNode
            {
                HashCode = policyInterface.GetHashCode(),
                Value = value,
                Next = _head
            };
        }


        #region Implementation


        #endregion


        #region Nested Types

        public class LinkedNode
        {
            public int HashCode;
            public object Value;
            public ContainerRegistration.LinkedNode Next;
        }


        #endregion
    }
}
