using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager
    {
        public InjectionMember<ConstructorInfo, object[]>[]? Constructors { get; private set; }

        public InjectionMember<FieldInfo, object>[]? Fields { get; private set; }

        public InjectionMember<PropertyInfo, object>[]? Properties { get; private set; }

        public InjectionMember<MethodInfo, object[]>[]? Methods { get; private set; }

        public InjectionMember[]? Other { get; private set; }


        #region Initializers

        public void Inject(params InjectionMember[] members)
        {
            var length = members.Length;
            Span<ushort> constructorsSpan = stackalloc ushort[length];
            Span<ushort> propertiesSpan = stackalloc ushort[length];
            Span<ushort> fieldsSpan = stackalloc ushort[length];
            Span<ushort> methodsSpan = stackalloc ushort[length];
            Span<ushort> otherSpan = stackalloc ushort[length];

            var constructors = 0;
            var properties = 0;
            var fields = 0;
            var methods = 0;
            var other = 0;

            for (var i = 0; i < length; i++)
            {
                var member = members[i];

                switch (member)
                {
                    case InjectionMember<ConstructorInfo, object[]> ctor:
                        constructorsSpan[constructors++] = (ushort)i;
                        break;

                    case InjectionMember<FieldInfo, object> field:
                        fieldsSpan[fields++] = (ushort)i;
                        break;

                    case InjectionMember<PropertyInfo, object> property:
                        propertiesSpan[properties++] = (ushort)i;
                        break;

                    case InjectionMember<MethodInfo, object[]> method:
                        methodsSpan[methods++] = (ushort)i;
                        break;

                    default:
                        otherSpan[other++] = (ushort)i;
                        break;
                }

                RequireBuild |= member.BuildRequired;
            }

            Constructors = 0 == constructors ? null
                : ExtractArray<InjectionMember<ConstructorInfo, object[]>>(members, constructorsSpan.Slice(0, constructors));

            Properties = 0 == properties ? null
                : ExtractArray<InjectionMember<PropertyInfo, object>>(members, propertiesSpan.Slice(0, properties));

            Methods = 0 == methods ? null
                : ExtractArray<InjectionMember<MethodInfo, object[]>>(members, methodsSpan.Slice(0, methods));

            Fields = 0 == fields ? null
                : ExtractArray<InjectionMember<FieldInfo, object>>(members, fieldsSpan.Slice(0, fields));

            Other = 0 == other ? null
                : ExtractArray<InjectionMember>(members, otherSpan.Slice(0, other));
        }

        #endregion


        private T[] ExtractArray<T>(InjectionMember[] members, Span<ushort> span) 
            where T : InjectionMember
        {
            var array = new T[span.Length];
            var count = 0;

            for (var i = 0; i < span.Length; i++)
            {
                array[count++] = Unsafe.As<T>(members[span[i]]);
            }

            return array;
        }
    }
}
