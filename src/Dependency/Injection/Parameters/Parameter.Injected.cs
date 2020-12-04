using System;
using System.Diagnostics;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// This class is used to pass values to injected parameters.
    /// </summary>
    [DebuggerDisplay("InjectionParameter: Type={ParameterType.Name ?? \"Any Type\"} Value={_value ?? \"null\"}")]
    public class InjectionParameter : ParameterBase
    {
        #region Fields

        private readonly object? _value;

        #endregion


        #region Constructors

        /// <summary>
        /// Configures the container to inject parameter with specified value
        /// </summary>
        /// <remarks>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="value">Value to be injected</param>
        /// <exception cref="ArgumentNullException">Throws and exception when value in null</exception>
        public InjectionParameter(object? value)
            : base((value ?? throw new ArgumentNullException($"The {nameof(value)} is 'null'. Unable to infer type of injected parameter\n" +
                $"To pass 'null' as a value, use InjectionParameter(Type, object) constructor")).GetType(), false) 
            => _value = value;

        /// <summary>
        /// Configures the container to inject parameter with specified value 
        /// as specified import <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="importType"><see cref="Type"/> of the injected import</param>
        /// <param name="value">Value to be injected</param>
        public InjectionParameter(Type importType, object? value)
            : base(importType ?? throw new ArgumentNullException(nameof(importType)), false) => _value = value;

        #endregion


        #region Implementation

        public override void GetImportInfo<TImport>(ref TImport import)
        {
            import.AllowDefault = false;
            import.Value = _value;
        }

        public override MatchRank Match(ParameterInfo parameter) 
            => ParameterType!.MatchTo(parameter.ParameterType);

        public override string ToString() 
            => $"InjectionParameter: Type={ParameterType!.Name} Value={_value ?? "null"}";

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
