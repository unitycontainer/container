﻿using System.Reflection;
using Unity.Container;

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
            : base(name, optional ? Defaults.DefaulOptionalResolver 
                                  : Defaults.DefaulRequiredResolver)
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
    }
}
