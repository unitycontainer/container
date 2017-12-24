using System;
using System.Reflection;
using Unity.Lifetime;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;
using Unity.Policy.Mapping;
using Unity.Storage;

namespace Unity.Registration
{
    public class TypeRegistration : InternalRegistration, 
                                    IContainerRegistration
    {
        #region Constructors

        public TypeRegistration(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
            : base(typeFrom ?? typeTo, string.IsNullOrEmpty(name) ? null : name)
        {
            MappedToType = typeTo;
            LifetimeManager = lifetimeManager ?? TransientLifetimeManager.Instance;

            if (LifetimeManager.InUse) throw new InvalidOperationException(Constants.LifetimeManagerInUse);
            LifetimeManager.InUse = true;

            // Always store MappingPolicy in this.Key this.Value
            if (typeFrom != null && typeFrom != typeTo)
            {
                Key = typeof(IBuildKeyMappingPolicy);
                Value = typeFrom.GetTypeInfo().IsGenericTypeDefinition && typeTo.GetTypeInfo().IsGenericTypeDefinition
                    ? new GenericTypeBuildKeyMappingPolicy(typeTo, name)
                    : (IBuildKeyMappingPolicy) new BuildKeyMappingPolicy(typeTo, name);

                // TODO: Optimize proper resolution path
                if ((null == injectionMembers || injectionMembers.Length == 0) && !(lifetimeManager is IRequireBuildUpPolicy))
                    Next = new LinkedNode<Type, IBuilderPolicy>
                    {
                        Key = typeof(IBuildPlanPolicy),
                        Value = new ResolveBuildUpPolicy()
                    };
            }
        }

        #endregion


        #region IContainerRegistration

        public Type RegisteredType => Type;

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="InternalRegistration.Type"/> property and this one will have the same value.
        /// </summary>
        public Type MappedToType { get; }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public LifetimeManager LifetimeManager { get; }

        #endregion


        #region IPolicyStore

        public override IBuilderPolicy Get(Type policyInterface)
        {
            if (typeof(ILifetimePolicy) == policyInterface)
                return LifetimeManager;

            return base.Get(policyInterface);
        }


        #endregion
    }
}
