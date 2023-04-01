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

                    AnalyzeInfo(ref context, ref current);
                    BuildUpInfo(ref context, ref current);
                    BuildUpMember(ref context, ref current);
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

        protected virtual void BuildUpInfo<TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
        {
            switch (info.DataValue.Type)
            {
                case DataType.None:
                    context.Resolve(ref info);
                    break;

                case DataType.Array:
                    BuildUpFromArray(ref context, ref info);
                    break;

                default:
                    break;
            };
        }

        protected virtual void BuildUpMember(ref TContext context, ref InjectionInfoStruct<TMemberInfo> info)
            => throw new NotImplementedException();

        protected virtual void BuildUpFromArray<TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
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
                            BuildUpFromArray(ref context, ref import);
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
