using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Resolution;


namespace Dependency.Overrides
{
    [TestClass]
    public abstract class ResolverOverrideTests
    {
        #region Initialization

        public const string Name = "name";

        public object OverrideValue { get; } = new object();

        protected abstract ResolverOverride GetResolverOverride();
        protected abstract ResolverOverride GetNamedResolverOverride();

        #endregion


        #region Resolver

        [TestMethod]
        public virtual void GetResolverTest()
        {
            var instance = GetResolverOverride();

            var resolver = instance.GetResolver<IResolveContext>(typeof(ResolverOverrideTests));
            Assert.IsNotNull(resolver);
        }

        [TestMethod]
        public virtual void ResolveTest()
        {
            var instance = GetResolverOverride();
            var context = new TestContext() as IResolveContext;

            var resolver = instance.GetResolver<IResolveContext>(typeof(ResolverOverrideTests));
            Assert.IsNotNull(resolver);

            var value = resolver(ref context);
            Assert.AreSame(OverrideValue, value);
        }

        [TestMethod]
        public virtual void ResolveWithOverrideTest()
        {
            var instance = GetNamedResolverOverride();
            var context = new TestContext() as IResolveContext;

            var resolver = instance.GetResolver<IResolveContext>(typeof(ResolverOverrideTests));
            Assert.IsNotNull(resolver);

            var value = resolver(ref context);
            Assert.AreSame(context, value);
        }

        #endregion


        #region Object

        [TestMethod]
        public void GetHashCodeTest()
        {
            // Act
            var typed = GetResolverOverride().GetHashCode();
            var named = GetNamedResolverOverride().GetHashCode();
            var typedTargeted = GetResolverOverride().OnType<ResolverOverrideTests>().GetHashCode();
            var namedTargeted = GetNamedResolverOverride().OnType<ResolverOverrideTests>().GetHashCode();

            // Produces same result every time
            Assert.AreEqual(typed, GetResolverOverride().GetHashCode());
            Assert.AreEqual(named, GetNamedResolverOverride().GetHashCode());
            Assert.AreEqual(typedTargeted, GetResolverOverride().OnType<ResolverOverrideTests>().GetHashCode());
            Assert.AreEqual(namedTargeted, GetNamedResolverOverride().OnType<ResolverOverrideTests>().GetHashCode());

            // But different value
            Assert.AreNotEqual(typed, named);
            Assert.AreNotEqual(typed, typedTargeted);
            Assert.AreNotEqual(typed, namedTargeted);
            Assert.AreNotEqual(named, typedTargeted);
            Assert.AreNotEqual(named, namedTargeted);
            Assert.AreNotEqual(typedTargeted, namedTargeted);
        }

        [TestMethod]
        public void EqualsTest()
        {
            // Arrange
            var resolver = GetResolverOverride();

            // Act
            var result = resolver.Equals((object)resolver);

            // Validate
            Assert.IsTrue(result);
            Assert.IsFalse(resolver.Equals(GetNamedResolverOverride()));
        }

        [TestMethod]
        public void EqualsWrongTest()
        {
            // Arrange
            var resolver = GetResolverOverride();

            // Validate
            Assert.IsFalse(resolver.Equals(this));
        }

        [TestMethod]
        public void EqualsOperatorTest()
        {
            // Arrange
            var typed = GetResolverOverride();
            var named = GetNamedResolverOverride();

            // Validate
            Assert.IsTrue( typed == GetResolverOverride());
            Assert.IsFalse(typed == named);
            Assert.IsFalse(null == named);
            Assert.IsFalse(typed == null);
        }

        [TestMethod]
        public void NotEqualsOperatorTest()
        {
            // Arrange
            var resolver = GetResolverOverride();

            // Validate
            Assert.IsTrue(resolver != GetNamedResolverOverride());
        }

        #endregion


        #region Test Data

        public class TestResolver : IResolve
        {
            public object Resolve<TContext>(ref TContext context) 
                where TContext : IResolveContext
            {
                return context;
            }
        }

        public class TestContext : IResolveContext
        {
            public IUnityContainer Container => throw new NotImplementedException();

            public Type Type => throw new NotImplementedException();

            public string Name => throw new NotImplementedException();

            public void Clear(Type type, string name, Type policyInterface)
            {
                throw new NotImplementedException();
            }

            public object Get(Type type, Type policyInterface)
            {
                throw new NotImplementedException();
            }

            public object Get(Type type, string name, Type policyInterface)
            {
                throw new NotImplementedException();
            }

            public object Resolve(Type type, string name)
            {
                throw new NotImplementedException();
            }

            public void Set(Type type, Type policyInterface, object policy)
            {
                throw new NotImplementedException();
            }

            public void Set(Type type, string name, Type policyInterface, object policy)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
