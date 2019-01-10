using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Injection
{
    class ValidatingProperty
    {

        protected  void ValidateInjectionMember(Type type)
        {

            //if (null == Data || Data is DependencyResolutionAttribute) return;

            //if (!Matches(Data, MemberType))
            //{
            //    throw new InvalidOperationException(
            //        $"Type of injector {Name} does not match member type '{MemberType}'");
            //}
        }


        //protected override void ValidateInjectionMember(Type type)
        //{
        //    base.ValidateInjectionMember(type);

        //    if (!MemberInfo.CanWrite)
        //    {
        //        throw new InvalidOperationException(
        //            $"The property {MemberInfo.Name} on type {MemberInfo.DeclaringType} is not settable.");
        //    }

        //    if (MemberInfo.GetIndexParameters().Length > 0)
        //    {
        //        throw new InvalidOperationException(
        //            $"The property {MemberInfo.Name} on type {MemberInfo.DeclaringType} is an indexer. Indexed properties cannot be injected.");
        //    }
        //}


        //protected virtual void ValidateInjectionMember(Type type)
        //{
        //    if (null != MemberInfo) return;

        //    // TODO: 5.9.0 Implement correct error message
        //    var signature = "xxx";//string.Join(", ", _arguments?.FromType(t => t.Name) ?? );
        //    var message = $"The type {type.FullName} does not have a {typeof(TMemberInfo).Name} that takes these parameters ({signature}).";
        //    throw new InvalidOperationException(message);
        //}

    }
}
