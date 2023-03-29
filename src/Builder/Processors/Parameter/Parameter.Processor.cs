using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Dependency;
using Unity.Extension;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public abstract partial class ParameterProcessor<TContext, TMemberInfo> : MemberProcessor<TContext, TMemberInfo, ParameterInfo, object[]>
        where TContext    : IBuilderContext
        where TMemberInfo : MethodBase
    {
        #region Constants

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected static readonly object?[] EmptyParametersArray = new object?[0];

        #endregion


        #region Fields
        
        protected Comparison<object[]?, MethodBase, int> MatchTo;
        
        // TODO: Provider optimization
        protected IInjectionInfoProvider<ParameterInfo> ParameterProvider;

        #endregion


        #region Constructors

        /// <inheritdoc/>
        public ParameterProcessor(IPolicies policies)
            : base(policies)
        {
            MatchTo               = policies.GetOrAdd<Comparison<object[]?, MethodBase, int>>(Matching.MatchData, OnMatchToChanged);
            ParameterProvider     = policies.GetOrAdd<IInjectionInfoProvider<ParameterInfo>>(this, OnProviderChanged);
        }

        #endregion


        #region Implementation

        protected override void Execute<TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ImportData data)
            => descriptor.MemberInfo.Invoke(context.Existing, (object[]?)data.Value);

        #endregion


        #region Policy Changes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMatchToChanged(Type? target, Type type, object? policy) 
            => MatchTo = (Comparison<object[]?, MethodBase, int>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnProviderChanged(Type? target, Type type, object? policy)
            => ParameterProvider = (IInjectionInfoProvider<ParameterInfo>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
