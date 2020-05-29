using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;

namespace Injection.Parameters
{
    public class ParameterBaseData
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

        public static IEnumerable<object[]> GetEqualsAnyTypeData()
        {
            yield return new object[] { new OptionalParameter(), };
            yield return new object[] { new OptionalParameter(string.Empty) };

            yield return new object[] { new ResolvedParameter() };
            yield return new object[] { new ResolvedParameter(string.Empty) };
        }

        public static IEnumerable<object[]> GetEqualsValueTypeData()
        {
            yield return new object[] { new InjectionParameter(0) };
            yield return new object[] { new InjectionParameter(typeof(int), 0) };
            yield return new object[] { new InjectionParameter<int>(0) };

            yield return new object[] { new OptionalParameter(typeof(int)) };
            yield return new object[] { new OptionalParameter<int>() };
            yield return new object[] { new OptionalParameter(typeof(int), string.Empty) };
            yield return new object[] { new OptionalParameter<int>(string.Empty) };

            yield return new object[] { new ResolvedParameter(typeof(int)) };
            yield return new object[] { new ResolvedParameter<int>() };
            yield return new object[] { new ResolvedParameter(typeof(int), string.Empty) };
            yield return new object[] { new ResolvedParameter<int>(string.Empty) };
        }

        public static IEnumerable<object[]> GetEqualsArrayTypeData()
        {
            yield return new object[] { new OptionalParameter(typeof(string[])) };
            yield return new object[] { new OptionalParameter<string[]>() };
            yield return new object[] { new OptionalParameter(typeof(string[]), string.Empty) };
            yield return new object[] { new OptionalParameter<string[]>(string.Empty) };

            yield return new object[] { new ResolvedParameter(typeof(string[])) };
            yield return new object[] { new ResolvedParameter<string[]>() };
            yield return new object[] { new ResolvedParameter(typeof(string[]), string.Empty) };
            yield return new object[] { new ResolvedParameter<string[]>(string.Empty) };

            yield return new object[] { new ResolvedArrayParameter(typeof(string)) };
            yield return new object[] { new ResolvedArrayParameter<string>() };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), "string") };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), typeof(string), "string") };

            yield return new object[] { new InjectionParameter(new string[0]) };
            yield return new object[] { new InjectionParameter(typeof(string[]), new string[0]) };
            yield return new object[] { new InjectionParameter<string[]>(new string[0]) };
        }

        public static IEnumerable<object[]> GetEqualsGenericTypeData()
        {
            yield return new object[] { new InjectionParameter(typeof(List<>), 0) };

            yield return new object[] { new OptionalParameter(typeof(List<>)) };
            yield return new object[] { new OptionalParameter(typeof(List<>), string.Empty) };

            yield return new object[] { new ResolvedParameter(typeof(List<>)) };
            yield return new object[] { new ResolvedParameter(typeof(List<>), string.Empty) };
        }


        public static IEnumerable<object[]> GetEqualsGenericArrayTypeData()
        {
            yield return new object[] { new InjectionParameter(typeof(List<>), 0) };

            yield return new object[] { new OptionalParameter(typeof(List<>)) };
            yield return new object[] { new OptionalParameter(typeof(List<>), string.Empty) };

            yield return new object[] { new ResolvedParameter(typeof(List<>)) };
            yield return new object[] { new ResolvedParameter(typeof(List<>), string.Empty) };

            yield return new object[] { new ResolvedArrayParameter(typeof(List<>)) };
            yield return new object[] { new ResolvedArrayParameter(typeof(List<>), 0) };
            yield return new object[] { new ResolvedArrayParameter(typeof(List<>), typeof(int), 0) };
        }



        public static IEnumerable<object[]> GetEqualsBaseTypeData()
        {
            yield return new object[] { new InjectionParameter(0) };
            yield return new object[] { new InjectionParameter(typeof(int), 0) };
            yield return new object[] { new InjectionParameter<string[]>(new string[0]) };

            yield return new object[] { new OptionalParameter(), };
            yield return new object[] { new OptionalParameter(typeof(int)) };
            yield return new object[] { new OptionalParameter<int>() };
            yield return new object[] { new OptionalParameter(string.Empty) };
            yield return new object[] { new OptionalParameter(typeof(int), string.Empty) };
            yield return new object[] { new OptionalParameter<int>(string.Empty) };


            yield return new object[] { new ResolvedParameter() };
            yield return new object[] { new ResolvedParameter(typeof(int)) };
            yield return new object[] { new ResolvedParameter<int>() };
            yield return new object[] { new ResolvedParameter(string.Empty) };
            yield return new object[] { new ResolvedParameter(typeof(int), string.Empty) };
            yield return new object[] { new ResolvedParameter<int>(string.Empty) };


            yield return new object[] { new ResolvedArrayParameter<int>() };
            yield return new object[] { new ResolvedArrayParameter(typeof(int)) };
            yield return new object[] { new ResolvedArrayParameter(typeof(int), 0) };
            yield return new object[] { new ResolvedArrayParameter(typeof(int), typeof(int), 0) };
        }










    }
}
