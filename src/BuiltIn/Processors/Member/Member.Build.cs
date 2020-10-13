using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public void Build(ref PipelineContext context, in ImportInfo<TMemberInfo> import, in ImportData data)
        {
            if (ImportType.Value == data.DataType)
            {
                SetValue(Unsafe.As<TDependency>(import.Info), context.Target!, data.Value);
                return;
            }

            ErrorInfo error = default;
            var contract = import.Contract;
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = data.DataType switch
            {
                ImportType.None => local.Resolve(),

                ImportType.Pipeline => local.GetValueRecursively(import.Info,
                    ((ResolveDelegate<PipelineContext>)data.Value!).Invoke(ref local)),

                // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            if (local.IsFaulted) return;

            SetValue(Unsafe.As<TDependency>(import.Info), context.Target!, local.Target);
        }
    }
}
