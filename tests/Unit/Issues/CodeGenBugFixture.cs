using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Unity.Tests.v5.Issues
{
    /// <summary>
    /// Test for dynamic method creation and the CLR bug. This test will only
    /// fail if run in a release build!
    /// </summary>
    [TestClass]
    public class CodeGenBugFixture
    {
        [TestMethod]
        public void ResolvedTypeHasStaticConstructorCalled()
        {
            IUnityContainer container = new UnityContainer();

            CodeGenBug result = container.Resolve<CodeGenBug>();
        }
    }

    public class CodeGenBug
    {
        public static readonly object TheStaticObject;

        static CodeGenBug()
        {
            TheStaticObject = new object();
        }

        [InjectionConstructor]
        public CodeGenBug()
            : this(-12, TheStaticObject)
        {
        }

        public CodeGenBug(int i, object parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("Static constructor was not called");
            }
        }
    }
}
