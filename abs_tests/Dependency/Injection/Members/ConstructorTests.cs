using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class ConstructorTests : MethodBaseTests<ConstructorInfo>
    {
        public override InjectionMember<ConstructorInfo, object[]> GetInjectionMember() => new InjectionConstructor();

        protected override InjectionMethodBase<ConstructorInfo> GetMatchToMember(string name, object[] data) => new InjectionConstructor(data);
        
        protected override ConstructorInfo[] GetMembers(Type type) => type.GetConstructors();

    }
}
