using System;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class FieldsProcessor
    {
        #region Overrides

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info, object resolver)
        {
            var value = PreProcessResolver(info, resolver);
            return (ref BuilderContext context) =>
            {
                info.SetValue(context.Existing, context.Resolve(info, context.Name, value));
                return context.Existing;
            };
        }

        #endregion



        #region Parameter Resolver Factories

        protected override ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null)
        {
            return (ref BuilderContext context) =>
            {
                ((FieldInfo)info).SetValue(context.Existing,
                    context.Resolve((FieldInfo)info, ((DependencyResolutionAttribute)attribute).Name, resolver ?? DependencyAttribute.Instance));

                return context.Existing;
            };
        }

        protected override ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object resolver, object defaultValue = null)
        {
            return (ref BuilderContext context) =>
            {
                try
                {
                    ((FieldInfo)info).SetValue(context.Existing,
                        context.Resolve((FieldInfo)info, ((DependencyResolutionAttribute)attribute).Name, resolver ?? OptionalDependencyAttribute.Instance));
                    return context.Existing;
                }
                catch
                {
                    ((FieldInfo)info).SetValue(context.Existing, defaultValue);
                    return context.Existing;
                }
            };
        }

        #endregion
    }
}
