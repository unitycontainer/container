using System.Reflection;

namespace Unity.Container
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        // TODO: placement
        private bool IsValid(ParameterInfo parameter, ref PipelineContext context)
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
