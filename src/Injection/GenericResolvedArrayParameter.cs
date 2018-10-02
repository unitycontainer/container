

using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Unity.Policy;
using Unity.ResolverPolicy;
using Unity.Utility;
using Unity.Delegates;

namespace Unity.Injection
{
    /// <summary>
    /// A <see cref="InjectionParameterValue"/> that lets you specify that
    /// an array containing the registered instances of a generic type parameter 
    /// should be resolved.
    /// </summary>
    public class GenericResolvedArrayParameter : InjectionParameterValue
    {
        private readonly string _genericParameterName;
        private readonly List<InjectionParameterValue> _elementValues = new List<InjectionParameterValue>();

        /// <summary>
        /// Create a new <see cref="GenericResolvedArrayParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public GenericResolvedArrayParameter(string genericParameterName, params object[] elementValues)
        {
            _genericParameterName = genericParameterName ?? throw new ArgumentNullException(nameof(genericParameterName));
            _elementValues.AddRange(ToParameters(elementValues));
        }

        /// <summary>
        /// Name for the type represented by this <see cref="InjectionParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName => _genericParameterName + "[]";

        /// <summary>
        /// Test to see if this parameter value has a matching type for the given type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if this parameter value is compatible with type <paramref name="t"/>,
        /// false if not.</returns>
        /// <remarks>A type is considered compatible if it is an array type of rank one
        /// and its element type is a generic type parameter with a name matching this generic
        /// parameter name configured for the receiver.</remarks>
        public override bool MatchesType(Type type)
        {
            var t = type ?? throw new ArgumentNullException(nameof(type));

            if (!t.IsArray || t.GetArrayRank() != 1)
            {
                return false;
            }

            Type elementType = t.GetElementType();
            return elementType.GetTypeInfo().IsGenericParameter && elementType.GetTypeInfo().Name == _genericParameterName;
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
            GuardTypeToBuildIsGeneric(typeToBuild);
            GuardTypeToBuildHasMatchingGenericParameter(typeToBuild);

            Type typeToResolve = typeToBuild.GetNamedGenericParameter(_genericParameterName);
            var elementPolicies = _elementValues.Select(pv => pv.GetResolverPolicy(typeToBuild)).ToArray();
            return new ResolvedArrayWithElementsResolverPolicy(typeToResolve, elementPolicies);
        }

        private void GuardTypeToBuildIsGeneric(Type typeToBuild)
        {
            if (!typeToBuild.GetTypeInfo().IsGenericType)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Constants.NotAGenericType,
                        typeToBuild.GetTypeInfo().Name,
                        _genericParameterName));
            }
        }

        private void GuardTypeToBuildHasMatchingGenericParameter(Type typeToBuild)
        {
            foreach (Type genericParam in typeToBuild.GetGenericTypeDefinition().GetTypeInfo().GenericTypeParameters)
            {
                if (genericParam.GetTypeInfo().Name == _genericParameterName)
                {
                    return;
                }
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Constants.NoMatchingGenericArgument,
                    typeToBuild.GetTypeInfo().Name,
                    _genericParameterName));
        }

        public override ResolveDelegate<TContext> GetResolver<TContext>(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
