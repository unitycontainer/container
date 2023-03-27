using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public partial class ConstructorStrategy<TContext> : ParameterStrategy<TContext, ConstructorInfo>
        where TContext : IBuilderContext
    {
        #region Fields

        protected MemberSelector<TContext, ConstructorInfo> SelectAlgorithmically;

        #endregion


        #region Constructors

        public ConstructorStrategy(IPolicies policies)
            : base(policies)
        {
            SelectAlgorithmically = policies.Get<MemberSelector<TContext, ConstructorInfo>>(OnAlgorithmChanged)
                ?? throw new InvalidOperationException("Constructor selector is not initialized");
        }

        #endregion


        #region Policy Changes

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnAlgorithmChanged(Type? target, Type type, object? policy) 
            => SelectAlgorithmically = (MemberSelector<TContext, ConstructorInfo>)(policy ?? throw new ArgumentNullException(nameof(policy)));

        protected override InjectionMember<ConstructorInfo, object[]>[]? GetInjectedMembers(RegistrationManager? manager)
            => manager?.Constructors;

        #endregion
    }
}
