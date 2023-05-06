using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod, TestProperty(TESTING_SPAN, TRAIT_CONTAINS)]
        public void ContainsEmptyTest()
        {
            // Arrange
            var type = Manager.GetType();

            // Validate
            Assert.IsFalse(Scope.Contains(new Contract( type, type.Name)));
        }
    }
}
