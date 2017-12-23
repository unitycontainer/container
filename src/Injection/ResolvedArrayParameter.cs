// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.Policy;
using Unity.ResolverPolicy;
using Unity.Utility;

namespace Unity.Injection
{
    /// <summary>
    /// A class that stores a type, and generates a 
    /// resolver object that resolves all the named instances or the
    /// type registered in a container.
    /// </summary>
    public class ResolvedArrayParameter : TypedInjectionValue
    {
        private readonly Type _elementType;
        private readonly List<InjectionParameterValue> _elementValues = new List<InjectionParameterValue>();

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given element type and collection of element values.
        /// </summary>
        /// <param name="elementType">The type of elements to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public ResolvedArrayParameter(Type elementType, params object[] elementValues)
            : this(elementType.MakeArrayType(), elementType, elementValues)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given array and element types and collection of element values.
        /// </summary>
        /// <param name="arrayParameterType">The type for the array of elements to resolve.</param>
        /// <param name="elementType">The type of elements to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        protected ResolvedArrayParameter(Type arrayParameterType, Type elementType, params object[] elementValues)
            : base(arrayParameterType, null)
        {
            _elementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
            _elementValues.AddRange(ToParameters(elementValues ?? throw new ArgumentNullException(nameof(elementValues))));
            foreach (InjectionParameterValue pv in _elementValues)
            {
                if (!pv.MatchesType(elementType))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Constants.TypesAreNotAssignable,
                            elementType,
                            pv.ParameterTypeName));
                }
            }
        }

        /// <summary>
        /// Return a <see cref="IResolverPolicy"/> instance that will
        /// return this types value for the parameter.
        /// </summary>
        /// <param name="typeToBuild">Type that contains the member that needs this parameter. Used
        /// to resolve open generic parameters.</param>
        /// <returns>The <see cref="IResolverPolicy"/>.</returns>
        public override IResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            var elementType = !_elementType.IsArray ? _elementType 
                : _elementType.GetArrayParameterType(typeToBuild.GenericTypeArguments);

            var elementPolicies = _elementValues.Select(pv => pv.GetResolverPolicy(typeToBuild))
                                                .ToArray();

            return new ResolvedArrayWithElementsResolverPolicy(elementType, elementPolicies);
        }
    }

    /// <summary>
    /// A generic version of <see cref="ResolvedArrayParameter"/> for convenience
    /// when creating them by hand.
    /// </summary>
    /// <typeparam name="TElement">Type of the elements for the array of the parameter.</typeparam>
    public class ResolvedArrayParameter<TElement> : ResolvedArrayParameter
    {
        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter{TElement}"/> that
        /// resolves to the given element generic type with the given element values.
        /// </summary>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public ResolvedArrayParameter(params object[] elementValues)
            : base(typeof(TElement[]), typeof(TElement), elementValues)
        {
        }
    }
}
