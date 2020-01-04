using System.Diagnostics;
using System.Reflection;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Processors
{
    public abstract partial class ParametersProcessor<TMemberInfo>
    {
        #region Resolvers

        protected virtual ResolveDelegate<BuilderContext>[] ParameterResolvers(MethodBase info)
        {
            ParameterInfo[] parameters = info.GetParameters();
            var resolvers = new ResolveDelegate<BuilderContext>[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                resolvers[i] = GetResolverDelegate(parameters[i]);

            return resolvers;
        }

        protected virtual ResolveDelegate<BuilderContext>[] ParameterResolvers(MethodBase info, object[] injectors)
        {
            ParameterInfo[] parameters = info.GetParameters();
            Debug.Assert(parameters.Length == injectors.Length);
            var resolvers = new ResolveDelegate<BuilderContext>[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                resolvers[i] = GetResolverDelegate(parameters[i], injectors[i]);

            return resolvers;
        }

        #endregion


        #region Parameters

        protected virtual ResolveDelegate<BuilderContext> GetResolverDelegate(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<BuilderContext>(info);

            return (ref BuilderContext context) => context.Resolve(info, attribute.Name, resolver);
        }

        protected virtual ResolveDelegate<BuilderContext> GetResolverDelegate(ParameterInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<BuilderContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
                return (ref BuilderContext context) => context.Override(info, attribute.Name, data);
            else
                return (ref BuilderContext context) => context.Resolve(info, attribute.Name, resolver);
        }

        #endregion
    }
}
