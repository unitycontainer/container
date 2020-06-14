using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// Base class for objects that can be used to configure what
    /// class members get injected by the container.
    /// </summary>
    public abstract class InjectionMember
    {
        /// <summary>
        /// This method performs diagnostic validation of provided <see cref="Type"/>.
        /// It evaluates if this member could be matched with appropriate <see cref="MemberInfo"/>
        /// of the <see cref="Type"/>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to validate this member with</param>
        /// <exception cref="ArgumentException">Thrown if member does not
        /// match with provided <see cref="Type"/></exception>
        public abstract void Validate(Type type);

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="registeredType">Type of interface being registered. If no interface,
        /// this will be null.</param>
        /// <param name="mappedToType">Type of concrete type being registered.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public abstract void AddPolicies<TContext, TPolicySet>(Type registeredType, Type mappedToType, string? name, ref TPolicySet policies)
                where TContext : IResolveContext
                where TPolicySet : IPolicySet;

        /// <summary>
        /// This injection member instructs engine, when type mapping is present, 
        /// to build type instead of resolving it
        /// </summary>
        /// <remarks>
        /// When types registered like this:
        /// 
        /// Line 1: container.RegisterType{OtherService}(new ContainerControlledLifetimeManager());  
        /// Line 2: container.RegisterType{IService, OtherService}();
        /// Line 3: container.RegisterType{IOtherService, OtherService}(new InjectionConstructor(container));
        /// 
        /// It is expected that IService resolves instance registered on line 1. But when IOtherService is resolved 
        /// it requires different constructor so it should be built instead.
        /// </remarks>
        public abstract bool BuildRequired { get; }

        /// <summary>
        /// Debugger Display support
        /// </summary>
        /// <param name="debug">Indicates if member is rendered in Debug mode</param>
        /// <returns>String representation on the member</returns>
        protected abstract string ToString(bool debug = false);
    }

    [DebuggerDisplay("{ToString(true)}")]
    public abstract class InjectionMember<TMemberInfo, TData> : InjectionMember,
                                                                IEquatable<TMemberInfo>
                                            where TMemberInfo : MemberInfo
    {
        #region Fields

        protected const string NoMatchFound = "No member matching data has been found.";

        protected TMemberInfo? Selection { get; set; }

        protected TMemberInfo? Info { get; set; }

        #endregion


        #region Constructors

        protected InjectionMember(string name, TData data)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Data = data;
        }

        protected InjectionMember(TMemberInfo info, TData data)
        {
            Selection = info;

            Info = info ?? throw new ArgumentNullException(nameof(info));
            Name = info.Name;
            Data = data;
        }

        #endregion


        #region Public Members

        public string? Name { get; }

        public virtual TData Data { get; }

        public abstract TMemberInfo MemberInfo(Type type);

        public abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

        // TODO: Redundant
        public bool IsInitialized => null != Selection;

        #endregion


        #region Equatable

        public virtual bool Equals(TMemberInfo? other)
        {
            return Selection?.Equals(other) ?? false;
        }

        public override bool Equals(object? obj)
        {
            switch (obj)
            {
                case TMemberInfo info:
                    return Equals(info);

                case IEquatable<TMemberInfo> equatable:
                    return equatable.Equals(Selection!);

                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return Selection?.GetHashCode() ?? 0;
        }

        #endregion


        #region Overrides

        public override void Validate(Type type) { }

        public override bool BuildRequired => true;

        public override void AddPolicies<TContext, TPolicySet>(Type registeredType, Type mappedToType, string? name, ref TPolicySet policies)
        {
            var select = policies.Get<Func<Type, InjectionMember, TMemberInfo>>() 
                      ?? SelectMember;

            Selection = select(mappedToType, this);
        }

        public override string ToString() => ToString(false);

        #endregion


        #region Implementation

        protected abstract TMemberInfo SelectMember(Type type, InjectionMember member);

        protected static bool MatchesType(Type type, Type match)
        {
            if (null == type) return true;

            if (match.IsAssignableFrom(type))
                return true;

            if (typeof(Array) == type && match.IsArray)
                return true;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var typeInfo = type.GetTypeInfo();
            var matchInfo = match.GetTypeInfo();

            if (typeInfo.IsGenericType && typeInfo.IsGenericTypeDefinition && matchInfo.IsGenericType &&
                typeInfo.GetGenericTypeDefinition() == matchInfo.GetGenericTypeDefinition())
                return true;
#else
            if (type.IsGenericType && type.IsGenericTypeDefinition && match.IsGenericType &&
                type.GetGenericTypeDefinition() == match.GetGenericTypeDefinition())
                return true;
#endif
            return false;
        }

        protected static bool MatchesObject(object data, Type match)
        {
            var type = data is Type ? typeof(Type) : data?.GetType();

            if (null == type) return true;

            return match.IsAssignableFrom(type);
        }


        #endregion
    }
}
