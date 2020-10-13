using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class MethodProcessor : ParameterProcessor<MethodInfo>
    {
        private void Build(ref PipelineContext context, MethodInfo info, object?[]? data)
        {
            var parameters = info.GetParameters();

            object?[] arguments = (0 == parameters.Length)
                ? EmptyParametersArray
                : Build(ref context, parameters, data);

            if (context.IsFaulted) return;

            info.Invoke(context.Target, arguments);
        }

        private void Build(ref PipelineContext context, MethodInfo info)
        {
            var parameters = info.GetParameters();

            object?[] arguments = (0 == parameters.Length)
                ? EmptyParametersArray
                : GetDependencies(ref context, parameters);

            if (context.IsFaulted) return;

            info.Invoke(context.Target, arguments);
        }
    }
}
