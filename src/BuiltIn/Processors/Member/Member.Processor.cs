using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData> : PipelineProcessor
                                                                 where TMemberInfo : MemberInfo
                                                                 where TDependency : class
    {
        #region Delegates

        /// <summary>
        /// Dependency analysis handler for <see cref="TMemberInfo"/>
        /// </summary>
        /// <typeparam name="TMemeber">Type of the member, <see cref="ConstructorInfo"/>, <see cref="FieldInfo"/>, and etc.</typeparam>
        /// <param name="memberInfo">The member to analyse</param>
        /// <param name="data">Associated data</param>
        /// <returns>Returns <see cref="DependencyInfo"/> struct containing dependency information</returns>
        public delegate DependencyInfo DependencyAnalyzer<TMemeber>(TMemeber memberInfo, object? data = null);

        #endregion


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

        /// <summary>
        /// Delegate holding dependency analizer
        /// </summary>
        protected DependencyAnalyzer<TMemberInfo> GetDependencyInfo { get; private set; }

        #endregion


        #region Constructors

        public MemberProcessor(Defaults defaults)
        {
            BindingFlags = defaults
                .GetOrAdd(typeof(TMemberInfo), DefaultBindingFlags, (object flags) => BindingFlags = (BindingFlags)flags);

            GetDependencyInfo = defaults
                .GetOrAdd<DependencyAnalyzer<TMemberInfo>>(OnGetDependencyInfo, 
                    (object handler) => GetDependencyInfo = (DependencyAnalyzer<TMemberInfo>)handler);
        }

        #endregion


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
        protected abstract TMemberInfo[] GetMembers(Type type);



        #region Dependency Management

        public virtual DependencyInfo OnGetDependencyInfo(TMemberInfo memberInfo, object? data)
        {
            return default;
        }


        #endregion


        #region Selection

        public virtual object Select(ref PipelineContext context)
        {
            throw new NotImplementedException();
            //
            //HashSet<object> memberSet = new HashSet<object>();

            //// Select Injected Members
            //if (null != builder.InjectionMembers)
            //{
            //    foreach (var injectionMember in builder.InjectionMembers)
            //    {
            //        if (injectionMember is InjectionMember<TMemberInfo, TData>)
            //            memberSet.Add(injectionMember);
            //    }
            //}

            //// Select Attributed members
            //foreach (var member in DeclaredMembers(builder.Type))
            //{
            //    if (member.IsDefined(typeof(DependencyResolutionAttribute), true))
            //        memberSet.Add(member);
            //}

            //return memberSet;
        }

        #endregion


        #region Implementation

        /// <summary>
        /// Returns attribute associated with dependency info
        /// </summary>
        /// <param name="info"><see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/> member</param>
        /// <returns>Attached attribute or null if nothing found</returns>
        protected abstract ImportAttribute? GetImportAttribute(TDependency info);

        /// <summary>
        /// Returns <see cref="Type"/> of the dependency from <see cref="ParameterInfo"/>, 
        /// <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/>
        /// </summary>
        /// <param name="info"><see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/> instance</param>
        /// <returns>Dependency type</returns>
        protected abstract Type DependencyType(TDependency info);


        protected virtual Type MemberType(TMemberInfo info) => throw new NotImplementedException();







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

        #endregion
    }
}
