using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.Injection
{
    public class InjectionField : MemberInfoBase<FieldInfo>
    {
        #region Constructors

        /// <summary>
        /// Configure the container to inject the given field name.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="option">Tells Unity if this field is optional.</param>
        public InjectionField(string name, ResolutionOption option = ResolutionOption.Required)
            : base(name, ResolutionOption.Optional == option ? OptionalDependencyAttribute.Instance 
                                                             : (object)DependencyAttribute.Instance)
        {
        }

        /// <summary>
        /// Configure the container to inject the given field name,
        /// using the value supplied.
        /// </summary>
        /// <param name="name">Name of property to inject.</param>
        /// <param name="value">InjectionParameterValue for property.</param>
        public InjectionField(string name, object value)
            : base(name, value)
        {
        }

        #endregion


        #region Overrides

        protected override FieldInfo DeclaredMember(Type type, string name)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0 
            return type.GetTypeInfo().GetDeclaredField(Selection.Name);
#else
            return type.GetField(Selection.Name);
#endif
        }

        public override IEnumerable<FieldInfo> DeclaredMembers(Type type)
        {
            foreach (var member in type.GetDeclaredFields())
            {
                if (!member.IsFamily && !member.IsPrivate &&
                    !member.IsInitOnly && !member.IsStatic)
                    yield return member;
            }
        }

        protected override Type MemberType => Selection.FieldType;

        public override string ToString()
        {
            return Data is DependencyResolutionAttribute
                ? $"Resolve.Field('{Name}')"
                : $"Inject.Field('{Name}', {Data})";
        }

        #endregion
    }
}
