using System;
using System.Reflection;
using Unity.Builder;
using Unity.Container.Storage;
using Unity.Lifetime;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Container.Registration
{
    public class TypeRegistration : LinkedMap<Type, IBuilderPolicy>, IContainerRegistration
    {
        #region Fields

        private static readonly TransientLifetimeManager TransientLifetimeManager = new TransientLifetimeManager();

        #endregion


        #region Constructors

        public TypeRegistration(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            Name = string.IsNullOrEmpty(name) ? null : name;
            RegisteredType = typeFrom ?? typeTo;
            MappedToType = typeTo;
            LifetimeManager = lifetimeManager ?? TransientLifetimeManager;

            if (LifetimeManager.InUse) throw new InvalidOperationException(Constants.LifetimeManagerInUse);
            LifetimeManager.InUse = true;

            if (typeFrom != null && typeFrom != typeTo)
            {
                if (typeFrom.GetTypeInfo().IsGenericTypeDefinition && typeTo.GetTypeInfo().IsGenericTypeDefinition)
                {
                    this[typeof(IBuildKeyMappingPolicy)] = new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(typeTo, name));
                }
                else
                {
                    var policy = (null != injectionMembers && injectionMembers.Length > 0) || lifetimeManager is IRequireBuildUpPolicy
                        ? new BuildKeyMappingPolicy(new NamedTypeBuildKey(typeTo, name))
                        : new ResolveMappingPolicy(new NamedTypeBuildKey(typeTo, name));

                    this[typeof(IBuildKeyMappingPolicy)] = policy;
                }
            }
        }

        #endregion


        #region IContainerRegistration

        /// <summary>
        /// The type that was passed to the <see cref="IUnityContainer.RegisterType"/> method
        /// as the "from" type, or the only type if type mapping wasn't done.
        /// </summary>
        public Type RegisteredType { get; }

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="RegisteredType"/> property and this one will have the same value.
        /// </summary>
        public Type MappedToType { get; }

        /// <summary>
        /// Name the type was registered under. Null for default registration.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public LifetimeManager LifetimeManager { get; }

        #endregion


        #region PolicyRegistry

        public override IBuilderPolicy this[Type policyInterface] {
            get
            {
                if (typeof(ILifetimePolicy) == policyInterface)
                    return LifetimeManager;
                
                return base[policyInterface];
            }

            set
            {
                if (typeof(ILifetimePolicy) == policyInterface)
                    throw new InvalidOperationException("Lifetime Manager should not be reset");

                base[policyInterface] = value;
            }
        }


        #endregion
    }
}
