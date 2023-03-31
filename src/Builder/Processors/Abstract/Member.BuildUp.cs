using System.Diagnostics;
using System.Reflection;
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

        protected virtual void BuildUpMember(ref TContext context, ref InjectionInfoStruct<TMemberInfo> info)
            => throw new NotImplementedException();
    }
}
