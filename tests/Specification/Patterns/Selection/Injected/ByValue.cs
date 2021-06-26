using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Injected
{
    public abstract partial class Pattern
    {
        [TestMethod("Matching Type"), TestProperty(SELECTION, BY_VALUE)]
        public virtual void Select_ByValue_Type_Match()
        {
            var target = BaselineTestType.MakeGenericType(TypesForward);

            // Arrange
            Container.RegisterInstance(RegisteredString)
                     .RegisterType(target, InjectionMember_Value(OverriddenString));

            // Act
            var instance = Container.Resolve(target) as SelectionBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);

            var parameters = instance.Data[2] as object[];
            Assert.IsNotNull(parameters);
            Assert.AreEqual(1, parameters.Length);

            var parameter = parameters[0];
            Assert.IsInstanceOfType(parameter, typeof(string));
            Assert.AreEqual(OverriddenString, parameter);
        }

        [TestMethod("Matching ValueType"), TestProperty(SELECTION, BY_VALUE)]
        public virtual void Select_ByValue_ValueType_Match()
        {
            var floatValue = 1.11f;
            var target = BaselineTestType.MakeGenericType(new[] 
            { 
                typeof(int), typeof(string), typeof(float) 
            });

            // Arrange
            Container.RegisterInstance(RegisteredString)
                     .RegisterType(target, InjectionMember_Args(new object[] { InjectedInt, floatValue }));

            // Act
            var instance = Container.Resolve(target) as SelectionBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, target);

            var parameters = instance.Data[6] as object[];
            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Length);

            Assert.IsInstanceOfType(parameters[0], typeof(int));
            Assert.AreEqual(InjectedInt, parameters[0]);

            Assert.IsInstanceOfType(parameters[1], typeof(float));
            Assert.AreEqual(floatValue, parameters[1]);
        }
    }
}
