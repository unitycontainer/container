using System.Diagnostics;
using System.Reflection;
using Unity.Builder;
using Unity.Dependency;
using Unity.Extension;
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
        protected ParameterInfoProvider ProvideParameterInfo;

        #endregion


        #region Constructors

        /// <inheritdoc/>
        public ParameterProcessor(IPolicies policies)
            : base(policies)
        {
            MatchTo              = policies.GetOrAdd<Comparison<object[]?, MethodBase, int>>(Matching.MatchData, OnMatchToChanged);
            ProvideParameterInfo = policies.GetOrAdd<ParameterInfoProvider>(ParameterInfoProvider, OnProvideParameterInfoChanged);
        }

        #endregion


        #region Implementation

        protected override void Execute<TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ImportData data)
            => descriptor.MemberInfo.Invoke(context.Existing, (object[]?)data.Value);

        private static object? GetDefaultValue(Type t)
            => (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                ? Activator.CreateInstance(t) : null;

        #endregion


        #region Policy Changes

        private void OnMatchToChanged(Type? target, Type type, object? policy) 
            => MatchTo = (Comparison<object[]?, MethodBase, int>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        private void OnProvideParameterInfoChanged(Type? target, Type type, object? policy)
            => ProvideParameterInfo = (ParameterInfoProvider)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
