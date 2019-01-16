using System;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A class that holds on to the given value and provides
    /// the required <see cref="IResolve"/>
    /// when the container is configured.
    /// </summary>
    public class InjectionParameter : ParameterBase, IResolve
    {
        #region Fields

        private readonly object _value;

        #endregion


        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="InjectionParameter"/> that stores
        /// the given value, using the runtime type of that value as the
        /// type of the parameter.
        /// </summary>
        /// <param name="value">Value to be injected for this parameter.</param>
        public InjectionParameter(object value)
            : base((value ?? throw new ArgumentNullException(nameof(value))).GetType())
        {
            _value = value;
        }

        /// <summary>
        /// Create an instance of <see cref="InjectionParameter"/> that stores
        /// the given value, associated with the given type.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterValue">InjectionParameterValue of the parameter</param>
        public InjectionParameter(Type parameterType, object parameterValue)
            : base(parameterType)
        {
            _value = parameterValue;
        }

        #endregion


        #region IResolve

        public object Resolve<TContext>(ref TContext context) 
            where TContext : IResolveContext
        {
            return _value;
        }

        #endregion
    }

    /// <summary>
    /// A generic version of <see cref="InjectionParameter"/> that makes it a
    /// little easier to specify the type of the parameter.
    /// </summary>
    /// <typeparam name="TParameter">Type of parameter.</typeparam>
    public class InjectionParameter<TParameter> : InjectionParameter
    {
        /// <summary>
        /// Create a new <see cref="InjectionParameter{TParameter}"/>.
        /// </summary>
        /// <param name="value">Value for the parameter to be injected.</param>
        public InjectionParameter(TParameter value)
            : base(typeof(TParameter), value)
        {
        }
    }
}
