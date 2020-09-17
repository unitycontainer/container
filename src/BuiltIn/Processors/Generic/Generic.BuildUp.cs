using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class GenericProcessor : PipelineProcessor
    {
        public override void PreBuildUp(ref PipelineContext pipeline)
        {
            //var type = (Type)context.Manager!.Data!;
            //var name = context.Name;

            //context.Existing = context.Resolve(type, name);
        }
    }
}
