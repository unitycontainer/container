using System;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual TMember? GetInjected<TMember>(RegistrationManager? registration) where TMember : class => null;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual Type MemberType(TDependency member) 
            => throw new NotImplementedException();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SetValue(TDependency info, object target, object? value) => throw new NotImplementedException();


        public void Build(ref PipelineContext context, in ImportInfo<TDependency> import, in ImportData data)
        {
            if (ImportType.Value == data.ImportType)
            {
                SetValue(Unsafe.As<TDependency>(import.Member), context.Target!, data.Value);
                return;
            }

            ErrorInfo error = default;
            var contract = new Contract(import.ContractType, import.ContractName);
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = data.ImportType switch
            {
                ImportType.None => context.Container.Resolve(ref local),

                ImportType.Pipeline => local.GetValueRecursively(import.Member,
                    ((ResolveDelegate<PipelineContext>)data.Value!).Invoke(ref local)),

                // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            if (local.IsFaulted) return;

            SetValue(Unsafe.As<TDependency>(import.Member), context.Target!, local.Target);
        }
    }
}
