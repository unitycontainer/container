using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class FieldTests : InjectionInfoBaseTests<FieldInfo>
    {
        // TODO: Issue #162
        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void InfoNullTest()
        //{
        //    _ = new InjectionField((FieldInfo)null);
        //}

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidationTest()
        {
            _ = new InjectionField(null);
        }

        #region Test Data

        protected override InjectionMember<FieldInfo, object> GetDefaultMember() => 
            new InjectionField("TestField");

        #endregion
    }
}
