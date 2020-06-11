using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class PropertyTests : MemberInfoBaseTests<PropertyInfo>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NameValidationTest()
        {
            _ = new InjectionProperty((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InfoValidationTest()
        {
            _ = new InjectionProperty((PropertyInfo)null);
        }

        [TestMethod]
        public virtual void OptionalVsRequiredTest()
        {
            var member = new InjectionProperty("TestProperty", true);
            Assert.IsInstanceOfType(member.Data, typeof(OptionalDependencyAttribute));
        }

        [TestMethod]
        public virtual void OptionalVsRequiredInfo()
        {
            var info = GetType().GetProperty(nameof(TestProperty));
            var member = new InjectionProperty(info, true);
            Assert.IsInstanceOfType(member.Data, typeof(OptionalDependencyAttribute));
        }

        #region Test Data

        public string TestProperty { get; }

        protected override InjectionMember<PropertyInfo, object> GetDefaultMember() => 
            new InjectionProperty("TestProperty");

        protected override InjectionMember<PropertyInfo, object> GetMember(Type type, int position, object value)
        {
            var info = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                           .Where(member => {
                               if (!member.CanWrite || 0 != member.GetIndexParameters().Length)
                                   return false;

                               var setter = member.GetSetMethod(true);
                               if (setter.IsPrivate || setter.IsFamily)
                                   return false;
                               
                               return true;
                           })
                           .Take(position)
                           .Last();

            return new InjectionProperty(info, value);
        }

        #endregion
    }
}
