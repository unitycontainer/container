using System.Reflection;

namespace Unity.BuiltIn
{
    public abstract partial class MethodBaseProcessor<TMemberInfo> : MemberProcessor<TMemberInfo, object[]>
                                                 where TMemberInfo : MethodBase
    {
    }
}
