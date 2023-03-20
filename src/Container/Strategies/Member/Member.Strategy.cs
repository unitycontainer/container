using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData> : BuilderStrategy, 
                                                                                    IInjectionProvider<TMemberInfo>
                                                                where TMemberInfo : MemberInfo
                                                                where TDependency : class
                                                                where TData       : class
    {
        #region Fields

        private static IEnumerable<InjectionMember<TMemberInfo, TData>>? _empty;

        #endregion


        #region Constructors

        protected MemberStrategy(IPolicies policies)
        {
            GetDeclaredMembers = policies.Get<DeclaredMembers<TMemberInfo>>(OnMembersSelectorChanged)!;
            ImportProvider = policies.CompareExchange<IInjectionProvider<TMemberInfo>>(this, null, OnProviderChnaged) ?? this;
        }

        #endregion


        #region Properties

        protected DeclaredMembers<TMemberInfo> GetDeclaredMembers { get; private set; }

        protected IInjectionProvider<TMemberInfo> ImportProvider { get; private set; }

        #endregion


        #region Implementation

        protected abstract void Execute<TContext, TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ImportData data)
            where TContext : IBuilderContext where TDescriptor : IInjectionInfo<TMemberInfo>;

        public abstract void ProvideInfo<TDescriptor>(ref TDescriptor descriptor)
            where TDescriptor : IInjectionInfo<TMemberInfo>;

        void IInjectionProvider.ProvideInfo<TDescriptor>(ref TDescriptor descriptor)
            => throw new NotImplementedException();

        protected abstract InjectionMember<TMemberInfo, TData>[]? GetInjectedMembers(RegistrationManager? manager);

        #endregion


        #region Change Notifications 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnProviderChnaged(Type? target, Type type, object? policy)
            => ImportProvider = (IInjectionProvider<TMemberInfo>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMembersSelectorChanged(Type? target, Type type, object? policy)
            => GetDeclaredMembers = (DeclaredMembers<TMemberInfo>)(policy ?? throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetDefaultValue(Type t)
            => (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                ? Activator.CreateInstance(t) : null;

        #endregion
    }
}
