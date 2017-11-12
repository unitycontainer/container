using System;
using System.Reflection;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Container.Registration
{
    public class TypeRegistration : IContainerRegistration,
                                    IBuildKeyMappingPolicy,
                                    IPolicyList,
                                    IBuildKey
    {
        #region Fields

        private IBuildPlanPolicy _buildPlan;
        private readonly Type _type;
        private readonly string _name;
        private readonly IPolicyList _parent;
        private ContainerRegistration.LinkedNode _head;
        private static readonly TransientLifetimeManager TransientLifetimeManager = new TransientLifetimeManager();

        #endregion



        #region Constructors

        public TypeRegistration(IPolicyList parenList, Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            _type = typeFrom ?? typeTo ?? throw new ArgumentNullException(nameof(typeTo));
            _parent = parenList ?? throw new ArgumentNullException(nameof(parenList));
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
            switch (policyInterface)
            {
                case IBuildKeyMappingPolicy _:
                    containingPolicyList = this;
                    return this;

                case ILifetimePolicy _:
                    containingPolicyList = this;
                    return LifetimeManager;

                case IBuildPlanPolicy _:
                    containingPolicyList = this;
                    return GetBuildPolicy();

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

                        containingPolicyList = this;
                        return node.Value as IBuilderPolicy;
                    }
                    break;
            }

            containingPolicyList = null;
            return _parent.Get(policyInterface, buildKey, out containingPolicyList);
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


        #region Implementation

        private IBuildPlanPolicy GetBuildPolicy()
        {
            if (null == _buildPlan)
            {
                lock (this)
                {
                    if (null == _buildPlan)
                    {
                        var planCreator = _parent.Get<IBuildPlanCreatorPolicy>(this, out var _);
                        if (planCreator != null)
                        {
                            //_buildPlan = planCreator.CreatePlan(context, context.BuildKey);
                        }
                    }
                }
            }

            return _buildPlan;
        }

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
