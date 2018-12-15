using System;
using System.Reflection;
using Unity.Policy;

namespace Unity.Injection
{
    /// <summary>
    /// A class that stores a name and type, and generates a 
    /// resolver object that resolves the parameter via the
    /// container.
    /// </summary>
    public class ResolvedParameter : TypedInjectionValue
    {
        #region Fields

        private readonly string _name;

        #endregion


        #region Constructors

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

        #endregion


        #region TypedInjectionValue

        public override ResolveDelegate<TContext> GetResolver<TContext>(Type type)
        {
            var info = ParameterType.GetTypeInfo();

            if (ParameterType.IsArray && ParameterType.GetElementType().GetTypeInfo().IsGenericParameter ||
                info.IsGenericType && info.ContainsGenericParameters || ParameterType.IsGenericParameter)
            {
                var parameterType = ParameterType.GetClosedParameterType(type.GetTypeInfo().GenericTypeArguments);
                return (ref TContext c) => c.Resolve(parameterType, _name);
            }

            return (ref TContext c) => c.Resolve(ParameterType, _name);
        }

        #endregion
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