// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Reflection;
using Unity.Policy;
using Unity.Utility;

namespace Unity.Injection
{
    /// <summary>
    /// Base class for <see cref="InjectionParameterValue"/> subclasses that let you specify that
    /// an instance of a generic type parameter should be resolved.
    /// </summary>
    public abstract class GenericParameterBase : InjectionParameterValue
    {
        private readonly string _genericParameterName;
        private readonly bool _isArray;
        private readonly string _resolutionKey;

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        protected GenericParameterBase(string genericParameterName)
            : this(genericParameterName, null)
        { }

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="resolutionKey">name to use when looking up in the container.</param>
        protected GenericParameterBase(string genericParameterName, string resolutionKey)
        {
            if ((genericParameterName ?? throw new ArgumentNullException(nameof(genericParameterName))).EndsWith("[]", StringComparison.Ordinal) ||
                genericParameterName.EndsWith("()", StringComparison.Ordinal))
            {
                _genericParameterName = genericParameterName.Replace("[]", String.Empty).Replace("()", String.Empty);
                _isArray = true;
            }
            else
            {
                _genericParameterName = genericParameterName;
                _isArray = false;
            }
            _resolutionKey = resolutionKey;
        }

        /// <summary>
        /// Name for the type represented by this <see cref="InjectionParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName
        {
            get { return _genericParameterName; }
        }

        /// <summary>
        /// Test to see if this parameter value has a matching type for the given type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if this parameter value is compatible with type <paramref name="t"/>,
        /// false if not.</returns>
        public override bool MatchesType(Type type)
        {
            var t = type ?? throw new ArgumentNullException(nameof(type));
            if (!_isArray)
            {
                return t.GetTypeInfo().IsGenericParameter && t.GetTypeInfo().Name == _genericParameterName;
            }
            return t.IsArray && t.GetElementType().GetTypeInfo().IsGenericParameter && t.GetElementType().GetTypeInfo().Name == _genericParameterName;
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
            if (_isArray)
            {
                typeToResolve = typeToResolve.MakeArrayType();
            }

            return DoGetResolverPolicy(typeToResolve, _resolutionKey);
        }

        /// <summary>
        /// Return a <see cref="IResolverPolicy"/> instance that will
        /// return this types value for the parameter.
        /// </summary>
        /// <param name="typeToResolve">The actual type to resolve.</param>
        /// <param name="resolutionKey">The resolution key.</param>
        /// <returns>The <see cref="IResolverPolicy"/>.</returns>
        protected abstract IResolverPolicy DoGetResolverPolicy(Type typeToResolve, string resolutionKey);

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
    }
}
