using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract class MemberProcessor<TContext>
        where TContext : IBuilderContext
    {
        /// <summary>
        /// Activation algorithm. This method performs instantiation using <see cref="Activator"/> 
        /// without creating and compiling a pipeline
        /// </summary>
        public virtual void BuildUp(ref TContext context)
        { }

        public virtual void BuildResolver(ref TContext context)
        { }

        public virtual void BuildExpression(ref TContext context)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Call hierarchy:
        /// <see cref="GetExpressions"/>  
        /// + <see cref="SelectMembers"/>
        ///   + <see cref="ExpressionsFromSelected"/>
        ///     + <see cref="BuildMemberExpression"/>
        ///       + <see cref="GetResolverExpression"/>
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="registration"></param>
        /// <returns></returns>
        //public abstract IEnumerable<Expression> GetExpressions(Type type, IPolicySet registration);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Call hierarchy:
        /// <see cref="GetResolver"/>
        /// + <see cref="SelectMembers"/>
        ///   + <see cref="ResolversFromSelected"/>
        ///     + <see cref="BuildMemberResolver"/>
        ///       + <see cref="GetResolverDelegate"/>
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="registration"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        //public abstract ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext>? seed);
    }

    public abstract partial class MemberProcessor<TContext, TMemberInfo, TData> : MemberProcessor<TContext>
        where TContext : IBuilderContext
        where TMemberInfo : MemberInfo
        where TData       : class
    {
        #region Fields

        protected InjectionInfoProvider<InjectionInfoStruct<TMemberInfo>, TMemberInfo> ProvideInjectionInfo;
        protected Func<InjectionMember<TMemberInfo, TData>, TMemberInfo[], int> MatchMember;
        protected Func<Type, TMemberInfo[]> GetDeclaredMembers;

        #endregion


        #region Constructors

        protected MemberProcessor(IPolicies policies)
        {
            ProvideInjectionInfo = policies.Get<InjectionInfoProvider<InjectionInfoStruct<TMemberInfo>, TMemberInfo>>(OnProvideInjectionInfoChanged) 
                                 ?? throw new InvalidOperationException();
            GetDeclaredMembers   = policies.Get<Func<Type, TMemberInfo[]>>(OnGetDeclaredMembersChanged) 
                                 ?? throw new InvalidOperationException();
            MatchMember          = policies.GetOrAdd(MemberMatch, OnMatchMemberChanged);
        }

        #endregion


        #region Implementation

        protected abstract InjectionMember<TMemberInfo, TData>[]? GetInjectedMembers(RegistrationManager? manager);

        protected abstract Type GetMemberType(TMemberInfo info);

        #endregion
    }
}
