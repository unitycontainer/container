﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Field.Compiled
{
    [TestClass]
    public class Attribute : Unity.Specification.Field.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Field.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }


    [TestClass]
    public class Overrides : Unity.Specification.Field.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }
   
}

namespace Field.Activated
{
    [TestClass]
    public class Attribute : Unity.Specification.Field.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Field.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Field.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }
}

namespace Field.Resolved
{
    [TestClass]
    public class Attribute : Unity.Specification.Field.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Field.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Field.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }
}