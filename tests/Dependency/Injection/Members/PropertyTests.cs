using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
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

        #region Test Data

        protected override InjectionMember<PropertyInfo, object> GetDefaultMember() => 
            new InjectionProperty("TestProperty");

        #endregion
    }
}
