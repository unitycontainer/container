using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Import;

namespace Unity.Injection
{
    /// <summary>
    /// This class is used to pass values to injected parameters.
    /// </summary>
    [DebuggerDisplay("InjectionParameter: Type={ParameterType?.Name ?? InferredType} Value={_value}")]
    public class InjectionParameter : ParameterBase
    {
        #region Fields

        private readonly object? _value;

        #endregion


        #region Constructors

        /// <summary>
        /// Configures the container to inject parameter with specified value
        /// </summary>
        /// <param name="value">Value to be injected</param>
        public InjectionParameter(object? value)
            : base(value?.GetType(), false) 
            => _value = value;

        /// <summary>
        /// Configures the container to inject dependency with specified value 
        /// by specified import <see cref="Type"/>
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the injected import</param>
        /// <param name="value">Value to be injected</param>
        /// <exception cref="ArgumentNullException">Throws and exception when 
        /// type is null</exception>
        public InjectionParameter(Type type, object? value)
            : base(type ?? throw new ArgumentNullException(nameof(type)), false) 
            => _value = value;

        #endregion


        #region Implementation

        /// <inheritdoc/>
        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor) 
            => descriptor.Data = _value;

        /// <inheritdoc/>
        public override MatchRank RankMatch(ParameterInfo parameter) 
            => ParameterType is null
                ? _value.MatchTo(parameter.ParameterType)
                : ParameterType.MatchTo(parameter.ParameterType);

        /// <inheritdoc/>
        public override string ToString() 
            => $"InjectionParameter: Type={ParameterType?.Name} Value={_value ?? "null"}";

        #endregion
    }


    #region Generic

    /// <summary>
    /// A generic version of <see cref="InjectionParameter"/>
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> of the injected import</typeparam>
    public class InjectionParameter<T> : InjectionParameter
    {
        /// <inheritdoc/>
        public InjectionParameter(T value)
            : base(typeof(T), value)
        {
        }
    }

    #endregion
}
