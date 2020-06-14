using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;

namespace Injection.Matching
{
    public abstract class MatchingBaseTests<TMember, TMemberInfo, TData>
        where TMemberInfo : MemberInfo
        where TMember     : InjectionMember<TMemberInfo, TData>
    {
        public virtual void Equatable_MemberInfo(Type type, TData data, int position, bool result)
        {
            // Arrange
            var members = GetSupportedMembers(type).ToArray();
            var named  = GetMember(members[position].Name, data);
            var initd  = GetMember(members[position], data);

            // Act
            var member = members[position];
            
            // Validate
            Assert.AreEqual(result, initd.Equals(member));
            Assert.AreEqual(result, named.Equals(member));
        }


        #region Implementation

        protected abstract TMember GetMember(object info, TData data);
        protected abstract IEnumerable<TMemberInfo> GetSupportedMembers(Type type);

        #endregion


        #region Test Data

        protected class TestClass
        {
            #region Fields

            public object Field_0;
            
            #endregion


            #region Constructors

            public TestClass() { }      // 0

            #endregion


            #region Properties

            public object Property_0 { get; set; }

            #endregion


            #region Methods

            public void TestMethod() { }    // 0

            #endregion



        }

        #endregion
    }
}
