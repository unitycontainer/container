using System.Collections;
using System.Diagnostics;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TData>
    {
        protected virtual void FromArray<TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
        {
            Debug.Assert(info.DataValue.Value is not null);
            Debug.Assert(info.ContractType.IsArray);

            var data = (object?[])info.DataValue.Value!;
            var type = info.ContractType.GetElementType()!;

            IList buffer;

            try
            {
                buffer = Array.CreateInstance(type, data.Length);

                for (var i = 0; i < data.Length; i++)
                {
                    var import = info.With(type!, data[i]);

                    AnalyzeInfo(ref context, ref import);

                    switch (import.DataValue.Type)
                    {
                        case DataType.None:
                            context.Resolve(ref import);
                            break;

                        case DataType.Array:
                            FromArray(ref context, ref import);
                            break;

                        default:
                            break;
                    }

                    if (context.IsFaulted)
                    {
                        info.DataValue = default;
                        return;
                    }

                    buffer[i] = import.DataValue.Value;
                }
            }
            catch (Exception ex)
            {
                context.Error(ex.Message);
                info.DataValue = default;
                return;
            }

            info.DataValue[DataType.Value] = buffer;
        }
    }
}
