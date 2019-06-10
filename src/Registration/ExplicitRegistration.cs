using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Registration
{
    [DebuggerDisplay("Registration.Explicit({Count})")]
    //[DebuggerTypeProxy(typeof(ExplicitRegistrationDebugProxy))]
    public class ExplicitRegistration : ImplicitRegistration
    {
        #region Constructors

        public ExplicitRegistration(UnityContainer owner, string? name, Type? type)
            : base(owner, name)
        {
            Next = null;
            Type = type;
            InjectionMembers = null;
            BuildRequired = false;
        }

        public ExplicitRegistration(UnityContainer owner, string? name, Type? type, LifetimeManager lifetimeManager)
            : base(owner, name, lifetimeManager)
        {
            Next = null;
            Type = type;
            InjectionMembers = null;
            BuildRequired = null != InjectionMembers && InjectionMembers.Any(m => m.BuildRequired);

            if (lifetimeManager is ContainerControlledLifetimeManager manager)
                manager.Scope = owner.Context;
        }

        public ExplicitRegistration(UnityContainer owner, string? name, Type? type, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
            : base(owner, name, lifetimeManager)
        {
            Type = type;
            Next = owner.Defaults;
            InjectionMembers = null != injectionMembers && 0 < injectionMembers.Length ? injectionMembers : null;
            BuildRequired = null != InjectionMembers && InjectionMembers.Any(m => m.BuildRequired) || lifetimeManager is PerResolveLifetimeManager;
            BuildType = GetTypeConverter();

            if (lifetimeManager is ContainerControlledLifetimeManager manager)
                manager.Scope = owner.Context;
        }

        #endregion


        #region Public Members

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="Type"/> property and this one will have the same value.
        /// </summary>
        public Type? Type { get; }

        public override bool BuildRequired { get; }

        public override Converter<Type, Type>? BuildType { get; }

        public virtual ResolveDelegate<PipelineContext>? Pipeline { get; internal set; }

        #endregion


        #region Implementation

        private Converter<Type, Type>? GetTypeConverter()
        {
            // Set mapping policy
#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (null != Type && Type.GetTypeInfo().IsGenericTypeDefinition)
#else
            if (null != Type && Type.IsGenericTypeDefinition)
#endif
            {
                var buildType = Type;
                return (Type t) =>
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0 || NET40
                    var targetTypeInfo = t.GetTypeInfo();
#else
                    var targetTypeInfo = t;
#endif
                    if (targetTypeInfo.IsGenericTypeDefinition)
                    {
                        // No need to perform a mapping - the source type is an open generic
                        return buildType;
                    }

                    if (targetTypeInfo.GenericTypeArguments.Length != buildType.GetTypeInfo().GenericTypeParameters.Length)
                        throw new ArgumentException("Invalid number of generic arguments in types: {registration.MappedToType} and {t}");

                    try
                    {
                        return buildType.MakeGenericType(targetTypeInfo.GenericTypeArguments);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new MakeGenericTypeFailedException(ex);
                    }
                };
            }

            return null;
        }

        #endregion


        #region Debug Support

        protected class ExplicitRegistrationDebugProxy : ImplicitRegistrationDebugProxy
        {
            private readonly ExplicitRegistration _registration;

            public ExplicitRegistrationDebugProxy(ExplicitRegistration set)
                : base(set)
            {
                _registration = set;
            }

            public Type? Type => _registration.Type;
        }

        #endregion
    }
}
