using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder.Policy;
using Unity.Policy;
using Unity.Injection;
using Unity.Storage;
using Unity.Utility;

namespace Unity
{
    /// <summary>
    /// An <see cref="InjectionMember"/> that configures the
    /// container to call a method as part of buildup.
    /// </summary>
    public class InjectionMethod : InjectionMember
    {
        private readonly string _methodName;
        private readonly List<InjectionParameterValue> _methodParameters;

        /// <summary>
        /// Create a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given methods with the given parameters.
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="methodParameters">Parameter values for the method.</param>
        public InjectionMethod(string methodName, params object[] methodParameters)
        {
            _methodName = methodName;
            _methodParameters = InjectionParameterValue.ToParameters(methodParameters).ToList();
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="registeredType">Type of interface registered, ignored in this implementation.</param>
        /// <param name="mappedToType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name, ref TPolicyList policies)
        {
            MethodInfo methodInfo = FindMethod(mappedToType);
            ValidateMethodCanBeInjected(methodInfo, mappedToType);

            SpecifiedMethodsSelectorPolicy selector =
                GetSelectorPolicy(policies, registeredType, name);
            selector.AddMethodAndParameters(methodInfo, _methodParameters);
        }

        public override bool BuildRequired => true;

        /// <summary>
        /// A small function to handle name matching. You can override this
        /// to do things like case insensitive comparisons.
        /// </summary>
        /// <param name="targetMethod">MethodInfo for the method you're checking.</param>
        /// <param name="nameToMatch">Name of the method you're looking for.</param>
        /// <returns>True if a match, false if not.</returns>
        protected virtual bool MethodNameMatches(MemberInfo targetMethod, string nameToMatch)
        {
            return (targetMethod ?? throw new ArgumentNullException(nameof(targetMethod))).Name == nameToMatch;
        }

        private MethodInfo FindMethod(Type typeToCreate)
        {
            foreach (MethodInfo method in typeToCreate.GetMethodsHierarchical())
            {
                if (MethodNameMatches(method, _methodName))
                {
                    if (_methodParameters.Matches(method.GetParameters().Select(p => p.ParameterType)))
                    {
                        return method;
                    }
                }
            }
            return null;
        }

        private void ValidateMethodCanBeInjected(MethodInfo method, Type typeToCreate)
        {
            GuardMethodNotNull(method, typeToCreate);
            GuardMethodNotStatic(method, typeToCreate);
            GuardMethodNotGeneric(method, typeToCreate);
            GuardMethodHasNoOutParams(method, typeToCreate);
            GuardMethodHasNoRefParams(method, typeToCreate);
        }

        private void GuardMethodNotNull(MethodInfo info, Type typeToCreate)
        {
            if (info == null)
            {
                ThrowIllegalInjectionMethod(Constants.NoSuchMethod, typeToCreate);
            }
        }

        private void GuardMethodNotStatic(MethodInfo info, Type typeToCreate)
        {
            if (info.IsStatic)
            {
                ThrowIllegalInjectionMethod(Constants.CannotInjectStaticMethod, typeToCreate);
            }
        }

        private void GuardMethodNotGeneric(MethodInfo info, Type typeToCreate)
        {
            if (info.IsGenericMethodDefinition)
            {
                ThrowIllegalInjectionMethod(Constants.CannotInjectGenericMethod, typeToCreate);
            }
        }

        private void GuardMethodHasNoOutParams(MethodInfo info, Type typeToCreate)
        {
            if (info.GetParameters().Any(param => param.IsOut))
            {
                ThrowIllegalInjectionMethod(Constants.CannotInjectMethodWithOutParams, typeToCreate);
            }
        }

        private void GuardMethodHasNoRefParams(MethodInfo info, Type typeToCreate)
        {
            if (info.GetParameters().Any(param => param.ParameterType.IsByRef))
            {
                ThrowIllegalInjectionMethod(Constants.CannotInjectMethodWithRefParams, typeToCreate);
            }
        }

        private void ThrowIllegalInjectionMethod(string message, Type typeToCreate)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    message,
                    typeToCreate.GetTypeInfo().Name,
                    _methodName,
                    string.Join(", ", _methodParameters.Select(mp => mp.ParameterTypeName))));
        }

        private static SpecifiedMethodsSelectorPolicy GetSelectorPolicy(IPolicyList policies, Type typeToCreate, string name)
        {
            var selector = policies.Get(typeToCreate, name, typeof(IMethodSelectorPolicy));
            if (!(selector is SpecifiedMethodsSelectorPolicy))
            {
                selector = new SpecifiedMethodsSelectorPolicy();
                policies.Set(typeToCreate, name, typeof(IMethodSelectorPolicy), selector);
            }
            return (SpecifiedMethodsSelectorPolicy)selector;
        }
    }
}
