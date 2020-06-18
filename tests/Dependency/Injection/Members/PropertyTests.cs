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
        public virtual void OptionalVsRequiredTest()
        {
            Assert.IsInstanceOfType(new InjectionProperty("TestProperty").Data, 
                                    typeof(DependencyAttribute));

            Assert.IsInstanceOfType(new InjectionProperty("TestProperty", false).Data,
                                    typeof(DependencyAttribute));

            Assert.IsInstanceOfType(new InjectionProperty("TestProperty", true).Data,
                                    typeof(OptionalDependencyAttribute));
        }
    }
}
