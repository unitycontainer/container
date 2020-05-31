using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Injection;
using Unity.Resolution;

namespace Injection.Parameters
{
    [TestClass]
    public class ResolutionTests
    {
        #region Fields

        private static ParameterInfo FirstInfo =
            typeof(TestClass<List<string>>).GetMethod(nameof(TestClass<List<string>>.FirstTestMethod))
                                           .GetParameters()
                                           .First();

        private static ParameterInfo ObjectInfo =
            typeof(TestClass<List<string>>).GetMethod(nameof(TestClass<List<string>>.TestMethod))
                                           .GetParameters()
                                           .First();

        private static ParameterInfo ArrayInfo =
            typeof(TestClass<List<string>>).GetMethod(nameof(TestClass<List<string>>.TestMethod))
                                           .GetParameters()
                                           .Last();

        private const string            StringConst = "test";
        private static List<string>     ListConst = new List<string> { StringConst };
        private static List<string>[]   ListArrayConst = new [] { ListConst };
        private static IResolveContext  Context = new DictionaryContext
        {
            { typeof(object),         ListArrayConst },
            { typeof(List<string>),   ListConst },
            { typeof(IList<string>),  ListConst },
            { typeof(List<string>[]), ListArrayConst },
        };

        #endregion


        #region IResolve

        [DataTestMethod]
        [DynamicData(nameof(GetIResolvers), DynamicDataSourceType.Method)]
        public void IResolveTest(IResolve resolver)
        {
            // Act
            var value = resolver.Resolve(ref Context);

            // Validate
            Assert.IsNotNull(value);
            Assert.AreSame(StringConst, value);
        }

        #endregion


        #region IResolverFactory<Type>


        [DataTestMethod]
        [DynamicData(nameof(GetResolveFactories), DynamicDataSourceType.Method)]
        public void TypeFactoryTest(IResolverFactory<Type> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(FirstInfo.ParameterType);

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref Context) as List<string>;

            Assert.IsNotNull(value);
            Assert.AreSame(ListConst, value);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolveFactories), DynamicDataSourceType.Method)]
        public void TypeArrayTest(IResolverFactory<Type> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(ArrayInfo.ParameterType);

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref Context) as List<string>[];

            Assert.IsNotNull(value);
            Assert.AreSame(ListArrayConst[0], value[0]);
        }

        [Ignore] // TODO: Issue #148 
        [DataTestMethod]
        [DynamicData(nameof(GetAllResolveFactories), DynamicDataSourceType.Method)]
        public void TypeObjectTest(IResolverFactory<Type> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(ObjectInfo.ParameterType);

            // Validate
            Assert.IsNotNull(resolver);

            var array = resolver(ref Context) as List<string>[];

            Assert.IsNotNull(array);
            Assert.IsTrue(0 < array.Length);
            Assert.AreEqual(ListArrayConst[0], array[0]);
        }

        #endregion


        #region IResolverFactory<ParameterInfo>

        [DataTestMethod]
        [DynamicData(nameof(GetResolveFactories), DynamicDataSourceType.Method)]
        public void InfoFactoryTest(IResolverFactory<ParameterInfo> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(FirstInfo);

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref Context) as List<string>;

            Assert.IsNotNull(value);
            Assert.AreSame(ListConst, value);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllResolveFactories), DynamicDataSourceType.Method)]
        public void InfoArrayTest(IResolverFactory<ParameterInfo> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(ArrayInfo);

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref Context) as List<string>[];

            Assert.IsNotNull(value);
            Assert.AreSame(ListArrayConst[0], value[0]);
        }

        [Ignore] // TODO: Issue #148 
        [DataTestMethod]
        [DynamicData(nameof(GetAllResolveFactories), DynamicDataSourceType.Method)]
        public void InfoObjectTest(IResolverFactory<ParameterInfo> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(ObjectInfo);

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref Context) as List<string>[];

            Assert.IsNotNull(value);
            Assert.AreSame(ListArrayConst[0], value[0]);
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> GetIResolvers()
        {
            yield return new object[] { new InjectionParameter(StringConst) };
            yield return new object[] { new InjectionParameter(typeof(string), StringConst) };
        }

        public static IEnumerable<object[]> GetResolveFactories()
        {
            yield return new object[] { new ResolvedParameter() };
            yield return new object[] { new ResolvedParameter(typeof(List<string>)) };
            yield return new object[] { new ResolvedParameter<List<string>>() };
            yield return new object[] { new ResolvedParameter(string.Empty) };
            yield return new object[] { new ResolvedParameter(typeof(List<string>), string.Empty) };
            yield return new object[] { new ResolvedParameter<List<string>>(string.Empty) };

            yield return new object[] { new OptionalParameter() };
            yield return new object[] { new OptionalParameter(typeof(List<string>)) };
            yield return new object[] { new OptionalParameter<List<string>>() };
            yield return new object[] { new OptionalParameter(string.Empty) };
            yield return new object[] { new OptionalParameter(typeof(List<string>), string.Empty) };
            yield return new object[] { new OptionalParameter<List<string>>(string.Empty) };

            yield return new object[] { new GenericParameter("T") };
            yield return new object[] { new GenericParameter("T", string.Empty) };

            yield return new object[] { new OptionalGenericParameter("T") };
            yield return new object[] { new OptionalGenericParameter("T", string.Empty) };
        }

        public static IEnumerable<object[]> GetAllResolveFactories()
        {
            yield return new object[] { new ResolvedArrayParameter(typeof(List<string>), ListConst,                                         // Constant
                                                                                         new InjectionParameter(ListConst),                 // IResolve
                                                                                        // TODO: typeof(IList<string>),                             // Type    
                                                                                         new ResolvedParameter(typeof(List<string>))) };    // Factory
            yield return new object[] { new GenericResolvedArrayParameter("T", ListConst,                                                   // Constant
                                                                               new InjectionParameter(ListConst),                           // IResolve
                                                                               typeof(IList<string>),                                       // Type    
                                                                               new ResolvedParameter(typeof(List<string>))) };              // Factory

            yield return new object[] { new ResolvedArrayParameter(typeof(List<string>), typeof(List<string>), ListConst, 
                                                                                                               new ResolvedParameter(typeof(List<string>))) };
            yield return new object[] { new ResolvedParameter() };
            yield return new object[] { new ResolvedParameter(typeof(List<string>[])) };
            yield return new object[] { new ResolvedParameter<List<string>[]>() };
            yield return new object[] { new ResolvedParameter(string.Empty) };
            yield return new object[] { new ResolvedParameter(typeof(List<string>[]), string.Empty) };
            yield return new object[] { new ResolvedParameter<List<string>[]>(string.Empty) };

            yield return new object[] { new OptionalParameter() };
            yield return new object[] { new OptionalParameter(typeof(List<string>[])) };
            yield return new object[] { new OptionalParameter<List<string>[]>() };
            yield return new object[] { new OptionalParameter(string.Empty) };
            yield return new object[] { new OptionalParameter(typeof(List<string>[]), string.Empty) };
            yield return new object[] { new OptionalParameter<List<string>[]>(string.Empty) };

            yield return new object[] { new GenericParameter("T[]") };
            yield return new object[] { new GenericParameter("T[]", string.Empty) };

            yield return new object[] { new OptionalGenericParameter("T[]") };
            yield return new object[] { new OptionalGenericParameter("T[]", string.Empty) };
        }

        public class DictionaryContext : Dictionary<Type, object>, IResolveContext
        {
            public IUnityContainer Container => throw new NotImplementedException();

            public Type Type => throw new NotImplementedException();

            public string Name => throw new NotImplementedException();

            public void Clear(Type type, string name, Type policyInterface) => throw new NotImplementedException();

            public object Get(Type type, Type policyInterface) => throw new NotImplementedException();

            public object Get(Type type, string name, Type policyInterface) => throw new NotImplementedException();

            public object Resolve(Type type, string name)
            {
                return this[type];
            }

            public void Set(Type type, Type policyInterface, object policy) => throw new NotImplementedException();

            public void Set(Type type, string name, Type policyInterface, object policy) => throw new NotImplementedException();
        }

        public class TestClass<T>
        {
            public void FirstTestMethod(T first) => throw new NotImplementedException();
            public void TestMethod(object second, T[] last) => throw new NotImplementedException();
        }

        #endregion
    }
}
