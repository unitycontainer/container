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

        public static object OverrideValue { get; } = new object();
        public static object ValueOverride { get; } = new TestResolverOverride();

        #endregion


        #region Resolver

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolvers), DynamicDataSourceType.Method)]
        public virtual void ResolveTest(ResolverOverride instance)
        {
            // Arrange
            var context = new TestContext() as IResolveContext;

            // Act
            var resolver = instance.GetResolver<IResolveContext>(typeof(ResolverOverrideTests));
            Assert.IsNotNull(resolver);

            // Validate
            var value = resolver(ref context);
            Assert.AreSame(OverrideValue, value);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetOverriddenResolvers), DynamicDataSourceType.Method)]
        public virtual void ResolveWithOverrideTest(ResolverOverride instance)
        {
            var context = new TestContext() as IResolveContext;

            var resolver = instance.GetResolver<IResolveContext>(typeof(ResolverOverrideTests));
            Assert.IsNotNull(resolver);

            var value = resolver(ref context);
            Assert.AreSame(context, value);
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

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolvers), DynamicDataSourceType.Method)]
        public void EqualsTest(ResolverOverride resolver)
        {
            // Act
            var result = resolver.Equals((object)resolver);

            // Validate
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolvers), DynamicDataSourceType.Method)]
        public void EqualsWrongTest(ResolverOverride resolver)
        {
            // Validate
            Assert.IsFalse(resolver.Equals(this));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolvers), DynamicDataSourceType.Method)]
        public void EqualsOperatorTest(ResolverOverride resolver)
        {
            // Validate
            Assert.IsFalse(null     == resolver);
            Assert.IsFalse(resolver == null);

            Assert.IsTrue(null     != resolver);
            Assert.IsTrue(resolver != null);
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> GetAllResolvers()
        {

            yield return new object[] { new FieldOverride(string.Empty, OverrideValue) };

            yield return new object[] { new PropertyOverride(string.Empty, OverrideValue) };

            yield return new object[] { new DependencyOverride(                          typeof(object),               OverrideValue) };
            yield return new object[] { new DependencyOverride(                                          string.Empty, OverrideValue) };
            yield return new object[] { new DependencyOverride(                          typeof(object), string.Empty, OverrideValue) };
            yield return new object[] { new DependencyOverride(typeof(ResolverOverride), typeof(object), string.Empty, OverrideValue) };

            yield return new object[] { new ParameterOverride(                string.Empty, OverrideValue) };
            yield return new object[] { new ParameterOverride(typeof(object),               OverrideValue) };
            yield return new object[] { new ParameterOverride(typeof(object), string.Empty, OverrideValue) };

        }

        public static IEnumerable<object[]> GetOverriddenResolvers()
        {

            yield return new object[] { new FieldOverride(string.Empty, ValueOverride) };

            yield return new object[] { new PropertyOverride(string.Empty, ValueOverride) };

            yield return new object[] { new DependencyOverride(typeof(object), ValueOverride) };
            yield return new object[] { new DependencyOverride(string.Empty, ValueOverride) };
            yield return new object[] { new DependencyOverride(typeof(object), string.Empty, ValueOverride) };
            yield return new object[] { new DependencyOverride(typeof(ResolverOverride), typeof(object), string.Empty, ValueOverride) };

            yield return new object[] { new ParameterOverride(string.Empty, ValueOverride) };
            yield return new object[] { new ParameterOverride(typeof(object), ValueOverride) };
            yield return new object[] { new ParameterOverride(typeof(object), string.Empty, ValueOverride) };

        }

        public class TestResolverOverride : IResolve
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
