using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class FieldTests : MemberInfoBaseTests<FieldInfo>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NameValidationTest()
        {
            _ = new InjectionField((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InfoNameValidationTest()
        {
            _ = new InjectionField((FieldInfo)null);
        }

        [TestMethod]
        public virtual void OptionalVsRequiredTest()
        {
            var member = new InjectionField("TestProperty", true);
            Assert.IsInstanceOfType(member.Data, typeof(OptionalDependencyAttribute));
        }

        [TestMethod]
        public virtual void OptionalVsRequiredInfo()
        {
            var info = GetType().GetField(nameof(TestField));
            var member = new InjectionField(info, true);
            Assert.IsInstanceOfType(member.Data, typeof(OptionalDependencyAttribute));
        }


        #region Test Data

        public string TestField;

        protected override InjectionMember<FieldInfo, object> GetDefaultMember() => 
            new InjectionField("TestField");

        protected override InjectionMember<FieldInfo, object> GetMember(Type type, int position, object value)
        {
            var info = type.GetFields(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance)
                           .Where(member => !member.IsFamily   && !member.IsPrivate && !member.IsInitOnly)
                           .Take(position)
                           .Last();

            return new InjectionField(info, value);
        }

        #endregion
    }
}
