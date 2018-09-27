

using System;
using System.Reflection;
using Unity.Policy;
using Unity.ResolverPolicy;
using Unity.Utility;

namespace Unity.Injection
{
    /// <summary>
    /// A class that stores a name and type, and generates a 
    /// resolver object that resolves the parameter via the
    /// container.
    /// </summary>
    public class ResolvedParameter : TypedInjectionValue
    {
        private readonly string _name;

        /// <summary>
        /// Construct a new <see cref="ResolvedParameter"/> that
        /// resolves to the given type.
        /// </summary>
        /// <param name="parameterType">Type of this parameter.</param>
        public ResolvedParameter(Type parameterType)
            : this(parameterType, null)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedParameter"/> that
        /// resolves the given type and name.
        /// </summary>
        /// <param name="parameterType">Type of this parameter.</param>
        /// <param name="name">Name to use when resolving parameter.</param>
        public ResolvedParameter(Type parameterType, string name)
            : base(parameterType, null)
        {
            _name = name;
        }

        /// <summary>
        /// Return a <see cref="IResolverPolicy"/> instance that will
        /// return this types value for the parameter.
        /// </summary>
        /// <param name="type">Type that contains the member that needs this parameter. Used
        /// to resolve open generic parameters.</param>
        /// <returns>The <see cref="IResolverPolicy"/>.</returns>
        public override IResolverPolicy GetResolverPolicy(Type type)
        {
            var typeToBuild = type ?? throw new ArgumentNullException(nameof(type));
            if (ParameterType.IsArray && ParameterType.GetElementType().GetTypeInfo().IsGenericParameter)
            {
                Type arrayType = ParameterType.GetClosedParameterType(typeToBuild.GetTypeInfo().GenericTypeArguments);
                return new NamedTypeDependencyResolverPolicy(arrayType, _name);
            }

            var info = ParameterType.GetTypeInfo();
            if (info.IsGenericType && info.ContainsGenericParameters || ParameterType.IsGenericParameter)
            {
                return new NamedTypeDependencyResolverPolicy(
                    ParameterType.GetClosedParameterType(typeToBuild.GetTypeInfo().GenericTypeArguments), _name);
            }

            return new NamedTypeDependencyResolverPolicy(ParameterType, _name);
        }
    }

    /// <summary>
    /// A generic version of <see cref="ResolvedParameter"/> for convenience
    /// when creating them by hand.
    /// </summary>
    /// <typeparam name="TParameter">Type of the parameter</typeparam>
    public class ResolvedParameter<TParameter> : ResolvedParameter
    {
        /// <summary>
        /// Create a new <see cref="ResolvedParameter{TParameter}"/> for the given
        /// generic type and the default name.
        /// </summary>
        public ResolvedParameter()
            : base(typeof(TParameter))
        {
        }

        /// <summary>
        /// Create a new <see cref="ResolvedParameter{TParameter}"/> for the given
        /// generic type and name.
        /// </summary>
        /// <param name="name">Name to use to resolve this parameter.</param>
        public ResolvedParameter(string name)
            : base(typeof(TParameter), name)
        {
        }
    }
}