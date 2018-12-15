using System;
using System.Globalization;
using System.Reflection;
using Unity.Policy;
using Unity.Resolution;

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
        /// <returns>True if this parameter value is compatible with type <paramref name="type"/>,
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

        public override ResolveDelegate<TContext> GetResolver<TContext>(Type type)
        {
            GuardTypeToBuildIsGeneric(type);
            GuardTypeToBuildHasMatchingGenericParameter(type);
            Type typeToResolve = GetNamedGenericParameter(type, _genericParameterName);
            if (_isArray)
            {
                typeToResolve = typeToResolve.MakeArrayType();
            }

            return GetResolver<TContext>(typeToResolve, _resolutionKey);
        }

        protected abstract ResolveDelegate<TContext> GetResolver<TContext>(Type type, string resolutionKey) 
            where TContext : IResolveContext;

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

        private static Type GetNamedGenericParameter(Type type, string parameterName)
        {
            TypeInfo openType = type.GetGenericTypeDefinition().GetTypeInfo();
            Type result = null;
            int index = -1;

            foreach (var genericArgumentType in openType.GenericTypeParameters)
            {
                if (genericArgumentType.GetTypeInfo().Name == parameterName)
                {
                    index = genericArgumentType.GenericParameterPosition;
                    break;
                }
            }
            if (index != -1)
            {
                result = type.GetTypeInfo().GenericTypeArguments[index];
            }
            return result;
        }

    }
}
