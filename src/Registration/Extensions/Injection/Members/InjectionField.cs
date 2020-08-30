using System;
using System.Reflection;

namespace Unity.Injection
{
    public class InjectionField : InjectionMember<FieldInfo, object>
    {
        #region Constructors

        /// <summary>
        /// Configures the container to inject a specified field with a resolved value.
        /// </summary>
        /// <param name="name">Name of field to inject.</param>
        /// <param name="optional">Tells Unity if this field is optional.</param>
        public InjectionField(string name, bool optional = false)
            : base(name, optional ? OptionalDependencyAttribute.Instance 
                                  : (object)DependencyAttribute.Instance)
        {
        }

        /// <summary>
        /// Configures the container to inject the given field with provided value.
        /// </summary>
        /// <param name="name">Name of the field to inject.</param>
        /// <param name="value">Value to be injected into the field</param>
        public InjectionField(string name, object value)
            : base(name, value)
        {
        }

        #endregion


        #region Overrides

        public override FieldInfo? MemberInfo(Type type) => type.GetField(Name);

        #endregion
    }
}
