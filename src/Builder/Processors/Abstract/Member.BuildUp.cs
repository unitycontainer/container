using System.Diagnostics;
using Unity.Injection;
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
                    BuildUp(ref context, ref current, ref current.DataValue);
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

        protected virtual void BuildUp<TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info)
        {
            switch (info.DataValue.Type)
            {
                case DataType.None:
                    FromContainer(ref context, ref info);
                    break;

                case DataType.Array:
                    FromArray(ref context, ref info);
                    break;

                default:
                    FromUnknown(ref context, ref info);
                    break;
            };
        }

        protected virtual void BuildUp<TDescriptor>(ref TContext context, ref TDescriptor info, ref ValueData data)
            where TDescriptor : IInjectionInfo<TMemberInfo> => throw new NotImplementedException();
    }
}
