using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    public class InjectionField : MemberInfoBase<FieldInfo>
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
        /// Configures the container to inject a specified field with a resolved value.
        /// </summary>
        /// <param name="info"><see cref="FieldInfo"/> of the field</param>
        /// <param name="optional">Tells Unity if this field is optional.</param>
        public InjectionField(FieldInfo info, bool optional = false)
            : base(info, optional ? OptionalDependencyAttribute.Instance
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

        /// <summary>
        /// Configures the container to inject the given field with provided value.
        /// </summary>
        /// <param name="info"><see cref="FieldInfo"/> of the field</param>
        /// <param name="value">Value to be injected into the field</param>
        public InjectionField(FieldInfo info, object value)
            : base(info, value)
        { }

        #endregion


        #region Overrides

        protected override FieldInfo? DeclaredMember(Type type, string name) => 
            type.GetField(Selection!.Name);

        public override IEnumerable<FieldInfo> DeclaredMembers(Type type) => 
            type.SupportedFields();

        protected override Type? MemberType => Selection?.FieldType;

        protected override string ToString(bool debug = false)
        {
            if (debug)
            {
                return base.ToString(debug);
            }
            else
            {
                return Data is DependencyResolutionAttribute
                    ? null == Selection 
                            ? $"Resolve.Field('{Name}')"        
                            : $"Resolve: '{Selection.DeclaringType}.{Name}'"
                    : null == Selection 
                            ? $"Inject.Field('{Name}', {Data})" 
                            : $"Inject: '{Selection.DeclaringType}.{Name}' with '{Data}'";
            }
        }

        #endregion
    }
}
