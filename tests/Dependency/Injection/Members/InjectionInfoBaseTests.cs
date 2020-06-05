using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Injection;
using Unity.Policy;
using Unity.Policy.Tests;
using Unity.Resolution;

namespace Injection.Members
{
    public abstract class InjectionInfoBaseTests<TMemberInfo> : InjectionBaseTests<TMemberInfo, object>
        where TMemberInfo : MemberInfo
    {
        #region Fields
        
        private static PropertyInfo MemberTypeProperty = typeof(MemberInfoBase<TMemberInfo>).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                                                                                            .Where(i => i.Name == "MemberType")
                                                                                            .First();
        #endregion


        [TestMethod]
        public virtual void MemberTypeTest()
        {
            // Arrange
            var member = GetDefaultMember();
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(TestClass<>), typeof(TestClass<>), null, ref cast);
            var value = MemberTypeProperty.GetValue(member);

            // Validate
            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(Type));
        }

        [TestMethod]
        public virtual void IsRequiredTest()
        {
            var member = GetDefaultMember();
            Assert.IsInstanceOfType(member.Data, typeof(DependencyAttribute));
        }



        #region Test Data



        #endregion
    }
}
