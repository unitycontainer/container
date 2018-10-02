using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Build;
using Unity.Builder.Selection;
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
        /// <returns>Sequence of methods to call.</returns>
        public virtual IEnumerable<object> SelectMethods<TContext>(ref TContext context)
            where TContext : IBuildContext
        {
            Type t = context.Type;
            var list = new List<SelectedMethod>();
            var candidateMethods = t.GetMethodsHierarchical()
                                    .Where(m => m.IsStatic == false && m.IsPublic);

            foreach (MethodInfo method in candidateMethods)
            {
                if (method.IsDefined(typeof(TMarkerAttribute), false))
                {
                    list.Add(CreateSelectedMethod(method));
                }
            }

            return list;
        }

        private SelectedMethod CreateSelectedMethod(MethodInfo method)
        {
            var result = new SelectedMethod(method);
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                result.AddParameterResolver(CreateResolver(parameter));
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
