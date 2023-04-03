using System.Diagnostics;
using System.Reflection;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class MethodProcessor
    {
        /// <inheritdoc/>
        public override void BuildUp<TContext>(ref TContext context)
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

                    BuildUpParameters(ref context, ref current);
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

        /// <inheritdoc/>
        protected override void BuildUpMember<TContext>(ref TContext context, ref InjectionInfoStruct<MethodInfo> info)
            => info.MemberInfo.Invoke(context.Existing, (object[]?)info.DataValue.Value);
    }
}
