using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;

namespace Injection.Matching
{
    public abstract class MethodBaseTests<TMember, TMemberInfo> : MatchingBaseTests<TMember, TMemberInfo, object[]>
                                              where TMember     : MethodBase<TMemberInfo>
                                              where TMemberInfo : MethodBase
    {
        [DataTestMethod]
        [DynamicData(nameof(EquatableMemberInfoData))]
        public override void Equatable_MemberInfo(Type type, object[] data, int position, bool result) => 
            base.Equatable_MemberInfo(type, data, position, result);


        #region Test Data

        public static IEnumerable<object[]> EquatableMemberInfoData
        {
            get
            {
                yield return new object[] { typeof(TestClass), null, 0, true };
            }
        }

        #endregion
    }
}
