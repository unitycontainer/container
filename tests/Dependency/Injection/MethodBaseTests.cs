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
        [DynamicData(nameof(MemberInfoData))]
        public virtual void MemberInfo(string name, object[] data, Type type, int index)
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

        public static IEnumerable<object[]> MemberInfoData
        {
            get 
            {
                yield return new object[]
                {
                    "int, int?",
                    new object[] { 5, null },
                    typeof(TestClass<int,string>),
                    6
                };
                yield return new object[]
                {
                    "Type as exact match",
                    new object[] { typeof(string), null },
                    typeof(TestClass<int,string>),
                    8
                };
                 
                yield return new object[]
                {
                    "Type as higher rank",
                    new object[] { typeof(string) },
                    typeof(TestClass<int,string>),
                    2
                };
                yield return new object[]
                {
                    "Exact match string",
                    new object[] { string.Empty },
                    typeof(TestClass<int,string>),
                    2
                };
                yield return new object[]
                {
                    "(string)null",
                    new object[] { (string)null },
                    typeof(TestClass<int,string>),
                    1
                };
                yield return new object[]
                {
                    "null",
                    new object[] { null },
                    typeof(TestClass<int,string>),
                    1
                };
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
                    "exact match long",
                    new object[] { (long)0 },
                    typeof(TestClass<int,string>),
                    3
                };
                yield return new object[]
                {
                    "exact match long?",
                    new object[] { new long?(0) },
                    typeof(TestClass<int,string>),
                    3
                };
                yield return new object[]
                {
                    "casting to long?",
                    new object[] { (long?)0 },
                    typeof(TestClass<int,string>),
                    3
                };
                yield return new object[]
                {
                    "nullable Int",
                    new object[] { new int?(0) },
                    typeof(TestClass<int,string>),
                    5
                };
                yield return new object[]
                {
                    "exact match Int",
                    new object[] { 0 },
                    typeof(TestClass<int,string>),
                    5
                };
                yield return new object[]
                {
                    "short as object",
                    new object[] { (short)0 },
                    typeof(TestClass<int,string>),
                    1
                };
                 
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


        #region Test Data

        public class TestClass<A,B>
        {
            public TestClass() { }                                      // 0
            public TestClass(object obj) { }                            // 1
            public TestClass(string s1) { }                             // 2
            public TestClass(long lng) { }                              // 3
            public TestClass(long? lng) { }                             // 4
            public TestClass(int intgr) { }                             // 5
            public TestClass(object o1, object o2) { }                  // 6
            public TestClass(string s1, object o2) { }                  // 7
            public TestClass(Type t1,   object o2) { }                  // 8
            public TestClass(int i1, bool b2) { }                       // 9

            public void TestMethod() { }                                // 0
            public void TestMethod(object obj) { }                      // 1
            public void TestMethod(string s1) { }                       // 2
            public void TestMethod(long lng) { }                        // 3
            public void TestMethod(long? lng) { }                       // 4
            public void TestMethod(int intgr) { }                       // 5
            public void TestMethod(object o1, object o2) { }            // 6
            public void TestMethod(string s1, object o2) { }            // 7
            public void TestMethod(Type t1,   object o2) { }            // 8
            public void TestMethod(int i1, bool i2) { }                 // 9
        }

        #endregion
    }
}
