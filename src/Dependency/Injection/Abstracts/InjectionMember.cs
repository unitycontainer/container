using System;
using System.Collections.Generic;
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
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="registeredType">Type of interface being registered. If no interface,
        /// this will be null.</param>
        /// <param name="mappedToType">Type of concrete type being registered.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public virtual void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name, ref TPolicyList policies)
                where TContext : IResolveContext
                where TPolicyList : IPolicyList
        {
        }

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
        public virtual bool BuildRequired => false;
    }


    public abstract class InjectionMember<TMemberInfo, TData> : InjectionMember, 
                                                                ISelect<TMemberInfo, TData>,
                                                                IEquatable<TMemberInfo>
                                            where TMemberInfo : MemberInfo
    {
        #region Constructors

        protected InjectionMember(TData data)
        {
            Data = data;
        }

        #endregion


        #region Properties

        protected TMemberInfo MemberInfo { get; set; }

        protected TData Data { get; set; }

        #endregion


        #region Methods

        protected abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

        protected virtual bool MemberInfoMatch(TMemberInfo info, TData data) => true;

        protected virtual void OnValidate(Type type)
        {
            if (null != MemberInfo) return;

            // TODO: 5.9.0 Implement correct error message
            var signature = "xxx";//string.Join(", ", _arguments?.Select(t => t.Name) ?? );
            var message = $"The type {type.FullName} does not have a {typeof(TMemberInfo).Name} that takes these parameters ({signature}).";
            throw new InvalidOperationException(message);
        }

        #endregion


        #region Interfaces

        public virtual (TMemberInfo, TData) Select(Type type)
        {
            return (MemberInfo, Data);
        }

        public virtual bool Equals(TMemberInfo other)
        {
#if NETSTANDARD1_0
            return MemberInfo?.Equals(other) ?? false;
#else
            return other?.MetadataToken == MemberInfo.MetadataToken;
#endif
        }

        #endregion


        #region Implementation

        public override void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name, ref TPolicyList policies)
        {
            foreach (var member in DeclaredMembers(mappedToType))
            {
                if (!MemberInfoMatch(member, Data))
                    continue;

                if (null != MemberInfo)
                {
                    // TODO: 5.9.0 Proper error message
                    var signature = "xxx";//string.Join(", ", _arguments?.Select(t => t.Name) ?? );
                    var message = $"The type {mappedToType.FullName} does not have a {typeof(TMemberInfo).Name} that takes these parameters ({signature}).";
                    throw new InvalidOperationException(message);
                }

                MemberInfo = member;
            }

            OnValidate(mappedToType);
        }

        #endregion
    }
}
