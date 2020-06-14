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
            return info switch
            {
                ConstructorInfo ctor => new InjectionConstructor(ctor, data),
                _ => new InjectionConstructor(data),
            };
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
            return info switch
            {
                MethodInfo method => new InjectionMethod(method, data),
                string name       => new InjectionMethod(name, data),
                _ => throw new System.NotImplementedException(),
            };
        }

        protected override IEnumerable<MethodInfo> GetSupportedMembers(Type type)
        {
            return type.SupportedMethods();
        }
    }

    [TestClass]
    public class FieldTests : MemberBaseTests<InjectionField, FieldInfo>
    {
        protected override InjectionField GetMember(object info, object data)
        {
            return info switch
            {
                FieldInfo field => new InjectionField(field, data),
                string name     => new InjectionField(name, data),
                _ => throw new System.NotImplementedException(),
            };
        }

        protected override IEnumerable<FieldInfo> GetSupportedMembers(Type type)
        {
            return type.SupportedFields();
        }
    }


    [TestClass]
    public class PropertyTests : MemberBaseTests<InjectionProperty, PropertyInfo>
    {
        protected override InjectionProperty GetMember(object info, object data)
        {
            return info switch
            {
                PropertyInfo prop => new InjectionProperty(prop, data),
                string name       => new InjectionProperty(name, data),
                _ => throw new System.NotImplementedException(),
            };
        }

        protected override IEnumerable<PropertyInfo> GetSupportedMembers(Type type)
        {
            return type.SupportedProperties();
        }
    }
}
