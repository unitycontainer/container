using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class PropertyProcessor
    {
        #region Overrides

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(PropertyInfo info, object resolver)
        {
            var value = PreProcessResolver(info, resolver);
            return (ref BuilderContext context) =>
            {
                info.SetValue(context.Existing, context.Resolve(info, context.Name, value));
                return context.Existing;
            };
        }

        #endregion
    }
}
