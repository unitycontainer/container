using System;
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

        public override FieldInfo? MemberInfo(Type type)
        {
            if (null != Info && Info.DeclaringType == type) return Info;

            return type.GetField(Name);
        }

        protected override string ToString(bool debug = false)
        {
            if (debug)
            {
                return base.ToString(debug);
            }
            else
            {
                return Data is DependencyResolutionAttribute
                    ? null == Info 
                            ? $"Resolve.Field('{Name}')"        
                            : $"Resolve: '{Info.DeclaringType}.{Name}'"
                    : null == Info
                            ? $"Inject.Field('{Name}', {Data})" 
                            : $"Inject: '{Info.DeclaringType}.{Name}' with '{Data}'";
            }
        }

        #endregion
    }
}
