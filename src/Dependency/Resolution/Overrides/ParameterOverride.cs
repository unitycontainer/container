using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that lets you
    /// override a named parameter passed to a constructor.
    /// </summary>
    public class ParameterOverride : ResolverOverride,
                                     IEquatable<ParameterInfo>
    {
        #region Fields

        protected readonly Type? Type;

        #endregion


        #region Constructors

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="name">Name of the constructor parameter.</param>
        /// <param name="value">InjectionParameterValue to pass for the constructor.</param>
        public ParameterOverride(string name, object? value)
            : base(name, value)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="value">Value to pass for the MethodBase.</param>
        public ParameterOverride(Type type, object? value)
            : base(null, value)
        {
            Type = type;
        }

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="name">Name of the constructor parameter.</param>
        /// <param name="value">Value to pass for the MethodBase.</param>
        public ParameterOverride(Type type, string? name, object? value)
            : base(name, value)
        {
            Type = type;
        }

        #endregion


        #region IEquatable


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object? other)
        {
            switch (other)
            {
                case ParameterInfo info:
                    return null != info &&
                          (null == Target || info.Member.DeclaringType == Target) &&
                          (null == Type || info.ParameterType == Type) &&
                          (null == Name || info.Name == Name);

                case ParameterOverride parameter:
                    return (null == Target || parameter.Target == Target) &&
                           (null == Type   || parameter.Type == Type) &&
                           (null == Name   || parameter.Name == Name);
                default:
                    return base.Equals(other);
            }
        }

        public bool Equals(ParameterInfo? other)
        {
            return null != other &&
                  (null == Target || other.Member.DeclaringType == Target) &&
                  (null == Type || other.ParameterType == Type) &&
                  (null == Name || other.Name == Name);
        }

        #endregion
    }
}
