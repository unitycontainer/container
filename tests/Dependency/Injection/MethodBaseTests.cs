using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    public abstract class MethodBaseTests<TMemberInfo> : InjectionMemberTests<TMemberInfo, object[]>
                                     where TMemberInfo : MethodBase
    {
        #region IMatchTo

        [DataTestMethod]
        [DynamicData(nameof(CompareToMethodBaseData))]
        [DynamicData(nameof(CompareToParametersData))]
        public virtual void MemberInfoTest(string name, object[] data, Type type, int index)
        {
            // Arrange 
            TMemberInfo[] members = GetMembers(type);
            var member = GetMatchToMember("TestMethod", data);

            // Act
            var info = member.MemberInfo(type);
            var actual = Array.IndexOf(members, info);

            // Validate
            Assert.AreEqual(index, actual);
        }

        public static IEnumerable<object[]> CompareToParametersData
        {
            get
            {
                // Generic
                yield return new object[]
                {
                    "GenericParameter(A), GenericParameter(T[])",
                    new object[] { new GenericParameter("A"), new GenericParameter("T[]") },
                    typeof(TestClass<object[], string>),
                    -1
                };
                yield return new object[]
                {
                    "GenericParameter(A), GenericParameter(B[])",
                    new object[] { new GenericParameter("A"), new GenericParameter("B[]") },
                    typeof(TestClass<object[], string>),
                    6
                };
                yield return new object[]
                {
                    "not matching object[] with GenericParameter(A[])",
                    new object[] { new GenericParameter("A[]") },
                    typeof(TestClass<object[], List<string>>),
                    -1
                };
                yield return new object[]
                {
                    "matching object[] with GenericParameter(A)",
                    new object[] { new GenericParameter("A") },
                    typeof(TestClass<object[], List<string>>),
                    2
                };
                yield return new object[]
                {
                    "matching Array with GenericParameter(A)",
                    new object[] { new GenericParameter("A") },
                    typeof(TestClass<Array, List<string>>),
                    2
                };
                yield return new object[]
                {
                    "matching int with GenericParameter(A)",
                    new object[] { new GenericParameter("A") },
                    typeof(TestClass<int, List<string>>),
                    2
                };
                yield return new object[]
                {
                    "no match",
                    new object[] { new GenericParameter("T") },
                    typeof(TestClass<int, List<string>>),
                    -1
                };
                yield return new object[]
                {
                    "non generics with GenericParameter(A)",
                    new object[] { new GenericParameter("A") },
                    typeof(SolidType),
                    -1
                };
            }
        }

        public static IEnumerable<object[]> CompareToMethodBaseData
        {
            get 
            {
                // Generic
                yield return new object[]
                {
                    "matching generics",
                    new object[] { typeof(List<>) },
                    typeof(TestClass<int, List<string>>),
                    3
                };
                yield return new object[]
                {
                    "matching generics",
                    new object[] { typeof(List<string>) },
                    typeof(TestClass<int, List<string>>),
                    3
                };
                yield return new object[]
                {
                    "closed generic on non-generic",
                    new object[] { typeof(List<string>) },
                    typeof(TestClass<int,string>),
                    1
                };
                // Arrays
                yield return new object[]
                {
                    "Array as type",
                    new object[] { typeof(Array) },
                    typeof(TestClass<Array,object[]>),
                    2
                };
                yield return new object[]
                {
                    "Array bool[]",
                    new object[] { new bool[] { false } },
                    typeof(TestClass<Array,object[]>),
                    2
                };
                yield return new object[]
                {
                    "Array string[]",
                    new object[] { new [] { string.Empty } },
                    typeof(TestClass<Array,object[]>),
                    2
                };
                yield return new object[]
                {
                    "Array object[]",
                    new object[] { new object[] { string.Empty } },
                    typeof(TestClass<Array,object[]>),
                    3
                };
                // Ranking
                yield return new object[]
                {
                    "Type as exact match",
                    new object[] { typeof(string) },
                    typeof(TestClass<int,Type>),
                    3
                };
                yield return new object[]
                {
                    "Type as higher rank",
                    new object[] { typeof(string) },
                    typeof(TestClass<int,string>),
                    3
                };
                yield return new object[]
                {
                    "Exact match string",
                    new object[] { string.Empty },
                    typeof(TestClass<int,string>),
                    3
                };
                // Defaulting to object
                yield return new object[]
                {
                    "Type of TestClass",
                    new object[] { typeof(TestClass<int, string>) },
                    typeof(TestClass<int,string>),
                    1
                };
                yield return new object[]
                {
                    "TestClass instance",
                    new object[] { new TestClass<int, string>() },
                    typeof(TestClass<int,string>),
                    1
                };
                yield return new object[]
                {
                    "short as object",
                    new object[] { (short)0 },
                    typeof(TestClass<int,string>),
                    1
                };
                // Nullable vs non nullable
                yield return new object[]
                {
                    "exact match long",
                    new object[] { (long)0 },
                    typeof(TestClass<long,long?>),
                    2
                };
                yield return new object[]
                {
                    "exact match long?",
                    new object[] { new long?(0) },
                    typeof(TestClass<long,long?>),
                    2
                };
                yield return new object[]
                {
                    "nullable Int",
                    new object[] { new int?(0) },
                    typeof(TestClass<int,string>),
                    2
                };
                yield return new object[]
                {
                    "exact match Int",
                    new object[] { 0 },
                    typeof(TestClass<int,string>),
                    2
                };
                // Data == null
                yield return new object[]
                {
                    "int, bool - negative",
                    new object[] { null, null },
                    typeof(TestClass<int,bool>),
                    -1
                };
                yield return new object[]
                {
                    "null",
                    new object[] { null },
                    typeof(TestClass<int,string>),
                    1
                };
                // No Data
                yield return new object[]
                {
                    "Default Closed Generic",
                    new object[] { },
                    typeof(TestClass<int,string>),
                    0
                };
                yield return new object[]
                {
                    "Default Open Generic",
                    new object[] { },
                    typeof(TestClass<,>),
                    0
                };
                yield return new object[]
                {
                    "Default Open Generic",
                    null,
                    typeof(TestClass<,>),
                    0
                };
            }
        }

        #endregion


        #region Implementation

        protected abstract MethodBase<TMemberInfo> GetMatchToMember(string name, object[] data);
        protected abstract TMemberInfo[] GetMembers(Type type);

        #endregion
    }


    #region Test Data

    public sealed class SolidType
    {
        public SolidType() { }                                      // 0
        public SolidType(object obj) { }                            // 1

        public void TestMethod() { }                                // 0
        public void TestMethod(object obj) { }                      // 1
    }

    public class TestClass<A, B>
    {
        public TestClass() { }                                      // 0
        public TestClass(object obj) { }                            // 1
        public TestClass(A a) { }                                   // 2
        public TestClass(B b) { }                                   // 3
        public TestClass(A a1, B b1) { }                            // 4
        public TestClass(bool b, object[] a1) { }                   // 5
        public TestClass(A a1, B[] b1) { }                          // 6

        public void TestMethod() { }                                // 0
        public void TestMethod(object obj) { }                      // 1
        public void TestMethod(A a) { }                             // 2
        public void TestMethod(B b) { }                             // 3
        public void TestMethod(A a1, B b1) { }                      // 4
        public void TestMethod(bool b, object[] a1) { }             // 5
        public void TestMethod(A a1, B[] b1) { }                    // 6
    }

    #endregion
}
