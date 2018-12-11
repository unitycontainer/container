using System;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Injection
{
    /// <summary>
    /// Base class for objects that can be used to configure what
    /// class members get injected by the container.
    /// </summary>
    public abstract class InjectionMember : IEquatable<Type>
    {
        #region Constructors

        protected InjectionMember()
        {
                
        }

        protected InjectionMember(Type targetType)
        {
            TargetType = targetType;
        }

        #endregion


        #region Targeted Type 

        protected Type TargetType { get; private set; }

        public virtual InjectionMember OnType<T>() => OnType(typeof(T));

        public virtual InjectionMember OnType(Type targetType)
        {
            TargetType = targetType;
            return this;
        }

        #endregion


        #region IEquatable<Type>

        public virtual bool Equals(Type other) => null == TargetType || other == TargetType;

        #endregion


        #region

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
            public virtual bool BuildRequired { get; }

        #endregion
    }
}
