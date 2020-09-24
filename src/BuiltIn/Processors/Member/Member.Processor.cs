using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TData> : PipelineProcessor
                                                    where TMemberInfo : MemberInfo
    {
        #region Delegates

        public delegate SelectionInfo<TMemberInfo, TData> SelectionInfoFromMemberInfo(TMemberInfo member);

        #endregion


        #region Fields

        /// <summary>
        /// Combination of <see cref="BindingFlags"/> to use when getting declared members
        /// </summary>
        protected BindingFlags BindingFlags { get; private set; }

        /// <summary>
        /// Function that selects <see cref="MemberInfo"/> and associated data from annotation
        /// </summary>
        protected SelectionInfoFromMemberInfo FromAnnotation { get; set; }

        #endregion


        #region Constructors

        public MemberProcessor(Defaults defaults)
        {
            // Add BindingFlags to default policies and subscribe to notifications
            var flags = defaults.Get(typeof(TMemberInfo), typeof(BindingFlags));
            if (null == flags)
            {
                BindingFlags = BindingFlags.Public | BindingFlags.Instance;
                defaults.Set(typeof(TMemberInfo), typeof(BindingFlags), BindingFlags, OnBindingFlagsChanged);
            }
            else
            {
                BindingFlags = (BindingFlags)flags;
            }

            // TODO: FromAnnotation
            FromAnnotation = FromAnnotationSelector;
        }

        #endregion




        // TODO: Optimization is required
        protected virtual SelectionInfo<TMemberInfo, TData> FromAnnotationSelector(TMemberInfo member) => default;

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



        #region Implementation

        private void OnBindingFlagsChanged(object policy) => BindingFlags = (BindingFlags)policy;


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

        protected virtual Type MemberType(TMemberInfo info) => throw new NotImplementedException();

        protected abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

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
