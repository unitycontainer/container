using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        protected virtual ResolveDelegate<PipelineContext>[]? ParameterPipelines(ref SelectionInfo<TMemberInfo, object[]> selection)
        {
            ParameterInfo[]? parameters = selection.MemberInfo?.GetParameters();

            if (null == parameters || 0 == parameters.Length) return null;

            var resolvers = new ResolveDelegate<PipelineContext>[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                resolvers[i] = CreatePipeline(parameters[i]);
            }

            return resolvers;
        }



        protected virtual ResolveDelegate<PipelineContext>[]? ParameterResolvers(MethodBase info)
        {
            ParameterInfo[] parameters = info.GetParameters();

            if (0 == parameters.Length) return null;

            var resolvers = new ResolveDelegate<PipelineContext>[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            { 
                resolvers[i] = CreatePipeline(parameters[i]);
            }

            return resolvers;
        }

        protected virtual ResolveDelegate<PipelineContext>[]? ParameterResolvers(MethodBase info, object[] data)
        {
            ParameterInfo[] parameters = info.GetParameters();

            if (0 == parameters.Length) return null;

            var resolvers = new ResolveDelegate<PipelineContext>[parameters.Length];
            Debug.Assert(parameters.Length == data.Length);

            for (var i = 0; i < parameters.Length; i++)
            { 
                resolvers[i] = CreatePipeline(parameters[i], data[i]);
            }

            return resolvers;
        }

        protected virtual ResolveDelegate<PipelineContext> CreatePipeline(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<PipelineContext>(info);

            return (ref PipelineContext context) => context.Resolve(info, attribute.Name, resolver);
        }

        protected virtual ResolveDelegate<PipelineContext> CreatePipeline(ParameterInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<PipelineContext>? resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
                return (ref PipelineContext context) => context.Override(info, attribute.Name, data);
            else
                return (ref PipelineContext context) => context.Resolve(info, attribute.Name, resolver);
        }



        #region Pre Processor

        protected virtual ResolveDelegate<PipelineContext>? PreProcessResolver(ParameterInfo info, DependencyResolutionAttribute attribute, object? data)
            => data switch
            {
                IResolve policy => policy.Resolve,
                IResolverFactory<ParameterInfo> fieldFactory => fieldFactory.GetResolver<PipelineContext>(info),
                IResolverFactory<Type> typeFactory => typeFactory.GetResolver<PipelineContext>(info.ParameterType),
                Type type when typeof(Type) != info.ParameterType => attribute.GetResolver<PipelineContext>(type),
                _ => null
            };

        #endregion

    }
}
