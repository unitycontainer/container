using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A class that stores a name and type, and generates a 
    /// resolver object that resolves the parameter via the
    /// container.
    /// </summary>
    [DebuggerDisplay("ResolvedParameter: Type={ParameterType?.Name ?? \"Any\"} Name={_name ?? \"null\"}")]
    public class ResolvedParameter : ParameterBase, 
                                     IResolverFactory<Type>,
                                     IResolverFactory<ParameterInfo>
    {
        #region Fields

        private readonly string? _name;

        #endregion


        #region Constructors

        public ResolvedParameter()
        {
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedParameter"/> that
        /// resolves to the given type.
        /// </summary>
        /// <param name="parameterType">Type of this parameter.</param>
        public ResolvedParameter(Type parameterType)
            : base(parameterType)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedParameter"/> that
        /// resolves the given type and name.
        /// </summary>
        /// <param name="name">Name to use when resolving parameter.</param>
        public ResolvedParameter(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedParameter"/> that
        /// resolves the given type and name.
        /// </summary>
        /// <param name="parameterType">Type of this parameter.</param>
        /// <param name="name">Name to use when resolving parameter.</param>
        public ResolvedParameter(Type parameterType, string? name)
            : base(parameterType)
        {
            _name = name;
        }

        #endregion


        #region IResolverFactory

        public ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext
        {
            if (IsInvalidParameterType)
            {
                return (ref TContext c) => c.Resolve(type, _name);
            }

            return (ref TContext c) => c.Resolve(ParameterType!, _name);
        }

        public ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info) 
            where TContext : IResolveContext
        {
            if (IsInvalidParameterType)
            {
                var type = info.ParameterType;
                return (ref TContext c) => c.Resolve(type, _name);
            }

            return (ref TContext c) => c.Resolve(ParameterType!, _name);
        }

        #endregion


        #region Overrides

        public override string ToString()
        {
            return $"ResolvedParameter: Type={ParameterType?.Name ?? "Any"} Name={_name ?? "null"}";
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
        public ResolvedParameter() : base(typeof(TParameter))
        {
        }

        /// <summary>
        /// Create a new <see cref="ResolvedParameter{TParameter}"/> for the given
        /// generic type and name.
        /// </summary>
        /// <param name="name">Name to use to resolve this parameter.</param>
        public ResolvedParameter(string name) : base(typeof(TParameter), name)
        {
        }
    }
}