using System;
using System.Reflection;

namespace Unity.Injection
{
    public class OptionalField : InjectionMemberInfo<FieldInfo>
    {
        #region Constructors

        /// <summary>
        /// Configures the container to inject a specified field with a resolved value.
        /// </summary>
        /// <param name="field">Name of field to inject.</param>
        public OptionalField(string field)
            : base(field)
        {
        }

        public OptionalField(string field, Type type)
            : base(field, type)
        {

        }

        public OptionalField(string field, Type type, string? name)
            : base(field, type, name)
        {

        }

        /// <summary>
        /// Configures the container to inject the given field with provided value.
        /// </summary>
        /// <param name="field">Name of the field to inject.</param>
        /// <param name="value">Value to be injected into the field</param>
        public OptionalField(string field, object value)
            : base(field, value)
        {
        }

        #endregion


        #region Implementation

        public override object? Resolve<TContext>(ref TContext context)
        {
            return base.Resolve(ref context);
        }

        #endregion
    }
}
