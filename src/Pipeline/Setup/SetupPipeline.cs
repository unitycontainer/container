using System;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity
{
    public class SetupPipeline : Pipeline 
    {
        protected const string error = "Invalid Pipeline Setup";

        public override ResolveDelegate<BuilderContext>? Build(ref PipelineBuilder builder)
        {
            // Pipeline
            var type = builder.Type;
            var name = builder.Name;
            var pipeline = builder.Pipeline() ?? ((ref BuilderContext c) => throw new ResolutionFailedException(type, name, error));

            return (ref BuilderContext context) =>
            {
                try
                {
                    // Build the type
                    return pipeline(ref context);
                }
                catch (Exception ex) when (ex.InnerException is InvalidRegistrationException &&
                                           null == context.DeclaringType)
                {
                    throw new ResolutionFailedException(context.Type, context.Name,
                        $"Resolution failed with error: {ex.Message}\n\nFor more detailed information run Unity in debug mode: new UnityContainer(enableDiagnostic: true)", ex);
                }
            };
        }
    }
}
