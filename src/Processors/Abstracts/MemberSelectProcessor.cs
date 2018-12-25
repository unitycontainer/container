using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class MemberInfoProcessor<TMemberInfo> : ISelect<TMemberInfo>
                                        where TMemberInfo : MemberInfo
    {
        #region ISelect

        public IEnumerable<object> Select(ref BuilderContext context)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
