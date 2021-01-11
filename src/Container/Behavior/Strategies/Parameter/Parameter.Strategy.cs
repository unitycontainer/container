using System;
using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo> : MemberStrategy<TMemberInfo, ParameterInfo, object[]>
                                               where TMemberInfo : MethodBase
    {
        #region Fields

        /// <summary>
        /// Global singleton containing empty parameter array
        /// </summary>
        protected static object?[] EmptyParametersArray = new object?[0];
        protected ImportDescriptionProvider<ParameterInfo, ImportDescriptor<ParameterInfo>> DescribeParameter { get; set; }

        #endregion


        #region Constructors

        /// <inheritdoc/>
        public ParameterStrategy(IPolicies policies)
            : base(policies)
        {
            DescribeParameter = policies.Get<ImportDescriptionProvider<ParameterInfo, ImportDescriptor<ParameterInfo>>>(OnParameterProviderChanged)!;
        }

        #endregion


        #region Implementation

        private void OnParameterProviderChanged(Type? target, Type type, object? policy)
            => DescribeParameter = (ImportDescriptionProvider<ParameterInfo, ImportDescriptor<ParameterInfo>>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
