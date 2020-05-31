using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class FieldTests : InjectionBaseTests<FieldInfo, object>
    {
        #region Test Data

        protected override InjectionMember<FieldInfo, object> GetDefaultMember() => 
            new InjectionField(nameof(TestClass.TestField));

        #endregion
    }
}
