using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Container;

namespace Unity.Injection
{
    /// <summary>
    /// Instances of this class instruct the container to optionally inject 
    /// corresponding parameters with values imported from this container
    /// </summary>
    /// <remarks>
    /// When the container fails to inject specified parameters with required
    /// import, the error is not generated. The parameter either injected with
    /// default value, or 'default(T)'
    /// </remarks>
    [DebuggerDisplay("OptionalParameter: Type={ParameterType?.Name ?? \"Any\"} Name={_name ?? \"null\"}")]
    public class OptionalParameter : ParameterBase
    {
        #region Fields

        private readonly string? _name;

        #endregion


        #region Constructors

        /// <summary>
        /// Configures the container to inject parameter with optional value resolved 
        /// from the container
        /// </summary>
        /// <remarks>
        /// The parameter is injected with value imported from the container. 
        /// The <see cref="Type"/> of imported contract is the <see cref="Type"/>
        /// of the parameter and no name.
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        public OptionalParameter()
            : base(null, true)
        {
        }

        /// <summary>
        /// Configures the container to optionally inject parameter with specified 
        /// <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="contractType">Type of this parameter.</param>
        public OptionalParameter(Type contractType)
            : base(contractType, true)
        {
        }

        /// <summary>
        /// Configures the container to optionally inject parameter with imported <see cref="Contract"/> 
        /// with the <see cref="Type"/> being the <see cref="Type"/> of the parameter and the
        /// specified name.
        /// </summary>
        /// <remarks>
        /// The parameter is injected with value imported from the container. The <see cref="Type"/> of 
        /// imported contract is the <see cref="Type"/> of the parameter and name of the 
        /// <see cref="Contract"/> is provided in <paramref name="contractName"/>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        public OptionalParameter(string contractName)
            : base(null, true)
        {
            _name = contractName;
        }

        /// <summary>
        /// Configures the container to optionally inject parameter with specified <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="contractType">Type of the <see cref="Contract"/></param>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        public OptionalParameter(Type contractType, string contractName)
            : base(contractType, true)
        {
            _name = contractName;
        }

        #endregion


        #region Implementation

        public override ReflectionInfo<Type> FillReflectionInfo(Type type)
            => new ReflectionInfo<Type>(type, ParameterType ?? type, _name, AllowDefault);

        public override ReflectionInfo<ParameterInfo> FillReflectionInfo(ParameterInfo member)
            => new ReflectionInfo<ParameterInfo>(member, ParameterType ?? member.ParameterType, _name, AllowDefault || member.HasDefaultValue);

        public override ReflectionInfo<FieldInfo> FillReflectionInfo(FieldInfo member)
            => new ReflectionInfo<FieldInfo>(member, ParameterType ?? member.FieldType, _name, AllowDefault);

        public override ReflectionInfo<PropertyInfo> FillReflectionInfo(PropertyInfo member)
            => new ReflectionInfo<PropertyInfo>(member, ParameterType ?? member.PropertyType, _name, AllowDefault);

        public override string ToString()
        {
            return $"OptionalParameter: Type={ParameterType?.Name ?? "Any"} Name={_name ?? "null"}";
        }

        #endregion
    }


    #region Generic

    /// <summary>
    /// A generic version of <see cref="OptionalParameter"/>
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of injected <see cref="Contract"/></typeparam>
    public class OptionalParameter<T> : OptionalParameter
    {
        /// <inheritdoc/>
        public OptionalParameter() : base(typeof(T))
        {
        }

        /// <inheritdoc/>
        public OptionalParameter(string name) : base(typeof(T), name)
        {
        }
    }

    #endregion
}
