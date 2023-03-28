using System.Reflection;
using Unity.Builder;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TContext, TMemberInfo>
    {
        // TODO: placement
        private bool IsValid(ParameterInfo parameter, ref BuilderContext context)
        {
            if (parameter.IsOut)
            {
                var message = $"Can not inject 'out' parameter {parameter}";
                context.Error(message);
                return false;
            }

            if (parameter.ParameterType.IsByRef)
            {
                var message = $"Can not inject 'ref' parameter {parameter}";
                context.Error(message);
                return false;
            }

            return true;
        }

        private bool IsValid(ref TContext context, ParameterInfo parameter)
        {
            if (parameter.IsOut)
            {
                var message = $"Can not inject 'out' parameter {parameter}";
                context.Error(message);
                return false;
            }

            if (parameter.ParameterType.IsByRef)
            {
                var message = $"Can not inject 'ref' parameter {parameter}";
                context.Error(message);
                return false;
            }

            return true;
        }
    }
}
