using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Unity;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class FieldTests : InjectionMemberTests<FieldInfo, object>
    {
        #region Test Field

        public string TestField;

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NameValidationTest()
        {
            _ = new InjectionField(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NameValueValidationTest()
        {
            _ = new InjectionField(null, string.Empty);
        }

        [TestMethod]
        public void OptionalVsRequiredTest()
        {
            // Validate
            Assert.AreSame(DependencyAttribute.Instance,         new InjectionField(nameof(TestField)).Data);
            Assert.AreSame(DependencyAttribute.Instance,         new InjectionField(nameof(TestField), false).Data);
            Assert.AreSame(OptionalDependencyAttribute.Instance, new InjectionField(nameof(TestField), true).Data);
        }


        [TestMethod]
        public void MemberInfoTest()
        {
            var info = GetType().GetField(nameof(TestField));

            // Validate
            Assert.AreSame(info, new InjectionField(nameof(TestField), false).MemberInfo(typeof(FieldTests)));
            Assert.AreSame(info, new InjectionField(nameof(TestField),  true).MemberInfo(typeof(FieldTests)));
            Assert.AreSame(info, new InjectionField(nameof(TestField)).MemberInfo(typeof(FieldTests)));
            Assert.AreSame(info, new InjectionField(nameof(TestField), string.Empty).MemberInfo(typeof(FieldTests)));
        }

        public override InjectionMember<FieldInfo, object> GetInjectionMember() => new InjectionField(string.Empty);
    }
}
