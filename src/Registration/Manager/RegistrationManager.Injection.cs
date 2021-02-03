using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager
    {
        public InjectionMethodBase<ConstructorInfo>? Constructor { get; private set; }

        public InjectionMember<FieldInfo, object>? Fields { get; private set; }

        public InjectionMember<PropertyInfo, object>? Properties { get; private set; }

        public InjectionMethodBase<MethodInfo>? Methods { get; private set; }

        public InjectionMember? Policies { get; private set; }


        #region Initializers

        public void Add(InjectionMember member)
        {
            unsafe
            {
                switch (member)
                {
                    case InjectionMethodBase<ConstructorInfo> ctor:
                        ctor.Next = Constructor;
                        Constructor = ctor;
                        break;

                    case InjectionMemberInfo<FieldInfo> field:
                        field.Next = Fields;
                        Fields = field;
                        break;

                    case InjectionMemberInfo<PropertyInfo> property:
                        property.Next = Properties;
                        Properties = property;
                        break;

                    case InjectionMethodBase<MethodInfo> method:
                        method.Next = Methods;
                        Methods = method;
                        break;

                    default:
                        member.Next = Policies;
                        Policies = member;
                        break;
                }
            }

            RequireBuild |= member.BuildRequired;
        }

        public void Add(IEnumerable<InjectionMember> members)
        {
            foreach (var member in members) Add(member);
        }

        #endregion
    }
}
