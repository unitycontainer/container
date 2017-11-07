// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        private readonly object[] _data;
        private readonly Type[] _types;

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a default constructor.
        /// </summary>
        public InjectionConstructor()
        {
            _data = new object[0];
            _types = new Type[0];
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
            _data = parameterValues ?? throw new ArgumentNullException(nameof(parameterValues));
            _types = _data.Select(o => o.GetType()).ToArray();
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
            ConstructorInfo ctor = FindConstructor(implementationType);

            var values = null == _data ? null : InjectionParameterValue.ToParameters(_data).ToArray();
            policies.Set<IConstructorSelectorPolicy>(new SpecifiedConstructorSelectorPolicy(ctor, values),
                                                     new NamedTypeBuildKey(implementationType, name));
        }

        private ConstructorInfo FindConstructor(Type typeToCreate)
        {
            foreach (var ctor in typeToCreate.GetTypeInfo()
                                             .DeclaredConstructors
                                             .Where(c => c.IsStatic == false && c.IsPublic))
            {
                if (ctor.GetParameters().ParametersMatch(_types))
                    return ctor;
            }

            var values = null != _data ? InjectionParameterValue.ToParameters(_data).Select(p => p.ParameterTypeName).ToArray() 
                                       : _types.Select(t => t.Name) ;

            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture, Constants.NoSuchConstructor, typeToCreate.FullName, string.Join(", ", values)));
        }
    }
}
