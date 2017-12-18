using System;
using System.Reflection;
using Unity.Builder;
using Unity.Lifetime;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;

namespace Unity.Registration
{
    public class TypeRegistration : InternalRegistration, IContainerRegistration
    {
        #region Constructors

        public TypeRegistration(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
            : base(typeFrom ?? typeTo, string.IsNullOrEmpty(name) ? null : name)
        {
            MappedToType = typeTo;
            LifetimeManager = lifetimeManager ?? TransientLifetimeManager.Instance;

            if (LifetimeManager.InUse) throw new InvalidOperationException(Constants.LifetimeManagerInUse);
            LifetimeManager.InUse = true;

            if (typeFrom != null && typeFrom != typeTo)
            {
                if (typeFrom.GetTypeInfo().IsGenericTypeDefinition && typeTo.GetTypeInfo().IsGenericTypeDefinition)
                {
                    Set(typeof(IBuildKeyMappingPolicy), new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeTo, name)));
                }
                else
                {
                    Set(typeof(IBuildKeyMappingPolicy), new BuildKeyMappingPolicy(new NamedTypeBuildKey(typeTo, name)));
                }

                if ((null == injectionMembers || injectionMembers.Length == 0) && !(lifetimeManager is IRequireBuildUpPolicy))
                    Set(typeof(IBuildPlanPolicy), new ResolveBuildUpPolicy());
            }
        }

        #endregion


        #region IContainerRegistration

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="InternalRegistration.RegisteredType"/> property and this one will have the same value.
        /// </summary>
        public Type MappedToType { get; }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public LifetimeManager LifetimeManager { get; }

        #endregion


        #region IPolicyMap

        public override IBuilderPolicy Get(Type policyInterface)
        {
            if (typeof(ILifetimePolicy) == policyInterface)
                return LifetimeManager;

            return base.Get(policyInterface);
        }


        #endregion
    }
}
