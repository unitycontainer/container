using Unity.Extension;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TData>
    {
        public override void BuildResolver(ref TContext context)
        {
            base.BuildResolver(ref context);
        }
    }
}
