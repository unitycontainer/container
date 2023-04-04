using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Processors
{
    public abstract class MemberProcessor
    {
        /// <summary>
        /// Activation algorithm. This method performs instantiation using <see cref="Activator"/> 
        /// without creating and compiling a pipeline
        /// </summary>
        public virtual void BuildUp<TContext>(ref TContext context)
            where TContext : IBuilderContext    
        { }

        public virtual void BuildResolver<TContext>(ref TContext context)
            where TContext : IBuildPlanContext<BuilderStrategyPipeline>
        { }

        public virtual void BuildExpression<TContext>(ref TContext context)
            where TContext : IBuildPlanContext<IEnumerable<Expression>>
        { }
    }

    public abstract partial class MemberProcessor<TMemberInfo, TData> : MemberProcessor
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
