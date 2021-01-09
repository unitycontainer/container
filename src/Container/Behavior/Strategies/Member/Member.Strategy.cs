using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

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

        protected ImportDescriptionProvider<TDependency, ImportInfo<TDependency>> DescribeImport { get; set; }

        #endregion


        #region Constructors

        protected MemberStrategy(IPolicies policies)
        {
            GetDeclaredMembers = policies.Get<TMemberInfo, DeclaredMembers<TMemberInfo>>(OnMembersSelectorChanged)!;
            DescribeImport = policies.Get<ImportDescriptionProvider<TDependency, ImportInfo<TDependency>>>(OnImportProviderChanged)!;
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SetValue(TDependency info, object target, object? value) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMembersSelectorChanged(Type? target, Type type, object? policy)
            => GetDeclaredMembers = (DeclaredMembers<TMemberInfo>)(policy ?? throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnImportProviderChanged(Type? target, Type type, object? policy) 
            => DescribeImport = (ImportDescriptionProvider<TDependency, ImportInfo<TDependency>>)(policy 
            ?? throw new ArgumentNullException(nameof(policy)));

        protected virtual ImportData GetDefault(ref ImportInfo<TDependency> import) 
            => import.DefaultData.IsValue
            ? new ImportData(import.DefaultData.Value, ImportType.Value)
            : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetDefaultValue(Type t)
            => (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                ? Activator.CreateInstance(t) : null;

        #endregion
    }
}
