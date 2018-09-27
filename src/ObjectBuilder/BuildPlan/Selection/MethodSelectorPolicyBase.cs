

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Utility;

namespace Unity.ObjectBuilder.BuildPlan.Selection
{
    /// <summary>
    /// Base class that provides an implementation of <see cref="IMethodSelectorPolicy"/>
    /// which lets you override how the parameter resolvers are created.
    /// </summary>
    /// <typeparam name="TMarkerAttribute">Attribute that marks methods that should
    /// be called.</typeparam>
    public abstract class MethodSelectorPolicyBase<TMarkerAttribute> : IMethodSelectorPolicy
        where TMarkerAttribute : Attribute
    {
        /// <summary>
        /// Return the sequence of methods to call while building the target object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>Sequence of methods to call.</returns>
        public virtual IEnumerable<Builder.Selection.SelectedMethod> SelectMethods(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Type t = context.BuildKey.Type;
            var candidateMethods = t.GetMethodsHierarchical()
                                    .Where(m => m.IsStatic == false && m.IsPublic);

            foreach (MethodInfo method in candidateMethods)
            {
                if (method.IsDefined(typeof(TMarkerAttribute), false))
                {
                    yield return CreateSelectedMethod(method);
                }
            }
        }

        private Builder.Selection.SelectedMethod CreateSelectedMethod(MethodInfo method)
        {
            var result = new Builder.Selection.SelectedMethod(method);
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                result.AddParameterResolver(this.CreateResolver(parameter));
            }

            return result;
        }

        /// <summary>
        /// Create a <see cref="IResolverPolicy"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameter">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected abstract IResolverPolicy CreateResolver(ParameterInfo parameter);
    }
}
