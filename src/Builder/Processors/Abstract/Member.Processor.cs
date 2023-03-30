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

    public abstract partial class MemberProcessor<TContext, TMemberInfo, TDependency, TData> : MemberProcessor<TContext>
        where TContext : IBuilderContext
        where TMemberInfo : MemberInfo
        where TDependency : class
        where TData       : class
    {
        #region Fields

        protected InjectionInfoProvider<InjectionInfoStruct<TMemberInfo>, TMemberInfo> ProvideInjectionInfo;
        protected Func<Type, TMemberInfo[]> GetDeclaredMembers;

        #endregion


        #region Constructors

        protected MemberProcessor(IPolicies policies)
        {
            ProvideInjectionInfo = policies.GetOrAdd<InjectionInfoProvider<InjectionInfoStruct<TMemberInfo>, TMemberInfo>>(InjectionInfoProvider, OnProvideInjectionInfoChanged);
            GetDeclaredMembers   = policies.Get<Func<Type, TMemberInfo[]>>(OnGetDeclaredMembersChanged) ?? throw new InvalidOperationException();
        }

        #endregion


        #region Implementation

        protected abstract void Execute<TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ValueData data)
            where TDescriptor : IInjectionInfo<TMemberInfo>;

        protected abstract InjectionMember<TMemberInfo, TData>[]? GetInjectedMembers(RegistrationManager? manager);

        protected abstract void InjectionInfoProvider<TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IInjectionInfo<TMemberInfo>;

        protected abstract Type GetMemberType(TMemberInfo info);

        #endregion


        #region Change Notifications 

        private void OnGetDeclaredMembersChanged(Type? target, Type type, object? policy)
            => GetDeclaredMembers = (Func<Type, TMemberInfo[]>)(policy 
            ?? throw new ArgumentNullException(nameof(policy)));

        private void OnProvideInjectionInfoChanged(Type? target, Type type, object? policy)
            => ProvideInjectionInfo = (InjectionInfoProvider<InjectionInfoStruct<TMemberInfo>, TMemberInfo>)(policy 
            ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
