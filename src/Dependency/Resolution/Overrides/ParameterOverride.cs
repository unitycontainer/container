using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that lets you
    /// override a named parameter passed to a constructor.
    /// </summary>
    public class ParameterOverride : ResolverOverride,
                                     IEquatable<ParameterInfo>,
                                     IResolve
    {
        #region Fields

        protected readonly object Value;

        #endregion


        #region Constructors

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="parameterName">Name of the constructor parameter.</param>
        /// <param name="parameterValue">InjectionParameterValue to pass for the constructor.</param>
        public ParameterOverride(string parameterName, object parameterValue)
            : base(null, null, parameterName)
        {
            Value = parameterValue;
        }

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterValue">Value to pass for the MethodBase.</param>
        public ParameterOverride(Type parameterType, object parameterValue)
            : base(null, parameterType, null)
        {
            Value = parameterValue;
        }

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterName">Name of the constructor parameter.</param>
        /// <param name="parameterValue">Value to pass for the MethodBase.</param>
        public ParameterOverride(Type parameterType, string parameterName, object parameterValue)
            : base(null, parameterType, parameterName)
        {
            Value = parameterValue;
        }

        #endregion


        #region IEquatable


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            switch (other)
            {
                case ParameterInfo info:
                    return Equals(info);

                case ParameterOverride parameter:
                    return (null == Target || parameter.Target == Target) &&
                           (null == Type   || parameter.Type == Type) &&
                           (null == Name   || parameter.Name == Name);
                default:
                    return base.Equals(other);
            }
        }

        public bool Equals(ParameterInfo other)
        {
            return null != other && 
                  (null == Target || other.Member.DeclaringType == Target) &&
                  (null == Type   || other.ParameterType == Type) &&
                  (null == Name   || other.Name == Name);
        }


        #endregion


        #region IResolverPolicy

        public object Resolve<TContext>(ref TContext context)
            where TContext : IResolveContext
        {
            if (Value is IResolve policy)
                return policy.Resolve(ref context);

            if (Value is IResolverFactory<Type> factory)
            {
                var resolveDelegate = factory.GetResolver<TContext>(Type);
                return resolveDelegate(ref context);
            }

            return Value;
        }

        #endregion
    }
}
