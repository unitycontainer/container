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
        /// <param name="fieldName">Name of field to inject.</param>
        public OptionalField(string fieldName)
            : base(fieldName)
        {
        }

        public OptionalField(string fieldName, Type contractType)
            : base(fieldName, contractType)
        {

        }

        public OptionalField(string fieldName, Type contractType, string? contractName)
            : base(fieldName, contractType, contractName)
        {

        }

        /// <summary>
        /// Configures the container to inject the given field with provided value.
        /// </summary>
        /// <param name="fieldName">Name of the field to inject.</param>
        /// <param name="value">Value to be injected into the field</param>
        public OptionalField(string fieldName, object value)
            : base(fieldName, value)
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
