using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity;
using Unity.Resolution;


namespace Resolution.Overrides
{
    [TestClass]
    public class ResolverOverrideTests
    {
        #region Fields

        public static object TestValue { get; } = new object();
        public static object ResolverOverride { get; } = new TestResolver();
        public static object FactoryOverride { get; } = new TestResolverFactory();

        #endregion


        #region Resolver

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolvers), DynamicDataSourceType.Method)]
        public virtual void ResolverTest(IResolve resolver)
        {
            // Arrange
            var context = new TestContext(typeof(ResolverOverrideTests)) as IResolveContext;

            // Act
            var value = resolver.Resolve(ref context);
            Assert.IsNotNull(value);

            // Validate
            Assert.AreSame(TestValue, value);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetOverriddenResolvers), DynamicDataSourceType.Method)]
        public virtual void ResolverWithOverride(IResolve resolver)
        {
            // Arrange
            var context = new TestContext(typeof(ResolverOverrideTests)) as IResolveContext;

            // Act
            var value = resolver.Resolve(ref context);
            Assert.IsNotNull(value);

            // Validate
            Assert.AreSame(context, value);
        }

        #endregion


        #region Factory

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolvers), DynamicDataSourceType.Method)]
        public virtual void GetResolverTest(ResolverOverride instance)
        {
            // Arrange
            var context = new TestContext() as IResolveContext;

            // Act
            var resolver = instance.GetResolver<IResolveContext>(typeof(ResolverOverrideTests));
            Assert.IsNotNull(resolver);

            // Validate
            var value = resolver(ref context);
            Assert.AreSame(TestValue, value);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetOverriddenResolvers), DynamicDataSourceType.Method)]
        public virtual void GetResolverWithOverride(ResolverOverride instance)
        {
            var context = new TestContext() as IResolveContext;

            var resolver = instance.GetResolver<IResolveContext>(typeof(ResolverOverrideTests));
            Assert.IsNotNull(resolver);

            var value = resolver(ref context);
            Assert.AreSame(context, value);
        }

        #endregion


        #region Validation

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidFieldTest()
        {
            _ = new FieldOverride(null, TestValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidPropertyTest()
        {
            _ = new PropertyOverride(null, TestValue);
        }

        #endregion


        #region Object

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolvers), DynamicDataSourceType.Method)]
        public void GetHashCodeTest(ResolverOverride instance)
        {
            // Produces same result every time
            var typed = instance.GetHashCode();
            Assert.AreEqual(typed, instance.GetHashCode());

            var once = instance.OnType<ResolverOverrideTests>().GetHashCode();
            Assert.AreEqual(once, instance.GetHashCode());

            var twice = instance.OnType(typeof(object)).GetHashCode();
            Assert.AreEqual(twice, instance.GetHashCode());

            // But different value
            Assert.AreNotEqual(typed, once);
            Assert.AreNotEqual(typed, twice);
            Assert.AreNotEqual(twice, once);
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> GetAllResolvers()
        {

            yield return new object[] { new FieldOverride(string.Empty, TestValue) };

            yield return new object[] { new PropertyOverride(string.Empty, TestValue) };

            yield return new object[] { new DependencyOverride(typeof(object), TestValue) };
            yield return new object[] { new DependencyOverride(string.Empty, TestValue) };
            yield return new object[] { new DependencyOverride(typeof(object), string.Empty, TestValue) };
            yield return new object[] { new DependencyOverride(typeof(ResolverOverride), typeof(object), string.Empty, TestValue) };
            yield return new object[] { new DependencyOverride<object>(TestValue) };
            yield return new object[] { new DependencyOverride<object>(string.Empty, TestValue) };

            yield return new object[] { new ParameterOverride(                string.Empty, TestValue) };
            yield return new object[] { new ParameterOverride(typeof(object),               TestValue) };
            yield return new object[] { new ParameterOverride(typeof(object), string.Empty, TestValue) };

        }

        public static IEnumerable<object[]> GetOverriddenResolvers()
        {
            yield return new object[] { new FieldOverride(string.Empty, ResolverOverride) };
            yield return new object[] { new FieldOverride(string.Empty, FactoryOverride) };

            yield return new object[] { new PropertyOverride(string.Empty, ResolverOverride) };
            yield return new object[] { new PropertyOverride(string.Empty, FactoryOverride) };

            yield return new object[] { new DependencyOverride(typeof(object), ResolverOverride) };
            yield return new object[] { new DependencyOverride(string.Empty, ResolverOverride) };
            yield return new object[] { new DependencyOverride(typeof(object), FactoryOverride) };
            yield return new object[] { new DependencyOverride(string.Empty, FactoryOverride) };
            yield return new object[] { new DependencyOverride(typeof(object), string.Empty, ResolverOverride) };
            yield return new object[] { new DependencyOverride(typeof(object), string.Empty, FactoryOverride) };
            yield return new object[] { new DependencyOverride(typeof(ResolverOverride), typeof(object), string.Empty, ResolverOverride) };
            yield return new object[] { new DependencyOverride(typeof(ResolverOverride), typeof(object), string.Empty, FactoryOverride) };
            yield return new object[] { new DependencyOverride<object>(ResolverOverride) };
            yield return new object[] { new DependencyOverride<object>(FactoryOverride) };
            yield return new object[] { new DependencyOverride<object>(string.Empty, ResolverOverride) };
            yield return new object[] { new DependencyOverride<object>(string.Empty, FactoryOverride) };
            yield return new object[] { new DependencyOverride<object>(typeof(ResolverOverrideTests), string.Empty, FactoryOverride) };

            yield return new object[] { new ParameterOverride(string.Empty,   ResolverOverride) };
            yield return new object[] { new ParameterOverride(string.Empty,   FactoryOverride) };
            yield return new object[] { new ParameterOverride(typeof(object), ResolverOverride) };
            yield return new object[] { new ParameterOverride(typeof(object), FactoryOverride) };
            yield return new object[] { new ParameterOverride(typeof(object), string.Empty, ResolverOverride) };
            yield return new object[] { new ParameterOverride(typeof(object), string.Empty, FactoryOverride) };

        }

        public class TestResolverFactory : IResolverFactory<Type>
        {
            public ResolveDelegate<TContext> GetResolver<TContext>(Type info) where TContext : IResolveContext
            {
                return (ref TContext context) => context;
            }
        }

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
            public TestContext() { }

            public TestContext(Type type)
            {
                Type = type;
            }

            public IUnityContainer Container => throw new NotImplementedException();

            public Type Type { get; }

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
