using System;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// This method returns an array of <see cref="MemberInfo"/> objects implemented
        /// by the <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// Each processor overrides this method and returns appropriate members. 
        /// Constructor processor returns an array of <see cref="ConstructorInfo"/> objects,
        /// Property processor returns objects of type <see cref="PropertyInfo"/>, and etc.
        /// </remarks>
        /// <param name="type"><see cref="Type"/> implementing members</param>
        /// <returns>A <see cref="Span{MemberInfo}"/> of appropriate <see cref="MemberInfo"/> objects</returns>
        protected abstract TMemberInfo[] GetMembers(Type type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool FillImportInfo(TDependency member, ref ImportInfo<TDependency> info)
            => throw new NotImplementedException();
      

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual TMember? GetInjected<TMember>(RegistrationManager? registration) where TMember : class => null;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual Type MemberType(TDependency member) => member.GetType();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual ImportData AsImportData(TDependency info, object? data) => default;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SetValue(TDependency info, object target, object? value) => throw new NotImplementedException();


        public void Build(ref PipelineContext context, in ImportInfo<TMemberInfo> import, in ImportData data)
        {
            if (ImportType.Value == data.DataType)
            {
                SetValue(Unsafe.As<TDependency>(import.Element), context.Target!, data.Value);
                return;
            }

            ErrorInfo error = default;
            var contract = new Contract(import.ContractType, import.ContractName);
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = data.DataType switch
            {
                ImportType.None => context.Container.Resolve(ref local),

                ImportType.Pipeline => local.GetValueRecursively(import.Element,
                    ((ResolveDelegate<PipelineContext>)data.Value!).Invoke(ref local)),

                // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            if (local.IsFaulted) return;

            SetValue(Unsafe.As<TDependency>(import.Element), context.Target!, local.Target);
        }

        public void Build(ref PipelineContext context, in ImportInfo<TDependency> import, in ImportData data)
        {
            if (ImportType.Value == data.DataType)
            {
                SetValue(Unsafe.As<TDependency>(import.Element), context.Target!, data.Value);
                return;
            }

            ErrorInfo error = default;
            // TODO: Store contract
            var contract = new Contract(import.ContractType, import.ContractName);
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = data.DataType switch
            {
                ImportType.None => context.Container.Resolve(ref local),

                ImportType.Pipeline => local.GetValueRecursively(import.Element,
                    ((ResolveDelegate<PipelineContext>)data.Value!).Invoke(ref local)),

                // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            if (local.IsFaulted) return;

            SetValue(Unsafe.As<TDependency>(import.Element), context.Target!, local.Target);
        }

    }
}
