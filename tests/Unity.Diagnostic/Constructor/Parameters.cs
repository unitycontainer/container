using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Extension;
using Unity.Specification.Diagnostic.Constructor.Parameters;

namespace Constructor
{
    [TestClass]
    public class Parameters : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer()
                .AddExtension(new Diagnostic());
        }
    }
}
