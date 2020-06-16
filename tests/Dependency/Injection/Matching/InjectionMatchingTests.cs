using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity;
using Unity.Injection;

namespace Injection.Matching
{
    [TestClass]
    public class ConstructorTests : MethodBaseTests<InjectionConstructor, ConstructorInfo>
    {
        protected override InjectionConstructor GetMember(object info, object[] data)
        {
            return info is ConstructorInfo ctor
                ? new InjectionConstructor(ctor, data)
                : new InjectionConstructor(data);
        }

        protected override IEnumerable<ConstructorInfo> GetSupportedMembers(Type type)
        {
            return type.SupportedConstructors();
        }
    }

    [TestClass]
    public class MethodTests : MethodBaseTests<InjectionMethod, MethodInfo>
    {
        protected override InjectionMethod GetMember(object info, object[] data)
        {
            return info is MethodInfo method
                ? new InjectionMethod(method, data)
                : new InjectionMethod(info as string, data);
        }

        protected override IEnumerable<MethodInfo> GetSupportedMembers(Type type)
        {
            return type.SupportedMethods();
        }
    }

    // TODO: Reenable
    //[TestClass]
    //public class FieldTests : MemberBaseTests<InjectionField, FieldInfo>
    //{
    //    protected override InjectionField GetMember(object info, object data)
    //    {
    //        return info is FieldInfo field
    //            ? new InjectionField(field, data)
    //            : new InjectionField(info as string, data);
    //    }

    //    protected override IEnumerable<FieldInfo> GetSupportedMembers(Type type)
    //    {
    //        return type.SupportedFields();
    //    }
    //}


    //[TestClass]
    //public class PropertyTests : MemberBaseTests<InjectionProperty, PropertyInfo>
    //{
    //    protected override InjectionProperty GetMember(object info, object data)
    //    {
    //        return info is PropertyInfo prop
    //            ? new InjectionProperty(prop, data)
    //            : new InjectionProperty(info as string, data);
    //    }

    //    protected override IEnumerable<PropertyInfo> GetSupportedMembers(Type type)
    //    {
    //        return type.SupportedProperties();
    //    }
    //}
}
