using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Extension;
using Unity.Specification.Diagnostic.Constructor.Types;

namespace Constructor
{
    [TestClass]
    public class Types : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer()
                .AddExtension(new Diagnostic());
        }
    }
}
