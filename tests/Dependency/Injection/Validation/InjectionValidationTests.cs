using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Injection.Validation
{
    [TestClass]
    public class InjectionValidationTests
    {
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [DynamicData(nameof(ValidMembers), DynamicDataSourceType.Method)]
        public void BaselineTest(InjectionMember member, Type _)
        {
            member.Validate(null);
        }

        [DataTestMethod]
        [DynamicData(nameof(ValidMembers), DynamicDataSourceType.Method)]
        public virtual void PassValidation(InjectionMember member, Type type)
        {
            member.Validate(type);
        }

        [DataTestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [DynamicData(nameof(InvalidMembers), DynamicDataSourceType.Method)]
        public virtual void FailVallidation(InjectionMember member, Type type)
        {
            member.Validate(type);
        }


        #region Test Data

        public static IEnumerable<object[]> ValidMembers()
        {
            yield return new object[] { new InjectionConstructor(typeof(AmbiguousClass), typeof(AmbiguousClass)), typeof(AmbiguousClass) };
        }

        public static IEnumerable<object[]> InvalidMembers()
        {
            yield return new object[] { new InjectionConstructor(),                                                 typeof(StaticClass) };
            yield return new object[] { new InjectionConstructor(typeof(AmbiguousClass), typeof(AmbiguousClass)),   typeof(StaticClass) };
            yield return new object[] { new InjectionConstructor(),                                                 typeof(AmbiguousClass) };
            yield return new object[] { new InjectionConstructor(typeof(AmbiguousClass)),                           typeof(AmbiguousClass) };
            yield return new object[] { new InjectionConstructor(new List<object>(),     typeof(AmbiguousClass)),   typeof(AmbiguousClass) };
            yield return new object[] { new InjectionConstructor(new List<object>()),                               typeof(PrivateClass) };
            yield return new object[] { new InjectionConstructor(typeof(AmbiguousClass), typeof(AmbiguousClass)),   typeof(PrivateClass) };
            yield return new object[] { new InjectionConstructor(new List<object>()),                               typeof(ProtectedClass) };
            yield return new object[] { new InjectionConstructor(typeof(AmbiguousClass), typeof(AmbiguousClass)),   typeof(ProtectedClass) };
        }

        public static class StaticClass
        {
            static StaticClass()
            {

            }
        }

        public class AmbiguousClass
        {
            static AmbiguousClass() { }
            public AmbiguousClass(Type _) { }
            public AmbiguousClass(object _) { }
            public AmbiguousClass(Type a, object b) { }
        }

        public class PrivateClass
        {
            static PrivateClass() { }
            private PrivateClass(Type _) { }
            private PrivateClass(object _) { }
        }

        public class ProtectedClass
        {
            static ProtectedClass() { }
            protected ProtectedClass(Type _) { }
            protected ProtectedClass(object _) { }
        }

        #endregion
    }
}
