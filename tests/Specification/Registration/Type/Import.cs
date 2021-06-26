using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;

namespace Injection
{
    public abstract partial class Pattern
    {

        //[DataTestMethod, DynamicData(nameof(Unsupported_Data), typeof(FixtureBase))]
        //public virtual void Registered_Unresolvable(string test, Type type)
        //{
        //    // Arrange
        //    RegisterUnResolvableTypes();

        //    // Act
        //    var instance = Container.Resolve(type, null);

        //    // Validate
        //    Assert.IsNotNull(instance);
        //    Assert.IsInstanceOfType(instance, type);
        //}

        //[DataTestMethod, DynamicData(nameof(Unsupported_Data), typeof(FixtureBase))]
        //public virtual void Registered_Unresolvable_Import(string test, Type type)
        //{
        //    // Arrange
        //    RegisterUnResolvableTypes();
        //    var target = BaselineTestType.MakeGenericType(type);


        //    // Act
        //    var instance = Container.Resolve(target, null) as FixtureBaseType;

        //    // Validate
        //    Assert.IsNotNull(instance);
        //    Assert.IsInstanceOfType(instance, target);
        //    Assert.AreEqual(Container.Resolve(type, null), instance.Value);
        //}


        //[DataTestMethod, DynamicData(nameof(BuiltInTypes_Data), typeof(FixtureBase))]
        //public virtual void BuiltIn_Interface(string test, Type type)
        //    => Assert_ResolutionSuccess(type);

    }
}
