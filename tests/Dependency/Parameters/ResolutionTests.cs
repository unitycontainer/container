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

        public const string DefaultValue = "default";

        public static ParameterInfo NoDefaultInfo =
            typeof(TestClass<string>).GetMethod(nameof(TestClass<string>.TestMethod))
                                     .GetParameters()
                                     .First();

        public static ParameterInfo DefaultInfo =
            typeof(TestClass<string>).GetMethod(nameof(TestClass<string>.TestMethod))
                                     .GetParameters()
                                     .Last();

        private static ParameterInfo ListInfo =
            typeof(TestClass<IList<string>>).GetMethod(nameof(TestClass<IList<string>>.TestMethod))
                                            .GetParameters()
                                            .First();
        private static ParameterInfo ArrayInfo =
            typeof(TestClass<IList<string>[]>).GetMethod(nameof(TestClass<IList<string>[]>.TestMethod))
                                            .GetParameters()
                                            .First();
        private static ParameterInfo MultiInfo =
            typeof(TestClass<IList<string>[][]>).GetMethod(nameof(TestClass<IList<string>[][]>.TestMethod))
                                            .GetParameters()
                                            .First();

        private static string             StringConst = "test";
        private static IList<string>      ListConst = new List<string> { StringConst };
        private static IList<string>[]    ArrayConst = new[] { ListConst };
        private static IList<string>[][]  MultiConst = new[] { ArrayConst };

        private static IResolveContext  Context = new DictionaryContext
        {
            { typeof(IList<string>),   ListConst },
            { typeof(IList<string>[]), ArrayConst },
        };

        #endregion

        [TestMethod]
        public void BaseLineTest()
        {
            Assert.AreEqual(typeof(IList<string>[]),   ArrayInfo.ParameterType);
            Assert.AreSame(ArrayConst, Context.Resolve(ArrayInfo.ParameterType, null));
            Assert.AreSame(ArrayConst, Context.Resolve(ArrayInfo.ParameterType, string.Empty));
        }


        #region IResolve

        [DataTestMethod]
        [DynamicData(nameof(GetIResolvers), DynamicDataSourceType.Method)]
        public void IResolveTest(IResolve resolver)
        {
            // Act
            var value = resolver.Resolve(ref Context);

            // Validate
            Assert.IsNotNull(value);
            Assert.AreSame(ArrayConst, value);
        }

        #endregion


        #region IResolverFactory<Type>

        [DataTestMethod]
        [DynamicData(nameof(ResolverFactoryData), DynamicDataSourceType.Method)]
        public void TypeFactoryTest(ParameterValue parameter, ParameterInfo info, object expected)
        {
            // Arrange
            if (!(parameter is IResolverFactory<Type> factory)) return;

            // Act
            var resolver = factory.GetResolver<IResolveContext>(info.ParameterType);

            // Validate
            Assert.IsNotNull(resolver);

            // Resolve
            var value = resolver(ref Context);

            // Validate
            Assert.IsNotNull(value);
            
            if (value is object[] array)
                Assert.AreSame(expected, array[0]);
            else
                Assert.AreSame(expected, value);
        }

        #endregion


        #region IResolverFactory<ParameterInfo>

        [DataTestMethod]
        [DynamicData(nameof(ResolverFactoryData), DynamicDataSourceType.Method)]
        public void InfoFactoryTest(ParameterValue parameter, ParameterInfo info, object expected)
        {
            // Arrange
            if (!(parameter is IResolverFactory<ParameterInfo> factory)) return;

            // Act
            var resolver = factory.GetResolver<IResolveContext>(info);

            // Validate
            Assert.IsNotNull(resolver);

            // Resolve
            var value = resolver(ref Context);

            // Validate
            Assert.IsNotNull(value);
            if (value is object[] array)
                Assert.AreSame(expected, array[0]);
            else
                Assert.AreSame(expected, value);
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> GetIResolvers()
        {
            yield return new object[] { new InjectionParameter(ArrayConst) };
            yield return new object[] { new InjectionParameter(ArrayInfo.ParameterType, ArrayConst) };
        }

        public static IEnumerable<object[]> ResolverFactoryData()
        {
            yield return new object[] { new ResolvedArrayParameter(ArrayInfo.ParameterType, new object[] { ArrayConst }),                MultiInfo, ArrayConst };
            yield return new object[] { new ResolvedParameter(),                                                                         ArrayInfo, ListConst };
            yield return new object[] { new ResolvedParameter(string.Empty),                                                             ArrayInfo, ListConst };
            yield return new object[] { new ResolvedParameter<IList<string>[]>(),                                                        ArrayInfo, ListConst };
            yield return new object[] { new ResolvedParameter<IList<string>[]>(string.Empty),                                            ArrayInfo, ListConst };
            yield return new object[] { new ResolvedParameter(ArrayInfo.ParameterType),                                                  ArrayInfo, ListConst };
            yield return new object[] { new ResolvedParameter(ArrayInfo.ParameterType, string.Empty),                                    ArrayInfo, ListConst };
            yield return new object[] { new ResolvedParameter(ListInfo.ParameterType),                                                   ListInfo,  ListConst };

            yield return new object[] { new OptionalParameter(),                                                                         ArrayInfo, ListConst };
            yield return new object[] { new OptionalParameter(string.Empty),                                                             ArrayInfo, ListConst };
            yield return new object[] { new OptionalParameter(ArrayInfo.ParameterType),                                                  ArrayInfo, ListConst };
            yield return new object[] { new OptionalParameter(ArrayInfo.ParameterType, string.Empty),                                    ArrayInfo, ListConst };
            yield return new object[] { new OptionalParameter<IList<string>[]>(),                                                        ArrayInfo, ListConst };
            yield return new object[] { new OptionalParameter<IList<string>[]>(string.Empty),                                            ArrayInfo, ListConst };
            yield return new object[] { new OptionalParameter(ListInfo.ParameterType),                                                   ListInfo,  ListConst };

            yield return new object[] { new GenericParameter("T"),                                                                       ArrayInfo, ListConst };
            yield return new object[] { new GenericParameter("T", string.Empty),                                                         ArrayInfo, ListConst };
            yield return new object[] { new GenericParameter("T[]"),                                                                     ArrayInfo, ListConst };
            yield return new object[] { new GenericParameter("T[]", string.Empty),                                                       ArrayInfo, ListConst };
                                                                                                                                         
            yield return new object[] { new OptionalGenericParameter("T"),                                                               ArrayInfo, ListConst };
            yield return new object[] { new OptionalGenericParameter("T", string.Empty),                                                 ArrayInfo, ListConst };
            yield return new object[] { new OptionalGenericParameter("T[]"),                                                             ArrayInfo, ListConst };
            yield return new object[] { new OptionalGenericParameter("T[]", string.Empty),                                               ArrayInfo, ListConst };
                                                                                                                                         
            yield return new object[] { new GenericResolvedArrayParameter("T", ListConst),                                               ArrayInfo, ListConst };
            yield return new object[] { new GenericResolvedArrayParameter("T", typeof(IList<string>)),                                   ArrayInfo, ListConst };
            yield return new object[] { new GenericResolvedArrayParameter("T", new InjectionParameter(ListConst)),                       ArrayInfo, ListConst };
            yield return new object[] { new GenericResolvedArrayParameter("T", new ResolvedParameter(typeof(IList<string>))),            ArrayInfo, ListConst };
                                                                                                                                         
            yield return new object[] { new ResolvedArrayParameter(ListInfo.ParameterType, ListConst),                                    ArrayInfo, ListConst };
            yield return new object[] { new ResolvedArrayParameter(ListInfo.ParameterType, typeof(IList<string>)),                        ArrayInfo, ListConst };
            yield return new object[] { new ResolvedArrayParameter(ListInfo.ParameterType, new InjectionParameter(ListConst)),            ArrayInfo, ListConst };
            yield return new object[] { new ResolvedArrayParameter(ListInfo.ParameterType, new ResolvedParameter(typeof(IList<string>))), ArrayInfo, ListConst };
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
            public void TestMethod(T array, string name = DefaultValue) => throw new NotImplementedException();
        }

        #endregion
    }
}
