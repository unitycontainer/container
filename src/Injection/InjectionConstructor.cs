// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Attributes;
using Unity.Builder;
using Unity.Builder.Policy;
using Unity.Policy;
using Unity.Registration;
using Unity.Utility;

namespace Unity.Injection
{
    /// <summary>
    /// A class that holds the collection of information
    /// for a constructor, so that the container can
    /// be configured to call this constructor.
    /// </summary>
    public class InjectionConstructor : InjectionMember
    {
        private readonly InjectionParameterValue[] _data;
        private readonly Type[] _types;

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a default constructor.
        /// </summary>
        public InjectionConstructor()
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameter types.
        /// </summary>
        /// <param name="types">The types of the parameters of the constructor.</param>
        public InjectionConstructor(params Type[] types)
        {
            _types = types ?? throw new ArgumentNullException(nameof(types));
        }

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameters.
        /// </summary>
        /// <param name="parameterValues">The values for the parameters, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public InjectionConstructor(params object[] parameterValues)
        {
            _data = (parameterValues ?? throw new ArgumentNullException(nameof(parameterValues)))
                .Select(InjectionParameterValue.ToParameter)
                .ToArray();
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="serviceType">Interface registered, ignored in this implementation.</param>
        /// <param name="implementationType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            var policy = null != _data  ? ConstructorByArguments(implementationType, _data) :
                         null != _types ? ConstructorByType(implementationType, _types)     :
                                          DefaultConstructor(implementationType);

            policies.Set<IConstructorSelectorPolicy>(policy, new NamedTypeBuildKey(implementationType, name));
        }

        private SpecifiedConstructorSelectorPolicy DefaultConstructor(Type typeToCreate)
        {
            foreach (var ctor in typeToCreate.GetTypeInfo()
                                             .DeclaredConstructors
                                             .Where(c => c.IsStatic == false && c.IsPublic))
            {
                if (!ctor.GetParameters().Select(p => p.ParameterType).Any())
                {
                    return new SpecifiedConstructorSelectorPolicy(ctor, new InjectionParameterValue[0]);
                }
            }

            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.NoSuchConstructor,
                    typeToCreate.FullName, string.Empty));
        }


        private SpecifiedConstructorSelectorPolicy ConstructorByArguments(Type typeToCreate, InjectionParameterValue[] data)
        {
            foreach (var ctor in typeToCreate.GetTypeInfo()
                .DeclaredConstructors
                .Where(c => c.IsStatic == false && c.IsPublic))
            {
                if (_data.Matches(ctor.GetParameters().Select(p => p.ParameterType)))
                {
                    return new SpecifiedConstructorSelectorPolicy(ctor, data); 
                }
            }

            string signature = string.Join(", ", data.Select(p => p.ParameterTypeName).ToArray());

            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.NoSuchConstructor,
                    typeToCreate.FullName,
                    signature));
        }

        private SpecifiedConstructorSelectorPolicy ConstructorByType(Type typeToCreate, Type[] types)
        {
            foreach (var ctor in typeToCreate.GetTypeInfo()
                                             .DeclaredConstructors
                                             .Where(c => c.IsStatic == false && c.IsPublic))
            {
                var parameters = ctor.GetParameters();
                if (parameters.ParametersMatch(types))
                {
                    return new SpecifiedConstructorSelectorPolicy(ctor, parameters.Select(ToResolvedParameter)
                                                                                  .ToArray());
                }
            }

            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture, Constants.NoSuchConstructor, 
                typeToCreate.FullName, string.Join(", ", _types.Select(t => t.Name))));
        }

        private InjectionParameterValue ToResolvedParameter(ParameterInfo parameter)
        {
            return new ResolvedParameter(parameter.ParameterType, parameter.GetCustomAttributes(false)
                                                                           .OfType<DependencyAttribute>()
                                                                           .FirstOrDefault()?.Name);
        }
    }
}
