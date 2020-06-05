using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class ConstructorTests : InjectionBaseTests<ConstructorInfo, object[]>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InfoValidationTest()
        {
            _ = new InjectionConstructor((ConstructorInfo)null);
        }

        #region Test Data

        protected override InjectionMember<ConstructorInfo, object[]> GetDefaultMember() => 
            new InjectionConstructor();

        protected override InjectionMember<ConstructorInfo, object[]> GetMember(Type type, int position, object data)
        {
            var info = type.GetTypeInfo()
                           .DeclaredConstructors
                           .Where(ctor => !ctor.IsFamily && !ctor.IsPrivate && !ctor.IsStatic)
                           .Take(position)
                           .Last();

            return new InjectionConstructor(info, data);
        }

        #endregion
    }
}
