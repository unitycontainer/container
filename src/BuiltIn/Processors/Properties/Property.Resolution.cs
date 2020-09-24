using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor
    {
        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(PropertyInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<PipelineContext>(info);

            return (ref PipelineContext context) =>
            {
                info.SetValue(context.Data, context.Resolve(info, attribute.Name, resolver));
                return context.Data;
            };
        }

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(PropertyInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
            {
                return (ref PipelineContext context) =>
                {
                    info.SetValue(context.Data, context.Override(info, attribute.Name, data));
                    return context.Data;
                };
            }
            else
            {
                return (ref PipelineContext context) =>
                {
                    info.SetValue(context.Data, context.Resolve(info, attribute.Name, resolver));
                    return context.Data;
                };
            }
        }
    }
}
