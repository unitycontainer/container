using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy;
using Unity.ResolverPolicy;
using Unity.Utility;

namespace Unity.Builder
{
    /// <summary>
    /// An implementation of <see cref="IMethodSelectorPolicy"/> that is aware
    /// of the build keys used by the Unity container.
    /// </summary>
    public class InvokedMethodsSelector : MemberSelectorPolicy<MethodInfo, object[]>, 
                                                    IMethodSelectorPolicy
    {
        /// <summary>
        /// Return the sequence of methods to call while building the target object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>Sequence of methods to call.</returns>
        public virtual IEnumerable<object> SelectMethods<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            return Select(ref context);
        }


        #region Overrides

        protected override IEnumerable<object> GetAttributedMembers(Type type)
        {
            return from method in type.GetMethodsHierarchical().Where(m => m.IsStatic == false && m.IsPublic)
                   where method.IsDefined(typeof(InjectionMethodAttribute), false)
                   select CreateSelectedMethod(method);
        }

        #endregion

        private SelectedMethod CreateSelectedMethod(MethodInfo method, object injector = null)
        {
            var result = new SelectedMethod(method);
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                result.AddParameterResolver(CreateResolver(parameter));
            }

            return result;
        }

        /// <summary>
        /// Create a <see cref="IResolve"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameter">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected IResolve CreateResolver(ParameterInfo parameter)
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
                    ? (IResolve) new OptionalDependencyResolvePolicy(parameter.ParameterType, dependencyAttribute.Name)
                    : null != attr.Name 
                        ? new NamedTypeDependencyResolvePolicy(parameter.ParameterType, attr.Name)
                        : null;
            }

            return null;
        }
    }
}
