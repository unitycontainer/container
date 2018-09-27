

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests.TestObjects
{
    // A class that contains another one which has another
    // constructor dependency. Used to validate recursive
    // buildup of constructor dependencies.
    public class ObjectWithTwoConstructorDependencies
    {
        private ObjectWithOneDependency oneDep;

        public ObjectWithTwoConstructorDependencies(ObjectWithOneDependency oneDep)
        {
            this.oneDep = oneDep;
        }

        public ObjectWithOneDependency OneDep
        {
            get { return oneDep; }
        }

        public void Validate()
        {
            Assert.IsNotNull(oneDep);
            oneDep.Validate();
        }
    }
}
