using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;

namespace Injection.Parameters
{
    [TestClass]
    public class ParameterValueTests
    {
        #region Test Data

        private static ParameterInfo ParamInfo =
            typeof(ParameterValueTests).GetMethod(nameof(GenericBaseTestMethod))
                                       .GetParameters()
                                       .First();

        private static ParameterInfo ArrayInfo =
            typeof(ParameterValueTests).GetMethod(nameof(GenericBaseTestMethod))
                                       .GetParameters()
                                       .Last();
        
        private static ParameterInfo AddInfo =
            typeof(List<>).GetMethod("Add")
                          .GetParameters()
                          .First();

        private static ParameterInfo AddStringInfo =
            typeof(List<string>).GetMethod("Add")
                                .GetParameters()
                                .First();

        public void GenericBaseTestMethod<TName, TArray>(TName value, TArray[] array) => throw new NotImplementedException();

        #endregion

        [DataTestMethod]
        [DynamicData(nameof(GetEqualsData), DynamicDataSourceType.Method)]
        public virtual void EqualsTest(ParameterValue parameter, Type type, bool result)
        {
            // Validate
            Assert.AreEqual(result, parameter.Equals(type));
        }

        public class GetType<T>
        { 
        }

        public static IEnumerable<object[]> GetEqualsData()
        {

            yield return new object[] { new InjectionParameter(typeof(List<string>), string.Empty),    typeof(List<>),              false };
            yield return new object[] { new InjectionParameter(typeof(List<string>), string.Empty),    typeof(List<string>),        true };
            yield return new object[] { new InjectionParameter(typeof(List<>), string.Empty),          typeof(List<string>),        false };
            yield return new object[] { new InjectionParameter(typeof(List<>), string.Empty),          typeof(List<>),              true };
            yield return new object[] { new InjectionParameter(AddInfo.ParameterType, string.Empty),   AddInfo.ParameterType,       true };
            yield return new object[] { new InjectionParameter(AddInfo.ParameterType, string.Empty),   AddStringInfo.ParameterType, false };
            yield return new object[] { new InjectionParameter(string.Empty),                          typeof(string),              true  };
            yield return new object[] { new InjectionParameter(typeof(string), string.Empty),          typeof(string),              true  };
            yield return new object[] { new InjectionParameter(string.Empty),                          ParamInfo.ParameterType,     false  };
            yield return new object[] { new InjectionParameter(typeof(string), string.Empty),          ParamInfo.ParameterType,     false  };
            yield return new object[] { new InjectionParameter(ParamInfo.ParameterType, string.Empty), ParamInfo.ParameterType,     true  };


            yield return new object[] { new OptionalParameter(),                              typeof(string), true  };
            yield return new object[] { new OptionalParameter(),                              typeof(object), true  };
            yield return new object[] { new OptionalParameter(),                              typeof(double), true  };
            yield return new object[] { new OptionalParameter(typeof(string)),                typeof(string), true  };
            yield return new object[] { new OptionalParameter(typeof(string)),                typeof(object), true  };
            yield return new object[] { new OptionalParameter(typeof(string)),                typeof(double), false };
            yield return new object[] { new OptionalParameter(string.Empty),                  typeof(string), true  };
            yield return new object[] { new OptionalParameter(string.Empty),                  typeof(object), true  };
            yield return new object[] { new OptionalParameter(string.Empty),                  typeof(double), true  };
            yield return new object[] { new OptionalParameter(typeof(string), string.Empty),  typeof(string), true  };
            yield return new object[] { new OptionalParameter(typeof(string), string.Empty),  typeof(object), true  };
            yield return new object[] { new OptionalParameter(typeof(string), string.Empty),  typeof(double), false };

            
            yield return new object[] { new ResolvedParameter(),                              typeof(string), true  };
            yield return new object[] { new ResolvedParameter(),                              typeof(object), true  };
            yield return new object[] { new ResolvedParameter(),                              typeof(double), true  };
            yield return new object[] { new ResolvedParameter(typeof(string)),                typeof(string), true  };
            yield return new object[] { new ResolvedParameter(typeof(string)),                typeof(object), true  };
            yield return new object[] { new ResolvedParameter(typeof(string)),                typeof(double), false };
            yield return new object[] { new ResolvedParameter(string.Empty),                  typeof(string), true  };
            yield return new object[] { new ResolvedParameter(string.Empty),                  typeof(double), true  };
            yield return new object[] { new ResolvedParameter(typeof(string), string.Empty),  typeof(string), true  };
            yield return new object[] { new ResolvedParameter(typeof(string), string.Empty),  typeof(object), true  };
            yield return new object[] { new ResolvedParameter(typeof(string), string.Empty),  typeof(double), false };


            yield return new object[] { new ResolvedArrayParameter(typeof(string)),                               typeof(string[]), true  };
            yield return new object[] { new ResolvedArrayParameter(typeof(string)),                               typeof(object[]), true  };
            yield return new object[] { new ResolvedArrayParameter(typeof(string)),                               typeof(double[]), false };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), string.Empty),                 typeof(string[]), true  };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), string.Empty),                 typeof(object[]), true  };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), string.Empty),                 typeof(double[]), false };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), typeof(string), string.Empty), typeof(string[]), true  };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), typeof(string), string.Empty), typeof(object[]), true  };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), typeof(string), string.Empty), typeof(double[]), false };


            yield return new object[] { new GenericParameter(ParamInfo.ParameterType.Name),               ParamInfo.ParameterType,               true };
            yield return new object[] { new GenericParameter(ParamInfo.ParameterType.Name, string.Empty), ParamInfo.ParameterType,               true };
            yield return new object[] { new GenericParameter(ParamInfo.ParameterType.Name),               ParamInfo.ParameterType,               true };
            yield return new object[] { new GenericParameter(ParamInfo.ParameterType.Name, string.Empty), ParamInfo.ParameterType,               true };
            yield return new object[] { new GenericParameter(ParamInfo.ParameterType.Name),               ArrayInfo.ParameterType,               false };
            yield return new object[] { new GenericParameter(ParamInfo.ParameterType.Name, string.Empty), ArrayInfo.ParameterType,               false };
            yield return new object[] { new GenericParameter(ParamInfo.ParameterType.Name),               typeof(string),                        false };
            yield return new object[] { new GenericParameter(ParamInfo.ParameterType.Name, string.Empty), typeof(string),                        false };
            yield return new object[] { new GenericParameter("T[]"),                                      AddInfo.ParameterType,                 false };
            yield return new object[] { new GenericParameter("T[]"),                                      AddInfo.ParameterType.MakeArrayType(), true };
            yield return new object[] { new GenericParameter("TArray[]"),                                 ArrayInfo.ParameterType,               true };


            yield return new object[] { new GenericResolvedArrayParameter("T[]"),                                      typeof(List<string>[]), false };
            yield return new object[] { new GenericResolvedArrayParameter(ArrayInfo.ParameterType.Name),               ArrayInfo.ParameterType, true };
            yield return new object[] { new GenericResolvedArrayParameter(ArrayInfo.ParameterType.Name, string.Empty), ArrayInfo.ParameterType, true };
            yield return new object[] { new GenericResolvedArrayParameter(ArrayInfo.ParameterType.Name),               typeof(string),          false };
            yield return new object[] { new GenericResolvedArrayParameter(ArrayInfo.ParameterType.Name, string.Empty), typeof(string),          false };
            yield return new object[] { new GenericResolvedArrayParameter(ArrayInfo.ParameterType.Name),               ParamInfo.ParameterType, false };
            yield return new object[] { new GenericResolvedArrayParameter(ArrayInfo.ParameterType.Name, string.Empty), ParamInfo.ParameterType, false };
        }
    }
}
