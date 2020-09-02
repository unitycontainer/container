using System;
using Unity.Pipeline;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class GenericProcessor : PipelineProcessor
    {
        public override void PreBuildUp(ref ResolutionContext context)
        {
            var type = (Type)context.Manager!.Data!;
            var name = context.Name;

            context.Existing = context.Resolve(type, name);
        }
    }
}
