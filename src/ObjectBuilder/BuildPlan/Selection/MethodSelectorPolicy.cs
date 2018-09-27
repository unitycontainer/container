

using System;
using System.Reflection;
using Unity.Policy;
using Unity.ResolverPolicy;

namespace Unity.ObjectBuilder.BuildPlan.Selection
{
    /// <summary>
    /// An implementation of <see cref="IMethodSelectorPolicy"/> that selects
    /// methods by looking for the given <typeparamref name="TMarkerAttribute"/>
    /// attribute on those methods.
    /// </summary>
    /// <typeparam name="TMarkerAttribute">Type of attribute used to mark methods
    /// to inject.</typeparam>
    public class MethodSelectorPolicy<TMarkerAttribute> : MethodSelectorPolicyBase<TMarkerAttribute>
        where TMarkerAttribute : Attribute
    {
        /// <summary>
        /// Create a <see cref="IResolverPolicy"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameter">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected override IResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            return new FixedTypeResolverPolicy((parameter ?? throw new ArgumentNullException(nameof(parameter))).ParameterType);
        }
    }
}
