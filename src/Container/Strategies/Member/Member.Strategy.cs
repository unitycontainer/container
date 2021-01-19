using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData> : BuilderStrategy, 
                                                                                    IImportProvider<TMemberInfo>
                                                                where TMemberInfo : MemberInfo
                                                                where TDependency : class
                                                                where TData       : class
    {
        #region Fields

        /// <summary>
        /// This method returns an array of <see cref="MemberInfo"/> objects implemented
        /// by the <see cref="Type"/>
        /// </summary>
        protected DeclaredMembers<TMemberInfo> GetDeclaredMembers;

        protected IImportProvider<TMemberInfo> MemberProvider { get; private set; }

        protected SelectorDelegate<InjectionMember<TMemberInfo, TData>, TMemberInfo[], int> IndexFromInjected;


        #endregion


        #region Constructors

        protected MemberStrategy(IPolicies policies)
        {
            GetDeclaredMembers = policies.Get<TMemberInfo, DeclaredMembers<TMemberInfo>>(OnMembersSelectorChanged)!;
            MemberProvider   = policies.CompareExchange<IImportProvider<TMemberInfo>>(this, null, OnProviderChnaged) ?? this;

            IndexFromInjected  = policies.Get<TMemberInfo, SelectorDelegate<InjectionMember<TMemberInfo, TData>, TMemberInfo[], int>>(OnSelectorChanged)!;
        }

        #endregion


        #region Import

        public abstract void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            where TContext : IBuilderContext where TDescriptor : IImportDescriptor<TMemberInfo>;

        void IImportProvider.ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
            => throw new NotImplementedException();

        #endregion




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void Execute(TMemberInfo info, object target, object? value) 
            => throw new NotImplementedException();




        #region Change Notifications 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnProviderChnaged(Type? target, Type type, object? policy)
            => MemberProvider = (IImportProvider<TMemberInfo>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMembersSelectorChanged(Type? target, Type type, object? policy)
            => GetDeclaredMembers = (DeclaredMembers<TMemberInfo>)(policy ?? throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSelectorChanged(Type? target, Type type, object? policy) 
            => IndexFromInjected = (SelectorDelegate<InjectionMember<TMemberInfo, TData>, TMemberInfo[], int>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetDefaultValue(Type t)
            => (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                ? Activator.CreateInstance(t) : null;

        #endregion
    }
}
