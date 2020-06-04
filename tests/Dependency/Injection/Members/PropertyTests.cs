using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Unity;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class PropertyTests : InjectionInfoBaseTests<PropertyInfo>
    {
        // TODO: Issue #162
        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void InfoNullTest()
        //{
        //    _ = new InjectionProperty((PropertyInfo)null);
        //}

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidationTest()
        {
            _ = new InjectionProperty(null);
        }


        [TestMethod]
        public virtual void OptionalVsRequiredTest()
        {
            var member = new InjectionProperty("TestProperty", ResolutionOption.Optional);
            Assert.IsInstanceOfType(member.Data, typeof(OptionalDependencyAttribute));
        }

        #region Test Data

        protected override InjectionMember<PropertyInfo, object> GetDefaultMember() => 
            new InjectionProperty("TestProperty");

        #endregion
    }
}
