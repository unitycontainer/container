using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity;
using Unity.Injection;
using Unity.Resolution;

namespace Injection.Parameters
{
    [TestClass]
    public class ResolutionTests
    {
        private const int               IntConst = 10;
        private const string            StringConst = "test";
        private static string[]         ArrayConst = new string[] { StringConst };
        private static List<string>     ListConst = new List<string> { StringConst };
        private static List<string>[]   ListArrayConst = new [] { ListConst };
        private static IResolveContext  Context = new TestContect
        {  
            { typeof(int),            IntConst },
            { typeof(string[]),       ArrayConst }, 
            { typeof(string),         StringConst },
            { typeof(List<string>),   ListConst },
            { typeof(List<string>[]), ListArrayConst },
        };

        [DataTestMethod]
        [DynamicData(nameof(GetIResolveData), DynamicDataSourceType.Method)]
        public void IResolveTest(IResolve resolver)
        {
            // Act
            var value = resolver.Resolve(ref Context);

            // Validate
            Assert.IsNotNull(value);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetResolveFactoryValueTypeData), DynamicDataSourceType.Method)]
        public void ResolveValueFactoryTypeTest(IResolverFactory<Type> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(typeof(int));

            // Validate
            Assert.IsNotNull(resolver);
            
            var value = resolver(ref Context);
            Assert.IsNotNull(value);
            Assert.AreEqual(IntConst, value);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetResolveFactoryArrayTypeData), DynamicDataSourceType.Method)]
        public void ResolveFactoryArrayTypeTest(IResolverFactory<Type> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(typeof(string[]));

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref Context) as string[];

            Assert.IsNotNull(value);
            Assert.AreSame(ArrayConst[0], value[0]);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetResolveFactoryGenericTypeData), DynamicDataSourceType.Method)]
        public void ResolveFactoryGenericTypeTest(IResolverFactory<Type> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(typeof(List<string>));

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref Context) as List<string>;

            Assert.IsNotNull(value);
            Assert.AreSame(ListConst, value);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetResolveFactoryTypeData), DynamicDataSourceType.Method)]
        public void ResolveFactoryTypeTest(IResolverFactory<Type> factory)
        {
            // Act
            var resolver = factory.GetResolver<IResolveContext>(typeof(List<string>[]));

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref Context) as List<string>[];

            Assert.IsNotNull(value);
            Assert.AreSame(ListArrayConst[0], value[0]);
        }


        #region Test Data

        public static IEnumerable<object[]> GetIResolveData()
        {
            yield return new object[] { new InjectionParameter(IntConst) };
            yield return new object[] { new InjectionParameter(typeof(int), IntConst) };
        }

        public static IEnumerable<object[]> GetResolveFactoryValueTypeData()
        {
            yield return new object[] { new ResolvedParameter() };
            yield return new object[] { new ResolvedParameter(typeof(int)) };
            yield return new object[] { new ResolvedParameter(string.Empty) };
            yield return new object[] { new ResolvedParameter(typeof(int), string.Empty) };

            yield return new object[] { new OptionalParameter() };
            yield return new object[] { new OptionalParameter(typeof(int)) };
            yield return new object[] { new OptionalParameter(string.Empty) };
            yield return new object[] { new OptionalParameter(typeof(int), string.Empty) };
        }

        public static IEnumerable<object[]> GetResolveFactoryArrayTypeData()
        {
            yield return new object[] { new ResolvedArrayParameter(typeof(string), StringConst) };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), typeof(string), StringConst) };

            yield return new object[] { new ResolvedParameter() };
            yield return new object[] { new ResolvedParameter(typeof(string[])) };
            yield return new object[] { new ResolvedParameter(string.Empty) };
            yield return new object[] { new ResolvedParameter(typeof(string[]), string.Empty) };

            yield return new object[] { new OptionalParameter() };
            yield return new object[] { new OptionalParameter(typeof(string[])) };
            yield return new object[] { new OptionalParameter(string.Empty) };
            yield return new object[] { new OptionalParameter(typeof(string[]), string.Empty) };
        }

        public static IEnumerable<object[]> GetResolveFactoryGenericTypeData()
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

        public static IEnumerable<object[]> GetResolveFactoryTypeData()
        {
            yield return new object[] { new ResolvedArrayParameter(typeof(List<string>), ListConst) };
            yield return new object[] { new ResolvedArrayParameter(typeof(List<string>), typeof(List<string>), ListConst) };

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

            yield return new object[] { new GenericResolvedArrayParameter("T", ListConst) };
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
