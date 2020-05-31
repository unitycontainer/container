using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class PropertyTests : InjectionInfoBaseTests<PropertyInfo>
    {
        #region Test Data

        protected override InjectionMember<PropertyInfo, object> GetDefaultMember() => 
            new InjectionProperty("TestProperty");

        #endregion
    }
}
