using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Attributes;
using Unity.Build;
using Unity.Builder.Selection;
using Unity.Policy;
using Unity.ResolverPolicy;
using Unity.Utility;

namespace Unity.ObjectBuilder.Policies
{
    /// <summary>
    /// An implementation of <see cref="IMethodSelectorPolicy"/> that is aware
    /// of the build keys used by the Unity container.
    /// </summary>
    public class DefaultUnityMethodSelectorPolicy : IMethodSelectorPolicy
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
                if (method.IsDefined(typeof(InjectionMethodAttribute), false))
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
        protected IResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            var attributes = parameter.GetCustomAttributes(false)
                                      .OfType<DependencyResolutionAttribute>()
                                      .ToArray();

            if (attributes.Length > 0)
            {
                // Since this attribute is defined with MultipleUse = false, the compiler will
                // enforce at most one. So we don't need to check for more.
                var attr = attributes[0];
                return attr is OptionalDependencyAttribute dependencyAttribute
                    ? (IResolverPolicy) new OptionalDependencyResolverPolicy(parameter.ParameterType, dependencyAttribute.Name)
                    : null != attr.Name 
                        ? new NamedTypeDependencyResolverPolicy(parameter.ParameterType, attr.Name)
                        : null;
            }

            return null;
        }
    }
}
