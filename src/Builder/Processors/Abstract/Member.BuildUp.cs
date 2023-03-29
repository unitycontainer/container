using System.Collections;
using System.Diagnostics;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TDependency, TData>
    {
        public override void BuildUp(ref TContext context)
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
                var member = members[i];
                var descriptor = new InjectionInfoStruct<TMemberInfo>(member, GetMemberType(member));

                try
                {

                    ProvideInjectionInfo(ref descriptor);
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
                    var @override = context.GetOverride<TMemberInfo, InjectionInfoStruct<TMemberInfo>>(ref descriptor);
                    if (@override is not null) descriptor.Data = @override.Resolve(ref context);

                    BuildUp(ref context, ref descriptor);

                    Execute(ref context, ref descriptor, ref descriptor.DataValue);
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


        protected virtual void BuildUp<TMember>(ref TContext context, ref InjectionInfoStruct<TMember> descriptor)
        {
            switch (descriptor.DataValue.Type)
            {
                case Storage.ValueType.None:
                    FromContainer(ref context, ref descriptor);
                    break;

                case Storage.ValueType.Array:
                    BuildUpArray(ref context, ref descriptor);
                    break;

                default:
                    FromUnknown(ref context, ref descriptor);
                    break;
            };
        }

        protected virtual void BuildUpArray<TMember>(ref TContext context, ref InjectionInfoStruct<TMember> descriptor)
        {
            Debug.Assert(descriptor.DataValue.Value is not null);
            Debug.Assert(descriptor.ContractType.IsArray);

            var data = (object?[])descriptor.DataValue.Value!;
            var type = descriptor.ContractType.GetElementType()!;

            IList buffer;

            try
            {
                buffer = Array.CreateInstance(type, data.Length);

                for (var i = 0; i < data.Length; i++)
                {
                    var import = descriptor.With(type!, data[i]);

                    switch (import.DataValue.Type)
                    { 
                        case Storage.ValueType.None:
                            FromContainer(ref context, ref import);
                            break;

                        case Storage.ValueType.Array:
                            BuildUpArray(ref context, ref import);
                            break;

                        case Storage.ValueType.Value:
                            break;

                        default:
                            FromUnknown(ref context, ref import);
                            break;
                    }

                    if (context.IsFaulted) {
                        descriptor.DataValue = default;
                        return;
                    }

                    buffer[i] = import.DataValue.Value;
                }
            }
            catch (Exception ex)
            {
                context.Error(ex.Message);
                descriptor.DataValue = default;
                return;
            }

            descriptor.DataValue[Storage.ValueType.Value] = buffer;
        }
    }
}
