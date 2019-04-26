using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Registration
{
    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Registration.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic());
        }
    }
}
