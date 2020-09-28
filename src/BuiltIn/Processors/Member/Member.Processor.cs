using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData> : PipelineProcessor
                                                                 where TMemberInfo : MemberInfo
                                                                 where TDependency : class
    {
        #region Constants

        /// <summary>
        /// Binding flags used to obtain declared members by default
        /// </summary>
        public const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        #endregion


        #region Fields

        /// <summary>
        /// Combination of <see cref="BindingFlags"/> to use when getting declared members
        /// </summary>
        protected BindingFlags BindingFlags { get; private set; }

        #endregion


        #region Constructors

        public MemberProcessor(Defaults defaults)
        {
            BindingFlags = defaults
                .GetOrAdd(typeof(TMemberInfo), DefaultBindingFlags, 
                    (object flags) => BindingFlags = (BindingFlags)flags);
        }

        #endregion


        #region Implementation

        /// <summary>
        /// This method returns an array of <see cref="MemberInfo"/> objects implemented
        /// by the <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// Each processor overrides this method and returns appropriate members. 
        /// Constructor processor returns an array of <see cref="ConstructorInfo"/> objects,
        /// Property processor returns objects of type <see cref="PropertyInfo"/>, and etc.
        /// </remarks>
        /// <param name="type"><see cref="Type"/> implementing members</param>
        /// <returns>A <see cref="Span{MemberInfo}"/> of appropriate <see cref="MemberInfo"/> objects</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract TMemberInfo[] GetMembers(Type type);

        /// <summary>
        /// Returns type of the member
        /// </summary>
        /// <param name="info"><see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/> instance</param>
        /// <returns>Type of the member</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract Type MemberType(TMemberInfo info);

        /// <summary>
        /// Returns <see cref="Type"/> of the dependency from <see cref="ParameterInfo"/>, 
        /// <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/>
        /// </summary>
        /// <param name="info">A <see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/> instance</param>
        /// <returns>Dependency type</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract Type DependencyType(TDependency info);

        /// <summary>
        /// Returns attribute associated with dependency info
        /// </summary>
        /// <param name="info"><see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/> member</param>
        /// <returns>Attached attribute or null if nothing found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract ImportAttribute? GetImportAttribute(TDependency info);

        #endregion



        protected virtual ResolveDelegate<PipelineContext>? PreProcessResolver(TMemberInfo info, DependencyResolutionAttribute attribute, object? data) => data switch
        {
            IResolve policy                                 => policy.Resolve,
            IResolverFactory<TMemberInfo> fieldFactory      => fieldFactory.GetResolver<PipelineContext>(info),
            IResolverFactory<Type> typeFactory              => typeFactory.GetResolver<PipelineContext>(MemberType(info)),
            Type type when typeof(Type) != MemberType(info) => attribute.GetResolver<PipelineContext>(type),
            _                                               => null
        };

        // TODO: Remove
        protected object? PreProcessResolver(TMemberInfo info, object? resolver)
        {
            switch (resolver)
            {
                case IResolve policy:
                    return (ResolveDelegate<PipelineContext>)policy.Resolve;

                case IResolverFactory<Type> factory:
                    return factory.GetResolver<PipelineContext>(MemberType(info));

                case Type type:
                    return typeof(Type) == MemberType(info)
                        ? type : (object)info;
            }

            return resolver;
        }

    }
}
