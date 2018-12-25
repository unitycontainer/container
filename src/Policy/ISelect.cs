using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;

namespace Unity.Policy
{
    public interface ISelect<TMemberInfo> where TMemberInfo : MemberInfo
    {
        IEnumerable<object> Select(ref BuilderContext context);
    }
}
