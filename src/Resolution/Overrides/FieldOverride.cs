using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified field.
    /// </summary>
    public class FieldOverride : ResolverOverride
    {
        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="FieldOverride"/>.
        /// </summary>
        /// <param name="name">The Field name.</param>
        /// <param name="value">InjectionParameterValue to use for the Field.</param>
        /// <param name="exact">Indicates if override has to match exactly</param>
        public FieldOverride(string name, object? value, bool exact = true)
            : base(name ?? throw new ArgumentNullException(nameof(name)), value, exact)
        {
        }

        #endregion


        #region  Match Target

        public override bool Equals(FieldInfo? other)
        {
            return null != other  && other.Name == Name &&
                  (null == Target || other.DeclaringType == Target);
        }

        #endregion
    }
}
