using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Unity;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class PropertyTests : InjectionMemberTests<PropertyInfo, object>
    {
        #region Test Property

        public string TestProperty { get; }

        #endregion


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NameValidationTest()
        {
            _ = new InjectionProperty((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NameValueValidationTest()
        {
            _ = new InjectionProperty(null, string.Empty);
        }

        [TestMethod]
        public void OptionalVsRequiredTest()
        {
            // Validate
            Assert.AreSame(DependencyAttribute.Instance, new InjectionProperty(nameof(TestProperty)).Data);
            Assert.AreSame(DependencyAttribute.Instance, new InjectionProperty(nameof(TestProperty), false).Data);
            Assert.AreSame(OptionalDependencyAttribute.Instance, new InjectionProperty(nameof(TestProperty), true).Data);
        }


        [TestMethod]
        public void MemberInfoTest()
        {
            var info = GetType().GetProperty(nameof(TestProperty));

            // Validate
            Assert.AreSame(info, new InjectionProperty(nameof(TestProperty), false).MemberInfo(typeof(PropertyTests)));
            Assert.AreSame(info, new InjectionProperty(nameof(TestProperty), true).MemberInfo(typeof(PropertyTests)));
            Assert.AreSame(info, new InjectionProperty(nameof(TestProperty)).MemberInfo(typeof(PropertyTests)));
            Assert.AreSame(info, new InjectionProperty(nameof(TestProperty), string.Empty).MemberInfo(typeof(PropertyTests)));
        }

        public override InjectionMember<PropertyInfo, object> GetInjectionMember() => new InjectionProperty(string.Empty);
    }
}
