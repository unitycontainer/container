using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData> : BuilderStrategy
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

        protected Extension.ImportProvider<TMemberInfo, MemberDescriptor<TMemberInfo>> DescribeImport { get; set; }

        protected SelectorDelegate<InjectionMember<TMemberInfo, TData>, TMemberInfo[], int> IndexFromInjected;


        #endregion


        #region Constructors

        protected MemberStrategy(IPolicies policies)
        {
            GetDeclaredMembers = policies.Get<TMemberInfo, DeclaredMembers<TMemberInfo>>(OnMembersSelectorChanged)!;
            DescribeImport = policies.Get<Extension.ImportProvider<TMemberInfo, MemberDescriptor<TMemberInfo>>>(this.OnMemberProviderChanged)!;
            IndexFromInjected  = policies.Get<TMemberInfo, SelectorDelegate<InjectionMember<TMemberInfo, TData>, TMemberInfo[], int>>(OnSelectorChanged)!;
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void Execute(TMemberInfo info, object target, object? value) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMembersSelectorChanged(Type? target, Type type, object? policy)
            => GetDeclaredMembers = (DeclaredMembers<TMemberInfo>)(policy ?? throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMemberProviderChanged(Type? target, Type type, object? policy) 
            => DescribeImport = (Extension.ImportProvider<TMemberInfo, MemberDescriptor<TMemberInfo>>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

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
