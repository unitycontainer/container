using System;
using System.Reflection;
using Unity.Policy;

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
            : base(parameterName)
        {
            Value = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue));
        }

        #endregion


        #region IEquatable


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other is ParameterInfo info)
                return Equals(info);

            return base.Equals(other);
        }

        public bool Equals(ParameterInfo other)
        {
            return (null == Target || other?.Member.DeclaringType == Target) &&
                   (null == Type   || other?.ParameterType == Type) &&
                   (null == Name   || other?.Name == Name);
        }


        #endregion


        #region IResolverPolicy

        public object Resolve<TContext>(ref TContext context)
            where TContext : IResolveContext
        {
            if (Value is IResolve policy)
                return policy.Resolve(ref context);

            if (Value is IResolverFactory factory)
            {
                var resolveDelegate = factory.GetResolver<TContext>(Type);
                return resolveDelegate(ref context);
            }

            return Value;
        }

        #endregion
    }
}
