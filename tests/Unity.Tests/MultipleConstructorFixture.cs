using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Exceptions;
using Unity.Tests.v5.TestSupport;

namespace Unity.Tests.v5
{
    [TestClass]
    public class MultipleConstructorTest
    {
        private IUnityContainer uc1 = new UnityContainer();

        /// <summary>
        /// Test with multiple constructors.
        /// </summary>
        [TestMethod]
        public void MultipleConstructorTestMethod()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<ClassWithMultipleConstructor>();
            AssertHelper.ThrowsException<ResolutionFailedException>(() => container.Resolve<ClassWithMultipleConstructor>());
        }

        internal class ClassWithMultipleConstructor
        {
            private object constructorDependency;

            public ClassWithMultipleConstructor()
            {
            }

            public ClassWithMultipleConstructor(object constructorDependency)
            {
                this.constructorDependency = constructorDependency;
            }

            public ClassWithMultipleConstructor(string s)
            {
                constructorDependency = s;
            }

            public object ConstructorDependency
            {
                get { return constructorDependency; }
            }
        }
    }
}