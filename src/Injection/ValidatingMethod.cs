using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.Injection
{
    public static class ValidatingMethod
    {
        public static Func<IEnumerable<MethodInfo>, object[], MethodInfo> Selector =
            (IEnumerable<MethodInfo> members, object[] data) =>
            {

                return null;
            };


        //protected override void ValidateInjectionMember(Type type)
        //{
        //    if (null == MemberInfo)
        //    {
        //        var signature = string.Join(", ", Data?.Select(d => d.ToString()) ?? Enumerable.Empty<string>());
        //        var message = $"No public method with signature {Name}({signature}) is available on type {type}.";
        //        throw new InvalidOperationException(message);
        //    }

        //    if (MemberInfo.IsStatic) ThrowIllegalMember("The method {0}.{1}({2}) is static. Static methods cannot be injected.", type);
        //    if (MemberInfo.IsGenericMethodDefinition) ThrowIllegalMember("The method {0}.{1}({2}) is an open generic method. Open generic methods cannot be injected.", type);
        //    if (MemberInfo.GetParameters().Any(param => param.IsOut)) ThrowIllegalMember("The method {0}.{1}({2}) has at least one out parameter. Methods with out parameters cannot be injected.", type);
        //    if (MemberInfo.GetParameters().Any(param => param.ParameterType.IsByRef)) ThrowIllegalMember("The method {0}.{1}({2}) has at least one ref parameter.Methods with ref parameters cannot be injected.", type);
        //}

    }
}
