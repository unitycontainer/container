using Microsoft.VisualStudio.TestTools.UnitTesting;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
#endif


namespace Selection.Injected
{
    public abstract partial class Pattern
    {
#if !UNITY_V4
        [TestMethod("Member with one parameter"), TestProperty(SELECTION, BY_COUNT)]
        public virtual void Select_ByCount_First()
        {
            var target = BaselineTestType.MakeGenericType(TypesForward);

            // Arrange
            Container.RegisterInstance(Name)
                     .RegisterType(target, InjectionMember_Value(new ResolvedParameter()));

            // Act
            var instance = Container.Resolve(target) as SelectionBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);

            var parameters = instance.Data[1] as object[];
            Assert.IsNotNull(parameters);
            Assert.AreEqual(1, parameters.Length);
            Assert.IsInstanceOfType(parameters[0], TypesForward[0]);
        }

        [TestMethod("Generic with one parameter"), TestProperty(SELECTION, BY_COUNT)]
        public virtual void Select_ByCount_First_Generic()
        {
            var target = BaselineTestType.MakeGenericType(TypesForward);

            // Arrange
            Container.RegisterInstance(Name)
                     .RegisterType(BaselineTestType, InjectionMember_Value(new ResolvedParameter()));

            // Act
            var instance = Container.Resolve(target) as SelectionBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);

            var parameters = instance.Data[1] as object[];
            Assert.IsNotNull(parameters);
            Assert.AreEqual(1, parameters.Length);
            Assert.IsInstanceOfType(parameters[0], TypesForward[0]);
        }

        [TestMethod("Reversed with one parameter"), TestProperty(SELECTION, BY_COUNT)]
        public virtual void Select_ByCount_First_Reversed()
        {
            var target = BaselineTestType.MakeGenericType(TypesReverse);

            // Arrange
            Container.RegisterInstance(Name)
                     .RegisterType(target, InjectionMember_Value(new ResolvedParameter()));

            // Act
            var instance = Container.Resolve(target) as SelectionBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);

            var parameters = instance.Data[1] as object[];
            Assert.IsNotNull(parameters);
            Assert.AreEqual(1, parameters.Length);
            Assert.IsInstanceOfType(parameters[0], TypesReverse[0]);
        }

        [TestMethod("Member with two parameters"), TestProperty(SELECTION, BY_COUNT)]
        public virtual void Select_ByCount_FirstTwo()
        {
            var target = BaselineTestType.MakeGenericType(TypesForward);

            // Arrange
            Container.RegisterInstance(Name)
                     .RegisterType(target, InjectionMember_Args(new [] 
                     { 
                         new ResolvedParameter(),
                         new ResolvedParameter()
                     }));

            // Act
            var instance = Container.Resolve(target) as SelectionBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);

            var parameters = instance.Data[4] as object[];
            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Length);
            Assert.IsInstanceOfType(parameters[0], TypesForward[0]);
            Assert.IsInstanceOfType(parameters[1], TypesForward[1]);
        }

        [TestMethod("Generic with two parameters"), TestProperty(SELECTION, BY_COUNT)]
        public virtual void Select_ByCount_FirstTwo_Generic()
        {
            var target = BaselineTestType.MakeGenericType(TypesForward);

            // Arrange
            Container.RegisterInstance(Name)
                     .RegisterType(BaselineTestType, InjectionMember_Args(new[]
                     {
                         new ResolvedParameter(),
                         new ResolvedParameter()
                     }));

            // Act
            var instance = Container.Resolve(target) as SelectionBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);

            var parameters = instance.Data[4] as object[];
            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Length);
            Assert.IsInstanceOfType(parameters[0], TypesForward[0]);
            Assert.IsInstanceOfType(parameters[1], TypesForward[1]);
        }

        [TestMethod("Reversed with two parameters"), TestProperty(SELECTION, BY_COUNT)]
        public virtual void Select_ByCount_FirstTwo_Reversed()
        {
            var target = BaselineTestType.MakeGenericType(TypesReverse);

            // Arrange
            Container.RegisterInstance(Name)
                     .RegisterType(target, InjectionMember_Args(new[]
                     {
                         new ResolvedParameter(),
                         new ResolvedParameter()
                     }));

            // Act
            var instance = Container.Resolve(target) as SelectionBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);

            var parameters = instance.Data[4] as object[];
            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Length);
            Assert.IsInstanceOfType(parameters[0], TypesReverse[0]);
            Assert.IsInstanceOfType(parameters[1], TypesReverse[1]);
        }

        [TestMethod("Mixed with two parameters"), TestProperty(SELECTION, BY_COUNT)]
        public virtual void Select_ByCount_FirstTwo_Mixed()
        {
            var target = BaselineTestType.MakeGenericType(TypesReverse);

            // Arrange
            Container.RegisterInstance(Name)
                     .RegisterType(target, InjectionMember_Args(new object[]
                     {
                         TypesReverse[0],
                         new ResolvedParameter()
                     }));

            // Act
            var instance = Container.Resolve(target) as SelectionBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);

            var parameters = instance.Data[4] as object[];
            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Length);
            Assert.IsInstanceOfType(parameters[0], TypesReverse[0]);
            Assert.IsInstanceOfType(parameters[1], TypesReverse[1]);
        }
#endif
    }
}
