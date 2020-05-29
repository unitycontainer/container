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

        public void TestMethod(List<string> first, List<string>[] last) => throw new NotImplementedException();

        private static ParameterInfo FirstInfo =
            typeof(ResolutionTests).GetMethod(nameof(TestMethod))
                                   .GetParameters()
                                   .First();

        private static ParameterInfo LastInfo =
            typeof(ResolutionTests).GetMethod(nameof(TestMethod))
                                   .GetParameters()
                                   .Last();

        private const string            StringConst = "test";
        private static List<string>     ListConst = new List<string> { StringConst };
        private static List<string>[]   ListArrayConst = new [] { ListConst };
        private static IResolveContext  Context = new TestContect
        {
            { typeof(List<string>),   ListConst },
            { typeof(IList<string>),  ListConst },
            { typeof(List<string>[]), ListArrayConst },
        };

        #endregion


        [DataTestMethod]
        [DynamicData(nameof(GetIResolveData), DynamicDataSourceType.Method)]
        public void IResolveTest(IResolve resolver)
        {
            // Act
            var value = resolver.Resolve(ref Context);

            // Validate
            Assert.IsNotNull(value);
            Assert.AreSame(StringConst, value);
        }


        [DataTestMethod]
        [DynamicData(nameof(GetResolveFactoryTypeData), DynamicDataSourceType.Method)]
        public void ResolveFactoryTypeTest(IResolverFactory<Type> factory)
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
        [DynamicData(nameof(GetResolveFactoryTypeData), DynamicDataSourceType.Method)]
        public void ResolveFactoryInfoTest(IResolverFactory<ParameterInfo> factory)
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
        [DynamicData(nameof(GetResolveFactoryArrayTypeData), DynamicDataSourceType.Method)]
        public void ResolveFactoryTypeArrayTest(IResolverFactory<Type> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(LastInfo.ParameterType);

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref Context) as List<string>[];

            Assert.IsNotNull(value);
            Assert.AreSame(ListArrayConst[0], value[0]);
        }


        [DataTestMethod]
        [DynamicData(nameof(GetResolveFactoryArrayTypeData), DynamicDataSourceType.Method)]
        public void ResolveFactoryInfoArrayTest(IResolverFactory<ParameterInfo> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(LastInfo);

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref Context) as List<string>[];

            Assert.IsNotNull(value);
            Assert.AreSame(ListArrayConst[0], value[0]);
        }


        #region Test Data

        public static IEnumerable<object[]> GetIResolveData()
        {
            yield return new object[] { new InjectionParameter(StringConst) };
            yield return new object[] { new InjectionParameter(typeof(string), StringConst) };
        }

        public static IEnumerable<object[]> GetResolveFactoryTypeData()
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

        public static IEnumerable<object[]> GetResolveFactoryArrayTypeData()
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

        public class TestContect : Dictionary<Type, object>, IResolveContext
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

        #endregion
    }
}
