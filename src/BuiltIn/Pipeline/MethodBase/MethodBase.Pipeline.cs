using System.Reflection;

namespace Unity.Pipeline
{
    public abstract partial class MethodBaseProcessor<TMemberInfo> : MemberProcessor<TMemberInfo, object[]>
                                                 where TMemberInfo : MethodBase
    {
    }
}
