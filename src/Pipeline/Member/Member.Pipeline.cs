using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;
using Unity.Resolution;

namespace Unity
{
    public abstract class MemberPipeline : Pipeline
    {
        #region Fields

        private UnityContainer _container;

        #endregion


        #region Constructor

        public MemberPipeline(UnityContainer container)
        {
            _container = container;
        }

        #endregion
    }

    public abstract partial class MemberPipeline<TMemberInfo, TData> : MemberPipeline
                                                   where TMemberInfo : MemberInfo
    {
        #region Constructors

        protected MemberPipeline(UnityContainer container)
            : base(container)
        {
        }

        #endregion


        #region Selection

        public virtual object Select(Type type, InjectionMember[]? injectionMembers)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != injectionMembers)
            {
                foreach (var injectionMember in injectionMembers)
                {
                    if (injectionMember is InjectionMember<TMemberInfo, TData>)
                        memberSet.Add(injectionMember);
                }
            }

            // Select Attributed members
            foreach (var member in DeclaredMembers(type))
            {
                if (member.IsDefined(typeof(DependencyResolutionAttribute), true))
                    memberSet.Add(member);
            }

            return memberSet;
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
