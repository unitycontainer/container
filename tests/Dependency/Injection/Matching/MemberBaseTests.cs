using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;

namespace Injection.Matching
{
    public abstract class MemberBaseTests<TMember, TMemberInfo> : MatchingBaseTests<TMember, TMemberInfo, object>
                                              where TMember     : MemberInfoBase<TMemberInfo>
                                              where TMemberInfo : MemberInfo
    {
        [DataTestMethod]
        [DynamicData(nameof(EquatableMemberInfoData))]
        public override void Equatable_MemberInfo(Type type, object data, int position, bool result) => 
            base.Equatable_MemberInfo(type, data, position, result);


        #region Test Data

        public static IEnumerable<object[]> EquatableMemberInfoData
        {
            get
            {
                yield return new object[] { typeof(TestClass), new object(), 0, true };
            }
        }

        #endregion
    }
}
