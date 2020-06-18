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
        public virtual void OptionalVsRequiredTest()
        {
            Assert.IsInstanceOfType(new InjectionField("TestProperty").Data, 
                                    typeof(DependencyAttribute));

            Assert.IsInstanceOfType(new InjectionField("TestProperty", false).Data,
                                    typeof(DependencyAttribute));

            Assert.IsInstanceOfType(new InjectionField("TestProperty", true).Data,
                                    typeof(OptionalDependencyAttribute));
        }

    }
}
