using System.Reflection;

namespace Unity.Processors
{
    public partial class MemberInfoProcessor
    {
    }

    public partial class MemberInfoProcessor<TMemberInfo> : MemberInfoProcessor
                                        where TMemberInfo : MemberInfo
    {
    }
}
