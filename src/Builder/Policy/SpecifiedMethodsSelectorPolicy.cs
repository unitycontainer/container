using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder.Selection;
using Unity.Injection;
using Unity.Policy;
using Unity.Utility;

namespace Unity.Builder.Policy
{
    /// <summary>
    /// A <see cref="IMethodSelectorPolicy"/> implementation that calls the specific
    /// methods with the given parameters.
    /// </summary>
    public class SpecifiedMethodsSelectorPolicy : IMethodSelectorPolicy
    {
        private readonly List<Tuple<MethodInfo, IEnumerable<InjectionParameterValue>>> _methods =
            new List<Tuple<MethodInfo, IEnumerable<InjectionParameterValue>>>();

        /// <summary>
        /// Add the given method and parameter collection to the list of methods
        /// that will be returned when the selector's <see cref="IMethodSelectorPolicy.SelectMethods"/>
        /// method is called.
        /// </summary>
        /// <param name="method">Method to call.</param>
        /// <param name="parameters">sequence of <see cref="InjectionParameterValue"/> objects
        /// that describe how to create the method parameter values.</param>
        public void AddMethodAndParameters(MethodInfo method, IEnumerable<InjectionParameterValue> parameters)
        {
            _methods.Add(new Tuple<MethodInfo, IEnumerable<InjectionParameterValue>>(method, parameters));
        }

        /// <summary>
        /// Return the sequence of methods to call while building the target object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>Sequence of methods to call.</returns>
        public IEnumerable<SelectedMethod> SelectMethods<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            var list = new List<SelectedMethod>();
            foreach (Tuple<MethodInfo, IEnumerable<InjectionParameterValue>> method in _methods)
            {
                Type typeToBuild = context.BuildKey.Type;
                SelectedMethod selectedMethod;
                var info = method.Item1.DeclaringType.GetTypeInfo();
                var methodHasOpenGenericParameters = method.Item1.GetParameters()
                                                           .Select(p => p.ParameterType.GetTypeInfo())
                                                           .Any(i => i.IsGenericType && i.ContainsGenericParameters);
                if (!methodHasOpenGenericParameters && !(info.IsGenericType && info.ContainsGenericParameters))
                {
                    selectedMethod = new SelectedMethod(method.Item1);
                }
                else
                {
                    var closedMethodParameterTypes = method.Item1
                        .GetClosedParameterTypes(typeToBuild.GetTypeInfo().GenericTypeArguments);

                    selectedMethod = new SelectedMethod(typeToBuild.GetMethodsHierarchical()
                        .Single(m => m.Name.Equals(method.Item1.Name) && 
                                     m.GetParameters().ParametersMatch(closedMethodParameterTypes))); 
                }

                AddParameterResolvers(typeToBuild, method.Item2, selectedMethod);

                list.Add(selectedMethod);
            }

            return list;
        }

        private static void AddParameterResolvers(Type typeToBuild,
            IEnumerable<InjectionParameterValue> parameterValues,
            SelectedMemberWithParameters result)
        {
            foreach (InjectionParameterValue parameterValue in parameterValues)
            {
                var resolver = parameterValue.GetResolverPolicy(typeToBuild);
                result.AddParameterResolver(resolver);
            }
        }

    }
}
