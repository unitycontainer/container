using System.Reflection;
using Unity.Strategies;

namespace Unity.Processors
{
    public class MemberInfoProcessor : BuilderStrategy
    {

    }

    public partial class MemberInfoProcessor<TMemberInfo, TData> : MemberInfoProcessor
                                               where TMemberInfo : MemberInfo
    {
    }
}
