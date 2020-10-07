using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.Injection
{
    public abstract class InjectionMemberInfo<TMemberInfo> : InjectionMember<TMemberInfo, object>, IResolve
                                         where TMemberInfo : MemberInfo
    {
        #region Constants

        protected static ResolveDelegate<PipelineContext> DefaultResolver
            = (ref PipelineContext context) => context.Resolve();

        #endregion


        #region Fields

        private readonly Type? _type;
        private readonly string? _name;

        #endregion


        #region Constructors

        protected InjectionMemberInfo(string member)
            : base(member, DefaultResolver)
        {
        }


        protected InjectionMemberInfo(string member, Type type)
            : base(member, DefaultResolver)
        {
            _type = type;
        }


        protected InjectionMemberInfo(string member, Type type, string? name)
            : base(member, DefaultResolver)
        {
            _type = type;
            _name = name;
        }


        protected InjectionMemberInfo(string member, object data)
            : base(member, data)
        {
        }

        #endregion


        #region Implementation

        public virtual object? Resolve<TContext>(ref TContext context) where TContext : IResolveContext
            => context.Resolve(_type ?? context.Type, _name);

        #endregion
    }
}
