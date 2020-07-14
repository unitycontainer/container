using System.Reflection;

namespace Unity.Pipeline
{
    public abstract class MemberProcessor : PipelineProcessor
    {
    }

    public abstract partial class MemberProcessor<TMemberInfo, TData> : MemberProcessor
                                                    where TMemberInfo : MemberInfo
    {
    }
}
