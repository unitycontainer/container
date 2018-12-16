using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.Injection
{
    public class InjectionField : MemberInfoMember<FieldInfo>
    {
        #region Constructors

        /// <summary>
        /// Configure the container to inject the given field name,
        /// using the value supplied.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="value">InjectionParameterValue for property.</param>
        public InjectionField(string name, object value = null)
            : base(name, value)
        {
        }

        protected InjectionField(FieldInfo info, object value = null)
            : base(info, value)
        {
        }

        #endregion

        protected override IEnumerable<FieldInfo> DeclaredMembers(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
