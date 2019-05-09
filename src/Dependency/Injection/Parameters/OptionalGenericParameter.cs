using System;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A <see cref="ParameterValue"/> that lets you specify that
    /// an instance of a generic type parameter should be resolved, providing the <see langword="null"/>
    /// value if resolving fails.
    /// </summary>
    public class OptionalGenericParameter : GenericBase
    {
        #region Constructors

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        public OptionalGenericParameter(string genericParameterName)
            : base(genericParameterName)
        { }

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="name">Registration name to use when looking up in the container.</param>
        public OptionalGenericParameter(string genericParameterName, string name)
            : base(genericParameterName, name)
        { }

        #endregion


        #region Overrides

        protected override ResolveDelegate<TContext> GetResolver<TContext>(Type type, string name)
        {
            return (ref TContext context) =>
            {
                try { return context.Resolve(type, name); }
                catch (Exception ex) 
                when (!(ex is CircularDependencyException))
                {
                    return null;
                }
            };
        }

        #endregion
    }
}
