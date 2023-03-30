using System.Collections;
using System.Diagnostics;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TData>
    {
        public override void BuildUp(ref TContext context)
        {
            Debug.Assert(null != context.Existing, "Target should never be null");
            var members = GetDeclaredMembers(context.Type);

            if (0 == members.Length) return;

            try
            {
                var enumerator = SelectMembers(ref context, members);
                while (enumerator.MoveNext())
                {
                    ref var current = ref enumerator.Current;

                    var @override = context.GetOverride<TMemberInfo, InjectionInfoStruct<TMemberInfo>>(ref current);
                    if (@override is not null) current.Data = @override.Resolve(ref context);

                    BuildUp(ref context, ref current);
                    Execute(ref context, ref current, ref current.DataValue);
                }
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
