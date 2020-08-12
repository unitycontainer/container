using System.Reflection;
using Unity.Pipeline;

namespace Unity.BuiltIn
{
    public abstract class MemberProcessor : PipelineProcessor
    {
    }

    public abstract partial class MemberProcessor<TMemberInfo, TData> : MemberProcessor
                                                    where TMemberInfo : MemberInfo
    {
    }
}
