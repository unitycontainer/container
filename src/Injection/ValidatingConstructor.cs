using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.Injection
{
    public static class ValidatingConstructor
    {
        public static Func<IEnumerable<ConstructorInfo>, object[], ConstructorInfo> Selector = 
            (IEnumerable<ConstructorInfo> members, object[] data) =>
        {
            ConstructorInfo MemberInfo = null;

            foreach (var member in members)
            {
                if (!data.MatchMemberInfo(member)) continue;
                if (null != MemberInfo) ThrowAmbiguousMember(MemberInfo, MemberInfo.DeclaringType);

                MemberInfo = member;
            }



            return null;
        };


        static void ThrowAmbiguousMember(ConstructorInfo info, Type type)
        {
            // TODO: 5.9.0 Proper error message
            var signature = "xxx";//string.Join(", ", _arguments?.FromType(t => t.Name) ?? );
            var message = $"The type {type.FullName} does not have a {typeof(ConstructorInfo).Name} that takes these parameters ({signature}).";
            throw new InvalidOperationException(message);
        }


        static void ValidateInjectionMember(Type type)
        {
            //if (null != MemberInfo) return;

            //var signature = string.Join(", ", Data?.Select(d => d.ToString()) ?? Enumerable.Empty<string>());
            //var message = $"No public constructor with signature ctor({signature}) is available on type {type}.";
            //throw new InvalidOperationException(message);
        }


        //static void ThrowAmbiguousMember(TMemberInfo info, Type type)
        //{
        //    // TODO: 5.9.0 Proper error message
        //    var signature = "xxx";//string.Join(", ", _arguments?.FromType(t => t.Name) ?? );
        //    var message = $"The type {type.FullName} does not have a {typeof(TMemberInfo).Name} that takes these parameters ({signature}).";
        //    throw new InvalidOperationException(message);
        //}

    }
}
