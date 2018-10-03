using Microsoft.Practices.Unity.Tests.TestObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Tests.v5.TestSupport;

namespace Unity.Tests.v5.TestObjects
{
    // An object that has constructor, property, and method injection dependencies.
    public class ObjectWithLotsOfDependencies
    {
        private ILogger ctorLogger;
        private ObjectWithOneDependency dep1;
        private ObjectWithTwoConstructorDependencies dep2;
        private ObjectWithTwoProperties dep3;

        public ObjectWithLotsOfDependencies(ILogger logger, ObjectWithOneDependency dep1)
        {
            this.ctorLogger = logger;
            this.dep1 = dep1;
        }

        [Dependency]
        public ObjectWithTwoConstructorDependencies Dep2
        {
            get { return dep2; }
            set { dep2 = value; }
        }

        [InjectionMethod]
        public void InjectMe(ObjectWithTwoProperties dep3)
        {
            this.dep3 = dep3;
        }

        public void Validate()
        {
            Assert.IsNotNull(ctorLogger);
            Assert.IsNotNull(dep1);
            Assert.IsNotNull(dep2);
            Assert.IsNotNull(dep3);

            dep1.Validate();
            dep2.Validate();
            dep3.Validate();
        }

        public ILogger CtorLogger
        {
            get { return ctorLogger; }
        }

        public ObjectWithOneDependency Dep1
        {
            get { return dep1; }
        }

        public ObjectWithTwoProperties Dep3
        {
            get { return dep3; }
        }
    }
}
