using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        private ResolveDelegate<PipelineContext> BuildPipeline(ref PipelineContext context)
        {
            switch (context.Registration?.Category)
            {
                case RegistrationCategory.Type:

                    // Check for Type Mapping
                    var registration = context.Registration;
                    if (null != registration && !registration.RequireBuild && context.Contract.Type != registration.Type)
                    {
                        var type = registration.Type!;
                        var name = context.Contract.Name;

                        return (ref PipelineContext c) => c.Resolve(type, name);
                    }

                    return _policies.BuildTypePipeline(ref context);
                
                case RegistrationCategory.Factory:
                    return _policies.BuildFactoryPipeline(ref context);
                
                case RegistrationCategory.Instance:
                    return _policies.BuildInstancePipeline(ref context);

                default: 
                    throw new NotImplementedException();
            }
        }
    }
}
