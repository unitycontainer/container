using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MethodBaseProcessor<TMemberInfo>
    {
        protected virtual Pipeline[] ParameterResolvers(MethodBase info)
        {
            ParameterInfo[] parameters = info.GetParameters();
            var resolvers = new Pipeline[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                resolvers[i] = GetResolverDelegate(parameters[i]);
            }

            return resolvers;
        }

        protected virtual Pipeline[] ParameterResolvers(MethodBase info, object[] data)
        {
            ParameterInfo[] parameters = info.GetParameters();
            var resolvers = new Pipeline[parameters.Length];

            Debug.Assert(parameters.Length == data.Length);

            for (var i = 0; i < parameters.Length; i++)
            {
                resolvers[i] = GetResolverDelegate(parameters[i], data[i]);
            }

            return resolvers;
        }

        protected virtual Pipeline GetResolverDelegate(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<ResolutionContext>(info);

            throw new NotImplementedException();

            // TODO: return (ref ResolutionContext context) => context.Resolve(info, attribute.Name, resolver);
        }

        protected virtual Pipeline GetResolverDelegate(ParameterInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<ResolutionContext>? resolver = PreProcessResolver(info, attribute, data);

            throw new NotImplementedException();

            // TODO: Implementation

            //if (null == resolver)
            //    return (ref ResolutionContext context) => context.Override(info, attribute.Name, data);
            //else
            //    return (ref ResolutionContext context) => context.Resolve(info, attribute.Name, resolver);
        }
    }
}
