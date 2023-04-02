using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        protected override void AnalyzeInfo<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
        {
            base.AnalyzeInfo(ref context, ref info);
        }
    }
}
