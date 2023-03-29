using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Policy;

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

    public abstract partial class MemberProcessor<TContext, TMemberInfo, TDependency, TData> : MemberProcessor<TContext>,
                                                                                     // TODO: Might be unnecessary
                                                                                     IInjectionInfoProvider<TMemberInfo>
        where TContext : IBuilderContext
        where TMemberInfo : MemberInfo
        where TDependency : class
        where TData       : class
    {
        #region Constants

        private static IEnumerable<InjectionMember<TMemberInfo, TData>>? _empty;

        #endregion


        #region Fields

        protected Func<Type, TMemberInfo[]> GetDeclaredMembers;

        #endregion


        #region Constructors

        protected MemberProcessor(IPolicies policies)
        {
            GetDeclaredMembers = policies.Get<Func<Type, TMemberInfo[]>>(OnGetDeclaredMembersChanged) 
                ?? throw new InvalidOperationException();

            var type = typeof(MemberSelector<IBuilderContext, TMemberInfo>);
            ImportProvider = policies.CompareExchange<IInjectionInfoProvider<TMemberInfo>>(this, null, OnProviderChanged) ?? this;
        }

        #endregion


        #region Properties


        protected IInjectionInfoProvider<TMemberInfo> ImportProvider { get; private set; }

        #endregion


        #region Implementation

        protected abstract void Execute<TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ImportData data)
            where TDescriptor : IInjectionInfo<TMemberInfo>;

        public abstract void ProvideInfo<TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IInjectionInfo<TMemberInfo>;

        void IInjectionInfoProvider.ProvideInfo<TDescriptor>(ref TDescriptor descriptor)
            => throw new NotImplementedException();

        protected abstract InjectionMember<TMemberInfo, TData>[]? GetInjectedMembers(RegistrationManager? manager);

        #endregion


        #region Change Notifications 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnProviderChanged(Type? target, Type type, object? policy)
            => ImportProvider = (IInjectionInfoProvider<TMemberInfo>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnGetDeclaredMembersChanged(Type? target, Type type, object? policy)
            => GetDeclaredMembers = (Func<Type, TMemberInfo[]>)(policy ?? throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetDefaultValue(Type t)
            => (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                ? Activator.CreateInstance(t) : null;

        #endregion
    }
}
