using System;
using System.Collections;
using System.Diagnostics;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            Debug.Assert(null != context.Existing, "Target should never be null");
            var members = GetDeclaredMembers(context.Type);

            if (0 == members.Length) return;

            int index, current = 0;
            Span<int> set = stackalloc int[members.Length];
            var injections = context.OfType<TMemberInfo, TData>();

            // Match injections with members
            for (var member = injections;
                     member is not null;
                     member = (InjectionMember<TMemberInfo, TData>?)member.Next)
            {
                current += 1;

                if (-1 == (index = SelectMember(member, members, ref set)))
                {
                    context.Error($"{member} doesn't match any members on type {context.Type}");
                    return;
                }

                if (0 != set[index]) continue;

                set[index] = current;
            }

            // Process members
            for (var i = 0; i < members.Length && !context.IsFaulted; i++)
            {
                var descriptor = new MemberDescriptor<TContext, TMemberInfo>(members[i]);

                try
                {
                    ImportProvider.ProvideImport<TContext, MemberDescriptor<TContext, TMemberInfo>>(ref descriptor);

                    if (0 <= (index = set[i] - 1))  
                    {
                        // Add injection, if match found
                        injections![index].ProvideImport<TContext, MemberDescriptor<TContext, TMemberInfo>>(ref descriptor);
                        descriptor.IsImport = true;
                    }
                }
                catch (Exception ex)    // Catch errors from custom providers
                {
                    context.Capture(ex);
                    return;
                }

                // Skip if not an import
                if (!descriptor.IsImport) continue;

                try
                {
                    var @override = context.GetOverride<TMemberInfo, MemberDescriptor<TContext, TMemberInfo>>(ref descriptor);
                    if (@override is not null) descriptor.Dynamic = @override.Value;

                    var finalData = BuildUp(ref context, ref descriptor);

                    Execute(ref context, ref descriptor, ref finalData);
                }
                catch (ArgumentException ex)
                {
                    context.Error(ex.Message);
                }
                catch (Exception exception)
                {
                    context.Capture(exception);
                }
            }
        }


        protected virtual ImportData BuildUp<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            return descriptor.ValueData.Type switch
            {
                ImportType.None     => FromContainer(ref context, ref descriptor),
                ImportType.Value    => descriptor.ValueData,
                ImportType.Pipeline => new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                                                     (ResolveDelegate<TContext>)descriptor.ValueData.Value!), ImportType.Value),
                ImportType.Dynamic   => FromDynamic(ref context, ref descriptor),
                ImportType.Arguments => BuildUpArray(ref context, ref descriptor),
                _ => default
            };
        }


        protected virtual ImportData BuildUpArray<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            var data = (IList)descriptor.ValueData.Value!;
            var type = descriptor.ContractType.GetElementType();

            IList buffer;

            try
            {
                buffer = Array.CreateInstance(type!, data.Count);

                for (var i = 0; i < data.Count; i++)
                {
                    var import = descriptor.With(type!, data[i]);

                    var result = import.ValueData.Type switch
                    {
                        ImportType.None => FromContainer(ref context, ref import),
                        ImportType.Value => import.ValueData,
                        ImportType.Arguments => BuildUpArray(ref context, ref import),

                        _ => FromDynamic(ref context, ref import),
                    };

                    buffer[i] = result.Value;
                }
            }
            catch (Exception ex)
            {
                context.Error(ex.Message);
                return default;
            }

            return new ImportData(buffer, ImportType.Value);
        }

    }
}
