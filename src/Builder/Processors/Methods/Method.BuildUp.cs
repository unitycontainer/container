using System.Diagnostics;
using System.Reflection;
using Unity.Injection;

namespace Unity.Processors
{
    public partial class MethodProcessor<TContext>
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
                    BuildUp(ref context, ref enumerator.Current);
                    Execute(ref context, ref enumerator.Current, ref enumerator.Current.DataValue);
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
    }
}
