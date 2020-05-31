using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class PropertyTests : InjectionBaseTests<PropertyInfo, object>
    {
        #region Test Data

        protected override InjectionMember<PropertyInfo, object> GetDefaultMember() => 
            new InjectionProperty(nameof(TestClass.TestProperty));

        #endregion
    }
}
