using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{

    [TestClass]
    public class Fields : Unity.Specification.Diagnostic.Field.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation())
                                       .AddExtension(new Diagnostic());
        }
    }
}

namespace Resolved
{

    [TestClass]
    public class Fields : Unity.Specification.Diagnostic.Field.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation())
                                       .AddExtension(new Diagnostic());
        }
    }
}



namespace Compiled.Field
{
    [TestClass]
    public class Attribute : Unity.Specification.Field.Attribute.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceCompillation());
        }
    }

    //[TestClass]
    //public class Injection : Unity.Specification.Field.Injection.SpecificationTests
    //{
    //    public override IUnityContainer GetContainer()
    //    {
    //        return new UnityContainer().AddExtension(new ForceCompillation());
    //    }
    //}


    //[TestClass]
    //public class Overrides : Unity.Specification.Field.Overrides.SpecificationTests
    //{
    //    public override IUnityContainer GetContainer()
    //    {
    //        return new UnityContainer().AddExtension(new ForceCompillation());
    //    }
    //}

}

namespace Resolved.Field
{
    [TestClass]
    public class Attribute : Unity.Specification.Field.Attribute.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceActivation());
        }
    }

    //[TestClass]
    //public class Injection : Unity.Specification.Field.Injection.SpecificationTests
    //{
    //    public override IUnityContainer GetContainer()
    //    {
    //        return new UnityContainer().AddExtension(new ForceActivation());
    //    }
    //}

    //[TestClass]
    //public class Overrides : Unity.Specification.Field.Overrides.SpecificationTests
    //{
    //    public override IUnityContainer GetContainer()
    //    {
    //        return new UnityContainer().AddExtension(new ForceActivation());
    //    }
    //}
}
