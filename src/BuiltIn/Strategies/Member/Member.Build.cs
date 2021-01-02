using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        protected ImportData Build(ref PipelineContext context, ref ImportInfo import)
        {
            // Local context
            ErrorInfo error = default;
            var contract = import.Contract;
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            // Process/Resolve data
            local.Target = import.Data.ImportType switch
            {
                ImportType.None => context.Container.Resolve(ref local),

                ImportType.Pipeline => local.GetValueRecursively(import.MemberInfo,
                    ((ResolveDelegate<PipelineContext>)import.Data.Value!).Invoke(ref local)),

                // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            // Check for errors
            if (local.IsFaulted)
            { 
                // Set nothing if no default
                if (!import.AllowDefault) return default;

                // Default value
                return import.Default;
            }

            return new ImportData(local.Target, ImportType.Value);
        }


        protected ImportData Build<TContext>(ref TContext context, ref ImportInfo import)
            where TContext : IBuilderContext
        {
            // Local context
            ErrorInfo error = default;
            var contract = import.Contract;
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            // Process/Resolve data
            local.Target = import.Data.ImportType switch
            {
                ImportType.None => context.Container.Resolve(ref local),

                ImportType.Pipeline => local.GetValueRecursively(import.MemberInfo,
                    ((ResolveDelegate<PipelineContext>)import.Data.Value!).Invoke(ref local)),

                // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            // Check for errors
            if (local.IsFaulted)
            {
                // Set nothing if no default
                if (!import.AllowDefault) return default;

                // Default value
                return import.Default;
            }

            return new ImportData(local.Target, ImportType.Value);
        }
    }
}
