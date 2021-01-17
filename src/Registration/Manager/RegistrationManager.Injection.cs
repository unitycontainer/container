using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;
using Unity.Storage;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager : ISequenceSegment<InjectionMember<ConstructorInfo, object[]>?>,
                                                        ISequenceSegment<InjectionMember<MethodInfo, object[]>?>,
                                                        ISequenceSegment<InjectionMember<FieldInfo, object>?>,
                                                        ISequenceSegment<InjectionMember<PropertyInfo, object>?>
    {
        #region Injection Constructors

        public InjectionMethodBase<ConstructorInfo>? Constructor { get; private set; }

        InjectionMember<ConstructorInfo, object[]>? ISequenceSegment<InjectionMember<ConstructorInfo, object[]>?>.Next
            => Constructor;

        int ISequenceSegment<InjectionMember<ConstructorInfo, object[]>?>.Length
            => Constructor?.Length ?? 0;

        #endregion


        #region Injection Fields

        public InjectionMemberInfo<FieldInfo>? Fields { get; private set; }

        InjectionMember<FieldInfo, object>? ISequenceSegment<InjectionMember<FieldInfo, object>?>.Next 
            => Fields;

        int ISequenceSegment<InjectionMember<FieldInfo, object>?>.Length 
            => Fields?.Length ?? 0;

        #endregion


        #region Injection Properties

        public InjectionMemberInfo<PropertyInfo>? Properties { get; private set; }


        InjectionMember<PropertyInfo, object>? ISequenceSegment<InjectionMember<PropertyInfo, object>?>.Next 
            => Properties;

        int ISequenceSegment<InjectionMember<PropertyInfo, object>?>.Length 
            => Properties?.Length ?? 0;

        #endregion


        #region Injection Methods

        public InjectionMethodBase<MethodInfo>? Methods { get; private set; }

        InjectionMember<MethodInfo, object[]>? ISequenceSegment<InjectionMember<MethodInfo, object[]>?>.Next 
            => Methods;

        int ISequenceSegment<InjectionMember<MethodInfo, object[]>?>.Length 
            => Methods?.Length ?? 0;

        #endregion


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
                        member.Next = _policies;
                        _policies = member;
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
