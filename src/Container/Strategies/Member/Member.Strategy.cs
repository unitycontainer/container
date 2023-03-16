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
                                                                                    IImportProvider<TMemberInfo>
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
            ImportProvider = policies.CompareExchange<IImportProvider<TMemberInfo>>(this, null, OnProviderChnaged) ?? this;
        }

        #endregion


        #region Properties

        protected DeclaredMembers<TMemberInfo> GetDeclaredMembers { get; private set; }

        protected IImportProvider<TMemberInfo> ImportProvider { get; private set; }

        #endregion


        #region Implementation

        protected abstract void Execute<TContext, TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ImportData data)
            where TContext : IBuilderContext where TDescriptor : IImportDescriptor<TMemberInfo>;

        public abstract void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            where TContext : IBuilderContext where TDescriptor : IImportDescriptor<TMemberInfo>;

        void IImportProvider.ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            => throw new NotImplementedException();

        protected abstract InjectionMember<TMemberInfo, TData>[]? GetInjectedMembers(RegistrationManager? manager);

        #endregion


        #region Change Notifications 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnProviderChnaged(Type? target, Type type, object? policy)
            => ImportProvider = (IImportProvider<TMemberInfo>)(policy
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
