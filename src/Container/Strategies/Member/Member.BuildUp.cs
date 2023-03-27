using System.Collections;
using System.Diagnostics;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TContext, TMemberInfo, TDependency, TData>
    {
        public virtual void PreBuildUp(ref TContext context)
        {
            Debug.Assert(null != context.Existing, "Target should never be null");
            var members = GetDeclaredMembers(context.Type);

            if (0 == members.Length) return;

            int index, current = 0;
            Span<int> set = stackalloc int[members.Length];
            var injections = GetInjectedMembers(context.Registration);

            // Match injections with members
            foreach (var member in injections ?? (_empty ??= Enumerable.Empty<InjectionMember<TMemberInfo, TData>>()))
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
                var descriptor = new MemberDescriptor<TMemberInfo>(members[i]);

                try
                {

                    ImportProvider.ProvideInfo(ref descriptor);
                    if (0 <= (index = set[i] - 1))
                    {
                        // Add injection, if match found
                        injections![index].ProvideInfo(ref descriptor);
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
                    var @override = context.GetOverride<TMemberInfo, MemberDescriptor<TMemberInfo>>(ref descriptor);
                    if (@override is not null) descriptor.Data = @override.Resolve(ref context);

                    BuildUp(ref context, ref descriptor);

                    Execute(ref context, ref descriptor, ref descriptor.ValueData);
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


        protected virtual void BuildUp<TMember>(ref TContext context, ref MemberDescriptor<TMember> descriptor)
        {
            switch(descriptor.ValueData.Type)
            {
                case ImportType.None:
                    FromContainer(ref context, ref descriptor);
                    break;

                case ImportType.Array:
                    BuildUpArray(ref context, ref descriptor);
                    break;

                default:
                    FromUnknown(ref context, ref descriptor);
                    break;
            };
        }

        protected virtual void BuildUpArray<TMember>(ref TContext context, ref MemberDescriptor<TMember> descriptor)
        {
            Debug.Assert(descriptor.ValueData.Value is not null);
            Debug.Assert(descriptor.ContractType.IsArray);

            var data = (object?[])descriptor.ValueData.Value!;
            var type = descriptor.ContractType.GetElementType()!;

            IList buffer;

            try
            {
                buffer = Array.CreateInstance(type, data.Length);

                for (var i = 0; i < data.Length; i++)
                {
                    var import = descriptor.With(type!, data[i]);

                    switch (import.ValueData.Type)
                    { 
                        case ImportType.None:
                            FromContainer(ref context, ref import);
                            break;

                        case ImportType.Array:
                            BuildUpArray(ref context, ref import);
                            break;

                        case ImportType.Value:
                            break;

                        default:
                            FromUnknown(ref context, ref import);
                            break;
                    }

                    if (context.IsFaulted) {
                        descriptor.ValueData = default;
                        return;
                    }

                    buffer[i] = import.ValueData.Value;
                }
            }
            catch (Exception ex)
            {
                context.Error(ex.Message);
                descriptor.ValueData = default;
                return;
            }

            descriptor.ValueData[ImportType.Value] = buffer;
        }
    }
}
