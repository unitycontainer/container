using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;
using static Injection.Members.InjectionMemberTests;

namespace Injection.Members
{
    public abstract class InjectionBaseTests<TMemberInfo, TData>
        where TMemberInfo : MemberInfo
    {
        [TestMethod]
        public virtual void InitializationTest()
        {
            // Arrange
            var member = GetDefaultMember();
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Validate
            Assert.IsNotNull(member);
            Assert.IsFalse(member.IsInitialized);

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(TestClass), typeof(TestClass), null, ref cast);

            // Validate
            Assert.IsTrue(member.IsInitialized);
        }

        [TestMethod]
        public virtual void DeclaredMembersTest()
        {
            // Act
            var member = GetDefaultMember();
            var members = member.DeclaredMembers(typeof(TestClass))
                                .ToArray();
            // Validate
            Assert.AreEqual(2, members.Length);
        }


        #region MemberInfo

        //[DataTestMethod]
        //[DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        //[ExpectedException(typeof(NullReferenceException))]
        //public virtual void MemberInfoNotInitializedTest(InjectionMember member, MemberInfo info)
        //{
        //    // Arrange
        //    MemberInfo selection = null;

        //    // Act
        //    switch (member)
        //    {
        //        case InjectionMember<MemberInfo, object> injector:
        //            selection = injector.MemberInfo(typeof(PolicySet));
        //            break;

        //        case InjectionMember<MemberInfo, object[]> method:
        //            selection = method.MemberInfo(typeof(PolicySet));
        //            break;
        //    }

        //    // Validate
        //    Assert.AreEqual(info, selection);
        //}

        //[DataTestMethod]
        //[DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        //public virtual void MemberInfoTest(InjectionMember member, MemberInfo info)
        //{
        //    // Arrange
        //    var set = new PolicySet();
        //    var cast = set as IPolicySet;
        //    MemberInfo selection = null;

        //    // Act
        //    member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);

        //    // Validate
        //    Assert.AreEqual(info, selection);

        //    // Validate
        //    switch (member)
        //    {
        //        case InjectionMember<MemberInfo, object> injector:
        //            Assert.AreEqual(info, injector.MemberInfo(typeof(PolicySet)));
        //            break;

        //        case InjectionMember<MemberInfo, object[]> method:
        //            Assert.AreEqual(info, method.MemberInfo(typeof(PolicySet)));
        //            break;
        //    }
        //}

        //[Ignore]
        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public virtual void MemberWrongInfoTest()
        //{
        //    // Arrange
        //    var set = new TestPolicySet();
        //    var cast = set as IPolicySet;
        //    var member = GetInjectionMember();

        //    // Act
        //    member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

        //    // Validate
        //    Assert.IsNull(member.MemberInfo(typeof(TestClassAttribute)));
        //}

        #endregion


        #region Test Data

        protected abstract InjectionMember<TMemberInfo, TData> GetDefaultMember();

        protected class TestClass
        {
            #region Constructors
            static TestClass() { }
            public TestClass() { }
            private TestClass(string _) { }
            protected TestClass(long _) { }
            internal TestClass(int _) { }
            #endregion

            #region Fields

            public readonly string TestReadonlyField;
            static string TestStaticField;
            public string TestField;
            private string TestPrivateField;
            protected string TestProtectedField;
            internal string TestInternalField;

            #endregion

            #region Properties
            public string TestReadonlyProperty { get; }
            static string TestStaticProperty { get; set; }
            public string TestProperty { get; set; }
            private string TestPrivateProperty { get; set; }
            protected string TestProtectedProperty { get; set; }
            internal string TestInternalProperty { get; set; }
            #endregion

            #region Methods
            static void TestMethod() { }
            public void TestMethod(string _) { }
            private void TestMethod(int _) { }
            protected void TestMethod(long _) { }
            internal void TestMethod(object _) { }
            #endregion
        }

        #endregion
    }
}
