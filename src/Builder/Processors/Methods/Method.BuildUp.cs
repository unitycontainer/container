using System.Diagnostics;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class MethodProcessor<TContext>
    {
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected override void BuildUp<TDescriptor>(ref TContext context, ref TDescriptor info, ref ValueData data)
            => info.MemberInfo.Invoke(context.Existing, (object[]?)data.Value);
    }
}
